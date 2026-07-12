namespace FTikNib.Core

type FeatureGroup =
  | Instruction
  | CFG
  | CallGraph
  | Data
  | Type

type FeatureValue =
  | IntValue of int
  | FloatValue of float
  | StringValue of string
  | BoolValue of bool

type Feature = {
  Name: string
  Group: FeatureGroup
  Value: FeatureValue
}

type FunctionFeatures = {
  BinaryId: string
  FunctionId: string
  Features: Feature list
}