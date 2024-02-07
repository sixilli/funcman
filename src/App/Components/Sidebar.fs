namespace App.Components

module Sidebar =
    open App
    open App.Types
    open Avalonia.FuncUI
    open Avalonia.FuncUI.Types
    open Avalonia.Layout
    open Avalonia.Controls
    open Avalonia.FuncUI.DSL
    type State =
        { selectedItem : Request option
          sidebarItems : Item array }
    let createFolderItem folder =
        Component.create ($"folder-{folder.id}", fun ctx ->
            DockPanel.create
                [ DockPanel.background "blue"
                  DockPanel.height 50
                  DockPanel.onPointerReleased (fun _ -> ItemStore.expandFolder folder)
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
        )

    let createRequestItem (request : Request) =
        Component.create ($"request-{request.id}", fun ctx ->
            DockPanel.create
                [ DockPanel.background "green"
                  DockPanel.onPointerPressed (fun _ -> ItemStore.setSelected request)
                  DockPanel.height 50
                  DockPanel.children
                      [ TextBlock.create
                            [ TextBlock.text <| request.method.ToString ()
                              TextBlock.dock Dock.Left
                              TextBlock.verticalAlignment VerticalAlignment.Center ]
                        TextBlock.create
                            [ TextBlock.verticalAlignment VerticalAlignment.Center
                              TextBlock.text request.name ] ] ]
        )

    let createSidebarItems (items : IWritable<ItemStore.T>) =
        Component.create ("create-sidebar-items", fun ctx ->
            let state = ctx.usePassed items
            let children =
                (Array.map
                    (fun item ->
                        match item with
                        | Folder folder -> createFolderItem folder :> IView
                        | Request request -> createRequestItem request :> IView
                    ) state.Current.items
                ) |> Array.toList

            ScrollViewer.create
                [ ScrollViewer.content (StackPanel.create [ StackPanel.background "Gray"; StackPanel.children children ]) ]
        )


    let view (items : IWritable<ItemStore.T>) =
        Component.create ("sidebar", fun ctx ->
            let state = ctx.usePassed items
            DockPanel.create
                [ Grid.column 0
                  DockPanel.verticalAlignment VerticalAlignment.Stretch
                  DockPanel.horizontalAlignment HorizontalAlignment.Stretch
                  DockPanel.background "Red"
                  DockPanel.children
                      [ Button.create
                            [ Button.dock Dock.Top
                              Button.onClick (fun _ -> ItemStore.addItem () ) ]
                        createSidebarItems state
                      ]]
        )
