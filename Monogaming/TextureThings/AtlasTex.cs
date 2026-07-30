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

}