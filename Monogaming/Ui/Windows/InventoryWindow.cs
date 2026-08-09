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

namespace Monogaming.Ui.Windows;

public class InventoryWindow : WindowElement
{
    private Inventory boundInventory;
    private Item playerHeldItem;

    ScrollBoxElement sonion;

    //can be used to facilitate other things like a sort button or smth
    private const int TopOffset = 20;

    public InventoryWindow(Inventory inventory, Hitbox drawBox) : base(AnchorPoint.TopLeft, drawBox, null)
    {
        boundInventory = inventory;

        sonion = new ScrollBoxElement("scrollBox", AnchorPoint.TopLeft, new TextureHandler(TextureHandler.TextureType.Default, "buttonRock.Unpressed"),
         new Hitbox(0, TopOffset, drawBox.Width, drawBox.Height - TopOffset), inventory.Items.ConvertAll(t => (ImDrawable)t));

        Children.Add(sonion);
    }



    public override void Update(GameTime gameTime)
    {
        if (boundInventory.HasChanged)
        {
            sonion.SetItems(boundInventory.Items.ConvertAll(t => (ImDrawable)t));
        }

        base.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
    }
}