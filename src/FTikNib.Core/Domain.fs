namespace FTikNib.Core

(*
type Architecture =
  | X86
  | X64
  | ARM32
  | ARM64
  | MIPS
  | PPC
  | UnknownArch of string

type OptimizationLevel =
  | O0
  | O1
  | O2
  | O3
  | Os
  | Oz
  | UnknownOpt of string
*)

type BinaryInfo = {
  BinaryId: string
  Path: string
  FileName: string
  Architecture: string
  Compiler: string option
  Optimization: string option
}

type FunctionInfo = {
  BinaryId: string
  FunctionId: string
  Name: string
  StartAddress: uint64
  EndAddress: uint64 option
  Size: uint64 option
  Architecture: string

  SectionName: string option
  SectionKind: string option
  IsExecutableSection: bool
  SymbolBinding: string
}

type FunctionPair = {
  LeftFunctionId: string
  RightFunctionId: string
  Label: bool
}