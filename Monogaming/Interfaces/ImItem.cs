using System;
using Microsoft.Xna.Framework.Graphics;
using Monogaming.Entities;

namespace Monogaming.Interfaces;

public interface ImItem
{
    bool IsVisible { get; } //controls if should be drawn
    void Draw(SpriteBatch spriteBatch, Hitbox screen);
    void Draw(SpriteBatch spriteBatch, Hitbox screen, Rectangle mask);

}
