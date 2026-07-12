# FTikNib Result Schemas

## 1. Purpose

This document defines the initial JSON result schemas used by FTikNib.

The schemas are intended to support the following pipeline:

```text
binary input
-> binary inspection
-> function extraction
-> function filtering
-> feature extraction
-> pair generation
-> similarity computation
-> evaluation
```

The current milestone focuses on binary inspection and function extraction result formats.

## 2. Binary Information Schema

`BinaryInfo` represents metadata for one binary file.

### Fields

| Field          | Type           | Description                                                |
| -------------- | -------------- | ---------------------------------------------------------- |
| `BinaryId`     | string         | Identifier for the binary                                  |
| `Path`         | string         | Path to the binary file                                    |
| `FileName`     | string         | File name of the binary                                    |
| `Architecture` | string         | Architecture name such as `x64`, `arm64`, or `unknown`     |
| `Compiler`     | string or null | Compiler name if known                                     |
| `Optimization` | string or null | Optimization level such as `O0`, `O1`, `O2`, `O3`, or null |

### Example

```json
{
  "BinaryId": "test_O0",
  "Path": "./data/samples/test_O0",
  "FileName": "test_O0",
  "Architecture": "unknown",
  "Compiler": null,
  "Optimization": null
}
```

## 3. Function Information Schema

`FunctionInfo` represents metadata for one function inside a binary.

### Fields

| Field          | Type            | Description                                      |
| -------------- | --------------- | ------------------------------------------------ |
| `BinaryId`     | string          | Identifier of the binary containing the function |
| `FunctionId`   | string          | Unique identifier for the function               |
| `Name`         | string          | Function name                                    |
| `StartAddress` | integer         | Function start address                           |
| `EndAddress`   | integer or null | Function end address if known                    |
| `Size`         | integer or null | Function size if known                           |
| `Architecture` | string          | Architecture of the function                     |

### Example

```json
{
  "BinaryId": "test_O0",
  "FunctionId": "test_O0:main:0x100003f40",
  "Name": "main",
  "StartAddress": 4294983488,
  "EndAddress": 4294983560,
  "Size": 72,
  "Architecture": "x64"
}
```

## 4. Binary Analysis Result Schema

`BinaryAnalysisResult` stores the result of binary inspection or function extraction.

### Fields

| Field       | Type   | Description                 |
| ----------- | ------ | --------------------------- |
| `Binary`    | object | Binary metadata             |
| `Functions` | array  | List of extracted functions |

### Example

```json
{
  "Binary": {
    "BinaryId": "test_O0",
    "Path": "./data/samples/test_O0",
    "FileName": "test_O0",
    "Architecture": "unknown",
    "Compiler": null,
    "Optimization": null
  },
  "Functions": []
}
```

After function extraction is implemented, `Functions` will contain function entries:

```json
{
  "Binary": {
    "BinaryId": "test_O0",
    "Path": "./data/samples/test_O0",
    "FileName": "test_O0",
    "Architecture": "x64",
    "Compiler": "clang",
    "Optimization": "O0"
  },
  "Functions": [
    {
      "BinaryId": "test_O0",
      "FunctionId": "test_O0:main:0x100003f40",
      "Name": "main",
      "StartAddress": 4294983488,
      "EndAddress": 4294983560,
      "Size": 72,
      "Architecture": "x64"
    }
  ]
}
```

## 5. Feature Schema

A feature represents one numeric property extracted from a function.

For easier JSON storage and later experiment processing, feature values should be stored in a flat format.

### Fields

| Field        | Type   | Description                |
| ------------ | ------ | -------------------------- |
| `BinaryId`   | string | Identifier of the binary   |
| `FunctionId` | string | Identifier of the function |
| `Name`       | string | Feature name               |
| `Group`      | string | Feature group              |
| `Value`      | number | Numeric feature value      |

### Example

```json
{
  "BinaryId": "test_O0",
  "FunctionId": "test_O0:main:0x100003f40",
  "Name": "inst_count",
  "Group": "Instruction",
  "Value": 42.0
}
```

## 6. Feature Groups

The planned feature groups are:

| Group         | Description                                         |
| ------------- | --------------------------------------------------- |
| `Instruction` | Instruction count and instruction category features |
| `CFG`         | Control-flow graph features                         |
| `CallGraph`   | Caller and callee related features                  |
| `Data`        | Immediate constant and string reference features    |
| `Type`        | Function argument and return type features          |

## 7. Pair Schema

A function pair is used for similarity evaluation.

### Fields

| Field             | Type    | Description                                         |
| ----------------- | ------- | --------------------------------------------------- |
| `LeftFunctionId`  | string  | First function ID                                   |
| `RightFunctionId` | string  | Second function ID                                  |
| `Label`           | boolean | `true` for positive pair, `false` for negative pair |

### Example

```json
{
  "LeftFunctionId": "test_O0:add:0x100003f00",
  "RightFunctionId": "test_O2:add:0x100003d80",
  "Label": true
}
```

## 8. Similarity Result Schema

A similarity result stores the score for one function pair.

### Fields

| Field             | Type            | Description                     |
| ----------------- | --------------- | ------------------------------- |
| `LeftFunctionId`  | string          | First function ID               |
| `RightFunctionId` | string          | Second function ID              |
| `Score`           | number          | Similarity score                |
| `Label`           | boolean or null | Ground-truth label if available |

### Example

```json
{
  "LeftFunctionId": "test_O0:add:0x100003f00",
  "RightFunctionId": "test_O2:add:0x100003d80",
  "Score": 0.87,
  "Label": true
}
```

## 9. Evaluation Result Schema

Evaluation results summarize the performance of similarity scoring.

### Fields

| Field              | Type           | Description             |
| ------------------ | -------------- | ----------------------- |
| `RocAuc`           | number or null | ROC-AUC score           |
| `AveragePrecision` | number or null | Average precision score |
| `TopKAccuracy`     | object         | Top-k accuracy values   |

### Example

```json
{
  "RocAuc": 0.91,
  "AveragePrecision": 0.88,
  "TopKAccuracy": {
    "1": 0.72,
    "5": 0.84,
    "10": 0.89
  }
}
```
