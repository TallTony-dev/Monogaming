using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monogaming.Interfaces;

namespace Monogaming.TextureThings;

public class AtlasTex : ImTexture
{
    public bool IsAnimated { get; } = false;
    public Rectangle SourceRect { get; private set; }
    public Texture2D Texture { get; private set; }

    public AtlasTex(Texture2D tex, Rectangle sourceRect)
    { 
        SourceRect = sourceRect;
        Texture = tex;
    }

    public AtlasTex Copy()
    {
        return new AtlasTex(Texture, SourceRect);
    }

    public void Draw(SpriteBatch batch, Vector2 pos)
    {
        batch.Draw(Texture, pos, SourceRect, Color.White);
    }
    public void Draw(SpriteBatch batch, Rectangle dest)
    {
        batch.Draw(Texture, dest, SourceRect, Color.White);
    }

}