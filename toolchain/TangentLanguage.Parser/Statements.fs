module TangentLanguage.Parser.Statements

open OurParserC
open Parser

type Key = 
| Keyword of Keywords.Keyword
| FunctionName of string

type Value =
| Value of string
| ValueWithArgPairs of string * (Key * Value) list
| ChildStatements of (Key * Value) list


let functionName = Parsers.character '$' <+@> Basic.name >> Parsed.map FunctionName

let key = functionName <|> (Keywords.keyword >> Parsed.map Keyword)
let value = Basic.string

exception TabError of int
let tabSpace currentSpaceNumbers = 
    pred (zeroOrMore (Parsers.character ' ' <|> Parsers.character '\t')) (fun l -> (List.length l) >= currentSpaceNumbers)
    >> Parsed.map List.length
    >> Parsed.mapError (fun _ -> TabError currentSpaceNumbers)
    

let rec statement (tabSpaceParser:int parser) input =
    let kvp = 
        (key <@+> Basic.whitespace0 <@+> Parsers.character ':' <@+> Basic.whitespace0 <+> value <@+> Basic.whitespace0 <@+> Basic.newline)
    input |> (
        let statementParser nextTabTabParser =
            (key <@+> Basic.whitespace0 <@+> Parsers.character ':' <@+> Basic.whitespace0 <@+> Basic.newline <+> childStatements nextTabTabParser
                >> Parsed.map (fun (k,v) -> k,ChildStatements v)) <|> 
            (kvp <+> childStatements nextTabTabParser >> Parsed.map (fun ((k,v),c) -> k,ValueWithArgPairs(v,c))) <|>
            (kvp >> Parsed.map (fun (a,b) -> a,Value b))
        tabSpaceParser @-> (((+) 1) >> tabSpace >> statementParser))
        

and childStatements (nextTabParser:int parser) = oneOrMore (statement nextTabParser)

let parse x = 
    Input.create x |> zeroOrMore (statement (tabSpace 0)) |> Parsed.raise
