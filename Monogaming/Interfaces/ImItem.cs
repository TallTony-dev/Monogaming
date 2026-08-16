using System;
using Microsoft.Xna.Framework.Graphics;
using Monogaming.Entities;

namespace Monogaming.Interfaces;

public interface ImItem
{
    void Draw(SpriteBatch spriteBatch, Hitbox screen, bool withBg);
    void Draw(SpriteBatch spriteBatch, Hitbox screen, Color color, bool withBg);

}
