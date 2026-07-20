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
    ImTexture texture;


    public UiElement(Vector2 startPos, int width, int height, ImTexture texture)
    {
        hitbox = new Hitbox(startPos, width, height);
        this.texture = texture;
    }

    public virtual void Update(GameTime gameTime)
    {
        if (texture.IsAnimated)
        {
            ((TextureAnimation)texture).Update(gameTime);
        }
    }

    public virtual void Draw(SpriteBatch spriteBatch, Hitbox screen)
    {
        texture.Draw(spriteBatch, hitbox.AsRect());
    }

}