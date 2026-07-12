namespace FTikNib.Features

open FTikNib.Core

module InstructionFeature =

  let extractPlaceholder (fn: FunctionInfo) =
    {
      BinaryId = fn.BinaryId
      FunctionId = fn.FunctionId
      Features = [
        {
          Name = "inst_count"
          Group = FeatureGroup.Instruction
          Value = FeatureValue.IntValue 0
        }
      ]
    }