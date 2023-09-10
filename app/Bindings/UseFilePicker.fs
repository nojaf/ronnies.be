module UseFilePicker

open Fable.Core.JsInterop

type FileContent = {| name : string ; content : string |}

type UseFilePickerOptions =
    {|
        readAs : string
        accept : string
        multiple : bool
        limitFilesConfig : {| max : int |}
        maxFileSize : int
        onFilesSuccessfullySelected :
            {|
                painFiles : obj array
                filesContent : FileContent array
            |}
                -> unit
    |}

type UseFilePickerResult =
    {|
        clear : unit -> unit
        filesContent : FileContent array
        loading : bool
        errors : string array
        openFilePicker : unit -> unit
        plainFiles : obj array
    |}

let useFilePicker : UseFilePickerOptions -> UseFilePickerResult =
    import "useFilePicker" "use-file-picker"
