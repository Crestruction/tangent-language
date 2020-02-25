module TangentLanguage.Parser.Test.Preprocessor

open TangentLanguage.Parser
open NUnit.Framework
open OurParserC

[<Test>]
let preprocessorTest () =
    """log: HelloWord #HelloWorld  
    log: HelloWorld2  """
    |> Preprocessor.preprocess
    |> fun (Preprocessor.Preprocess x) -> x
    |> printfn "%s"

