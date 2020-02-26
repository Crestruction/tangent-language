module TangentLanguage.Parser.Test.Parsing

open TangentLanguage.Parser
open NUnit.Framework
open OurParserC
open Parser

[<Test>]
let parseExamples () =
    let parse = System.IO.File.ReadAllText >> Statements.parse
    System.IO.Directory.EnumerateFiles (__SOURCE_DIRECTORY__ + "../../../examples/")
    |> Seq.iter (fun x -> 
        printfn "------ %s ------" (x.[1 + x.LastIndexOf '/' ..])
        printfn ""
        try
            printfn "%A" (parse x)
        with Parsed.ParsedException(e,t) ->
            printfn "%A" e
            printfn "%A" t
        printfn ""
        printfn "")