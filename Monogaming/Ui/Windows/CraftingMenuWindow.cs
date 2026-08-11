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

    private Recipe prevSelected = null;
    private ScrollBoxElement sonion;

    private Inventory inv;

    private const int TopOffset = 10;
    private const int ScrollBoxPadding = 3;

    public CraftingMenuWindow(Inventory inv, Hitbox drawBox) : base(AnchorPoint.TopLeft, drawBox, null)
    {
        this.inv = inv;
        sonion = new ScrollBoxElement("scrollBox", AnchorPoint.TopLeft, new TextureHandler(TextureHandler.TextureType.Default, "buttonRock.Unpressed"),
         new Hitbox(ScrollBoxPadding, 0, drawBox.Width - ScrollBoxPadding * 2, drawBox.Height - ScrollBoxPadding)
         , RecipeBook.GetAvaliableRecipes(GameState.ActiveSave, inv).ConvertAll(t => ((ImItem)t.recipe, !t.canCraft)));

        Children.Add(sonion);
    }



    public override void Update(GameTime gameTime)
    {
        if (inv.HasChanged)
        {
            sonion.SetItems(RecipeBook.GetAvaliableRecipes(GameState.ActiveSave, inv).ConvertAll(t => ((ImItem)t.recipe, !t.canCraft)));
        }
        
        base.Update(gameTime);

        
        
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
    }
}