namespace FTikNib.Filtering

open System
open System.Text.RegularExpressions

module FunctionName =

  let normalizePlatformPrefix (name: string) =
    if String.IsNullOrWhiteSpace name then
      name
    else
      name.TrimStart('_')

  let removeCompilerSuffix (name: string) =
    name
    |> fun value ->
        Regex.Replace(
          value,
          @"\.(constprop|isra|part)\.\d+$",
          "",
          RegexOptions.CultureInvariant)
    |> fun value ->
        Regex.Replace(
          value,
          @"\.cold(\.\d+)?$",
          "",
          RegexOptions.CultureInvariant)
    |> fun value ->
        Regex.Replace(
          value,
          @"\.llvm\.[0-9A-Fa-f]+$",
          "",
          RegexOptions.CultureInvariant)

  let normalizeForComparison name =
    name
    |> normalizePlatformPrefix
    |> removeCompilerSuffix