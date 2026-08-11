using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
namespace Monogaming.Items;

public static class RecipeBook
{
    //strings are recipe names
    private static Dictionary<string, Recipe> recipeList = new();


    public static List<(bool canCraft, Recipe recipe)> GetAvaliableRecipes(Save save, Inventory inv)
    {
        List<Recipe> list = recipeList.Values.ToList();
        List<(bool canCraft, Recipe recipe)> retval = new();
        foreach (Recipe r in list)
        {
            if (r.Ingredients.Any(t => save.IsItemFound(t.Item1)))
            {
                retval.Add((r.CanCraft(inv), r));
            }
        }
        return retval;
    }


    public static void LoadFromFile(string fileName)
    {
        try {
            string path = "./GameData/" + fileName + ".json";

            Stream textStream = File.OpenRead(path);
            JsonDocument document = JsonDocument.Parse(textStream);
            JsonElement root = document.RootElement;
            string title;
            if ((title = root.GetProperty("Title").GetString()) != "recipes")
            {
                throw new JsonException($"Title of Json at path {path} was unexpected value: {title}.");
            }
            string name = root.GetProperty("RecipeBookName").GetString();

            int count = 0;

            foreach (JsonProperty obj in root.EnumerateObject())
            {
                if (obj.Value.ValueKind == JsonValueKind.Object)
                {
                    try
                    {
                        JsonElement recipe = obj.Value;
                        string workbench = recipe.GetProperty("Workbench").GetString();
                        float difficulty = recipe.GetProperty("Difficulty").GetSingle();

                        List<(string, int)> ingredients = new();
                        foreach (var property in recipe.GetProperty("Ingredients").EnumerateObject())
                        {
                            string itemID = property.Name;
                            if (!ItemDict.ContainsItem(itemID))
                            {
                                Debug.WriteWarn($"Cannot find any loaded item with id {itemID}");
                            }
                            int itemCount = property.Value.GetInt32();
                            ingredients.Add((itemID, itemCount));
                        }

                        List<(string, int)> outputs = new();
                        foreach (var property in recipe.GetProperty("Outputs").EnumerateObject())
                        {
                            string itemID = property.Name;
                            if (!ItemDict.ContainsItem(itemID))
                            {
                                Debug.WriteWarn($"Cannot find any loaded item with id {itemID}");
                            }
                            int itemCount = property.Value.GetInt32();
                            ingredients.Add((itemID, itemCount));
                        }

                        recipeList.Add(obj.Name, new Recipe(ingredients, outputs, obj.Name, workbench, difficulty));
                        count++;
                    }
                    catch
                    {
                        Debug.WriteError($"Failed to parse recipe {obj} from RecipeBook {name}");
                    }
                }
            }
            Debug.WriteInfo($"Parsed/Loaded {count} recipes from RecipeBook {name}");
        }
        catch (Exception ex)
        {
            Debug.WriteError($"Failed to load RecipeBook with name {fileName} with exception {ex.Message}");
        }
    }
}
