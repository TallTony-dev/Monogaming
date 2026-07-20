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


    private ImTexture texture;

    public Entity(Vector2 startPos, int width, int height, ImTexture texture)
    {
        hitbox = new Hitbox(startPos, width, height);
        this.texture = texture;
    }

    public virtual void Update(GameTime gameTime)
    {
        if (texture.IsAnimated)
        {
            ((AnimatedTexture)texture).Update(gameTime);
        }
    }

    protected void SetTexture(ImTexture tex)
    {
        texture = tex;
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
            texture.Draw(spriteBatch, hitbox.AsRect());
        }
    }
}