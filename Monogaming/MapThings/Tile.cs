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
    public TileTex Texture { get; private set; }
    public string TextureID { get; private set; }
    public string ID { get; private set; }

    [JsonConstructor]
    public Tile(string ID, string TextureID, bool Walkable)
    {
        this.ID = ID;
        this.TextureID = TextureID;
        this.Walkable = Walkable;
        Texture = (TileTex)Textures.GetTexture(TextureID);
        //Texture = Textures.GetFrameOfTexture(TextureID, 0);
    }

    public Tile Copy()
    {
        return new Tile(ID, TextureID, Walkable);
    }

    public void SetTileTex(TileTex.TileTexType ind)
    {
        if (Texture != null)
        {
            Texture.SetTileTex(ind);
        }
    }

    public void Update(GameTime gameTime)
    {
        // if (Texture.IsAnimated)
        // {
        //     Texture.Update(gameTime);
        // }
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 pos)
    {
        // Texture.Draw(spriteBatch, pos);
        Texture.Draw(spriteBatch, new Rectangle((int)pos.X, (int)pos.Y, Map.TileDims, Map.TileDims));
    }


}