namespace App.Components

open App


module Sidebar =
    open App.Types
    open Avalonia.FuncUI.Types
    open Avalonia.Layout
    open Elmish
    open Avalonia.Controls
    open Avalonia.FuncUI.DSL
    open System.Net.Http
    open System

    type State =
        { selectedItem : Request option
          sidebarItems : Item array }
    
    let init () = { selectedItem = None; sidebarItems =  [||] }

    type Msg =
        | NewItem of Item
        | SetSelected of Request
        | FolderClicked of Folder
        | UpdateItems of RequestUpdate

    let update msg state (itemStore : ItemStore.Observer) =
        match msg with
        | SetSelected request ->
            itemStore.UpdateSelected (Some request)
            state, Cmd.none, None
        | FolderClicked folder ->
            let folderComparison item =
                match item with
                | Folder f -> f = folder
                | _ -> false

            let folderIdx = Array.tryFindIndex folderComparison state.sidebarItems

            match folderIdx with
            | Some idx ->
                let newSidebarItems = Array.copy state.sidebarItems
                let (Folder folder) = newSidebarItems[idx]

                newSidebarItems[idx] <-
                    Folder
                        { folder with
                            isExpanded = (not folder.isExpanded) }

                { state with sidebarItems = newSidebarItems }, Cmd.none, None
            | None -> state, Cmd.none, None
        | NewItem item ->
            itemStore.AddItem item
            state, Cmd.none, None
        | UpdateItems update ->
            itemStore.UpdateItem update
            state, Cmd.none, None

    

    let createFolderItem folder (dispatcher : Msg -> unit) =
        DockPanel.create
            [ DockPanel.background "blue"
              DockPanel.height 50
              DockPanel.onPointerReleased ((fun _ -> dispatcher (FolderClicked folder)), SubPatchOptions.OnChangeOf folder)
              DockPanel.children
                  [ TextBlock.create
                        [ TextBlock.text (string folder.isExpanded)
                          TextBlock.dock Dock.Left
                          TextBlock.verticalAlignment VerticalAlignment.Center ]
                    TextBlock.create
                        [ TextBlock.text folder.name
                          TextBlock.dock Dock.Right
                          TextBlock.verticalAlignment VerticalAlignment.Center ]
                    TextBlock.create [ TextBlock.text "" ] ] ]

    let createRequestItem (request : Request) (dispatcher : Msg -> unit) =
        DockPanel.create
            [ DockPanel.background "green"
              DockPanel.onPointerPressed ((fun _ -> dispatcher (SetSelected request)), SubPatchOptions.OnChangeOf request)
              DockPanel.height 50
              DockPanel.children
                  [ TextBlock.create
                        [ TextBlock.text <| request.method.ToString ()
                          TextBlock.dock Dock.Left
                          TextBlock.verticalAlignment VerticalAlignment.Center ]
                    TextBlock.create
                        [ TextBlock.verticalAlignment VerticalAlignment.Center
                          TextBlock.text request.name ] ] ]

    let createSidebarItems (state : State) (dispatcher : Msg -> unit) =
        let children =
            (Array.map
                (fun item ->
                    match item with
                    | Folder folder -> createFolderItem folder dispatcher :> IView
                    | Request request -> createRequestItem request dispatcher :> IView)
                state.sidebarItems
             |> Array.toList)

        ScrollViewer.create
            [ ScrollViewer.content (StackPanel.create [ StackPanel.background "Gray"; StackPanel.children children ]) ]


    let view (state : State) (dispatcher : Msg -> unit) =
        DockPanel.create
            [ Grid.column 0
              DockPanel.verticalAlignment VerticalAlignment.Stretch
              DockPanel.horizontalAlignment HorizontalAlignment.Stretch
              DockPanel.background "Red"
              DockPanel.children
                  [ Button.create
                        [ Button.dock Dock.Top
                          Button.onClick (fun _ ->
                              dispatcher (
                                  NewItem (
                                      Request
                                          { name = "hello mark"
                                            method = HttpMethod.Post
                                            url = "cyan is cool"
                                            requestParameters = "arstioen"
                                            createdAt = DateTime.Now
                                            updatedAt = DateTime.Now 
                                          }
                                  )
                              ))
                          Button.content "new item" ]
                    createSidebarItems state dispatcher ] ]
