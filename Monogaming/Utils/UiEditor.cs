using System.IO;
using System.Text;
using System.Threading.Tasks;
using Apos.Input;
using Microsoft.Xna.Framework;
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
public class UiEditor
{
    GameState gameStateRef;
    GameWindow windowRef;
    UiElement workingElement;

    UiElement selected;

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
    private void AddBlankElementToSelected()
    {
        UiElement blank = new UiElement("newElement", AnchorPoint.TopLeft, new Hitbox(0, 0, 64, 32), null);
        
        if (selected != null)
        {
            selected.Children.Add(blank);
        }
        else if (workingElement != null)
        {
            workingElement.Children.Add(blank);
        }
        else
        {
            workingElement = new UiElement("root", AnchorPoint.TopLeft, null, null);
        }
        workingElement = blank;
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
            case ("AddElement"):
                AddBlankElementToSelected();
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

    private void SetFloatFromString(ref float f, string s)
    {
        try
        {
            f = Convert.ToSingle(s);
        }
        catch
        {
            Debug.WriteError($"Failed to parse {s} to float");
        }
    }

    private void SelectElement(UiElement element)
    {
        if (element != null) {
            ((TextInputElement)gameStateRef.Ui.GetElement("uiEditorMenu", "xPosElement")).
            BindString((s) => SetFloatFromString(ref element.DrawBox.GetPosRef().X, s));
        }

        selected = element;
    }


    ICondition select = new MouseCondition(MouseButton.LeftButton);
    bool isDragging = false;
    public void Update(GameTime gameTime)
    {
        if (workingElement != null) {
            // workingElement.Update(gameTime);

            if (select.Pressed())
            {
                SelectElement(workingElement.GetTopCollision(InputHelper.NewMouse.Position.ToVector2()));
                isDragging = true;
            }
            else if (isDragging && !select.Held())
            {
                isDragging = false;
            }

            if (selected != null)
            {
                //display some interaction window here to play with the properties
                // selected.


                if (isDragging)
                {
                    
                    Vector2 innerScale = new Vector2(
                    (float)Camera.ScreenWidth / innerBounds.Width, 
                    (float)Camera.ScreenHeight / innerBounds.Height);
                    selected.AdjustRealPos((InputHelper.NewMouse.Position - InputHelper.OldMouse.Position).ToVector2() / innerScale);
                    // selected.AdjustRealPos(MeowGame.ScaledMouseDelta);
                }

            }

            workingElement.UpdatePos(innerBounds);
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Textures.BlankTexture, innerBounds, Color.White);
        if (selected != null)
        {
            Rectangle rect = selected.ScaledDrawBox.AsRect();
            rect.Inflate(2,2);
            spriteBatch.Draw(Textures.BlankTexture, rect, Color.Black);
        }
        workingElement?.Draw(spriteBatch);
        
    }



}
