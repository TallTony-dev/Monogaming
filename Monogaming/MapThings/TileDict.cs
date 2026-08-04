using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monogaming.Entities;
using Monogaming.Interfaces;
using Monogaming.TextureThings;
using System.Data;

namespace Monogaming.MapThings;

public static class TileDict
{
    

    private static Dictionary<string, Tile> loadedTiles = new();

    public static Tile GetTileCopy(string tileID)
    {
        if (tileID != null && loadedTiles.ContainsKey(tileID))
        {
            return loadedTiles[tileID].Copy();
        }
        return null;
    }

    public static void LoadTiles(string fileName)
    {
        string path = "./GameData/" + fileName + ".json";

        Stream textStream = File.OpenRead(path);
        JsonDocument document = JsonDocument.Parse(textStream);
        JsonElement root = document.RootElement;
        string title;
        if ((title = root.GetProperty("Title").GetString()) != "tiles")
        {
            throw new JsonException($"Title of Json at path {path} was unexpected value: {title}.");
        }
        string name = root.GetProperty("TileBookName").GetString();

        int count = 0;

        foreach (JsonProperty obj in root.EnumerateObject())
        {
            if (obj.Value.ValueKind == JsonValueKind.Object)
            {
                try
                {
                    string id = obj.Value.GetProperty("ID").GetString();
                    if (loadedTiles.ContainsKey(id))
                    {
                        throw new DuplicateNameException($"Attempted to load tile with id {id} twice");
                    }

                    loadedTiles.Add(id, JsonSerializer.Deserialize<Tile>(obj.Value));
                    count++;
                }
                catch (Exception ex)
                {
                    Debug.WriteError($"Failed to parse tile {obj} from TileBook {name} with exception {ex.Message}");
                }
            }
        }
        Debug.WriteInfo($"Parsed/Loaded {count} tiles from TileBook {name}");
    }




}