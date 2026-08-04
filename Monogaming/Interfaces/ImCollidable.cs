using System;
using Microsoft.Xna.Framework.Graphics;
using Monogaming.Entities;

namespace Monogaming.Interfaces;

public interface ImCollidable
{
    bool IsSolid { get; }
    public Hitbox DrawBox { get; }
}