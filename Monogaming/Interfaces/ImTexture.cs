using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monogaming.Entities;

namespace Monogaming.Interfaces;

public interface ImTexture
{
    bool IsAnimated { get; }
    void Draw(SpriteBatch batch, Vector2 pos);
    void Draw(SpriteBatch batch, Rectangle dest);
    void Draw(SpriteBatch batch, Rectangle dest, Rectangle mask);
    void Draw(SpriteBatch batch, Vector2 pos, Color color);
    void Draw(SpriteBatch batch, Rectangle dest, Color color);
    ImTexture Copy();
}