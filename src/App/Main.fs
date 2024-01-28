namespace App

open App.Types


module Main =
    open Elmish
    open App.Components
    open Avalonia.FuncUI.DSL
    open Avalonia.Controls
    open Types

    type State =
        { sidebar : Sidebar.State
          requestEditor : RequestEditor.State
          count : int }

    let init () =
        { sidebar = Sidebar.init ()
          requestEditor = RequestEditor.init ()
          count = 0 },
        Cmd.none
        
    type Msg =
        | SidebarMsg of Sidebar.Msg
        | RequestEditorMsg of RequestEditor.Msg
        | ItemStoreUpdate of ItemStore.State
        | Increment

    let update (itemStore : ItemStore.Observer) msg state =
        match msg with
        | SidebarMsg sidebarMsg ->
            let s, cmd, external = Sidebar.update sidebarMsg state.sidebar itemStore
            let mapped = Cmd.map SidebarMsg cmd
            { state with sidebar = s }, mapped
        | RequestEditorMsg reqMsg ->
            let s, cmd, external = RequestEditor.update reqMsg state.requestEditor itemStore
            let mapped = Cmd.map SidebarMsg cmd
            { state with requestEditor = s }, mapped
        | Increment -> { state with count = state.count + 1 }, Cmd.none
        | ItemStoreUpdate u ->
            let sidebarUpdate = { state.sidebar with selectedItem = u.selected; sidebarItems = u.items }
            let editorUpdate = { state.requestEditor with selectedRequest = u.selected }
            { state with sidebar = sidebarUpdate; requestEditor = editorUpdate }, Cmd.none

    let view (state : State) (dispatch : Msg -> unit) =
        Grid.create
            [ Grid.columnDefinitions "*, 4, 4*"
              Grid.children
                  [ Sidebar.view state.sidebar (SidebarMsg >> dispatch)
                    GridSplitter.create [ GridSplitter.background "White"; GridSplitter.column 1; Grid.column 1 ]
                    RequestEditor.view state.requestEditor (RequestEditorMsg >> dispatch) ] ]
