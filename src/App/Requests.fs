module App.Requests

open FSharp.Data

let get (url : string) =
    async {
        let! resp = Http.AsyncRequest (url, headers = [ "Accept", "application/json" ], httpMethod = "GET")
        return resp
    }
