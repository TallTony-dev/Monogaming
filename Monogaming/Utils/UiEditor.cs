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
using Monogaming.Ui.Windows;

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
    UiElement copied;

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
        return (result == DialogResult.Okay, output + ".json");
    }
    private void AddElementToEditor(UiElement element)
    {
        if (selected != null)
        {
            selected.Children.Add(element);
        }
        else if (workingElement != null)
        {
            workingElement.Children.Add(element);
        }
        else if (workingElement == null)
        {
            workingElement = new UiElement("root", AnchorPoint.TopLeft, null, null);
            workingElement.Children.Add(element);
        }
        element.ActivateElement();
        selected = element;
    }
    private void AddBlankElementToSelected(string type)
    {
        UiElement blank;

        //lol ts sucks
        switch (type) {
            case("buttonElement"):
                blank = new ButtonElement("newElement", AnchorPoint.TopLeft, new Hitbox(0, 0, 64, 32), null, null, "button", null);
            break;
            case("textInputElement"):
                blank = new TextInputElement("newElement", AnchorPoint.TopLeft, new Hitbox(0, 0, 64, 32), "default", null);
            break;
            case("dropDownElement"):
                blank = new DropDownElement("newElement", AnchorPoint.TopLeft, new Hitbox(0, 0, 64, 32), null, "default", null);
            break;
            case("inventoryElement"):
                blank = new InventoryElement("newElement", AnchorPoint.TopLeft, new Hitbox(0, 0, 64, 32), null);
            break;
            case("windowElement"):
                blank = new WindowElement("newElement", AnchorPoint.TopLeft, new Hitbox(0, 0, 64, 32), null);
            break;
            default:
                blank = new UiElement("newElement", AnchorPoint.TopLeft, new Hitbox(0, 0, 64, 32), null);
            break;
        }
        
        AddElementToEditor(blank);
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
                AddBlankElementToSelected(args.GetProperty("Type").ToString());
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
        workingElement.ActivateElement(true);
        Debug.WriteInfo($"Loaded menu from {path}");
    }

    public void SaveWorkingElement(string path)
    {
        if (File.Exists(path))
        {
            File.Move(path, path + ".bckp", true);
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

    private bool SetFloatFromString(ref float f, string s)
    {
        try
        {
            f = Convert.ToSingle(s);
            return true;
        }
        catch
        {
            Debug.WriteError($"Failed to parse {s} to float");
            return false;
        }
    }

    private string GetStringFromFloat(ref float f)
    {
        return MathF.Ceiling(f).ToString();
    }
    private bool SetIntFromString(ref int i, string s)
    {
        try
        {
            i = Convert.ToInt32(s);
            return true;
        }
        catch
        {
            Debug.WriteError($"Failed to parse {s} to int");
            return false;
        }
    }
    private string GetStringFromInt(ref int i)
    {
        return i.ToString();
    }
    private bool SetStringFromString(ref string s, string setto)
    {
        s = setto;
        return true;
    }

    private void SelectElement(UiElement element)
    {
        //mmmm yummy spaghetti sure hope i dont have to debug this
        //also im only letting this slide cause its a dev tool
        if (element != null) {
            ((TextInputElement)gameStateRef.Ui.GetElement("uiEditorMenu", "xPosElement")).
            BindString((s) => SetFloatFromString(ref element.DrawBox.GetPosRef().X, s), 
            () => GetStringFromFloat(ref element.DrawBox.GetPosRef().X));

            ((TextInputElement)gameStateRef.Ui.GetElement("uiEditorMenu", "yPosElement")).
            BindString((s) => SetFloatFromString(ref element.DrawBox.GetPosRef().Y, s), 
            () => GetStringFromFloat(ref element.DrawBox.GetPosRef().Y));

            ((TextInputElement)gameStateRef.Ui.GetElement("uiEditorMenu", "heightElement")).
            BindString((s) => SetIntFromString(ref element.DrawBox.GetHeightRef(), s), 
            () => GetStringFromInt(ref element.DrawBox.GetHeightRef()));

            ((TextInputElement)gameStateRef.Ui.GetElement("uiEditorMenu", "widthElement")).
            BindString((s) => SetIntFromString(ref element.DrawBox.GetWidthRef(), s), 
            () => GetStringFromInt(ref element.DrawBox.GetWidthRef()));

            ((TextInputElement)gameStateRef.Ui.GetElement("uiEditorMenu", "idElement")).
            BindString((s) => { element.ID = s; return true; }, 
            () => element.ID);

            if (element is TextElement textElement)
            {
                ((TextInputElement)gameStateRef.Ui.GetElement("uiEditorMenu", "textEnterElement")).
                BindString((s) => { textElement.Text = s; return true; }, 
                () => textElement.Text);
            }
            else if (element is ButtonElement buttonElement)
            {
                ((TextInputElement)gameStateRef.Ui.GetElement("uiEditorMenu", "textEnterElement")).
                BindString((s) => { buttonElement.Text = s; return true; }, 
                () => buttonElement.Text);
            }
            else if (element is TextInputElement textInputElement)
            {
                ((TextInputElement)gameStateRef.Ui.GetElement("uiEditorMenu", "textEnterElement")).
                BindString((s) => { textInputElement.Text = s; return true; }, 
                () => textInputElement.Text);
            }

        }

        selected = element;
    }

    private void DeleteElement(UiElement element)
    {
        workingElement.DeleteElement(element);
    }

    bool isDragging = false;
    public void Update(GameTime gameTime)
    {
        if (workingElement != null) {
            // workingElement.Update(gameTime);

            if (InputConditions.click.Pressed() && innerBounds.Contains(InputHelper.NewMouse.Position.ToVector2()))
            {
                SelectElement(workingElement.GetTopCollision(InputHelper.NewMouse.Position.ToVector2()));
                isDragging = true;
            }
            else if (isDragging && !InputConditions.click.Held())
            {
                isDragging = false;
                selected?.RoundRealPos();
            }

            if (InputConditions.paste.Pressed() && copied != null)
            {
                //I know there's probably a better way to do this but should be fine here
                //Didnt wanna make a copy constructor for all the derived elements lolol :^)
                string s = JsonSerializer.Serialize(copied);
                AddElementToEditor(JsonSerializer.Deserialize<UiElement>(s));
                Debug.WriteInfo("Pasted");
            }

            if (selected != null)
            {
                //display some interaction window here to play with the properties
                // selected.
                if (InputConditions.delete.Pressed())
                {
                    DeleteElement(selected);
                    return;
                }

                if (InputConditions.copy.Pressed())
                {
                    copied = selected;
                    Debug.WriteInfo("Copied");
                }

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
