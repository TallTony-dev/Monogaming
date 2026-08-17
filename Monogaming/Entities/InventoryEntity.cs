using Monogaming.Entities;
using Microsoft.Xna.Framework;
using Monogaming.Interfaces;
using Monogaming.TextureThings;
using System.Text.Json;
using System.Text.Json.Serialization;
using Monogaming.Items;


namespace Monogaming.Entities;

public class InventoryEntity : InteractableEntity
{
    public Inventory Inventory { get; private set; }

    [JsonConstructor]
    public InventoryEntity(string name, string id, bool IsSolid, Hitbox drawBox, Inventory Inventory, TextureHandler textureHandler) :
    base(name, id, IsSolid, drawBox, textureHandler)
    {
        this.Inventory = Inventory;
    }

    public override Entity CopyToPos(Vector2 pos)
    {
        Hitbox newBox = new Hitbox(pos, DrawBox.Width, DrawBox.Height);
        return new InventoryEntity(Name, ID, IsSolid, newBox, Inventory, TextureHandler.Copy());
    }
    
    public bool TryAddTo(Item item)
    {
        return Inventory.AddItem(item);
    }
    
    /// <summary>
    /// Tries to get the last item out of inv and removes it, or else null
    /// </summary>
    /// <returns></returns>
    public Item TryTakeFrom()
    {
        Item i = Inventory.Items.Count > 0 ? Inventory.TakeItemFrom(Inventory.Items.Count - 1) : null;
        return i;
    }



    /// <summary>
    /// Player can call this to do something of some sort
    /// </summary>
    public override void Interact()
    {
        
    }

    public override void Update(GameTime gameTime)
    {
        Inventory.Update(gameTime);
        base.Update(gameTime);
    }

}