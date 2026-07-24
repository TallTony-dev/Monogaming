using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monogaming.Entities;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Monogaming.MapThings;

public class Map
{
    public static readonly int TileDims = 32;


    //TODO: make this contain chunks and have a method for the chunks to communicate that
    //a new one should be moved to.

    private Chunk activeChunk;
    public bool isLoading { get; private set; } = false;

    public async Task LoadChunkAsync(string fileName)
    {
        try {
            isLoading = true;
            string path = "./GameData/Chunks/" + fileName + ".json";

            Stream textStream = File.OpenRead(path);
            activeChunk = await JsonSerializer.DeserializeAsync<Chunk>(textStream);
            isLoading = false;
        }
        catch (Exception ex)
        {
            Debug.WriteError($"Failed to load chunk {fileName} with error {ex.Message}");
        }
    }

    public void Update(GameTime gameTime)
    {
        if (!isLoading)
        {
            activeChunk?.Update(gameTime);
        }
    }

    public void Draw(SpriteBatch spriteBatch, Hitbox screen)
    {
        if (!isLoading)
        {
            activeChunk?.Draw(spriteBatch, screen);
        }
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