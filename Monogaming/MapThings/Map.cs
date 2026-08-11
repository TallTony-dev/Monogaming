using Microsoft.Xna.Framework;
using Monogaming.Entities;
using System;
using System.IO;
using System.Threading.Tasks;
using Monogaming.Ui;

namespace Monogaming.MapThings;

public class Map
{
    public static readonly int TileDims = 32;

    private GameState gameStateref;
    private Camera cameraRef;
    private Chunk activeChunk;
    public bool isLoading { get; private set; } = true;

    public Map(Camera cameraRef, GameState gameState)
    {
        this.cameraRef = cameraRef;
        gameStateref = gameState;
    }


    public async Task LoadChunkAsync(string path)
    {
        try {
            isLoading = true;

            Stream textStream = File.OpenRead(path);

            var options = new JsonSerializerOptions { WriteIndented = true };
            options.Converters.Add(new Vector2JsonConverter());

            if (activeChunk != null) {
                gameStateref.Save(GameState.ActiveSave.Name);
            }

            //TransitionManager.AddTransition(new SlideLeftTransition(() => !isLoading))

            activeChunk = await JsonSerializer.DeserializeAsync<Chunk>(textStream, options);
            
            isLoading = false;
        }
        catch (Exception ex)
        {
            Debug.WriteError($"Failed to load chunk {Path.GetFileName(path)} with error {ex.Message}");
        }
    }


    /// <summary>
    /// Null if not available
    /// </summary>
    public Chunk GetLoadedChunk()
    {
        if (!isLoading)
        {
            return activeChunk;
        }
        return null;
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

    public (int x, int y, bool offscreen) GetTileCoordAtScreenPos(Vector2 screenPos)
    {  
        Vector2 screen = ScreenPosToWorldPos(screenPos);

        (int x, int y) tilePos = WorldPosToAbsTilePos(screen);
        (int ix, int iy) old = tilePos;
        tilePos.x = Math.Clamp(tilePos.x, 0, activeChunk.Width);
        tilePos.y = Math.Clamp(tilePos.y, 0, activeChunk.Height);
        return (tilePos.x, tilePos.y, tilePos != old);
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

    public void SettleLoadedChunk()
    {
        if (!isLoading)
        {
            activeChunk?.SettleTiles();
        }
    }
    public static Vector2 TilePosToWorldPos(int x, int y)
    {
        return new Vector2(x,y) * TileDims;
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
        return ((int)MathF.Round(v.X), (int)MathF.Round(v.Y));
    }

    public Vector2 WorldPosToScreenPos(Vector2 pos)
    {
        return (pos - cameraRef.Screen.Pos) * cameraRef.Scale;
    }
    public Vector2 ScreenPosToWorldPos(Vector2 pos)
    {
        return pos / cameraRef.Scale + cameraRef.Screen.Pos;
    }



}