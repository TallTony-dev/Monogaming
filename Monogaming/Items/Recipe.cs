using System;
using System.Collections.Generic;
using System.Linq;

namespace Monogaming.Items;

public class Recipe
{
    //string in list is id or an expression for getting ids, int is count
    public List<(string, int)> Ingredients { get; private set; } 
    public List<(string, int)> Outputs { get; private set; }
    public float RecipeDifficulty { get; private set; } //could be time, could be nothing
    public string RecipeWorkbench { get; private set; }
    public string RecipeName { get; private set; }
    
    public Recipe(List<(string, int)> ingredients, List<(string, int)> outputs, 
    string recipeName, string workbench, float recipeDifficulty = 0)
    {
        Ingredients = ingredients;
        Outputs = outputs;
        RecipeDifficulty = recipeDifficulty;
        RecipeWorkbench = workbench;
        RecipeName = recipeName;
    }


    public bool CanCraft(Inventory inventory)
    {
        int count = 0;
        foreach ((string s, int cnt) in Ingredients)
        {
            if (inventory.GetCountOfItem(s) >= cnt)
            {
                count++;
            }
        }
        return count == Ingredients.Count();
    }
    public bool TryCraft(Inventory inventory)
    {
        if (!CanCraft(inventory))
        {
            return false;
        }

        foreach ((string s, int cnt) in Ingredients)
        {
            inventory.RemoveItem(s, cnt);
        }
        return true;
    }

}
