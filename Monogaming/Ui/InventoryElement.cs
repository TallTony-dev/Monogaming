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

public class InventoryElement : UiElement
{
    [JsonIgnore]
    Inventory boundInventory;

    [JsonConstructor]
    public InventoryElement(string ID, Hitbox drawBox, List<UiElement> Children) : base(ID, drawBox, Children)
    {
        
    }

    /// <summary>
    /// Bind the passed inventory to be displayed by the inventory element
    /// </summary>
    public void BindInventory(Inventory inventory)
    {
        boundInventory = inventory;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
    }
}