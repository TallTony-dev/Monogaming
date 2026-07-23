using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;
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

    public Vector2 GetCollision(Hitbox hitbox)
    {
        throw new NotImplementedException();
    }

    public Rectangle AsRect()
    {
        return new Rectangle((int)Pos.X, (int)Pos.Y, Width, Height);
    }

}