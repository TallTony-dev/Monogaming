using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monogaming.Interfaces;

namespace Monogaming.TextureThings;

public class AtlasTex : ImTexture
{
    [JsonIgnore]
    public bool IsAnimated { get; } = false;

    [JsonPropertyName("frame")]
    public Rectangle SourceRect { get; private set; }
    public Texture2D Texture { get; private set; } //ref to the one made by textures.cs so its fine for efficiency okkk :((
    

    public AtlasTex(Texture2D tex, Rectangle sourceRect)
    { 
        SourceRect = sourceRect;
        Texture = tex;
    }

    public ImTexture Copy()
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
    public void Draw(SpriteBatch batch, Rectangle dest, Rectangle mask)
    {
        Rectangle newSrc = SourceRect;
        Rectangle drawSpot = Rectangle.Intersect(dest, mask);
        Vector2 scale = new Vector2((float)mask.Width / dest.Width, (float)mask.Height / dest.Height);

        newSrc.Width = (int)(newSrc.Width * scale.X);
        newSrc.Height = (int)(newSrc.Height * scale.Y); 
        newSrc.X = (mask.X - dest.X) * SourceRect.Width / dest.Width;
        newSrc.Y = (mask.Y - dest.Y) * SourceRect.Height / dest.Height;

        batch.Draw(Texture, drawSpot, newSrc, Color.White);
    }
    public void Draw(SpriteBatch batch, Vector2 pos, Color color)
    {
        // Rectangle rect = new Rectangle((int)pos.X, (int)pos.Y, Texture.Width, Texture.Height);
        // batch.Draw(Texture, rect, SourceRect, color, 0, Vector2.Zero, SpriteEffects.None, pos.Y);
        batch.Draw(Texture, pos, SourceRect, color);
    }
    public void Draw(SpriteBatch batch, Rectangle dest, Color color)
    {
        // batch.Draw(Texture, dest, SourceRect, color, 0, Vector2.Zero, SpriteEffects.None, dest.Y);
        batch.Draw(Texture, dest, SourceRect, color);
    }

}