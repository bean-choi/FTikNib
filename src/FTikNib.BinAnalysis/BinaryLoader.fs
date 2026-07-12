namespace FTikNib.BinAnalysis

open System.IO
open FTikNib.Core

type LoadedBinary = {
  Binary: BinaryInfo
  RawPath: string
}

module BinaryLoader =

  let load path =
    if not (File.Exists path) then
      invalidArg "path" $"Binary file does not exist: {path}"

    let fileName = Path.GetFileName path

    {
      Binary = {
        BinaryId = fileName
        Path = path
        FileName = fileName
        Architecture = "unknown"
        Compiler = None
        Optimization = None
      }
      RawPath = path
    }