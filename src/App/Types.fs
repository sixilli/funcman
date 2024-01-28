namespace App

module Types =
    open System.Net.Http
    open System
    
    type Item =
        | Request of Request
        | Folder of Folder

    and Request =
        { name : string
          method : HttpMethod
          url : string
          requestParameters : string
          createdAt : DateTime
          updatedAt: DateTime }

    and Folder =
        { name : string
          mutable items : Item array
          isExpanded : bool
          createdAt : DateTime
          updatedAt: DateTime }
        
    type RequestUpdate = { oldRequest : Request; newRequest : Request }
    
    type ItemManagerEvent =
    | ItemsChanged of Item
    | SelectedChanged of Request