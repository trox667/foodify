module Items.ItemInMemory

open Items
open Microsoft.Extensions.DependencyInjection
open System.Collections

let find (inMemory :Hashtable) (criteria: ItemCriteria) : Item[] =
  match criteria with
  | All -> inMemory.Values |> Seq.cast |> Array.ofSeq

let save (inMemory: Hashtable) (item: Item) : Item =
  inMemory.Add(item.Id, item) |> ignore
  item

type IServiceCollection with
  member this.AddItemInMemory (inMemory : Hashtable) =
    this.AddSingleton<ItemFind>(find inMemory) |> ignore
    this.AddSingleton<ItemSave>(save inMemory) |> ignore