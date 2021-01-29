module Model

type MainOperationOption = Copy | Retention
type OverwriteOption = Overwrite | Skip
type WhatIfOption = WhatIf | DoItReally

type CopyFile =
    { Source: string
      Destination: string
      SizeInBytes: int64
      ExistsAtDest: bool
      FileCopied: bool }


type CopyJob =
    { From: string
      To: string
      SearchPattern: string option
      Regex: string option
      OverwriteMode: OverwriteOption
      WhatIf: WhatIfOption
      SourceFiles: CopyFile [] option
      CopiedFiles: CopyFile [] option }