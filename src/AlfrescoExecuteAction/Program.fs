// Learn more about F# at http://fsharp.org

open System
open System.Text
open System.Net.Http.Headers
open AlfrescoAuthApi
open AlfrescoCoreApi
open Newtonsoft.Json
open System.IO

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

let executeAction url ticket body =
    let core = AlfrescoCore()
    core.Init(url, ticket)
    let response =
        core.ActionExecAsync(actionBodyExec = body)
        |> Async.AwaitTask
        |> Async.RunSynchronously

    let json = JsonConvert.SerializeObject(response)
    printfn "%s" json

[<EntryPoint>]
let main argv =

    // http://localhost:8080/share/page/folder-details?nodeRef=workspace://SpacesStore/93ca01f1-d126-4ab2-a9bd-2cda27f65fe5

    let url = "http://localhost:8082"
    let user = "admin"
    let password = "admin"
    let ticket = getTicket url user password

    let json = File.ReadAllText("http/AddFeature.json")
    let actionBody = JsonConvert.DeserializeObject<ActionBodyExec>(json)

    executeAction url ticket actionBody

    0 // return an integer exit code
