using Monogaming.Entities;
using Microsoft.Xna.Framework;
using Monogaming.Interfaces;
using Monogaming.TextureThings;
using System.Text.Json;
using System.Text.Json.Serialization;
using Monogaming.Items;


namespace Monogaming.Entities;

public class ItemEntity : InteractableEntity
{
    public Item Item { get; private set; }


    [JsonConstructor]
    public ItemEntity(Hitbox DrawBox, Item Item) :
    base(Item.Name, Item.ID, false, DrawBox, new TextureHandler(TextureHandler.TextureType.Default, Item.TextureID))
    {
        this.Item = Item;
    }

    public override Entity CopyToPos(Vector2 pos)
    {
        Hitbox newBox = new Hitbox(pos, DrawBox.Width, DrawBox.Height);
        return new ItemEntity(newBox, Item);
    }

    /// <summary>
    /// Player can call this to do something of some sort
    /// </summary>
    public override void Interact()
    {
        
    }

    public override void Draw(SpriteBatch spriteBatch, Hitbox screen)
    {
        base.Draw(spriteBatch, screen);
        Item.Draw(spriteBatch, DrawBox.AsRect());
    }


}