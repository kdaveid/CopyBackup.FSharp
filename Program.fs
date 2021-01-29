open System
open FSharp.CommandLine
open MyConsole
open Copy
open Model


let whatIfOption =
    commandOption {
        names [ "w"; "whatif" ]
        description "Set to whatIf means the code will run but only displaying instead of really doing it"
        takes (regex @"w(hatif)?$" |> asConst WhatIf)
        takes (regex @"d(oit)?$" |> asConst DoItReally)
    }

let operationOption =
    commandOption {
        names [ "c"; "command" ]
        description "Command option: what you want to do"
        takes (regex @"c(opy)?$" |> asConst Copy)
        takes (regex @"r(etention)?$" |> asConst Retention)
    }

let overwriteOption =
    commandOption {
        names [ "o"; "overwrite" ]
        description "Overwrite option if file at destination already exists"
        takes (regex @"o(verwrite)?$" |> asConst Overwrite)
        takes (regex @"s(kip)?$" |> asConst Skip)
    }

// let fileOption =
//   commandOption {
//     names ["f"; "file"]
//     description "Name of a file to use (Default index: 0)"
//     takes (format("%s:%i").withNames ["filename"; "index"])
//     takes (format("%s").map (fun filename -> (filename, 0)))
//     suggests (fun _ -> [CommandSuggestion.Files None])
//   }

type Verbosity =
    | Quiet
    | Normal
    | Full
    | Custom of int

let verbosityOption =
    commandOption {
        names [ "v"; "verbosity" ]
        description "Display this amount of information in the log."
        takes (regex @"q(uiet)?$" |> asConst Quiet)
        takes (regex @"n(ormal)?$" |> asConst Quiet)
        takes (regex @"f(ull)?$" |> asConst Full)
        takes (format("custom:%i").map(fun level -> Custom level))
        takes (format("c:%i").map(fun level -> Custom level))
    }


let test =
    { From = @"C:\Users\david.keller\source\repos\copybackup"
      To = @"C:\Temp"
      SearchPattern = Some "*.*"
      Regex = None
      OverwriteMode = Skip
      WhatIf = WhatIf
      SourceFiles = None
      CopiedFiles = None }


let printOptions verbosity operation whatIf =
    "Options set to:" |> header |> prnt
    printf " Operation: %A\n Verbosity: %A\n Mode: " operation verbosity

    match whatIf with
    | WhatIf -> info <| "What If enabled, just checking\n"
    | DoItReally -> warn <| sprintf "Really doing it!\n"


let mainOps verbosity operation overwrite whatIf =
    printOptions verbosity operation whatIf

    let test =
        { test with
              WhatIf = whatIf
              OverwriteMode = overwrite }

    Copy.Copy test


let mainCommand () =
    command {
        name "copy"
        description "The copy command."

        opt whatif in whatIfOption
                      |> CommandOption.zeroOrExactlyOne
                      |> CommandOption.whenMissingUse DoItReally

        opt operation in operationOption
                         |> CommandOption.zeroOrExactlyOne
                         |> CommandOption.whenMissingUse Copy

        opt overwrite in overwriteOption
                         |> CommandOption.zeroOrExactlyOne
                         |> CommandOption.whenMissingUse Skip
        opt verbosity in verbosityOption
                         |> CommandOption.zeroOrExactlyOne
                         |> CommandOption.whenMissingUse Normal
        // do printfn "%A,%A" verbosity operationOption
        mainOps verbosity operation overwrite whatif
        return 0
    }






[<EntryPoint>]
let main argv =
    mainCommand () |> Command.runAsEntryPoint argv
