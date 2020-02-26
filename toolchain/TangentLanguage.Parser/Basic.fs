module TangentLanguage.Parser.Basic

open OurParserC
open Parser



type LineComment = string option

exception EarlyEOF

let any =
    Parsers.anyChar
    >> Parsed.mapError (fun _ -> EarlyEOF)

exception RequireWhitespace
let whitespace0 = zeroOrMore (Parsers.character ' ') >> Parsed.ignore
let whitespace1 = oneOrMore (Parsers.character ' ') >> Parsed.ignore >> Parsed.map (fun _ -> RequireWhitespace)


exception IsNotAName
let name = 
    oneOrMore (
        pred any (fun x -> Seq.exists ((=) x) ['.';':';'$';' ';'\n'] |> not)
        >> Parsed.mapError (fun _ -> IsNotAName))
        >> Parsed.map (List.toArray >> System.String)

exception IsNotAString
let string = 
    oneOrMore (pred any ((<>) '\n')) 
    >> Parsed.map (List.toArray >> System.String)
    >> Parsed.mapError (fun _ -> IsNotAString)

let comment : LineComment parser = zeroOrOne (whitespace0 <+@> Parsers.character '#' <+@> string)

exception RequireNewLine
let newline = 
    let ch = Parsers.character '\n' <|> Parsers.character '\r' <||||> Parsers.literal "\r\n"
    comment <+> zeroOrMore (whitespace0 <+@> comment <@+> ch) >> Parsed.mapError (fun _ -> RequireNewLine)