namespace App

open System
open System.Net.Http
open Types
open Avalonia.FuncUI

type ItemStore =
    { Items : IWritable<Item array>
      Selected : IWritable<Request option> }

    static member init() =
        { Selected = new State<_> (None)
          Items =
            new State<_> (
                [| Request
                       { id = Guid.NewGuid()
                         name = "test"
                         method = HttpMethod.Get
                         url = "http://www.notgoogle.com"
                         requestParameters = "todo"
                         previousResponse = None 
                         createdAt = DateTime.Now
                         updatedAt = DateTime.Now }
                   Request
                       { id = Guid.NewGuid()
                         name = "other test"
                         method = HttpMethod.Post
                         url = "http://www.google.com"
                         requestParameters = "todo"
                         previousResponse = None 
                         createdAt = DateTime.Now
                         updatedAt = DateTime.Now }
                   Folder
                       { id = Guid.NewGuid()
                         name = "my dank requests"
                         items = [|
                             Request {
                               id = Guid.NewGuid()
                               name = "test"
                               method = HttpMethod.Get
                               url = "http://www.notgoogle.com"
                               previousResponse = None 
                               requestParameters = "todo"
                               createdAt = DateTime.Now
                               updatedAt = DateTime.Now }
                         |]
                         isExpanded = false
                         createdAt = DateTime.Now
                         updatedAt = DateTime.Now } |]
            ) }

    member this.addItems(items : Item array) =
        this.Items.Set (Array.append this.Items.Current items)

    member this.addItem() =
        let item =
            Request
                { id = Guid.NewGuid()
                  name = "other test"
                  method = HttpMethod.Post
                  url = "http://www.google.com"
                  requestParameters = "todo"
                  previousResponse = None 
                  createdAt = DateTime.Now
                  updatedAt = DateTime.Now }

        this.addItems [| item |]

    member this.setSelected(request : Request) =
        let tryFindRequest folder =
            Array.tryFind
                (function
                | Request r -> r.id = request.id
                | _ -> false)
                folder.items

        let foldFn acc item =
            match acc, item with
            | Some _, _ -> acc
            | None, Request r when r.id = request.id -> Some r
            | None, Folder f ->
                match tryFindRequest f with
                | Some (Request r) -> Some r
                | _ -> None
            | _ -> None

        this.Selected.Set (Array.fold foldFn None this.Items.Current)

    member this.expandFolder(folder : Folder) =
        let mutable itemsCopy = Array.copy this.Items.Current

        let rec findItemToReplace (arr : Item array) =
            if Array.isEmpty arr then
                ()
            else
                let mutable idx = 0

                while idx < arr.Length do
                    match arr[idx] with
                    | Folder f when f.id = folder.id ->
                        arr[idx] <- Folder { folder with isExpanded = not folder.isExpanded; updatedAt = DateTime.Now }
                        idx <- arr.Length
                    | Folder f -> findItemToReplace f.items.Current
                    | _ -> ()

                    idx <- idx + 1

        findItemToReplace itemsCopy
        this.Items.Set itemsCopy

    member this.updateRequest(newRequest : Request) =
        let mutable itemsCopy = Array.copy this.Items.Current

        let mutable updatedItem : Item option = None
        let rec findItemToReplace (arr : Item array) =
            if Array.isEmpty arr then
                ()
            else
                let mutable idx = 0

                while idx < arr.Length do
                    match arr[idx] with
                    | Folder f -> findItemToReplace f.items.Current
                    | Request r when r.id = newRequest.id ->
                        let updatedRequest = Request { newRequest with updatedAt = DateTime.Now }
                        printfn "update"
                        updatedItem <- Some updatedRequest
                        arr[idx] <- updatedRequest
                            

                        idx <- arr.Length
                    | _ -> ()

                    idx <- idx + 1

        findItemToReplace itemsCopy
        this.Items.Set itemsCopy
        match updatedItem with
        | Some (Request r) -> this.Selected.Set (Some r)
        | _ -> ()


[<RequireQualifiedAccess>]
module StateStore =
    let itemStore = ItemStore.init ()