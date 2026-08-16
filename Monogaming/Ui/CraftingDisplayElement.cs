using System;
using Monogaming.Entities;
using Microsoft.Xna.Framework;
using Monogaming.Items;
using Microsoft.Xna.Framework.Input;
using Monogaming.Interfaces;
using Monogaming.TextureThings;
using Microsoft.Xna.Framework.Content;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace Monogaming.Ui;

public class CraftingDisplayElement : UiElement
{
    private const int ItemPadding = 3;
    private const int MinItemWidth = 30;
    private const int MaxItemWidth = 60;
    private const int PrefItemsPerInputRow = 8;
    private const int PrefItemsPerOutputRow = 3;
    private const int InputYOffset = 90;
    private const int OutputYOffset = 10;

    private ButtonElement craftOneElement;
    
    // public int ToCraft { get; set; } = 0;

    private Inventory inventoryRef;
    private Recipe selectedRecipe;

    private List<string> workbenches;

    private List<(Hitbox box, Item item)> InputItemBoxes = new();
    private List<(Hitbox box, Item item)> OutputItemBoxes = new();

    public CraftingDisplayElement(string ID, AnchorPoint Anchor, Hitbox drawBox, Inventory inventory, List<string> workbenches) : base(ID, Anchor, drawBox, null)
    {
        this.workbenches = workbenches;
        UpdateHitboxes();
        inventoryRef = inventory;
        Children.Add(craftOneElement = new ButtonElement("craftOneElement", AnchorPoint.BottomLeft, new Hitbox(5, -5, 64, 32),
        new TextureHandler(TextureHandler.TextureType.Default, "buttonRock.Unpressed"), null, "Craft 1x", null));
    }

    public void SetSelectedRecipe(Recipe recipe)
    {
        selectedRecipe = recipe;
    }

    private static int GetItemWidth(int prefItemsPerRow, Hitbox ScaledDrawBox)
    {
        return Math.Clamp(ScaledDrawBox.Width / prefItemsPerRow + ItemPadding * prefItemsPerRow, MinItemWidth, MaxItemWidth);
    }



    private static List<(Hitbox, Item)> GetHitboxesForItems(Hitbox ScaledDrawBox, Recipe recipe,
    int prefItemsPerRow, int yOffset, bool getIngredients)
    {
        List<(Hitbox, Item)> values = new();

        int itemWidth = GetItemWidth(prefItemsPerRow, ScaledDrawBox);
        int itemsPerRow = ScaledDrawBox.Width / itemWidth;

        int xPerItem = itemWidth + ItemPadding;
        int itemCount = getIngredients ? recipe.Ingredients.Count : recipe.Outputs.Count;
        int rowCount = itemCount / itemsPerRow + 1;

        for (int i = 0; i < rowCount; i++) 
        {
            for (int j = 0; j < itemsPerRow; j++)
            {
                int ind = j + i * itemsPerRow;
                if (ind >= itemCount)
                {
                    break;
                }
                Hitbox hitbox = new Hitbox(ScaledDrawBox.Pos + new Vector2(ItemPadding + xPerItem * j, i * xPerItem + ItemPadding + yOffset),
                itemWidth, itemWidth);
                values.Add((hitbox, getIngredients ? recipe.Ingredients[ind] : recipe.Outputs[ind]));
            }
        }
        return values;
    }   

    private void UpdateHitboxes()
    {
        if (selectedRecipe != null) {
            InputItemBoxes.Clear();
            OutputItemBoxes.Clear();

            InputItemBoxes = GetHitboxesForItems(ScaledDrawBox, selectedRecipe, PrefItemsPerInputRow, InputYOffset, true);
            OutputItemBoxes = GetHitboxesForItems(ScaledDrawBox, selectedRecipe, PrefItemsPerOutputRow, OutputYOffset, false);
        }
    }

    public override void UpdatePos(Rectangle window)
    {
        base.UpdatePos(window);
        UpdateHitboxes();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        // ToCraft = 0;
        //TODO: Check if children are clicked and set flags for parent off of that
        if (selectedRecipe != null) {
            if (craftOneElement.WasClicked)
            {
                // ToCraft = 1;
                selectedRecipe.TryCraft(inventoryRef, workbenches);
            }
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (selectedRecipe != null)
        {
            foreach (var i in InputItemBoxes)
            {
                int invCnt = inventoryRef.GetCountOfItem(i.item.ID);
                i.item.Draw(spriteBatch, i.box, invCnt >= i.item.Count ? Color.White : Color.DimGray, true);
                Hitbox otherTextBox = new Hitbox(i.box.Pos + new Vector2(i.box.Width - Item.TextOffset, i.box.Height - Item.TextHeightOffset * 2),
                Item.TextWidth, Item.TextHeight);
                otherTextBox.DrawStringInto(spriteBatch, "(" + invCnt.ToString() + ")", Color.White);
                
            }

            foreach (var i in OutputItemBoxes)
            {
                i.item.Draw(spriteBatch, i.box, true);
            }
        }

        base.Draw(spriteBatch);
    }
}