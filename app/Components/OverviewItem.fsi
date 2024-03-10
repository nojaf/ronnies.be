module Components.OverviewItem

open Fable.Core

type OverviewItemProps =
    {|
        id: string
        location: RonnyLocation
        users: Map<uid, string>
    |}

[<ExportDefault>]
val OverviewItem: props: OverviewItemProps -> JSX.Element
