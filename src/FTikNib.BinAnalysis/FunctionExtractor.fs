namespace FTikNib.BinAnalysis

open B2R2.FrontEnd.BinFile
open FTikNib.Core

module FunctionExtractor =

  let private isValidFunctionSymbol (sym: BinSymbol) =
    sym.IsDefined
    && sym.Address <> 0UL
    && sym.Kind = BinSymbolKind.FunctionSymbol
    && not (System.String.IsNullOrWhiteSpace sym.Name)

  let private makeFunctionId binaryId name addr =
    $"{binaryId}:{name}:0x{addr:x}"

  let private toFunctionInfo (loaded: LoadedBinary) (sym: BinSymbol) =
    let size = sym.Size
    let endAddr =
      size
      |> Option.map (fun sz -> sym.Address + sz)

    {
      BinaryId = loaded.Binary.BinaryId
      FunctionId = makeFunctionId loaded.Binary.BinaryId sym.Name sym.Address
      Name = sym.Name
      StartAddress = sym.Address
      EndAddress = endAddr
      Size = size
      Architecture = loaded.Binary.Architecture
    }

  let private entryPointFallback (loaded: LoadedBinary) =
    match loaded.Handle.File.EntryPoint with
    | Some entry ->
        [
          {
            BinaryId = loaded.Binary.BinaryId
            FunctionId = makeFunctionId loaded.Binary.BinaryId "_entry" entry
            Name = "_entry"
            StartAddress = entry
            EndAddress = None
            Size = None
            Architecture = loaded.Binary.Architecture
          }
        ]
    | None ->
        []

  let extractFunctions (loaded: LoadedBinary) : FunctionInfo list =
    match loaded.Handle.File.SymbolTable with
    | Some symtab ->
        let functions =
          symtab.Symbols
          |> Array.filter isValidFunctionSymbol
          |> Array.sortBy _.Address
          |> Array.map (toFunctionInfo loaded)
          |> Array.distinctBy _.StartAddress
          |> Array.toList

        if List.isEmpty functions then
          entryPointFallback loaded
        else
          functions

    | None ->
        entryPointFallback loaded