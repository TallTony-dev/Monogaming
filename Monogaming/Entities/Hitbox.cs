using System;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using Monogaming.TextureThings;

namespace Monogaming.Entities;

public class Hitbox
{
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

    /// <summary>
    /// returns the vector out for the passed in hitbox
    /// </summary>
    public Vector2 GetCollision(Hitbox hitbox)
    {
        Vector2 otherPos = hitbox.Pos;
        Vector2 otherMax = otherPos + new Vector2(hitbox.Width, hitbox.Height);
        Vector2 max = Pos + new Vector2(Width, Height);

        throw new NotImplementedException();


    }

    public Rectangle AsRect()
    {
        return new Rectangle((int)Pos.X, (int)Pos.Y, Width, Height);
    }

    public void DrawStringInto(SpriteBatch spriteBatch, string text)
    {
        Vector2 scale = Dims / Textures.MainFont.MeasureString(text);
        Vector2 offsetPt = (Pos + Dims / 2) - Textures.MainFont.MeasureString(text) / 2 * scale;
        spriteBatch.DrawString(Textures.MainFont, text, offsetPt, Color.Gray, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
    }
}