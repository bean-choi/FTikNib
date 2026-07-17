open System
open FTikNib.Core
open FTikNib.BinAnalysis
open FTikNib.Experiment
open FTikNib.Filtering

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
  printfn "  ftiknib compare-functions <binary1> <binary2> ... --out <compare.json>"

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

      let result: BinaryAnalysisResult = {
        Binary = loaded.Binary
        Functions = []
      }

      Json.save outPath result
      printfn "Loaded binary: %s" loaded.Binary.FileName
      printfn "Architecture: %s" loaded.Binary.Architecture
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
      printfn "Extracted %d functions" functions.Length
      printfn "Saved function list to %s" outPath
      0
  | _ when argv.Length >= 4 && argv[0] = "compare-functions" ->
    let outIndex =
      argv
      |> Array.tryFindIndex ((=) "--out")

    match outIndex with
    | None ->
        eprintfn "Missing --out <compare.json>"
        printUsage ()
        1

    | Some idx when idx + 1 >= argv.Length ->
        eprintfn "Missing output path after --out"
        printUsage ()
        1

    | Some idx ->
        let binaryPaths =
          argv[1 .. idx - 1]
          |> Array.toList

        let outPath = argv[idx + 1]

        if List.isEmpty binaryPaths then
          eprintfn "No input binaries were provided."
          printUsage ()
          1
        else
          let results: BinaryAnalysisResult list =
            binaryPaths
            |> List.map (fun binaryPath ->
                let loaded = BinaryLoader.load binaryPath
                let functions = FunctionExtractor.extractFunctions loaded

                printfn "Extracted %d functions from %s" functions.Length loaded.Binary.FileName

                {
                  Binary = loaded.Binary
                  Functions = functions
                })

          let comparison = FunctionComparison.compare results

          Json.save outPath comparison

          printfn "Compared %d binaries" results.Length
          printfn "Found %d unique function names" comparison.Rows.Length
          printfn "Saved comparison result to %s" outPath
          0
  | [| "filter-functions"; inputPath; "--out"; outPath |] ->
    let input =
      Json.load<BinaryAnalysisResult> inputPath

    let filterResult =
      FunctionFilter.filterDefault input.Functions

    let output: FunctionFilteringResult = {
      Binary = input.Binary
      OriginalCount = input.Functions.Length
      IncludedFunctions = filterResult.Included
      ExcludedFunctions =
        filterResult.Excluded
        |> List.map (fun excluded ->
            {
              Function = excluded.Function
              Reasons = excluded.Reasons
            })
    }

    Json.save outPath output

    printfn "Original functions: %d" input.Functions.Length
    printfn "Filtered functions: %d" output.IncludedFunctions.Length
    printfn "Excluded functions: %d" filterResult.Excluded.Length
    printfn "Saved filtered function list to %s" outPath

    0
  | _ ->
      printUsage ()
      1