using System;
using System.Collections.Generic;
using System.Linq;
using Monogaming.Interfaces;
using Monogaming.Entities;

namespace Monogaming.Items;

public class Recipe : ImItem
{
    //string in list is id or an expression for getting ids, int is count
    public List<Item> Ingredients { get; private set; } 
    public List<Item> Outputs { get; private set; }
    public float RecipeDifficulty { get; private set; } //could be time, could be nothing
    public string RecipeWorkbench { get; private set; }
    public string RecipeName { get; private set; }
    
    public Recipe(List<Item> ingredients, List<Item> outputs, 
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
        foreach (Item i in Ingredients)
        {
            if (inventory.GetCountOfItem(i.ID) >= i.Count)
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

        foreach (Item i in Ingredients)
        {
            inventory.RemoveItem(i.ID, i.Count);
        }
        return true;
    }

    public void Draw(SpriteBatch spriteBatch, Hitbox box)
    {
        if (Outputs.Count > 0)
        {
            Outputs[0].Draw(spriteBatch, box);
        }
    }
    public void Draw(SpriteBatch spriteBatch, Hitbox box, Color color)
    {
        if (Outputs.Count > 0)
        {
            Outputs[0].Draw(spriteBatch, box, color);
        }
    }
}
