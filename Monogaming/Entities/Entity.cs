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
    public bool IsVisible { get; private set; }
    public bool IsActive { get; private set; } // determines if should be updated
    
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

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        if (!String.IsNullOrEmpty(textureId))
        {
            spriteBatch.Draw(Textures.Get2DTex(textureId), 
            hitbox.AsRect(), Color.White);
        }
    }
}