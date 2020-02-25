module TangentLanguage.Parser.Preprocessor

type PreprocessedSource = Preprocess of string

let preprocess (src:string) =
    src.Replace('\r','\n').Split('\n')
    |> Array.map (
        (fun x -> x + "#") 
        >> (fun x -> x.[..(x.IndexOf '#') - 1])
        >> (fun x -> x.TrimEnd()))
    |> Array.filter (not << System.String.IsNullOrWhiteSpace)
    |> Array.reduce (fun a b -> a + "\n" + b)
    |> Preprocess
    
