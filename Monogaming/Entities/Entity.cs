using System;
using Monogaming.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monogaming.Interfaces;
using Monogaming.TextureThings;

namespace Monogaming.Entities;

public class Entity : ImDrawable, ImUpdatable
{
    public bool IsVisible { get; private set; } = true;
    public bool IsActive { get; private set; } = true; // determines if should be updated
    
    public Hitbox hitbox;


    protected string textureId = "";

    public Entity(Vector2 startPos, int width, int height, string textureId)
    {
        hitbox = new Hitbox(startPos, width, height);
        this.textureId = textureId;
    }

    public virtual void Update(GameTime gameTime)
    {
        
    }
    //TEMPTEMPTEMP
    public virtual void SetPos(Vector2 pos)
    {
        //should be private
        hitbox.SetPos(pos);
    }

    public virtual void UpdatePos(Vector2 deltaPos)
    {
        //should be private
        hitbox.UpdatePos(deltaPos);
    }

    public virtual void Draw(SpriteBatch spriteBatch, Hitbox screen)
    {
        if (screen.Intersects(hitbox))
        {
            if (!String.IsNullOrEmpty(textureId))
            {
                spriteBatch.Draw(Textures.Get2DTex(textureId), 
                hitbox.AsRect(), Color.White);
            }
        }
    }
}