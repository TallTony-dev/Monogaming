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
    private Item SelectedItem { get => (Item)sonion.SelectedItem; }

    ScrollBoxElement sonion;

    //can be used to facilitate other things like a sort button or smth
    private const int TopOffset = 10;
    private const int ScrollBoxPadding = 3;

    public InventoryWindow(Inventory inventory, Hitbox drawBox) : base(AnchorPoint.TopLeft, drawBox, null)
    {
        boundInventory = inventory;

        sonion = new ScrollBoxElement("scrollBox", AnchorPoint.TopLeft, new TextureHandler(TextureHandler.TextureType.Default, "buttonRock.Unpressed"),
         new Hitbox(ScrollBoxPadding, 0, drawBox.Width - ScrollBoxPadding * 2, drawBox.Height - ScrollBoxPadding), inventory.Items.ConvertAll(t => (ImItem)t));

        Children.Add(sonion);
    }



    public override void Update(GameTime gameTime)
    {
        if (boundInventory.HasChanged)
        {
            sonion.SetItems(boundInventory.Items.ConvertAll(t => (ImItem)t));
        }

        base.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
    }
}