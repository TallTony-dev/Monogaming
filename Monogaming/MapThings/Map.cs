using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monogaming.Entities;

namespace Monogaming.MapThings;

public class Map
{
    public static readonly int TileDims = 32;


    //TODO: make this contain chunks and have a method for the chunks to communicate that
    //a new one should be moved to.

    public void Update(GameTime gameTime)
    {
        
    }

    public void Draw(SpriteBatch spriteBatch, Hitbox screen)
    {
        
    }

    public static Vector2 TilePosToWorldPos(Vector2 tilePos)
    {
        return tilePos * TileDims;
    }
    public static Vector2 WorldPosToTilePos(Vector2 worldPos)
    {
        return worldPos / TileDims;
    }
    public static (int x, int y) WorldPosToAbsTilePos(Vector2 worldPos)
    {
        Vector2 v = WorldPosToTilePos(worldPos);
        return ((int)v.X, (int)v.Y);
    }
    public static Vector2 WorldPosToScreenPos(Vector2 pos)
    {
        return pos;
    }



}