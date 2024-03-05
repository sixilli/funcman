namespace App

module Requests =
    open FSharp.Data
    open System
    open Types
    
    let makeRequest (req : Request) =
        async {
            let! resp = Http.AsyncRequest (req.url, headers = [ "Accept", "application/json" ], httpMethod = req.method.ToString (), silentHttpErrors = true)
            return resp
        }
