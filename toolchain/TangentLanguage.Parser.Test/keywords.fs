module TangentLanguage.Parser.Test.Keywords

open TangentLanguage.Parser
open NUnit.Framework
open OurParserC
open Parser


[<Test>]
let keywordTest () =
    "if cond true false custum switch cond "
    |> Input.create
    |> (zeroOrMore (Keywords.keyword <@+> Parsers.character ' '))
    |> printfn "%A"
