using System.IO;
using System.Text;
using System.Threading.Tasks;
using Apos.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monogaming.Entities;
using Monogaming.MapThings;
using System.Reflection; //ruh roh raggy
using Monogaming.TextureThings;
using Monogaming.Ui;
using NativeFileDialogNET;

namespace Monogaming.Utils;
/// <summary>
/// A class dedicated to simply act as a level editor, intended to function when control is 
/// given to it and it alone.
/// </summary>
public class MapEditor
{
    GameState gameStateRef;
    string placedTileID;
    bool failedLastPlace;


    public MapEditor(GameState gameState)
    {
        gameStateRef = gameState;

        ((TextInputElement)gameStateRef.Ui.GetElement("tileMapEditorMenu", "tileIDElement")).
        BindString((s) => { placedTileID = s; return true; }, 
        () => placedTileID);
    }

    private static async Task<(bool ok,string path)> AskUserOpenFile()
    {
        using var selectFileDialog = new NativeFileDialog()
        .SelectFile();
        DialogResult result = selectFileDialog.Open(out string output, "./GameData/Menus");
        return (result == DialogResult.Okay, output);
    }
    private static async Task<(bool ok,string path)> AskUserSaveFile()
    {
        using var selectFileDialog = new NativeFileDialog()
        .SaveFile();
        DialogResult result = selectFileDialog.Open(out string output, "./GameData/Menus");
        return (result == DialogResult.Okay, output + ".json");
    }
    

    ICondition click = new MouseCondition(MouseButton.LeftButton);
    ICondition copy = new AllCondition(
        new KeyboardCondition(Keys.C), 
        new KeyboardCondition(Keys.LeftControl)
    );
    ICondition paste = new AllCondition(
        new KeyboardCondition(Keys.V), 
        new KeyboardCondition(Keys.LeftControl)
    );
    ICondition delete = new AllCondition(
        new KeyboardCondition(Keys.Back), 
        new KeyboardCondition(Keys.LeftControl)
    );


    public void Update(GameTime gameTime)
    {
        Map m = gameStateRef.Map;
        if (!m.isLoading)
        {
            Vector2 mPos = InputHelper.NewMouse.Position.ToVector2();
            (int x, int y) mouseTPos = m.GetTileCoordAtScreenPos(mPos);

            if (click.Held())
            {
                bool b = m.TrySetTileAtPos(mouseTPos.x, mouseTPos.y, placedTileID);
                Debug.WriteInfo($"Tried to set tile at {mouseTPos} to {placedTileID} with {b}");
            }

        }

    }

    public void Draw(SpriteBatch spriteBatch)
    {
        
    }



}
