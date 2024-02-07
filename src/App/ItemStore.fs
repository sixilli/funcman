namespace App

module ItemStore =
    open System
    open System.Net.Http
    open Types
    open Avalonia.FuncUI

    type T = {
        items: Item array
        selected: Request option
    }
    let state: IWritable<T> = new State<T>({ items = [||]; selected = None })
    let init () =
        let starterItems = {
          selected = None
          items =  
            [| Request
                   { id = Guid.NewGuid().ToString()
                     name = "test"
                     method = HttpMethod.Get
                     url = "http://www.notgoogle.com"
                     requestParameters = "todo"
                     createdAt = DateTime.Now
                     updatedAt = DateTime.Now }
               Request
                   { id = Guid.NewGuid().ToString()
                     name = "other test"
                     method = HttpMethod.Post
                     url = "http://www.google.com"
                     requestParameters = "todo"
                     createdAt = DateTime.Now
                     updatedAt = DateTime.Now }
               Folder
                   { id = Guid.NewGuid().ToString()
                     name = "my dank requests"
                     items = [||]
                     isExpanded = false
                     createdAt = DateTime.Now
                     updatedAt = DateTime.Now } |] }
        state.Set starterItems
        state
        
    let addItems (item : Item array) =
        let currState = state.Current
        let newItems = Array.append currState.items item
        state.Set { currState with items = newItems }
        
    let addItem () = 
        let item = Request{
             id = Guid.NewGuid().ToString()
             name = "other test"
             method = HttpMethod.Post
             url = "http://www.google.com"
             requestParameters = "todo"
             createdAt = DateTime.Now
             updatedAt = DateTime.Now }
        addItems [|item|]
    
    let setSelected request =
        state.Set { state.Current with selected = Some request }
        
    let expandFolder (folder : Folder) =
        let currState = state.Current
        let mutable itemsCopy = Array.copy currState.items
        let rec findItemToReplace (arr : Item array) =
            if Array.isEmpty arr then () else
            let mutable idx = 0
            while idx < arr.Length do
                match arr[idx] with
                | Folder f when f.id = folder.id ->
                    arr[idx] <- Folder{ f with isExpanded = not f.isExpanded }
                    idx <- arr.Length
                | Folder f -> findItemToReplace f.items
                | _ -> ()
                idx <- idx + 1
                
        findItemToReplace itemsCopy
        state.Set { currState with items = itemsCopy }
        
    let updateRequest (newRequest : Request) =
        let currState = state.Current
        let mutable itemsCopy = Array.copy currState.items
        let rec findItemToReplace (arr : Item array) =
            if Array.isEmpty arr then () else
            let mutable idx = 0
            while idx < arr.Length do
                match arr[idx] with
                | Folder f -> findItemToReplace f.items
                | Request r when r.id = newRequest.id ->
                    arr[idx] <- Request { newRequest with updatedAt = DateTime.Now }
                    idx <- arr.Length
                | _ -> ()
                idx <- idx + 1
        findItemToReplace itemsCopy
        state.Set { currState with items = itemsCopy }
        
        
        // member this.UpdateItem (update : RequestUpdate) =
        //     let mutable itemsCopy = Array.copy state.items
        //     let itemsMatch =
        //         match state.selected with
        //         | Some selected -> selected.id = update.newRequest.id && selected.id = update.oldRequest.id
        //         | None -> false
        //     let rec findItemToReplace (arr : Item array) =
        //         if Array.isEmpty arr then () else
        //         let mutable idx = 0
        //         while idx < arr.Length do
        //             match arr[idx] with
        //             | Folder f -> findItemToReplace f.items
        //             | Request r ->
        //                 if r.id = update.oldRequest.id && r.id = update.newRequest.id then (
        //                     arr[idx]<-Request update.newRequest
        //                     idx <- arr.Length )
        //             idx <- idx + 1
        //             
        //     match itemsMatch with
        //     | true ->
        //         findItemToReplace itemsCopy
        //         state.items <- itemsCopy
        //         itemsChanged.Trigger { state with items = itemsCopy }
        //     | false -> ()
        // member this.UpdateSelected (selectionUpdate : Request option) =
        //     let update = { state with selected = selectionUpdate }
        //     state.selected <- selectionUpdate
        //     selectedChanged.Trigger update
        // member this.AddItem (item : Item) =
        //     let newItems = Array.append state.items [| item |]
        //     let update = { state with items = newItems }
        //     state.items <- newItems
        //     newItemsitemsChanged.Trigger update