namespace FTikNib.BinAnalysis

open System.IO
open B2R2.FrontEnd
open FTikNib.Core

type LoadedBinary = {
  Binary: BinaryInfo
  RawPath: string
  Handle: BinHandle
}

module BinaryLoader =

  let private normalizeArchitecture (isa: obj) =
    isa.ToString().ToLowerInvariant()

  let load path =
    if not (File.Exists path) then
      invalidArg "path" $"Binary file does not exist: {path}"

    let handle = BinHandle(path)
    let fileName = Path.GetFileName path
    let arch = normalizeArchitecture handle.File.ISA

    {
      Binary = {
        BinaryId = fileName
        Path = path
        FileName = fileName
        Architecture = arch
        Compiler = None
        Optimization = None
      }
      RawPath = path
      Handle = handle
    }