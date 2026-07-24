using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monogaming.Entities;
using Monogaming.Interfaces;
using Monogaming.TextureThings;

namespace Monogaming.MapThings;

public class Tile : ImUpdatable
{
    [JsonIgnore]
    public bool IsActive { get; private set; } = true;
    public bool Walkable { get; private set; }
    public ImTexture Texture { get; private set; }
    public string TextureID { get; private set; }
    public string ID { get; private set; }

    [JsonConstructor]
    public Tile(string ID, string TextureID, bool Walkable)
    {
        this.ID = ID;
        this.TextureID = TextureID;
        this.Walkable = Walkable;
        Texture = Textures.GetTexture(TextureID);
    }
    public Tile(ImTexture Texture, bool Walkable)
    {
        this.Texture = Texture;
        this.Walkable = Walkable;
    }

    public Tile Copy()
    {
        return new Tile(Texture, Walkable);
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
        Texture.Draw(spriteBatch, new Rectangle((int)pos.X, (int)pos.Y, Map.TileDims, Map.TileDims));
    }
}