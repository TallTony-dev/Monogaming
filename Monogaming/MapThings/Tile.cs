using Monogaming.Interfaces;
using Monogaming.TextureThings;
using System.Text.Json.Serialization;

namespace Monogaming.MapThings;

public class Tile : ImUpdatable
{
    [JsonIgnore]
    public bool IsActive { get; private set; } = true;
    public bool Walkable { get; private set; } //if things can move over it
    public bool Blocking { get; private set; } //if it blocks things from moving
    public TileTex Texture { get; private set; }
    public string TextureID { get; private set; }
    public string ID { get; private set; }

    [JsonConstructor]
    public Tile(string ID, string TextureID, bool Walkable, bool Blocking)
    {
        this.ID = ID;
        this.TextureID = TextureID;
        this.Walkable = Walkable;
        this.Blocking = Blocking;
        Texture = (TileTex)Textures.GetTexture(TextureID);
        //Texture = Textures.GetFrameOfTexture(TextureID, 0);
    }

    public Tile Copy()
    {
        return new Tile(ID, TextureID, Walkable, Blocking);
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