namespace Items

type Item = {
  Id: string
  Text: string
  kcal: int32
  Date: string
}

type ItemSave = Item -> Item

type ItemCriteria = 
  | All

type ItemFind = ItemCriteria -> Item[]

type ItemDelete = string -> bool