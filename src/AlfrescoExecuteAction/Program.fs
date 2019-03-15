// Learn more about F# at http://fsharp.org

open System
open System.Text
open System.Net.Http.Headers
open AlfrescoAuthApi
open AlfrescoCoreApi
open Newtonsoft.Json

let basicToken user password =
    let byteArray = Encoding.ASCII.GetBytes(sprintf "%s:%s" user password)
    let base64 = Convert.ToBase64String(byteArray)
    AuthenticationHeaderValue("Basic", base64)

let getTicket url user password =
    let auth = AlfrescoAuth()
    auth.Init(url)
    let response =
        auth.CreateTicketAsync(TicketBody(UserId = user, Password = password))
        |> Async.AwaitTask
        |> Async.RunSynchronously
    (response.Entry.Id)

let executeAction url ticket =
    let core = AlfrescoCore()
    core.Init(url, ticket)

    let exec = ActionBodyExec()
    exec.ActionDefinitionId <- ""
    exec.Params

    //core.ActionExecAsync()

[<EntryPoint>]
let main argv =

    let url = "http://localhost:8082"
    let user = "admin"
    let password = "admin"
    let ticket = getTicket url user password

    let json =
        """{
            "actionDefinitionId": "aaa",
            "params": {
                "a" : 100,
                "b" : 200
            }
        }"""

    let obj = JsonConvert.DeserializeObject<ActionBodyExec>(json)
    printfn "%A" obj.ActionDefinitionId
    printfn "%A" obj.Params

    printfn "%A" <| obj.ToJson()

    0 // return an integer exit code
