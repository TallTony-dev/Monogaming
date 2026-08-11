using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using Monogaming.Interfaces;

namespace Monogaming.Items;

public static class ItemDict
{
    private static Dictionary<string, Item> items = new();
    
    public static Item GetNewItem(string itemID)
    {
        return GetNewItem(itemID, 1);
    }
    public static Item GetNewItem(string itemID, int count)
    {
        if (items.ContainsKey(itemID))
        {
            Item i = items[itemID];
            return i.Copy(count);
        }
        else
        {
            Debug.WriteError($"Tried to get non existent/loaded item {itemID}");
            return null;
        }
    }

    public static bool ContainsItem(string itemID)
    {
        return items.ContainsKey(itemID);
    }

    public static void LoadFromFile(string fileName)
    {
        try {
            string path = "./GameData/" + fileName + ".json";

            Stream textStream = File.OpenRead(path);
            JsonDocument document = JsonDocument.Parse(textStream);
            JsonElement root = document.RootElement;
            string title;
            if ((title = root.GetProperty("Title").GetString()) != "items")
            {
                throw new JsonException($"Title of Json at path {path} was unexpected value: {title}.");
            }

            string name = root.GetProperty("ItemBookName").GetString();
            int count = 0;

            foreach (JsonProperty obj in root.EnumerateObject())
            {
                if (obj.Value.ValueKind == JsonValueKind.Object)
                {
                    try
                    {
                        string id = obj.Value.GetProperty("ID").GetString();
                        items.Add(id, JsonSerializer.Deserialize<Item>(obj.Value));
                        count++;
                    }
                    catch
                    {
                        Debug.WriteError($"Failed to parse item {obj} from ItemBook {name}");
                    }
                }
            }
            Debug.WriteInfo($"Parsed/Loaded {count} items from ItemBook {name}");
        }
        catch (Exception ex)
        {
            Debug.WriteError($"Failed to load ItemDict with name {fileName} with exception {ex.Message}");
        }
    }

}