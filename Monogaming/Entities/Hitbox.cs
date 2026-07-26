using System;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;

namespace Monogaming.Entities;

public class Hitbox
{

    public Vector2 Pos { get; protected set; }
    
    [JsonIgnore]
    public Vector2 Center
    {
        get
        {
            return new Vector2(Pos.X + Width / 2, Pos.Y + Height / 2);
        }
    }
    public int Width { get; protected set; }
    public int Height { get; protected set; }
    public Hitbox(Vector2 startPos, int width, int height)
    {
        Pos = startPos;
        Width = width;
        Height = height;
    }
    [JsonConstructor]
    public Hitbox(int width, int height)
    {
        Width = width;
        Height = height;
        Pos = Vector2.Zero;
    }

    public virtual void SetPos(Vector2 pos)
    {
        Pos = pos;
    }
    public virtual void UpdatePos(Vector2 deltaPos)
    {
        Pos += deltaPos;
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

}