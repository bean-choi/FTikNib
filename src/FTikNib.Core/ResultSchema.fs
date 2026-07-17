namespace FTikNib.Core

type BinaryAnalysisResult = {
  Binary: BinaryInfo
  Functions: FunctionInfo list
}

type FeatureExtractionResult = {
  Binary: BinaryInfo
  Functions: FunctionFeatures list
}

type FunctionPresence = {
  BinaryId: string
  FunctionId: string
  Name: string
  StartAddress: uint64
  Size: uint64 option
}

type FunctionComparisonRow = {
  Name: string
  PresentIn: FunctionPresence list
  MissingIn: string list
}

type FunctionComparisonResult = {
  Binaries: BinaryInfo list
  Rows: FunctionComparisonRow list
}

type SimilarityResult = {
  LeftFunctionId: string
  RightFunctionId: string
  Score: float
  Label: bool option
}

type EvaluationResult = {
  RocAuc: float option
  AveragePrecision: float option
  TopKAccuracy: Map<int, float>
}
