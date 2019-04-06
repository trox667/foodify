// Learn more about F# at http://fsharp.org

open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Giraffe
open Items.Http
open Items.ItemInMemory
open System.Collections

let routes = 
  choose [
    ItemHttp.handlers ]

let configureApp (app: IApplicationBuilder) =
  app.UseGiraffe routes

let configureServices (services: IServiceCollection) =
  services.AddGiraffe() |> ignore
  services.AddItemInMemory(Hashtable()) |> ignore

[<EntryPoint>]
let main argv =
    WebHostBuilder()
      .UseKestrel()
      .Configure(Action<IApplicationBuilder> configureApp)
      .ConfigureServices(configureServices)
      .Build()
      .Run()
    0 // return an integer exit code
