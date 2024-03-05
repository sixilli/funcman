namespace App



module Types =
    open System
    open System.Net.Http

    type Item =
        | Request of Request
        | Folder of Folder

    and Request =
        { id : Guid
          name : string
          method : HttpMethod
          url : string
          requestParameters : string
          previousResponse : string option
          createdAt : DateTime
          updatedAt : DateTime }

    and Folder =
        { id : Guid
          name : string
          mutable items : Item array
          isExpanded : bool
          createdAt : DateTime
          updatedAt : DateTime }

    let httpMethodToNum (method : HttpMethod) =
        match method with
        | m when HttpMethod.Get = m -> 0
        | m when HttpMethod.Patch = m -> 1
        | m when HttpMethod.Post = m -> 2
        | m when HttpMethod.Put = m -> 3
        | m when HttpMethod.Delete = m -> 4
        | _ -> -1
