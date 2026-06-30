using System.Reflection.Metadata.Ecma335;
using Microsoft.Xna.Framework;

namespace Monogaming.Entities;

public class Hitbox
{
    public Hitbox(Vector2 startPos, int width, int height)
    {
        Pos = startPos;
        Width = width;
        Height = height;
    }
    public Vector2 Pos { get; protected set; }
    public Vector2 Center
    {
        get
        {
            return new Vector2(Pos.X + Width / 2, Pos.Y + Height / 2);
        }
    }
    public int Width { get; protected set; }
    public int Height { get; protected set; }

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

    public Rectangle AsRect()
    {
        return new Rectangle((int)Pos.X, (int)Pos.Y, Width, Height);
    }

}