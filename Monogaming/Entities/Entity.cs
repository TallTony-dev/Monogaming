using System;
using Monogaming.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monogaming.Interfaces;
using Monogaming.TextureThings;
using System.Text.Json;

namespace Monogaming.Entities;

public class Entity : ImDrawable, ImUpdatable
{
    public bool IsVisible { get; private set; } = true;
    public bool IsActive { get; private set; } = true; // determines if should be updated
    
    public Hitbox DrawBox;

    public string Name { get; private set; }
    public string ID { get; private set; } //used to identify entity in code
    private TextureHandler textureHandler;

    public Entity(Vector2 startPos, int drawWidth, int drawHeight, TextureHandler handler)
    {
        DrawBox = new Hitbox(startPos, drawWidth, drawHeight);
        textureHandler = handler;
    }

    public Entity(JsonProperty entityProp)
    {
        JsonElement entityObj = entityProp.Value;
        textureHandler = new TextureHandler(entityObj.GetProperty("Textures"));

        ID = entityObj.GetProperty("ID").GetString();
        Name = entityProp.Name;

        JsonElement drawBoxElement = entityObj.GetProperty("DrawBox");
        DrawBox = new Hitbox(Vector2.Zero, drawBoxElement.GetProperty("Width").GetInt32(),
        drawBoxElement.GetProperty("Height").GetInt32());

    }

    public virtual Entity CopyToPos(Vector2 pos)
    {
        return new Entity(pos, DrawBox.Width, DrawBox.Height, textureHandler.Copy());
    }

    public virtual void Update(GameTime gameTime)
    {
        textureHandler.Update(gameTime);
    }


    //SHOULD BE PROTECTED
    public void SetTexture(TextureHandler.TextureType type)
    {
        textureHandler.SetTexture(type);
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
            textureHandler.Draw(spriteBatch, DrawBox.AsRect());
        }
    }
}