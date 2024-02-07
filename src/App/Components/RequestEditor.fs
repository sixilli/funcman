namespace App.Components

module RequestEditor =
    open App
    open App.Types
    open Avalonia.Controls
    open Avalonia.FuncUI
    open Avalonia.FuncUI.DSL
    open Avalonia.Layout

    let emptyDisplay =
        DockPanel.create [ DockPanel.children [ TextBlock.create [ TextBlock.text "select a request fam" ] ] ]

    let addressBar (request : IWritable<Request>) =
        Component.create ("address-bar", fun ctx ->
            DockPanel.create [
                DockPanel.dock Dock.Top
                DockPanel.children [
                    TextBox.create [
                        TextBox.text request.Current.url
                        TextBox.onTextChanged (fun text ->
                            match text with
                            | text when text <> request.Current.url -> ItemStore.updateRequest { request.Current with url = text }
                            | _ -> ()
                        )
                        TextBox.horizontalAlignment HorizontalAlignment.Stretch
                    ]
                ]
            ]
        )
    let requestEditor (request : IWritable<Request>) =
        Component.create ("editorWrapper", fun ctx ->
            DockPanel.create [
                DockPanel.children [
                    TextBlock.create [
                        TextBlock.text request.Current.name
                    ]
                    addressBar request
                ]
            ]
        )

    let view () =
        Component.create ("editorMain", fun ctx ->
            let state = ctx.usePassed ItemStore.state
            DockPanel.create
                [ Grid.column 2
                  DockPanel.children
                      [ match state.Current.selected with
                        | Some request ->
                            let request = ctx.useState request
                            requestEditor request
                        | None -> emptyDisplay ] ]

        )