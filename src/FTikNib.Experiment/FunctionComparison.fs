namespace FTikNib.Experiment

open FTikNib.Core

module FunctionComparison =

  let private normalizeFunctionName (name: string) =
    if System.String.IsNullOrWhiteSpace name then
      name
    else
      // macOS Mach-O symbol은 _main, _add처럼 앞에 _가 붙을 수 있으므로 제거합니다.
      name.TrimStart('_')

  let private toPresence (fn: FunctionInfo) =
    {
      BinaryId = fn.BinaryId
      FunctionId = fn.FunctionId
      Name = fn.Name
      StartAddress = fn.StartAddress
      Size = fn.Size
    }

  let compare (results: BinaryAnalysisResult list) : FunctionComparisonResult =
    let binaries =
      results
      |> List.map _.Binary

    let allBinaryIds =
      binaries
      |> List.map _.BinaryId

    let allFunctions =
      results
      |> List.collect (fun result -> result.Functions)

    let rows =
      allFunctions
      |> List.groupBy (fun fn -> normalizeFunctionName fn.Name)
      |> List.map (fun (normalizedName, functions) ->
          let present =
            functions
            |> List.map toPresence
            |> List.sortBy _.BinaryId

          let presentBinaryIds =
            present
            |> List.map _.BinaryId
            |> Set.ofList

          let missing =
            allBinaryIds
            |> List.filter (fun binaryId -> not (Set.contains binaryId presentBinaryIds))

          {
            Name = normalizedName
            PresentIn = present
            MissingIn = missing
          })
      |> List.sortBy _.Name

    {
      Binaries = binaries
      Rows = rows
    }