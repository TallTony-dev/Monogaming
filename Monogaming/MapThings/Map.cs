using Monogaming.Entities;
using Monogaming.Ui;
using System.IO;
using System.Threading.Tasks;

namespace Monogaming.MapThings;

public class Map
{
    public static readonly int TileDims = 32;

    private GameState gameStateref;
    private Camera cameraRef;
    public static Chunk LiveChunk { get; private set; } // refactored this late so many times it is passed in calls anyways // what the hell was I talking about
    public static Map LiveMap { get; private set; } // refactored this late so many times it is passed in calls anyways // what the hell was I talking about 2
    public bool isLoading { get; private set; } = true;

    public Map(Camera cameraRef, GameState gameState)
    {
        this.cameraRef = cameraRef;
        gameStateref = gameState;
        LiveMap = this;
    }


    public async Task LoadChunkAsync(string path)
    {
        try
        {
            isLoading = true;

            Stream textStream = File.OpenRead(path);

            var options = new JsonSerializerOptions { WriteIndented = true };
            options.Converters.Add(new Vector2JsonConverter());

            if (LiveChunk != null)
            {
                gameStateref.Save(GameState.ActiveSave.Name);
            }

            //TransitionManager.AddTransition(new SlideLeftTransition(() => !isLoading))

            LiveChunk = await JsonSerializer.DeserializeAsync<Chunk>(textStream, options);

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
            return LiveChunk;
        }
        return null;
    }

    public void Update(GameTime gameTime)
    {
        if (!isLoading)
        {
            LiveChunk?.Update(gameTime);
        }
    }

    public void Draw(SpriteBatch spriteBatch, Hitbox screen)
    {
        if (!isLoading)
        {
            LiveChunk?.Draw(spriteBatch, screen);
        }
    }

    public void DrawLights(SpriteBatch spriteBatch, Hitbox screen)
    {
        if (!isLoading)
        {
            LiveChunk?.DrawLights(spriteBatch, screen);
        }
    }

    public (int x, int y, bool offscreen) GetTileCoordAtScreenPos(Vector2 screenPos)
    {
        Vector2 screen = ScreenPosToWorldPos(screenPos);

        (int x, int y) tilePos = WorldPosToAbsTilePos(screen);
        (int ix, int iy) old = tilePos;
        tilePos.x = Math.Clamp(tilePos.x, 0, LiveChunk.Width);
        tilePos.y = Math.Clamp(tilePos.y, 0, LiveChunk.Height);
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
        LiveChunk.ChangeTile(x, y, tile);
    }

    public void SettleLoadedChunk()
    {
        if (!isLoading)
        {
            LiveChunk?.SettleTiles();
        }
    }
    public static Vector2 TilePosToWorldPos(int x, int y)
    {
        return new Vector2(x, y) * TileDims;
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