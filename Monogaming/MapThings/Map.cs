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
    private Camera cameraRef;
    private Chunk activeChunk;
    public bool isLoading { get; private set; } = true;

    public Map(Camera cameraRef)
    {
        this.cameraRef = cameraRef;
    }


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

    public async void HandleCommand(JsonElement Args)
    {
        switch (Args.GetProperty("UiCommand").GetString())
        {
            case ("SettleLoadedChunk"):
                if (!isLoading)
                {
                    activeChunk?.SettleTiles();
                }
            break;
            default:
            Debug.WriteWarn("Fell out of Map command handler");
            break;
        }
    }

    public (int x, int y) GetTileCoordAtScreenPos(Vector2 screenPos)
    {  
        Vector2 origin = cameraRef.Screen.Pos;
        Vector2 screenOff = ScreenPosToWorldPos(screenPos);

        (int x, int y) tilePos = WorldPosToAbsTilePos(origin + screenOff);
        tilePos.x = Math.Clamp(tilePos.x, 0, activeChunk.Width);
        tilePos.y = Math.Clamp(tilePos.y, 0, activeChunk.Height);
        return tilePos;
    }

    /// <summary>
    /// Returns null if the tile string doesnt exist
    /// </summary>
    public bool TrySetTileAtPos(int x, int y, string tile)
    {
        Tile t;
        if ((t = TileDict.GetTileCopy(tile)) != null)
        {
            SetTileAtPos(x, y, t);
            return true;
        }
        return false;
    }   
    private void SetTileAtPos(int x, int y, Tile tile)
    {
        activeChunk.ChangeTile(x, y, tile);
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

    public Vector2 WorldPosToScreenPos(Vector2 pos)
    {
        return pos * cameraRef.Scale;
    }
    public Vector2 ScreenPosToWorldPos(Vector2 pos)
    {
        return pos / cameraRef.Scale;
    }



}