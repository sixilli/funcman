namespace App.Components

open App


module RequestEditor =
    open App.Types
    open Elmish
    open Avalonia.Controls
    open Avalonia.FuncUI.DSL
    open Avalonia.Layout

    type State = { selectedRequest : Request option }
    

    type Msg =
        | SetSelectedRequest of Request
        | SendRequest of Request
        | UpdateRequest of RequestUpdate
        
    type ExternalMsg =
        | PersistRequestUpdate of RequestUpdate

    let update msg state (itemStore : ItemStore.Observer) =
        match msg with
        | SendRequest request -> state, Cmd.none, None
        | SetSelectedRequest request ->
            itemStore.UpdateSelected (Some request)
            state, Cmd.none, None
        | UpdateRequest update ->
            itemStore.UpdateItem update
            state, Cmd.none, None

    let init () = { selectedRequest = None }

    let emptyDisplay (state : State) (dispatcher : Msg -> unit) =
        DockPanel.create [ DockPanel.children [ TextBlock.create [ TextBlock.text "select a request fam" ] ] ]

    let addressBar (state : State) (dispatcher : Msg -> unit) (request : Request) =
        DockPanel.create [
            DockPanel.dock Dock.Top
            DockPanel.children [
                TextBox.create [
                    TextBox.text (string request.url)
                    TextBox.onTextChanged (fun text ->
                        match text with
                        | newUrl when newUrl <> request.url ->
                            dispatcher (UpdateRequest { oldRequest = request; newRequest = { request with url = text } })
                        | _ -> () )
                    TextBox.horizontalAlignment HorizontalAlignment.Stretch
                ]
            ]
        ]
    let requestEditor (state : State) (dispatcher : Msg -> unit) (request : Request) =
        DockPanel.create [
            DockPanel.children [
                TextBlock.create [
                    TextBlock.text request.name
                ]
                addressBar state dispatcher request
            ]
        ]

    let view (state : State) (dispatcher : Msg -> unit) =
        DockPanel.create
            [ Grid.column 2
              DockPanel.children
                  [ match state.selectedRequest with
                    | Some request -> requestEditor state dispatcher request
                    | None -> emptyDisplay state dispatcher ] ]
