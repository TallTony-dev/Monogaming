using Monogaming.Items;
using Monogaming.TextureThings;
using System.Text.Json.Serialization;


namespace Monogaming.Entities;

public class ItemEntity : InteractableEntity
{
    public Item Item { get; private set; }

    const float Damping = 6f;
    Vector2 randKindaUnitVec;

    [JsonConstructor]
    public ItemEntity(Hitbox DrawBox, Item Item, bool anim = false) :
    base(Item.Name, Item.ID, false, DrawBox, new TextureHandler(TextureHandler.TextureType.Default, Item.TextureID))
    {
        this.Item = Item;
        //this isn't actually a unit vector, but having both be random might lead to better look anyways
        if (anim)
        {
            randKindaUnitVec = new Vector2((float)Random.Shared.NextDouble() * 2 - 1, (float)Random.Shared.NextDouble() * 2 - 1);
        }
        else
        {
            randKindaUnitVec = Vector2.Zero;
        }
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

    public override void Update(GameTime gameTime)
    {
        DrawBox.ChangePos(randKindaUnitVec);
        randKindaUnitVec -= randKindaUnitVec * Damping * (float)gameTime.ElapsedGameTime.TotalSeconds;
        base.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch, Hitbox screen)
    {
        base.Draw(spriteBatch, screen);
        Item.Draw(spriteBatch, DrawBox, false);
    }


}