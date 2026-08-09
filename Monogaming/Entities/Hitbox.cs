using System;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using Monogaming.TextureThings;

namespace Monogaming.Entities;

public class Hitbox
{
    private const int TextDrawXOffset = 3;
    private Vector2 _pos;
    private int _width;
    private int _height;

    [JsonIgnore]
    public Vector2 Center
    {
        get
        {
            return new Vector2(Pos.X + Width / 2, Pos.Y + Height / 2);
        }
    }
    [JsonIgnore]
    public Vector2 Pos { get => _pos; set => _pos = value; }
    [JsonIgnore]
    public Vector2 Dims { get => new Vector2(Width, Height); }



    public int Width { get => _width; set => _width = value; }
    public int Height { get => _height; set => _height = value; }

    public int X { get => (int)Pos.X; }
    public int Y { get => (int)Pos.Y; }

    [JsonConstructor]
    public Hitbox(int X, int Y, int width, int height)
    {
        Pos = new Vector2(X,Y);
        Width = width;
        Height = height;
    }
    public Hitbox(Vector2 pos, int width, int height)
    {
        Pos = pos;
        Width = width;
        Height = height;
    }
    public Hitbox(int width, int height)
    {
        Width = width;
        Height = height;
    }
    public Hitbox(Hitbox hitbox)
    {
        Pos = hitbox.Pos;
        Width = hitbox.Width;
        Height = hitbox.Height;
    }

    public void CeilPos()
    {
        Pos = Vector2.Ceiling(Pos);
    }
    public virtual void SetPos(Vector2 pos)
    {
        Pos = pos;
    }
    public virtual void ChangePos(Vector2 deltaPos)
    {
        Pos += deltaPos;
    }
    public virtual void ChangeDims(Vector2 deltaDims)
    {
        Width += (int)deltaDims.X;
        Height += (int)deltaDims.Y;
    }
    public ref Vector2 GetPosRef()
    {
        return ref _pos;
    }
    public ref int GetWidthRef()
    {
        return ref _width;
    }
    public ref int GetHeightRef()
    {
        return ref _height;
    }

    public bool Intersects(Hitbox hitbox)
    {
        return Pos.X < hitbox.Pos.X + hitbox.Width && Pos.X + Width > hitbox.Pos.X && 
        Pos.Y < hitbox.Pos.Y + hitbox.Height && Pos.Y + Height > hitbox.Pos.Y;
    }
    public bool Intersects(Vector2 pos)
    {
        return Pos.X < pos.X && Pos.X + Width > pos.X && Pos.Y < pos.Y && Pos.Y + Height > pos.Y;
    }
    public bool Intersects(Point pos)
    {
        return Pos.X < pos.X && Pos.X + Width > pos.X && Pos.Y < pos.Y && Pos.Y + Height > pos.Y;
    }

    
    public Vector2 GetClosestPointTo(Hitbox hitbox)
    {
        Vector2 delta = Center - hitbox.Center;
        Vector2 val = hitbox.Pos;
        if (Math.Abs(delta.X) >= Math.Abs(delta.Y))
        {
            bool isOtherOnLeftSide = Math.Abs(Pos.X + Width - hitbox.Pos.X) >= Math.Abs(hitbox.Pos.X + hitbox.Width - Pos.X);
            val.X = Pos.X + (isOtherOnLeftSide ? -hitbox.Width : Width);
        }
        if (Math.Abs(delta.X) <= Math.Abs(delta.Y))
        {
            bool isOtherOnTopSide = Math.Abs(Pos.Y + Height - hitbox.Pos.Y) >= Math.Abs(hitbox.Pos.Y + hitbox.Height - Pos.Y);
            val.Y = Pos.Y + (isOtherOnTopSide ? -hitbox.Height : Height);
        }
        return val;
    }

    public Rectangle ToRect()
    {
        return new Rectangle((int)Pos.X, (int)Pos.Y, Width, Height);
    }

    public void DrawStringInto(SpriteBatch spriteBatch, string text, float size = 0.7f)
    {
        Vector2 paddedDims = Dims - new Vector2(TextDrawXOffset * 2, 0);
        var stringLen = Textures.MainFont.MeasureString(text);

        Vector2 scale = paddedDims / stringLen;
        // scale.Y = scale.X;
        scale = Vector2.Min(new Vector2(size), scale);

        Vector2 offsetPt = Pos + Dims / 2 + new Vector2(TextDrawXOffset, 0) - stringLen / 2 * scale;
        spriteBatch.DrawString(Textures.MainFont, text, offsetPt, Color.Indigo, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
    }
}