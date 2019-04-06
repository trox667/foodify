namespace Items.Http

open Giraffe
open Microsoft.AspNetCore.Http
open Items
open FSharp.Control.Tasks.V2
open System

module ItemHttp =
  let handlers : HttpFunc -> HttpContext -> HttpFuncResult =
    choose [
      POST >=> route "/items" >=>
        fun next context ->
          task {
            let save = context.GetService<ItemSave>()
            let! item = context.BindJsonAsync<Item>()
            let item = { item with Id = ShortGuid.fromGuid(Guid.NewGuid())}
            return! json (save item) next context
          }
      
      GET >=> route "/items" >=>
        fun next context ->
          let find = context.GetService<ItemFind>()
          let items = find ItemCriteria.All
          json items next context

      PUT >=> routef "/items/%s" (fun id ->
        fun next context ->
          task {
            let save = context.GetService<ItemSave>()
            let! item = context.BindJsonAsync<Item>()
            let item = {item with Id = id}
            return! json (save item) next context
          })

      DELETE >=> routef "/items/%s" (fun id ->
        fun next context ->
          let delete = context.GetService<ItemDelete>()
          json (delete id) next context)
    ]

