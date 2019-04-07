module Items.ItemInSqlite

open Items
open Microsoft.Extensions.DependencyInjection
open System
open System.Data.SQLite

let createDB(connection: SQLiteConnection) =
    let itemsSql =
        "create table if not exists Items (" +
        "Id varchar," +
        "Text varchar," +
        "kcal int," +
        "Date varchar)"
    let itemsCmd = new SQLiteCommand(itemsSql, connection)
    itemsCmd.ExecuteNonQuery() |> ignore

let queryDB(connection: SQLiteConnection) : Item [] =
    let querySql = "select * from Items order by Date desc"
    let queryCmd = new SQLiteCommand(querySql, connection)

    let readItem (reader: SQLiteDataReader): Item =
        let id = reader.["Id"].ToString()
        let text = reader.["Text"].ToString()
        let kcal = reader.["kcal"].ToString() |> int
        let date = reader.["Date"].ToString()
        {
            Id = id;
            Text = text;
            kcal = kcal;
            Date = date;
        }

    let readerToList (reader: SQLiteDataReader) =
        let rec toList  acc =
            match reader.Read() with
            | true -> toList (readItem reader :: acc)
            | false -> acc
        toList []

    queryCmd.ExecuteReader() |> readerToList |> Array.ofList

let insertDB(connection: SQLiteConnection, item: Item) =
    let insertSql = 
        "insert into Items(Id, Text, kcal, Date) " +
        "values (@id, @text, @kcal, @date)"
    let insertCmd = new SQLiteCommand(insertSql, connection)
    insertCmd.Parameters.AddWithValue("@id", item.Id) |> ignore
    insertCmd.Parameters.AddWithValue("@text", item.Text) |> ignore
    insertCmd.Parameters.AddWithValue("@kcal", item.kcal) |> ignore
    insertCmd.Parameters.AddWithValue("@date", item.Date) |> ignore
    insertCmd.ExecuteNonQuery() |> ignore

let find (connection: SQLiteConnection) (criteria: ItemCriteria) : Item [] =
    match criteria with
    | All ->
        queryDB(connection)

let save (connection: SQLiteConnection) (item: Item) : Item =
    insertDB(connection, item) |> ignore
    item

type IServiceCollection with
    member this.AddItemInDB(connection: SQLiteConnection) =
        this.AddSingleton<ItemFind>(find connection) |> ignore
        this.AddSingleton<ItemSave>(save connection) |> ignore
