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

  let private tryGetSection
    (loaded: LoadedBinary)
    (address: uint64)
    : BinSection option =

    match
      BinFileOps.tryFindSectionByAddr
        loaded.Handle.File
        address
    with
    | Ok section ->
        Some section

    | Error _ ->
        None

  let private sectionKindToString kind =
    match kind with
    | BinSectionKind.CodeSection ->
        "CodeSection"
    | BinSectionKind.DataSection ->
        "DataSection"
    | BinSectionKind.UninitializedDataSection ->
        "UninitializedDataSection"
    | BinSectionKind.ThreadLocalStorageSection ->
        "ThreadLocalStorageSection"
    | BinSectionKind.ResourceSection ->
        "ResourceSection"
    | BinSectionKind.DebugSection ->
        "DebugSection"
    | BinSectionKind.MetadataSection ->
        "MetadataSection"
    | BinSectionKind.DynamicLinkageSection ->
        "DynamicLinkageSection"
    | BinSectionKind.UnknownSection ->
        "UnknownSection"

  let private symbolBindingToString binding =
    match binding with
    | BinSymbolBinding.LocalBinding ->
        "Local"
    | BinSymbolBinding.GlobalBinding ->
        "Global"
    | BinSymbolBinding.WeakBinding ->
        "Weak"
    | BinSymbolBinding.UnknownBinding ->
        "Unknown"

  let private toFunctionInfo (loaded: LoadedBinary) (sym: BinSymbol) =
    let size = sym.Size
    let endAddr =
      size
      |> Option.map (fun sz -> sym.Address + sz)
    let section =
      tryGetSection loaded sym.Address

    {
      BinaryId = loaded.Binary.BinaryId
      FunctionId = makeFunctionId loaded.Binary.BinaryId sym.Name sym.Address
      Name = sym.Name
      StartAddress = sym.Address
      EndAddress = endAddr
      Size = size
      Architecture = loaded.Binary.Architecture
      SectionName =
        section
        |> Option.map (fun sec ->
            sec.Name)
      SectionKind =
        section
        |> Option.map (fun sec ->
            sectionKindToString sec.Kind)
      IsExecutableSection =
        section
        |> Option.exists (fun sec ->
            sec.Kind = BinSectionKind.CodeSection)
      SymbolBinding =
        symbolBindingToString sym.Binding
    }

  let private entryPointFallback (loaded: LoadedBinary) =
    match loaded.Handle.File.EntryPoint with
    | Some entry ->
        let section =
          tryGetSection loaded entry
        [
          {
            BinaryId = loaded.Binary.BinaryId
            FunctionId = makeFunctionId loaded.Binary.BinaryId "_entry" entry
            Name = "_entry"
            StartAddress = entry
            EndAddress = None
            Size = None
            Architecture = loaded.Binary.Architecture
            SectionName =
              section
              |> Option.map (fun sec ->
                  sec.Name)
            SectionKind =
              section
              |> Option.map (fun sec ->
                  sectionKindToString sec.Kind)
            IsExecutableSection =
              section
              |> Option.exists (fun sec ->
                  sec.Kind = BinSectionKind.CodeSection)
            SymbolBinding = "Unknown"
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
          |> Array.toList

        if List.isEmpty functions then
          entryPointFallback loaded
        else
          functions

    | None ->
        entryPointFallback loaded