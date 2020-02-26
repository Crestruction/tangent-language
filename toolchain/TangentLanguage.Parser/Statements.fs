module TangentLanguage.Parser.Statements

open OurParserC
open Parser

type Key = 
| Keyword of Keywords.Keyword
| FunctionName of string

type Value =
| Value of string
| ValueWithArgPairs of string * Statement list
and Statement = Key*Value


let functionName = Parsers.character '$' <+@> Basic.name >> Parsed.map FunctionName

let key = functionName <|> (Keywords.keyword >> Parsed.map Keyword)
let value = Basic.string

exception TabError of int
let tabSpace currentSpaceNumbers = 
    let ch = Parsers.character ' ' <|> Parsers.character '\t' >> Parsed.map (function | '\t' -> 4 | ' ' -> 1 | _ -> 0)
    pred (zeroOrMore ch) (fun l -> (List.sum l) >= currentSpaceNumbers)
    >> Parsed.map List.sum
    >> Parsed.mapError (fun _ -> TabError currentSpaceNumbers)
    


let rec statement (tabSpaceParser:int parser) input : Statement parsed =
    let kvp = 
        (key <@+> Basic.whitespace0 <@+> Parsers.character ':' <@+> Basic.whitespace0 <+> value <@+> Basic.newline)
    input |> (
        let statementParser nextTabTabParser =
            (key <@+> Basic.whitespace0 <@+> Parsers.character ':' <+> Basic.newline <+> childStatements nextTabTabParser
                >> Parsed.map (fun ((k,comment),v) -> k,ValueWithArgPairs ("",v))) <|> 
            (kvp <+> childStatements nextTabTabParser >> Parsed.map (fun ((k,v),s) -> k,ValueWithArgPairs(v,s))) <|>
            (kvp >> Parsed.map (fun (k,v) -> k,Value v))
        (tabSpaceParser @-> (((+) 1) >> tabSpace >> statementParser)))
        

and childStatements (nextTabParser:int parser) : Statement list parser = oneOrMore (statement nextTabParser)

let parse x = 
    Input.create x |> zeroOrMore (statement (tabSpace 0)) |> Parsed.raise
