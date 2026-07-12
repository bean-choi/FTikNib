namespace FTikNib.Core

type BinaryAnalysisResult = {
  Binary: BinaryInfo
  Functions: FunctionInfo list
}

type FeatureExtractionResult = {
  Binary: BinaryInfo
  Functions: FunctionFeatures list
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