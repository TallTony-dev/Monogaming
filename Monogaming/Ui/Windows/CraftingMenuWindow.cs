using System;
using Monogaming.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Monogaming.Interfaces;
using Monogaming.TextureThings;
using Microsoft.Xna.Framework.Content;
using Monogaming.Ui;
using System.Text.Json.Serialization;
using Monogaming.Items;
using Apos.Input;
using System.Linq;

namespace Monogaming.Ui.Windows;

public class CraftingMenuWindow : WindowElement
{
    private Recipe prevSelected { get => (Recipe)scrollSon.SelectedItem; }
    private ScrollBoxElement scrollSon;
    private CraftingDisplayElement craftingDisplaySon;

    private List<string> workbenches;
    private Inventory inv;

    private const int TopOffset = 10;
    private const int ScrollBoxPadding = 3;
    private const float SpaceRatio = 0.5f;
    public CraftingMenuWindow(Inventory inv, List<string> workbenches, Hitbox drawBox) : base(AnchorPoint.TopLeft, drawBox, null)
    {
        this.inv = inv;
        this.workbenches = workbenches;
        scrollSon = new ScrollBoxElement("scrollBox", AnchorPoint.TopLeft, new TextureHandler(TextureHandler.TextureType.Default, "buttonRock.Unpressed"),
         new Hitbox(ScrollBoxPadding, 0, (int)(drawBox.Width * SpaceRatio) - ScrollBoxPadding * 2, drawBox.Height - ScrollBoxPadding)
         , RecipeBook.GetAvaliableRecipes(GameState.ActiveSave, inv, workbenches).ConvertAll(t => ((ImItem)t.recipe, !t.canCraft)));

        craftingDisplaySon = new CraftingDisplayElement("craftingDisplay", AnchorPoint.TopLeft, 
        new Hitbox(ScrollBoxPadding + (int)(drawBox.Width * SpaceRatio), 0, (int)(drawBox.Width / SpaceRatio) - ScrollBoxPadding * 2, drawBox.Height - ScrollBoxPadding), inv);

        Children.Add(scrollSon);
        Children.Add(craftingDisplaySon);
    }


    public void SetWorkbenchesAvailable(List<string> workbenches)
    {
        this.workbenches = workbenches;
    }

    public override void Update(GameTime gameTime)
    {
        if (inv.HasChanged)
        {
            scrollSon.SetItems(RecipeBook.GetAvaliableRecipes(GameState.ActiveSave, inv, workbenches).ConvertAll(t => ((ImItem)t.recipe, !t.canCraft)));
        }
        
        base.Update(gameTime);

        if (scrollSon.SelectedItem != null)
        {
            craftingDisplaySon.SetSelectedRecipe((Recipe)scrollSon.SelectedItem);
        }
        
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
    }
}