namespace App


module AppState =
    let items = ItemStore.init ()
module Main =
    open App.Components
    open Avalonia.FuncUI
    open Avalonia.FuncUI.DSL
    open Avalonia.Controls

    let view  =
        Component(fun ctx ->
            let state = ctx.usePassed ItemStore.state
            Grid.create
                [ Grid.columnDefinitions "*, 4, 4*"
                  Grid.children
                      [
                        Sidebar.view state
                        GridSplitter.create [ GridSplitter.background "White"; GridSplitter.column 1; Grid.column 1 ]
                        RequestEditor.view ()
                      ]
                  ]
        )
