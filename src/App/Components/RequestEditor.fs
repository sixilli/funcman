namespace App.Components

open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Avalonia.Media


module RequestEditor =
    open System.Net.Http
    open App
    open App.Types
    open Avalonia.FuncUI
    open Avalonia.Layout

    let emptyDisplay () =
        DockPanel.create [ DockPanel.children [ TextBlock.create [ TextBlock.text "select a request fam" ] ] ]

    let responseDisplay () =
        Component.create (
            $"response-display",
            fun ctx ->
                let request = ctx.usePassedRead StateStore.itemStore.Selected
                
                match request.Current with
                | Some request ->
                    StackPanel.create
                        [ StackPanel.children
                              [ TextBlock.create
                                    [ TextBlock.text (Option.defaultValue "" request.previousResponse) ] ] ]
                | _ -> DockPanel.create []
        )

    let methodDropdown () =
        Component.create (
            $"method-dropdown",
            fun ctx ->
                let request = ctx.usePassedRead StateStore.itemStore.Selected
                                
                let methodFromIndex idx =
                    match idx with
                    | 0 -> HttpMethod.Get
                    | 1 -> HttpMethod.Patch
                    | 2 -> HttpMethod.Post
                    | 3 -> HttpMethod.Put
                    | 4 -> HttpMethod.Delete
                    | _ -> HttpMethod.Get
                    
                match request.Current with
                | Some request ->
                    let currIdx = httpMethodToNum request.method
                    ComboBox.create
                        [ ComboBox.onSelectedIndexChanged (fun newIdx ->
                              if newIdx <> currIdx then
                                  StateStore.itemStore.updateRequest
                                      { request with
                                          method = (methodFromIndex newIdx) })
                          ComboBox.dataItems [ "Get"; "Patch"; "Post"; "Put"; "Delete" ]
                          ComboBox.selectedIndex currIdx ]
                | _ -> DockPanel.create []
        )

    let nameBox () =
        Component.create (
            $"namebox",
            fun ctx ->
                let request = ctx.usePassedRead StateStore.itemStore.Selected
                
                match request.Current with
                | Some request ->
                    DockPanel.create
                        [ DockPanel.children
                              [ TextBox.create
                                    [ TextBox.text request.name
                                      TextBox.onTextChanged (fun text ->
                                          match text with
                                          | text when text <> request.name ->
                                              StateStore.itemStore.updateRequest { request with name = text }
                                          | _ -> ())
                                      TextBox.horizontalAlignment HorizontalAlignment.Stretch ] ] ]
                | _ -> DockPanel.create []
        )

    let addressBar () =
        Component.create (
            $"address-bar",
            fun ctx ->
                let request = ctx.usePassedRead StateStore.itemStore.Selected
                
                match request.Current with
                | Some request -> 
                    DockPanel.create
                        [ DockPanel.dock Dock.Top
                          DockPanel.children
                              [ TextBox.create
                                    [ TextBox.text request.url
                                      TextBox.onTextChanged (fun text ->
                                          match text with
                                          | text when text <> request.url ->
                                              StateStore.itemStore.updateRequest { request with url = text }
                                          | _ -> ())
                                      TextBox.horizontalAlignment HorizontalAlignment.Stretch ] ] ]
                | _ -> DockPanel.create []
        )

    let requestEditor () =
        Component.create (
            "editorWrapper",
            fun ctx ->
                let request = ctx.usePassedRead StateStore.itemStore.Selected

                match request.Current with
                | Some request ->
                    StackPanel.create
                        [ StackPanel.children
                              [ TextBlock.create [ TextBlock.text request.name ]
                                addressBar ()
                                methodDropdown ()
                                nameBox ()
                                Button.create
                                    [ Button.content "GOOOOOOOOO"
                                      Button.background (Colors.Green.ToString ())
                                      Button.onClick (fun _ ->
                                          Requests.makeRequest request
                                          |> Async.RunSynchronously
                                          |> (fun response ->
                                              { request with
                                                  previousResponse = Some (response.Body.ToString ()) })
                                          |> StateStore.itemStore.updateRequest) ]
                                Button.create
                                    [ Button.content "CLEAR THIS NAO"
                                      Button.background (Colors.Red.ToString ())
                                      Button.onClick (fun _ ->
                                          StateStore.itemStore.updateRequest
                                              { request with
                                                  previousResponse = None }) ]
                                responseDisplay () ] ]
                | _ -> DockPanel.create []
        )

    let view (selected : IReadable<Request option>) =
        Component.create (
            $"editor-main",
            fun ctx ->
                let selected = ctx.usePassedRead selected
                
                DockPanel.create
                    [ DockPanel.children
                          [
                            match selected.Current with
                            | Some r -> requestEditor ()
                            | None -> TextBlock.create [ TextBlock.text "bruh" ]
                    ] ]
        )
