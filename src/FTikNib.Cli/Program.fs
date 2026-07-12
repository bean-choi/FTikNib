open System
open FTikNib.Core
open FTikNib.BinAnalysis

let printUsage () =
  printfn "FTikNib"
  printfn ""
  printfn "Usage:"
  printfn "  ftiknib inspect-binary <binary> --out <result.json>"
  printfn "  ftiknib extract-functions <binary> --out <functions.json>"
  printfn "  ftiknib filter-functions <functions.json> --out <filtered.json>"
  printfn "  ftiknib extract-features <binary> --functions <filtered.json> --out <features.json>"
  printfn "  ftiknib make-pairs <features.json> --config <config.yml> --out <pairs.json>"
  printfn "  ftiknib evaluate <pairs.json> --out <eval.json>"

let getOutPath args =
  args
  |> Array.tryFindIndex ((=) "--out")
  |> Option.bind (fun idx ->
      if idx + 1 < args.Length then Some args[idx + 1]
      else None)

[<EntryPoint>]
let main argv =
  match argv with
  | [| "inspect-binary"; binaryPath; "--out"; outPath |] ->
      let loaded = BinaryLoader.load binaryPath
      let result: BinaryAnalysisResult= {
        Binary = loaded.Binary
        Functions = []
      }
      Json.save outPath result
      printfn "Saved binary analysis result to %s" outPath
      0

  | [| "extract-functions"; binaryPath; "--out"; outPath |] ->
      let loaded = BinaryLoader.load binaryPath
      let functions = FunctionExtractor.extractFunctions loaded
      let result: BinaryAnalysisResult = {
        Binary = loaded.Binary
        Functions = functions
      }
      Json.save outPath result
      printfn "Saved function list to %s" outPath
      0

  | _ ->
      printUsage ()
      1