namespace App.Components


module Sidebar =
    open System
    open App.Types
    open Avalonia.Media
    open App
    open Avalonia.FuncUI
    open Avalonia.FuncUI.Types
    open Avalonia.Layout
    open Avalonia.Controls
    open Avalonia.FuncUI.DSL

    let createRequestItem (request : IReadable<Request>) (now : IReadable<DateTime>) =
        Component.create (
            $"request-{request.Current.id}-{request.Current.updatedAt}-{now.Current.Millisecond}",
            fun ctx ->
                let now = ctx.usePassedRead now
                DockPanel.create
                    [ DockPanel.onPointerPressed ((fun _ -> StateStore.itemStore.setSelected request.Current), SubPatchOptions.Always)
                      DockPanel.background "Transparent"
                      DockPanel.height 50
                      DockPanel.margin 5
                      DockPanel.children
                          [ TextBlock.create
                                [ TextBlock.text <| request.Current.method.ToString ()
                                  TextBlock.dock Dock.Left
                                  TextBlock.verticalAlignment VerticalAlignment.Center ]
                            TextBlock.create
                                [ TextBlock.verticalAlignment VerticalAlignment.Center
                                  TextBlock.text request.Current.name ] ] ]
        )

    let rec createFolderItem (folder : IReadable<Folder>) (now : IReadable<DateTime>) =
        Component.create (
            $"folder-{folder.Current.id}-{folder.Current.updatedAt}-{now.Current.Millisecond}",
            fun ctx ->
                let now = ctx.usePassedRead now
                StackPanel.create
                    [ StackPanel.children
                          [ DockPanel.create
                                [ DockPanel.height 50
                                  DockPanel.margin 5
                                  DockPanel.background "Transparent"
                                  DockPanel.onPointerPressed (fun _ -> StateStore.itemStore.expandFolder folder)
                                  DockPanel.children
                                      [ TextBlock.create
                                            [ TextBlock.text folder.Current.name
                                              TextBlock.dock Dock.Right
                                              TextBlock.verticalAlignment VerticalAlignment.Center ]
                                        TextBlock.create
                                            [ TextBlock.text (string folder.Current.isExpanded)
                                              TextBlock.dock Dock.Left
                                              TextBlock.verticalAlignment VerticalAlignment.Center ]
                                        TextBlock.create [ TextBlock.text "" ]
                                        ] ]
                            if folder.Current.isExpanded then
                                folder.Current.items
                                |> Array.toList
                                |> ctx.useState
                                |> State.sequenceBy (function Folder f -> f.id | Request r -> r.id)
                                |> List.map (fun item ->
                                    match item with
                                    | Folder folder -> createFolderItem folder :> IView
                                    | Request request -> createRequestItem request :> IView )] ] 
        )
        

    let createSidebarItems (items : IReadable<Item array>) =
        Component.create (
            "sidebar-builder",
            fun ctx ->
                let items = ctx.usePassedRead (items, renderOnChange = true)
                

                ScrollViewer.create
                    [ ScrollViewer.content (
                          StackPanel.create [
                              StackPanel.background "Gray"
                              StackPanel.children (
                                  items.Current
                                  |> Array.toList
                                  |> ctx.useState
                                  |> State.sequenceBy (function Folder f -> f.id | Request r -> r.id)
                                  |> List.map (fun item ->
                                      match item.Current with
                                      | Folder f -> createFolderItem f :> IView
                                      | Request r -> createRequestItem r :> IView)
                              )
                          ]
                    )]
        )


    let view (items : IReadable<Item array>) =
        Component.create (
            "sidebar",
            fun ctx ->
                let items = ctx.usePassedRead items

                DockPanel.create
                    [ DockPanel.verticalAlignment VerticalAlignment.Stretch
                      DockPanel.horizontalAlignment HorizontalAlignment.Stretch
                      DockPanel.background "Red"
                      DockPanel.children
                          [ Button.create
                                [ Button.dock Dock.Top
                                  Button.content "new request"
                                  Button.background (Colors.Brown.ToString ())
                                  Button.onClick (fun _ -> StateStore.itemStore.addItem ()) ]
                            createSidebarItems items ] ]
        )
