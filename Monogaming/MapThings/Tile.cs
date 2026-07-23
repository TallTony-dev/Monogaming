using System;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monogaming.Entities;
using Monogaming.Interfaces;
using Monogaming.TextureThings;

namespace Monogaming.MapThings;

public class Tile : ImUpdatable
{
    public bool IsActive { get; private set; } = true;
    public bool Walkable { get; private set; }
    public ImTexture Texture { get; private set; }

    public Tile(JsonProperty property)
    {
        Texture = Textures.GetTexture(property.Value.GetProperty("Texture").GetString());
        Walkable = property.Value.GetProperty("Walkable").GetBoolean();
    }

    public void Update(GameTime gameTime)
    {
        if (Texture.IsAnimated)
        {
            var a = Texture as TextureAnimation;
            a.Update(gameTime);
        }
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 pos)
    {
        Texture.Draw(spriteBatch, pos);
    }
}