namespace App

open System

module Types =
    open System.Net.Http
    open System
    
    type Item =
        | Request of Request
        | Folder of Folder

    and Request =
        { id: string
          name : string
          method : HttpMethod
          url : string
          requestParameters : string
          createdAt : DateTime
          updatedAt: DateTime }

    and Folder =
        { id: string
          name : string
          mutable items : Item array
          isExpanded : bool
          createdAt : DateTime
          updatedAt: DateTime }
        
    type RequestUpdate = { oldRequest : Request; newRequest : Request }
    
    let itemsAreEqual a b =
        match a, b with
        | Request a, Request b -> a.id = b.id
        | Folder a, Folder b -> a.id = b.id
        | _ -> false
    
    type ItemManagerEvent =
    | ItemsChanged of Item
    | SelectedChanged of Request