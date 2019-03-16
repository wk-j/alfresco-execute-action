// Learn more about F# at http://fsharp.org

open System
open System.Text
open System.Net.Http.Headers
open AlfrescoAuthApi
open AlfrescoCoreApi
open Newtonsoft.Json
open System.IO
open System
open System.Threading
open AlfrescoApi.Custom

type Options = {
    User: string
    Password: string
    Url: string
    Action: FileInfo option
    TargetPath: string
}

let defaultOptions() =
    { User = "admin"
      Password = "admin"
      Action = None
      Url = "http://localhost:8082"
      TargetPath = ""}

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


let targetPathId url path ticket =
    let core = AlfrescoApi.Custom.CustomApiClient(url, ticket)
    let response =
        core.GetNodeByRelativePath(path)
        |> Async.AwaitTask
        |> Async.RunSynchronously
    response.Uuid

let rec parseArgs options args =
    match args with
    | "--user" :: xs | "-u" :: xs ->
        match xs with
        | value :: xss -> parseArgs { options with User = value } xss
        | _ -> parseArgs options xs
    | "--password" :: xs | "-p" :: xs ->
        match xs with
        | value :: xss -> parseArgs { options with Password = value } xss
        | _ -> parseArgs options xs
    | "--target-path" :: xs | "-t" :: xs ->
        match xs with
        | value :: xss -> parseArgs { options with TargetPath = value } xss
        | _ -> parseArgs options xs
    | "--url" :: xs | "-h" :: xs ->
        match xs with
        | value :: xss -> parseArgs { options with Url = value } xss
        | _ -> parseArgs options xs
    | [value] ->
        let action =
            if File.Exists(value) then Some (FileInfo(value))
            else None
        { options with Action = action}
    | _ -> options

[<EntryPoint>]
let main argv =
    let options = parseArgs (defaultOptions()) (List.ofArray argv)
    let url = options.Url
    let user = options.User
    let password = options.Password
    let ticket = getTicket url user password
    let action  = options.Action
    let targetPath = options.TargetPath


    match action with
    | Some file ->
        let json = File.ReadAllText(file.FullName)
        let actionBody = JsonConvert.DeserializeObject<ActionBodyExec>(json)
        if targetPath <> "" then
            let uuid = targetPathId url targetPath ticket
            actionBody.TargetId <- uuid
        executeAction url ticket actionBody
        printfn "> Add action - %A to %A" (actionBody.ActionDefinitionId) (actionBody.TargetId)
        0
    | None ->
        -1

