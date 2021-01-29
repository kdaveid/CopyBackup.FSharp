module Helpers

open Model
open MyConsole
open System

let toHumanReadable (len:int64) =
    match float32(len) with
    | i when i >= 1000000000000.0f -> sprintf "%.2f %s" (i / 1000000000000.0f) "TB" 
    | i when i >= 1000000000.0f -> sprintf "%.2f %s" (i / 1000000000.0f) "GB" 
    | i when i >= 1000000.0f -> sprintf "%.2f %s" (i / 1000000.0f)  "MB" 
    | i when i >= 1000.0f -> sprintf "%.2f %s" (i / 1000.0f) "KB" 
    | i -> sprintf "%.2f %s" i "Bytes" 



let printFiles (files: CopyFile [] option) =
    "Files to copy: " |> header |> prnt
    match files with
    | Some(f) -> f
                |> Array.iter (fun c -> printfn " %s | Exists at destination: %b" c.Source c.ExistsAtDest)
    | None -> printfn " No files found at source"
    files

let printJob (job: CopyJob) requiredDiskSpace availableSpace=
    "Job settings: " |> header |> prnt
    printfn " Source: %s" job.From
    printfn " Destination: %s" job.To

    match job.SourceFiles with
    | Some (x) -> printfn " Files found: %d" x.Length
    | None -> printfn " No files found at source"

    requiredDiskSpace
    |> toHumanReadable
    |> printfn " Required disk space: %s"

    availableSpace
    |> toHumanReadable
    |> printfn " Free available disk space at %s: %s" job.To.[..2]

    printFiles job.SourceFiles