open System
open FSharp.CommandLine
open MyConsole
open Copy
open Model

// Source: https://github.com/cannorin/FSharp.CommandLine

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

// how this works?

// let sourceOption =
//     commandOption {
//         names [ "s"; "source" ]
//         description "Source path"
//         takes (regex @"s(source)?$" |> asConst string)
//     }
// let destinationOption =
//     commandOption {
//         names [ "d"; "destination" ]
//         description "Destination path"
//         takes (regex @"d(destination)?$"  |> asConst string)
//     }



// from example:

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
        takes (format("custom:%i").map(Custom))
        takes (format("c:%i").map(Custom))
    }


let init =
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


let mainOps verbosity operation overwrite whatIf source dest =
    printOptions verbosity operation whatIf

    let options =
        { init with
              WhatIf = whatIf
              OverwriteMode = overwrite
              From = source
              To = dest }

    Copy.Copy options


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

        // This is somehow crappy...

        // opt source in sourceOption |> CommandOption.zeroOrExactlyOne 
        // opt destination in destinationOption |> CommandOption.zeroOrExactlyOne

        // let src =             
        //   match source with
        //     | Some x -> x "böa"
        //     | None -> failwith "Source parameter not found. Aborting"
        // let bla = sprintf "%s" 
          
        // let dest =
        //     match destination with
        //     | Some x -> x "b"
        //     | None -> failwith "Destination parameter not found. Aborting"

        mainOps verbosity operation overwrite whatif "src" "dest"
        return 0
    }






[<EntryPoint>]
let main argv =
    mainCommand () |> Command.runAsEntryPoint argv
