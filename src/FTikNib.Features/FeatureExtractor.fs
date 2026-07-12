namespace FTikNib.Features

open FTikNib.Core

type IFeatureExtractor =
  abstract member Extract: FunctionInfo -> FunctionFeatures

module FeatureExtractor =

  let merge (features: FunctionFeatures list) =
    match features with
    | [] -> None
    | head :: tail ->
        let merged =
          tail
          |> List.collect _.Features
          |> List.append head.Features

        Some {
          BinaryId = head.BinaryId
          FunctionId = head.FunctionId
          Features = merged
        }