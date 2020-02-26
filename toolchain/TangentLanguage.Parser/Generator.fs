module TangentLanguage.Parser.Generator

let newLine = "\r\n"

let rec generateKey : Statements.Key -> string = function
| Statements.Key.FunctionName funcName -> "$" + funcName
| Statements.Key.Keyword x -> 
    match x with
    | Keywords.Call -> "call"
    | Keywords.Log -> "log" 
    | Keywords.If -> "if"
    | Keywords.Cond -> "cond"
    | Keywords.True -> "true"
    | Keywords.False -> "false"
    | Keywords.Switch -> "switch"
    | Keywords.Case -> "case"
    | Keywords.Prog -> "prog"
    | Keywords.While -> "while"
    | Keywords.Eval -> "eval"
    | Keywords.Include -> "include"
    | Keywords.UserCustumKeyword x -> x

let rec generateValue spaceLevel : Statements.Value -> string = function
| Statements.Value v -> v
| Statements.ValueWithArgPairs (v,s) -> v + newLine + generate (spaceLevel+1) s

and generateKVP kv spaceLevel =
    generateKey (fst kv) + ": " + generateValue spaceLevel (snd kv)

and generate spaceLevel : Statements.Statement list -> string = function
| [] -> ""
| x::tail ->
    let spaces = Array.init (4*spaceLevel) (fun _ -> ' ') |> System.String
    let kvpStr = generateKVP x spaceLevel
    spaces + kvpStr + generate spaceLevel tail
    