using System.IO;

namespace Monogaming.Entities;

public static class EntityDict
{
    //string is ID
    private static Dictionary<string, Entity> entityDict = new();


    public static void LoadEntities(string fileName)
    {
        try
        {
            string path = "./Content/GameData/" + fileName + ".json";

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


    /// <summary>
    /// Gets the main copy of the entity by reference to save cost of checking values
    /// WARNING: DO NOT MODIFY RETURNED VAL
    /// </summary>
    public static Entity GetReadOnlyEntity(string id)
    {
        if (id != null && entityDict.ContainsKey(id))
        {
            return entityDict[id];
        }
        return null;
    }

    public static bool Contains(string id)
    {
        return id != null && entityDict.ContainsKey(id);
    }


}