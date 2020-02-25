module TangentLanguage.Parser.Test.Basic

open TangentLanguage.Parser.Basic
open NUnit.Framework
open OurParserC
open Parser



[<Test>]
let anyTest () = 
     Input.create " "
     |> any
     |> printfn "%A"

[<Test>]
let anyFailedTest () = 
    Input.create ""
    |> any
    |> printfn "%A"

[<Test>]
let tabTest () = 
    Input.create "    \t    "
    |> (tab <+> tab)
    |> printfn "%A"

[<Test>]
let tabFailedTest () = 
    Input.create " 1   \t    "
    |> (tab <+> tab)
    |> printfn "%A"

[<Test>]
let newlineTest () = 
    Input.create "\n"
    |> newline
    |> printfn "%A"

[<Test>]
let newlineFailedTest () = 
    Input.create "\t"
    |> newline
    |> printfn "%A"

[<Test>]
let nameTest () = 
    Input.create "一个名字:"
    |> name
    |> printfn "%A"

[<Test>]
let nameFailedTest () = 
    Input.create "$一个名字:"
    |> name
    |> printfn "%A"

[<Test>]
let stringTest () = 
    Input.create "super\nsuper"
    |> name
    |> printfn "%A"

[<Test>]
let stringFailedTest () = 
    Input.create "\nsuper"
    |> name
    |> printfn "%A"

