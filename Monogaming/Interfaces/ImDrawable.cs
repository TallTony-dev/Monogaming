using System;
using Microsoft.Xna.Framework.Graphics;
using Monogaming.Entities;

namespace Monogaming.Interfaces;

public interface ImDrawable
{
    bool IsVisible { get; } //controls if should be drawn
    void Draw(SpriteBatch spriteBatch, Hitbox screen);
}
