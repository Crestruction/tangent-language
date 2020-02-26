module TangentLanguage.Parser.Statements

open OurParserC
open Parser

type Key = 
| Keyword of Keywords.Keyword
| FunctionName of string

type Value =
| Value of string
| ValueWithArgPairs of string * Statement list
| ChildStatements of Statement list
and Statement = (Key*Value) option * (Basic.LineComment * Basic.LineComment list)


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
        (key <@+> Basic.whitespace0 <@+> Parsers.character ':' <@+> Basic.whitespace0 <+> value <+> Basic.newline <@+> Basic.newline)
    input |> (
        let statementParser nextTabTabParser =
            (key <@+> Basic.whitespace0 <@+> Parsers.character ':' <+> Basic.newline <@+> Basic.newline <+> childStatements nextTabTabParser
                >> Parsed.map (fun ((k,comment),v) -> Some(k,ChildStatements v),comment)) <|> 
            (kvp <+> childStatements nextTabTabParser >> Parsed.map (fun (((k,v),comment),c) -> Some(k,ValueWithArgPairs(v,c)),comment)) <|>
            (kvp >> Parsed.map (fun ((k,v),comment) -> Some(k,Value v),comment))
        (tabSpaceParser @-> (((+) 1) >> tabSpace >> statementParser)))
        

and childStatements (nextTabParser:int parser) : Statement list parser = oneOrMore (statement nextTabParser)

let parse x = 
    Input.create x |> zeroOrMore (statement (tabSpace 0)) |> Parsed.raise
