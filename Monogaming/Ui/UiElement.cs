using System;
using Monogaming.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monogaming.Interfaces;
using Monogaming.TextureThings;
using System.Runtime.Intrinsics.X86;
using Microsoft.Xna.Framework.Content;

namespace Monogaming.Ui;

public class UiElement : ImDrawable, ImUpdatable
{
    public bool IsVisible { get; private set; } = true;
    public bool IsActive { get; private set; } = true;
    
    public Hitbox hitbox { get; protected set; }
    string textureId;


    public UiElement(Vector2 startPos, int width, int height, string textureId)
    {
        hitbox = new Hitbox(startPos, width, height);
        this.textureId = textureId;
    }

    public virtual void Update(GameTime gameTime)
    {
        
    }

    public virtual void Draw(SpriteBatch spriteBatch, Hitbox screen)
    {
        if (!String.IsNullOrEmpty(textureId))
        {
            spriteBatch.Draw(Textures.Get2DTex(textureId), 
            hitbox.AsRect(), Color.White);
        }
    }

}