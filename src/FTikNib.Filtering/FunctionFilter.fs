namespace FTikNib.Filtering

open System
open System.Text.RegularExpressions
open FTikNib.Core

module FunctionFilter =

  let private runtimeFunctionNames =
    set [
      "_start"
      "start"
      "_init"
      "init"
      "_fini"
      "fini"
      "__libc_start_main"
      "__cxa_finalize"
      "__do_global_dtors_aux"
      "frame_dummy"
      "register_tm_clones"
      "deregister_tm_clones"
      "__stack_chk_fail"
      "__stack_chk_guard"
      "__gmon_start__"
      "dyld_stub_binder"
    ]

  let private normalizedName (fn: FunctionInfo) =
    FunctionName.normalizePlatformPrefix fn.Name

  let private hasEmptyName (fn: FunctionInfo) =
    String.IsNullOrWhiteSpace fn.Name

  let private hasInvalidAddress (fn: FunctionInfo) =
    fn.StartAddress = 0UL

  let private hasZeroSize (fn: FunctionInfo) =
    fn.Size = Some 0UL

  let private isRuntimeFunction (fn: FunctionInfo) =
    let name = normalizedName fn
    Set.contains name runtimeFunctionNames

  let private isImportStub (fn: FunctionInfo) =
    let name = fn.Name.ToLowerInvariant()

    name.Contains("@plt")
    || name.Contains(".plt")
    || name.EndsWith("$plt", StringComparison.Ordinal)
    || name.Contains("__stub")
    || name.StartsWith("j_", StringComparison.Ordinal)

  let private isCompilerGeneratedFunction (fn: FunctionInfo) =
    let name = normalizedName fn

    Regex.IsMatch(
      name,
      @"\.(constprop|isra|part)\.\d+$",
      RegexOptions.CultureInvariant)
    || Regex.IsMatch(
      name,
      @"\.cold(\.\d+)?$",
      RegexOptions.CultureInvariant)
    || Regex.IsMatch(
      name,
      @"\.llvm\.[0-9A-Fa-f]+$",
      RegexOptions.CultureInvariant)

  let private basicReasons options (fn: FunctionInfo) =
    [
      if hasEmptyName fn then
        EmptyName

      if hasInvalidAddress fn then
        InvalidAddress

      if options.RemoveZeroSize && hasZeroSize fn then
        ZeroSize

      if options.RemoveRuntimeFunctions && isRuntimeFunction fn then
        RuntimeFunction

      if options.RemoveImportStubs && isImportStub fn then
        ImportStub

      if
        options.RemoveCompilerGeneratedFunctions
        && isCompilerGeneratedFunction fn
      then
        CompilerGeneratedFunction
    ]

  let private removeDuplicates
    (functions: FunctionInfo list)
    : FunctionInfo list * ExcludedFunction list =

    let folder
      (seenAddresses, seenIds, included, excluded)
      (fn: FunctionInfo) =

      let reasons =
        [
          if Set.contains fn.StartAddress seenAddresses then
            DuplicateAddress

          if Set.contains fn.FunctionId seenIds then
            DuplicateFunctionId
        ]

      if List.isEmpty reasons then
        (
          Set.add fn.StartAddress seenAddresses,
          Set.add fn.FunctionId seenIds,
          fn :: included,
          excluded
        )
      else
        (
          seenAddresses,
          seenIds,
          included,
          {
            Function = fn
            Reasons = reasons
          }
          :: excluded
        )

    let _, _, included, excluded =
      functions
      |> List.sortBy _.StartAddress
      |> List.fold folder (Set.empty, Set.empty, [], [])

    List.rev included, List.rev excluded

  let filter
    (options: FunctionFilterOptions)
    (functions: FunctionInfo list)
    : FunctionFilterResult =

    let classified =
      functions
      |> List.map (fun fn -> fn, basicReasons options fn)

    let initiallyIncluded =
      classified
      |> List.choose (fun (fn, reasons) ->
          if List.isEmpty reasons then Some fn
          else None)

    let initiallyExcluded =
      classified
      |> List.choose (fun (fn, reasons) ->
          if List.isEmpty reasons then
            None
          else
            Some {
              Function = fn
              Reasons = reasons
            })

    let included, duplicateExcluded =
      removeDuplicates initiallyIncluded

    {
      Included = included
      Excluded = initiallyExcluded @ duplicateExcluded
    }

  let filterDefault functions =
    filter FunctionFilterOptions.defaultOptions functions