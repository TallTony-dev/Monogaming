using System;
using Monogaming.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monogaming.Interfaces;
using Monogaming.TextureThings;
using System.Runtime.Intrinsics.X86;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Monogaming.Ui;

public class UI : ImUpdatable
{
    public bool IsVisible { get; private set; } = true;
    public bool IsActive { get; private set; } = true;
    Dictionary<string, UiElement> menus = new();


    private const string UIPath = "./GameData/Menus/";
    public UI()
    {
        //add menus here cause i dont wanna do more json lol
        //haha suck it we're doing more json lolol enjoy scalability loser
        int count = 0;

        foreach (string fileName in Directory.EnumerateFiles(UIPath))
        {
            try
            {
                Stream textStream = File.OpenRead(fileName);
                JsonDocument document = JsonDocument.Parse(textStream);
                JsonElement root = document.RootElement;
                if (root.GetProperty("Title").GetString() != "menu")
                {
                    throw new JsonException($"Error loading {fileName}, attempted to load a json not titled with menu");
                }
                menus.Add(root.GetProperty("ID").GetString(), 
                JsonSerializer.Deserialize<UiElement>(root.GetProperty("RootElement")));
                count++;
            }
            catch (Exception ex)
            {
                Debug.WriteError($"Failed to load menu {fileName} with error {ex.Message}");
            }

        }
        Debug.WriteInfo($"Loaded {count} menus from {UIPath}");
    }

    public void Update(GameTime gameTime)
    {
        foreach (UiElement menu in menus.Values)
        {
            if (menu.IsActive)
            {
                menu.Update(gameTime);
            }
        }
    }

    public void HandleWindowChange(object sender, EventArgs args)
    {
        //tell elements to update their sizes to match new window
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (UiElement menu in menus.Values)
        {
            if (menu.IsVisible)
            {
                menu.Draw(spriteBatch);
            }
        }
    }
    //TODO: implement this wrapper and also an animation handler one too
}