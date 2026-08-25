using System.IO;
using System.Linq;
namespace Monogaming.Items;

public static class RecipeBook
{
    //strings are recipe names
    private static Dictionary<string, Recipe> recipeList = new();


    public static List<(Recipe recipe, bool canCraft)> GetAvaliableRecipes(Save save, Inventory inv, List<string> workbenches)
    {
        List<Recipe> list = recipeList.Values.ToList();
        List<(Recipe recipe, bool canCraft)> retval = new();
        foreach (Recipe r in list)
        {
            if (r.Ingredients.Any(t => save.IsItemFound(t.ID)))
            {
                retval.Add((r, r.CanCraft(inv, workbenches)));
            }
        }
        return retval;
    }

    public static IEnumerable<Recipe> GetRecipesForWorkbench(string workbenchName)
    {
        return recipeList.Values.Where(t => t.RecipeWorkbench == workbenchName);
    }

    public static void LoadFromFile(string fileName)
    {
        try
        {
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

                        List<Item> ingredients = new();
                        foreach (var property in recipe.GetProperty("Ingredients").EnumerateObject())
                        {
                            string itemID = property.Name;
                            if (!ItemDict.ContainsItem(itemID))
                            {
                                Debug.WriteWarn($"Cannot find any loaded item with id {itemID}");
                            }
                            int itemCount = property.Value.GetInt32();
                            ingredients.Add(ItemDict.GetNewItem(itemID, itemCount));
                        }

                        List<Item> outputs = new();
                        foreach (var property in recipe.GetProperty("Outputs").EnumerateObject())
                        {
                            string itemID = property.Name;
                            if (!ItemDict.ContainsItem(itemID))
                            {
                                Debug.WriteWarn($"Cannot find any loaded item with id {itemID}");
                            }
                            int itemCount = property.Value.GetInt32();
                            outputs.Add(ItemDict.GetNewItem(itemID, itemCount));
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
