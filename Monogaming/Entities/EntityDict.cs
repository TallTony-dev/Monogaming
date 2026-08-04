using System;
using Monogaming.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monogaming.Interfaces;
using Monogaming.TextureThings;
using System.Text.Json;
using System.IO;
using System.Collections.Generic;

namespace Monogaming.Entities;

public static class EntityDict
{
    //string is ID
    private static Dictionary<string, Entity> entityDict = new();

    
    public static void LoadEntities(string fileName)
    {
        try
        {
            string path = "./GameData/" + fileName + ".json";

            Stream textStream = File.OpenRead(path);
            JsonDocument document = JsonDocument.Parse(textStream);
            JsonElement root = document.RootElement;

            string name = root.GetProperty("EntityBookName").GetString();

            int loadCount = 0;
            foreach (JsonProperty prop in root.EnumerateObject())
            {
                if (!(prop.Value.ValueKind == JsonValueKind.Object))
                {
                    continue;
                }
                try
                {
                    Entity entity = JsonSerializer.Deserialize<Entity>(prop.Value);

                    entityDict.Add(prop.Value.GetProperty("ID").GetString(), entity);
                    loadCount++;
                }
                catch (Exception ex)
                {
                    Debug.WriteError($"Failed to load entity {prop.Name} in {name} with error {ex.Message}");
                }

            }



            Debug.WriteInfo($"Loaded {loadCount} entity descriptions from EntityDict {name}");
        }
        catch (Exception ex)
        {
            Debug.WriteError($"Failed to load entities from {fileName} with error {ex.Message}");
        }
    }

    public static Entity CreateEntity(string id, Vector2 pos)
    {
        if (id != null && entityDict.ContainsKey(id))
        {
            return entityDict[id].CopyToPos(pos);
        }
        return null;
    }

    public static bool Contains(string id)
    {
        return id != null && entityDict.ContainsKey(id);
    }


}