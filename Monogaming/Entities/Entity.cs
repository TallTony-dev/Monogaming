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

public class Entity : ImDrawable, ImUpdatable
{   
    [JsonIgnore]
    public bool IsVisible { get; private set; } = true;
    [JsonIgnore]
    public bool IsActive { get; private set; } = true; // determines if should be updated
    
    public Hitbox DrawBox { get; private set; }

    public string Name { get; private set; }
    public string ID { get; private set; } //used to identify entity in code
    public TextureHandler TextureHandler { get; private set; }


    public Entity(Vector2 startPos, int drawWidth, int drawHeight, TextureHandler handler)
    {
        DrawBox = new Hitbox(startPos, drawWidth, drawHeight);
        TextureHandler = handler;
    }

    [JsonConstructor]
    public Entity(string name, string id, Hitbox drawBox, TextureHandler textureHandler)
    {
        Name = name;
        ID = id;
        DrawBox = drawBox;
        TextureHandler = textureHandler;
    }

    public virtual Entity CopyToPos(Vector2 pos)
    {
        return new Entity(pos, DrawBox.Width, DrawBox.Height, TextureHandler.Copy());
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
    public virtual void SetDrawPos(Vector2 pos)
    {
        //should be private
        DrawBox.SetPos(pos);
    }

    public virtual void UpdateDrawPos(Vector2 deltaPos)
    {
        //should be private
        DrawBox.UpdatePos(deltaPos);
    }

    public virtual void Draw(SpriteBatch spriteBatch, Hitbox screen)
    {
        if (screen.Intersects(DrawBox))
        {
            TextureHandler.Draw(spriteBatch, DrawBox.AsRect());
        }
    }
}