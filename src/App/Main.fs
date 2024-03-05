namespace App


module Main =
    open App.Components
    open Avalonia.FuncUI
    open Avalonia.FuncUI.DSL
    open Avalonia.Controls
    open Avalonia.FuncUI.Types

    let view =
        Component (fun ctx ->
            let selected = ctx.usePassedRead StateStore.itemStore.Selected
            let items = ctx.usePassedRead StateStore.itemStore.Items
            //ctx.useEffect ((fun _ -> printfn "selected updated"), [EffectTrigger.AfterChange selected])
            
            Grid.create
                [ Grid.columnDefinitions "*, 4, 4*"
                  Grid.children
                      [
                        ContentControl.create [
                            Grid.column 0
                            ContentControl.content (Sidebar.view items)
                        ]
                        GridSplitter.create [ GridSplitter.background "White"; GridSplitter.column 1; Grid.column 1 ]
                        ContentControl.create [
                            Grid.column 2
                            ContentControl.content (RequestEditor.view selected)
                        ]
                      ]])