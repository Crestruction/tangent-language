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
| Include
| UserCustumKeyword of string

exception IsNotAKeyword
let keyword =
    Basic.name
    >> Parsed.map (function
    | "call" -> Call
    | "log" -> Log
    | "if" -> If
    | "cond" -> Cond
    | "true" -> True
    | "false" -> False
    | "switch" -> Switch
    | "case" -> Case
    | "prog" -> Prog
    | "while" -> While
    | "eval" -> Eval
    | "include" -> Include
    | x -> UserCustumKeyword x)
    >> Parsed.mapError (fun _ -> IsNotAKeyword)
    

