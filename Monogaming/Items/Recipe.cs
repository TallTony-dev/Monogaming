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


    public bool CanCraft(Inventory inventory, List<string> workbenches)
    {
        int count = 0;
        foreach (Item i in Ingredients)
        {
            if (inventory.GetCountOfItem(i.ID) >= i.Count)
            {
                count++;
            }
        }
        return count == Ingredients.Count && 
        (RecipeWorkbench == "any" || workbenches.Any(t => t == RecipeWorkbench))
        && inventory.CanStoreItems(Outputs);
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
        return count == Ingredients.Count;
    }

    public bool TryCraft(Inventory inventory, List<string> workbenches)
    {
        return TryCraft(inventory, inventory, workbenches);
    }

    public bool TryCraft(Inventory inputInv, Inventory outputInv, List<string> workbenches)
    {
        if (!CanCraft(inputInv, workbenches))
        {
            return false;
        }

        foreach (Item i in Ingredients)
        {
            inputInv.RemoveItem(i.ID, i.Count);
        }
        foreach (Item i in Outputs)
        {
            outputInv.AddItem(ItemDict.GetNewItem(i.ID, i.Count));
        }
        inputInv.ClearEmpties();
        return true;
    }

    public void Draw(SpriteBatch spriteBatch, Hitbox box, bool withBg)
    {
        Draw(spriteBatch, box, Color.White, withBg);
    }
    public void Draw(SpriteBatch spriteBatch, Hitbox box, Color color, bool withBg)
    {
        if (Outputs.Count > 0)
        {
            Outputs[0].Draw(spriteBatch, box, color, withBg);
        }
    }
}
