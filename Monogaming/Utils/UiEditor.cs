using System.IO;
using Microsoft.Xna.Framework;
using Monogaming.Entities;
using Monogaming.MapThings;
using Monogaming.Ui;

namespace Monogaming.Utils;
/// <summary>
/// A class dedicated to simply act as a level editor, intended to function when control is 
/// given to it and it alone.
/// </summary>
public class UiEditor
{
    GameState gameStateRef;

    UiElement workingElement;
    public UiEditor(GameState gameState)
    {
        gameStateRef = gameState;
    }


    public void LoadElement(string path)
    {
        Stream textStream = File.OpenRead(path);
        workingElement = JsonSerializer.Deserialize<UiElement>(textStream);
    }

    public void SaveWorkingElement(string path)
    {
        if (File.Exists(path))
        {
            File.Move(path, path + ".bckp");
        }
        UI.SerializeMenu(workingElement);
    }

    public void Update(GameTime gameTime)
    {
        
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        
    }



}
