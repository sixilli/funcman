namespace App

module ItemStore =
    open System
    open System.Net.Http
    open Types

    type State = {
        mutable items: Item array
        mutable selected: Request option
    }

    type Observer() =
        let mutable state = {
          selected = None
          items =  
            [| Request
                   { name = "test"
                     method = HttpMethod.Get
                     url = "http://www.google.com"
                     requestParameters = "todo"
                     createdAt = DateTime.Now
                     updatedAt = DateTime.Now }
               Request
                   { name = "other test"
                     method = HttpMethod.Post
                     url = "http://www.google.com"
                     requestParameters = "todo"
                     createdAt = DateTime.Now
                     updatedAt = DateTime.Now }
               Folder
                   { name = "my dank requests"
                     items = [||]
                     isExpanded = false
                     createdAt = DateTime.Now
                     updatedAt = DateTime.Now } |] }
        let itemsChanged = Event<State> ()
        let selectedChanged = Event<State> ()
        member this.ItemsChanged = itemsChanged.Publish
        member this.SelectedChanged = selectedChanged.Publish
        member this.UpdateItem (update : RequestUpdate) =
            let mutable itemsCopy = Array.copy state.items
            let equalItems (a: Request) (b: Request) = a.createdAt = b.createdAt && a.name = b.name
            let rec findItemToReplace (arr : Item array) =
                if Array.isEmpty arr then () else
                let mutable idx = 0
                while idx < arr.Length - 1 do
                    match arr[idx] with
                    | Folder f -> findItemToReplace f.items
                    | Request r ->
                        if equalItems r update.oldRequest then (
                            arr[idx]<-Request update.newRequest
                            idx <- arr.Length )
                    idx <- idx + 1
            findItemToReplace itemsCopy
            state.items <- itemsCopy
            itemsChanged.Trigger { state with items = itemsCopy }
        member this.UpdateSelected (selectionUpdate : Request option) =
            let update = { state with selected = selectionUpdate }
            state.selected <- selectionUpdate
            selectedChanged.Trigger update
        member this.AddItem (item : Item) =
            let newItems = Array.append state.items [| item |]
            let update = { state with items = newItems }
            state.items <- newItems
            itemsChanged.Trigger update