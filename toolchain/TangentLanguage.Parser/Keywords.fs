module TangentLanguage.Parser.Keywords

open OurParserC
open Parser

type Keyword =
| Call
| Log
| If
| Cond
| True
| False
| Switch
| Case
| Prog
| While
| Eval
| UserCustumKeyword of string

let keyword =
    let (-->) x y = Parsers.literal x >> Parsed.map (fun _ -> y) 
    [
        "call" --> Call
        "log" --> Log
        "if" --> If
        "cond" --> Cond
        "true" --> True
        "false" --> False
        "switch" --> Switch
        "case" --> Case
        "prog" --> Prog
        "while" --> While
        "eval" --> Eval ]
    |> List.reduce (<|>)
    |> fun x -> x <|> (Basic.name >> Parsed.map UserCustumKeyword)


