module TangentLanguage.Parser.Basic

open OurParserC
open Parser

exception EarlyEOF

let any =
    Parsers.anyChar
    >> Parsed.mapError (fun _ -> EarlyEOF)

exception ErrorTab
let tab =
    let space = Parsers.character ' '
    Parsers.character '\t'
    <||||> (space <+> space <+> space <+> space)
    >> Parsed.ignore
    >> Parsed.mapError (fun _ -> ErrorTab)


let newline = Parsers.character '\n'

exception IsNotAName
let name = 
    oneOrMore (
        pred any (fun x -> Seq.exists ((=) x) ['.';':';'$';' ';'\n'] |> not)
        >> Parsed.mapError (fun _ -> IsNotAName))
        >> Parsed.map (List.toArray >> System.String)

let string = 
    oneOrMore (pred any ((<>) '\n')) 
    >> Parsed.map (List.toArray >> System.String)

