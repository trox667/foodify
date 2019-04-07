// Learn more about F# at http://fsharp.org

open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Giraffe
open Items.Http
open Items.ItemInMemory
open Items.ItemInSqlite
open System.Collections
open System.Data.SQLite
open System.IO

let routes = 
  choose [
    ItemHttp.handlers ]

let configureApp (app: IApplicationBuilder) =
  app.UseGiraffe routes

let setupDB () =
    let dbFileName = "/Users/mrieken/Documents/fsharp/foodify/items.sql"
    let connString = sprintf "Data Source=%s;Version=3;" dbFileName
    if not (File.Exists dbFileName) then
        SQLiteConnection.CreateFile(dbFileName)
    let connection = new SQLiteConnection(connString)
    connection.Open()
    createDB(connection) |> ignore
    connection

let configureServices (services: IServiceCollection) =
  services.AddGiraffe() |> ignore
  //services.AddItemInMemory(Hashtable()) |> ignore
  services.AddItemInDB (setupDB()) |> ignore

[<EntryPoint>]
let main argv =
    WebHostBuilder()
      .UseKestrel()
      .Configure(Action<IApplicationBuilder> configureApp)
      .ConfigureServices(configureServices)
      .Build()
      .Run()
    0 // return an integer exit code
