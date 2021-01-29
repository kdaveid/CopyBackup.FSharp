module Copy

open System.IO
open System.Text.RegularExpressions
open Helpers
open MyConsole
open Model
open System.Diagnostics

let private getFiles path searchPattern =
    match searchPattern with
    | Some (searchPattern) -> Directory.GetFiles(path, searchPattern)
    | None -> Directory.GetFiles(path)

let private filterRegex regex fileList =
    match regex with
    | None -> fileList
    | Some (regex) ->
        fileList
        |> Array.filter
            (fun f ->
                Regex
                    .Match(f, regex, RegexOptions.IgnoreCase)
                    .Success)

let private getFileList (job: CopyJob) =
    getFiles job.From job.SearchPattern
    |> filterRegex job.Regex
    |> Array.map
        (fun file ->
            let fi = FileInfo(file)
            let dest = Path.Combine(job.To, fi.Name)

            { Source = file
              Destination = dest
              SizeInBytes = fi.Length
              ExistsAtDest = File.Exists(dest)
              FileCopied = false })


let private getTargetSpace (str: string) =
    let di = DriveInfo(str.[..2])
    di.AvailableFreeSpace

let private calculateNeededDiskBytes (files: CopyFile []) =
    files
    |> Array.filter (fun file -> not file.ExistsAtDest)
    |> Array.sumBy (fun file -> file.SizeInBytes)


let copyFile file overwrite whatIf =
    if whatIf = DoItReally then
        let sw = Stopwatch.StartNew()
        printfn "Copying file %s ..." file.Source
        File.Copy(file.Source, file.Destination, (overwrite = Overwrite))
        printfn "Finished copying file in %d ms\n" sw.ElapsedMilliseconds
        let update = { file with FileCopied = true }
        update
    else
        printfn "WhatIf active: Skipping file %s" file.Source
        file


let private copyJob job =
    "Copy job" |> header |> prnt
    match job.SourceFiles with
    | Some (files) ->
        let copiedFiles = files
                        |> Array.filter (fun file -> not file.ExistsAtDest || job.OverwriteMode = Overwrite)
                        |> Array.map (fun file -> copyFile file job.OverwriteMode job.WhatIf)
                        |> Some
        match copiedFiles with 
        | Some(files) -> printfn "%d files have been copied " files.Length
        | None -> printfn "No files have been copied"
        copiedFiles
    | None ->
        (printfn "No files found at source - skipping"
         job.SourceFiles)


let Copy (job: CopyJob) =
    let files = getFileList job
    let job = { job with SourceFiles = Some(files) }

    let requiredDiskSpace = calculateNeededDiskBytes files
    let availableSpace = getTargetSpace job.To

    printJob job requiredDiskSpace availableSpace |> ignore

    if availableSpace > requiredDiskSpace then
        let copiedFiles = copyJob job
        { job with CopiedFiles = copiedFiles}
    else
        failwith "not enough disk space!"
