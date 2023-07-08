module ReactWebcam

open React

type VideoConstraints =
    {|
        // width : int
        // height : int
        facingMode : obj
    |}

[<RequireQualifiedAccess>]
type WebcamProp =
    | Audio of bool
    | ScreenshotFormat of string
    | VideoConstraints of VideoConstraints
    | OnUserMediaError of (unit -> unit)

    interface IProp

[<AllowNullLiteral>]
type WebcamRef =
    /// Returns a base64 encoded string of the current webcam image
    abstract getScreenshot : unit -> string

    /// Returns a base64 encoded string of the current webcam image
    abstract getScreenshot : {| width : int ; height : int |} -> string

let inline Webcam props =
    ofImportWithoutChildren "default" "react-webcam" props
