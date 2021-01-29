module MyConsole

open System

let log =
    let lockObj = obj()
    fun color s ->
        lock lockObj (fun _ ->
            Console.ForegroundColor <- color
            printf "%s" s
            Console.ResetColor())

let complete = log ConsoleColor.Magenta
let ok = log ConsoleColor.Green
let info = log ConsoleColor.Cyan
let warn = log ConsoleColor.Yellow
let error = log ConsoleColor.Red

let header str =
    let s = "---------------------------------"
    sprintf "\n%s\n%s\n%s" s str s

let prnt str = printfn "%s" str

