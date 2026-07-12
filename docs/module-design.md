# FTikNib Module Design

## 1. Goal

FTikNib은 기존 TikNib의 binary code similarity analysis 흐름을 F#과 B2R2 기반으로 재구현하는 프로젝트이다.

현재 1단계의 목표는 전체 구현을 시작하기 전에 솔루션 구조, 프로젝트 구조, 공통 타입, CLI 명령어 구조, 결과 저장 형식을 설계하는 것이다.

## 2. Solution Structure

```text
FTikNib/
├── src/
│   ├── FTikNib.Core/
│   ├── FTikNib.BinAnalysis/
│   ├── FTikNib.Filtering/
│   ├── FTikNib.Features/
│   ├── FTikNib.Experiment/
│   └── FTikNib.Cli/
├── tests/
├── data/
│   ├── samples/
│   └── outputs/
└── docs/
```

## 3. Project Responsibilities

### FTikNib.Core

`FTikNib.Core` contains common domain types and result schemas shared by all other modules.

Main responsibilities:

* Define binary information types
* Define function information types
* Define feature-related types
* Define result schemas
* Provide JSON save/load utilities

This module should not depend on B2R2. Other modules should depend on `FTikNib.Core`.

### FTikNib.BinAnalysis

`FTikNib.BinAnalysis` is responsible for binary loading and function extraction.

Main responsibilities:

* Load a binary file from a given path
* Connect to B2R2
* Extract function-level information from a binary
* Convert B2R2-specific data into FTikNib common types

This module is the main place where B2R2 dependency should be used.

### FTikNib.Filtering

`FTikNib.Filtering` is responsible for selecting functions that should be used for feature extraction and experiments.

Main responsibilities:

* Remove functions that are not suitable for analysis
* Remove compiler-generated or library-related functions if needed
* Apply filtering options
* Produce a filtered function list

Filtering is separated from feature extraction because TikNib performs function selection before computing features.

### FTikNib.Features

`FTikNib.Features` is responsible for feature extraction from functions.

Main feature groups:

* Instruction features
* CFG features
* Call graph features
* Data features
* Type features

Each feature extractor should receive function-level information and return a common feature representation.

### FTikNib.Experiment

`FTikNib.Experiment` is responsible for similarity experiments.

Main responsibilities:

* Generate positive and negative function pairs
* Compute similarity scores between functions
* Evaluate results using metrics such as ROC-AUC, average precision, and Top-k accuracy

This module should use extracted features instead of directly analyzing binaries.

### FTikNib.Cli

`FTikNib.Cli` provides command-line access to the project.

Planned CLI commands:

```text
ftiknib inspect-binary <binary> --out <result.json>
ftiknib extract-functions <binary> --out <functions.json>
ftiknib filter-functions <functions.json> --out <filtered.json>
ftiknib extract-features <binary> --functions <filtered.json> --out <features.json>
ftiknib make-pairs <features.json> --config <config.yml> --out <pairs.json>
ftiknib evaluate <pairs.json> --out <eval.json>
```

Currently, the first test target is:

```bash
dotnet run --project src/FTikNib.Cli -- inspect-binary ./data/samples/test_O0 --out ./data/outputs/result.json
```

## 4. Dependency Direction

The intended dependency direction is:

```text
FTikNib.Core
↑
├── FTikNib.BinAnalysis
├── FTikNib.Filtering
├── FTikNib.Features
├── FTikNib.Experiment
└── FTikNib.Cli
```

`FTikNib.Cli` may reference all major modules because it connects the whole pipeline.

`FTikNib.BinAnalysis` should depend on B2R2.

Other modules should avoid direct dependency on B2R2 whenever possible.

## 5. Design Principle

The most important design principle is to isolate B2R2-specific code inside `FTikNib.BinAnalysis`.

Other modules should work with FTikNib-defined common types such as `BinaryInfo`, `FunctionInfo`, and `FunctionFeatures`.

This makes the project easier to test, extend, and compare with the original TikNib implementation.
