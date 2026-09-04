using Apos.Input;
using Monogaming.Entities;
using Monogaming.MapThings;
using Monogaming.Monogaming.Utils;
using Monogaming.Ui;
using NativeFileDialogNET;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Monogaming.Utils;
/// <summary>
/// A class dedicated to simply act as a level editor, intended to function when control is 
/// given to it and it alone.
/// </summary>
public class MapEditor
{
    GameState gameStateRef;

    bool isDrawing = false;

    string placedTileID;
    string placedEntityID;

    Entity selectedEntity;
    bool isDragging;

    public MapEditor(GameState gameState)
    {
        gameStateRef = gameState;

        ((TextInputElement)gameStateRef.Ui.GetElement("tileMapEditorMenu", "tileIDElement")).
        BindString((s) => { placedTileID = s; return true; }, () => placedTileID);

        ((TextInputElement)gameStateRef.Ui.GetElement("tileMapEditorMenu", "addElement.entityIDElement")).
        BindString((s) => { placedEntityID = s; return true; }, () => placedEntityID);
    }

    public async void HandleCommand(JsonElement Args)
    {
        switch (Args.GetProperty("UiCommand").GetString())
        {
            case ("SettleLoadedChunk"):
                gameStateRef.Map.SettleLoadedChunk();
                break;
            case ("SaveLoadedChunk"):
                (bool ok, string path) = await FileSystemUtils.AskUserSaveFile(Path.Join(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Content/GameData/Chunks"));
                if (ok)
                {
                    TrySaveLoadedChunk(path);
                }
                break;
            case ("AddEntity"):
                gameStateRef.Map.GetLoadedChunk()?.AddEntity(placedEntityID, Vector2.Zero);
                break;
            case ("LoadChunk"):
                (bool ok2, string path2) = await FileSystemUtils.AskUserOpenFile(Path.Join(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Content/GameData/Chunks"));
                if (ok2)
                {
                    await gameStateRef.Map.LoadChunkAsync(path2);
                }
                break;
            case ("ToggleDraw"):
                isDrawing = !isDrawing;
                break;
            default:
                Debug.WriteWarn("Fell out of Map command handler");
                break;
        }
    }

    public void Update(GameTime gameTime)
    {
        gameStateRef.Ui.GetElement("tileMapEditorMenu", "toggleDrawElement").
        DrawColor = isDrawing ? Color.Green : Color.Red;



        Map m = gameStateRef.Map;
        if (!m.isLoading)
        {
            Vector2 mPos = InputHelper.NewMouse.Position.ToVector2();
            Vector2 mWorldPos = m.ScreenPosToWorldPos(mPos);
            (int x, int y, bool offscreen) mouseTPos = m.GetTileCoordAtScreenPos(mPos);

            if (isDrawing && !mouseTPos.offscreen)
            {
                if (InputConditions.click.Held())
                {
                    m.TrySetTileAtPos(mouseTPos.x, mouseTPos.y, placedTileID);
                }
            }
            else
            {
                if (InputConditions.click.Pressed())
                {
                    selectedEntity = m.GetLoadedChunk()?.GetTopEntityCollision(mWorldPos);
                    isDragging = true;
                }
                else if (isDragging && !InputConditions.click.Held())
                {
                    isDragging = false;
                    m.GetLoadedChunk()?.SettleEntities();
                }



                if (isDragging && selectedEntity != null)
                {
                    selectedEntity.DrawBox.ChangePos(InputHelper.NewMouse.Position.ToVector2() - InputHelper.OldMouse.Position.ToVector2());
                }
            }

        }

    }

    // public void Draw(SpriteBatch spriteBatch)
    // {

    // }

    private void TrySaveLoadedChunk(string path)
    {
        Chunk chunk = gameStateRef.Map.GetLoadedChunk();
        if (chunk != null)
        {
            if (File.Exists(path))
            {
                File.Move(path, path + ".bckp", true);
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            options.Converters.Add(new Vector2JsonConverter());
            File.WriteAllText(path, JsonSerializer.Serialize(chunk, options));

            Debug.WriteInfo($"Saved loaded chunk to {path}");
        }
    }



}
