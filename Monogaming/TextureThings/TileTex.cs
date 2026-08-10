using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monogaming.Interfaces;

namespace Monogaming.TextureThings;

public class TileTex : ImTexture
{
    [JsonIgnore]
    public bool IsAnimated { get; } = false;
    [JsonIgnore]
    private TileTexType currentType = TileTexType.AllSides;

    [JsonPropertyName("frame")]
    public List<Rectangle> SourceRects { get; private set; }
    public Texture2D Texture { get; private set; } //ref to the one made by textures.cs so its fine for efficiency okkk :((
    

    public TileTex(Texture2D tex, List<Rectangle> sourceRects)
    { 
        SourceRects = sourceRects;
        Texture = tex;
    }

    public void SetTileTex(TileTexType type)
    {
        currentType = type;
    }

    public ImTexture Copy()
    {
        return new TileTex(Texture, SourceRects);
    }

    public void Draw(SpriteBatch batch, Vector2 pos)
    {
        batch.Draw(Texture, pos, SourceRects[(int)currentType], Color.White);
    }
    public void Draw(SpriteBatch batch, Rectangle dest)
    {
        batch.Draw(Texture, dest, SourceRects[(int)currentType], Color.White);
    }
    public void Draw(SpriteBatch batch, Rectangle dest, Rectangle mask)
    {
        throw new NotImplementedException();
        //lol
    }
    public void Draw(SpriteBatch batch, Vector2 pos, Color color)
    {
        batch.Draw(Texture, pos, SourceRects[(int)currentType], color);
    }
    public void Draw(SpriteBatch batch, Rectangle dest, Color color)
    {
        batch.Draw(Texture, dest, SourceRects[(int)currentType], color);
    }
    public enum TileTexType 
    { AllSides = 0, LeftTopBottom, TopBottom, TopRightBottom, ExBottomRight, ExBottomLeft,
    LeftRightTop, LeftTop, Top, TopRight, ExTopRight, ExTopLeft, 
    LeftRight, Left, None, Right, _, _2, 
    LeftRightBottom, LeftBottom, Bottom, BottomRight, _3, _4 }
}