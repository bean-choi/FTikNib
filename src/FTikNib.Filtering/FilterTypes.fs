namespace FTikNib.Filtering

open FTikNib.Core

type FilterReason =
  | EmptyName
  | InvalidAddress
  | ZeroSize
  | RuntimeFunction
  | ImportStub
  | CompilerGeneratedFunction
  | DuplicateAddress
  | DuplicateFunctionId

type FunctionFilterOptions = {
  RemoveZeroSize: bool
  RemoveRuntimeFunctions: bool
  RemoveImportStubs: bool
  RemoveCompilerGeneratedFunctions: bool
}

type ExcludedFunction = {
  Function: FunctionInfo
  Reasons: FilterReason list
}

type FunctionFilterResult = {
  Included: FunctionInfo list
  Excluded: ExcludedFunction list
}

type FunctionFilteringResult = {
  Binary: BinaryInfo
  OriginalCount: int
  IncludedFunctions: FunctionInfo list
  ExcludedFunctions: ExcludedFunction list
}

module FunctionFilterOptions =

  let defaultOptions = {
    RemoveZeroSize = true
    RemoveRuntimeFunctions = true
    RemoveImportStubs = true
    RemoveCompilerGeneratedFunctions = true
  }