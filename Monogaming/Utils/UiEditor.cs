using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Monogaming.Entities;
using Monogaming.MapThings;
using Monogaming.TextureThings;
using Monogaming.Ui;
using NativeFileDialogNET;

namespace Monogaming.Utils;
/// <summary>
/// A class dedicated to simply act as a level editor, intended to function when control is 
/// given to it and it alone.
/// </summary>
public class UiEditor
{
    GameState gameStateRef;
    GameWindow windowRef;
    UiElement workingElement;


    private const int XPadding = 50;
    private const int YPadding = 60;
    Rectangle innerBounds;


    public UiEditor(GameState gameState, GameWindow window)
    {
        gameStateRef = gameState;
        windowRef = window;
        windowRef.ClientSizeChanged -= HandleWindowChange;
        windowRef.ClientSizeChanged += HandleWindowChange;
        HandleWindowChange(null, null);
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
        return (result == DialogResult.Okay, output);
    }

    public async void HandleCommand(JsonElement args)
    {
        switch (args.GetProperty("UiCommand").GetString())
        {
            case ("LoadFile"):
                (bool loadok,string loadPath) = await AskUserOpenFile();
                if (loadok)
                {
                    LoadElement(loadPath);
                }
                else
                {
                    Debug.WriteInfo("Ui file open not ok");
                }
            break;
            case ("SaveFile"):
                (bool saveok,string savePath) = await AskUserSaveFile();
                if (saveok)
                {
                    SaveWorkingElement(savePath);
                }
                else
                {
                    Debug.WriteInfo("Ui file save not ok");
                }
            break;
            default:
            Debug.WriteWarn($"Fell out of ui editor command handling with {args}");
            break;
        }
    }

    public void LoadElement(string path)
    {
        Stream textStream = File.OpenRead(path);
        workingElement = JsonSerializer.Deserialize<UiElement>(textStream);
        workingElement.UpdatePos(innerBounds);
        workingElement.ActivateElement();
        Debug.WriteInfo($"Loaded menu from {path}");
    }

    public void SaveWorkingElement(string path)
    {
        if (File.Exists(path))
        {
            File.Move(path, path + ".bckp");
        }
        File.WriteAllText(path, UI.SerializeMenu(workingElement));
        Debug.WriteInfo($"Saved working menu to {path}");
    }

    public void HandleWindowChange(object sender, EventArgs args)
    {
        Rectangle realWindow = windowRef.ClientBounds;
        Vector2 scale = new Vector2(realWindow.Width / (float)Camera.ScreenWidth, realWindow.Height / (float)Camera.ScreenHeight);
        innerBounds = new Rectangle((int)(XPadding * scale.X), (int)(YPadding * scale.Y),
        (int)((Camera.ScreenWidth - XPadding * 2) * scale.X), (int)((Camera.ScreenHeight - YPadding * 2) * scale.Y));
    }
    public void Update(GameTime gameTime)
    {
        workingElement?.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Textures.BlankTexture, innerBounds, Color.White);
        workingElement?.Draw(spriteBatch);
    }



}
