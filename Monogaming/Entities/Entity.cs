using System;
using Monogaming.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monogaming.Interfaces;
using Monogaming.TextureThings;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Monogaming.Entities;

//just so derived classes can be parsed from json
[JsonPolymorphic(TypeDiscriminatorPropertyName = "EntityType")]

[JsonDerivedType(typeof(Entity), typeDiscriminator: "entity")]
[JsonDerivedType(typeof(InteractableEntity), typeDiscriminator: "interactableEntity")]
[JsonDerivedType(typeof(InventoryEntity), typeDiscriminator: "inventoryEntity")]
[JsonDerivedType(typeof(TileEntity), typeDiscriminator: "tileEntity")]
[JsonDerivedType(typeof(ItemEntity), typeDiscriminator: "itemEntity")]

public class Entity : ImDrawable, ImUpdatable, ImCollidable
{   
    [JsonIgnore]
    public bool IsVisible { get; private set; } = true;
    [JsonIgnore]
    public bool IsActive { get; private set; } = true; // determines if should be updated
    public bool IsSolid { get; private set; } = true;

    public Hitbox DrawBox { get; private set; }

    public string Name { get; private set; }
    public string ID { get; private set; } //used to identify entity in code
    public TextureHandler TextureHandler { get; private set; }


    [JsonConstructor]
    public Entity(string name, string id, bool IsSolid, Hitbox drawBox, TextureHandler textureHandler)
    {
        Name = name;
        ID = id;
        this.IsSolid = IsSolid;
        DrawBox = drawBox;
        TextureHandler = textureHandler;
    }

    public virtual Entity CopyToPos(Vector2 pos)
    {
        Hitbox newBox = new Hitbox(pos, DrawBox.Width, DrawBox.Height);
        return new Entity(Name, ID, IsSolid, newBox, TextureHandler.Copy());
    }

    public virtual void Update(GameTime gameTime)
    {
        TextureHandler.Update(gameTime);
    }

    //SHOULD BE PROTECTED
    public void SetTexture(TextureHandler.TextureType type)
    {
        TextureHandler.SetTexture(type);
    }

    //TEMPTEMPTEMP
    public void SetPos(Vector2 pos)
    {
        //should be private
        DrawBox.SetPos(pos);
    }
    public void AdjustPos(Vector2 deltaPos)
    {
        DrawBox.ChangePos(deltaPos);
    }

    public virtual void Draw(SpriteBatch spriteBatch, Hitbox screen)
    {
        if (screen.Intersects(DrawBox))
        {
            TextureHandler.Draw(spriteBatch, DrawBox.ToRect());
        }
    }
    public virtual void DrawAtPos(SpriteBatch spriteBatch, Vector2 pos, Color color)
    {
        TextureHandler.Draw(spriteBatch, DrawBox.ToRect() with { X = (int)pos.X, Y = (int)pos.Y }, color);
    }
}