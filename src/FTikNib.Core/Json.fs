namespace FTikNib.Core

open System.IO
open System.Text.Json
open System.Text.Json.Serialization

module Json =

  let private options =
    let opt = JsonSerializerOptions(WriteIndented = true)
    opt.Converters.Add(JsonStringEnumConverter())
    opt

  let save<'T> (path: string) (value: 'T) =
    let json = JsonSerializer.Serialize(value, options)
    File.WriteAllText(path, json)

  let load<'T> (path: string) =
    let json = File.ReadAllText(path)
    JsonSerializer.Deserialize<'T>(json, options)