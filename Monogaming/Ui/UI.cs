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
using System.Text.Json.Serialization;

namespace Monogaming.Ui;

public class UI : ImUpdatable
{
    public bool IsVisible { get; private set; } = true;
    public bool IsActive { get; private set; } = true;
    Dictionary<string, UiElement> menus = new();

    private GameWindow gameWindowRef;

    private const string UIPath = "./GameData/Menus/";
    public UI(GameWindow windowRef)
    {
        gameWindowRef = windowRef;

        //add menus here cause i dont wanna do more json lol
        //haha suck it we're doing more json lolol enjoy scalability loser
        int count = 0;

        foreach (string fileName in Directory.EnumerateFiles(UIPath))
        {
            try
            {
                Stream textStream = File.OpenRead(fileName);
                menus.Add(Path.GetFileNameWithoutExtension(fileName), 
                JsonSerializer.Deserialize<UiElement>(textStream));
                count++;
            }
            catch (Exception ex)
            {
                Debug.WriteError($"Failed to load menu {fileName} with error {ex.Message}");
            }

        }
        HandleWindowChange(this, EventArgs.Empty);
        menus["mainMenu"].ActivateElement();
        Debug.WriteInfo($"Loaded {count} menus from {UIPath}");
    }

    public string SerializeMenu(string menuName)
    {
        return SerializeMenu(menus[menuName]);
    }
    public static string SerializeMenu(UiElement element)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        options.Converters.Add(new Vector2JsonConverter());

        return JsonSerializer.Serialize(element, options);
    }
    public void ActivateElement(string elementID)
    {
        try
        {
            menus[elementID].ActivateElement();
        }
        catch (Exception ex)
        {
            Debug.WriteError($"Tried to activate non existent element {elementID} with error {ex.Message}");
        }
    }
    public void DeactivateElement(string elementID)
    {
        try
        {
            menus[elementID].DeactivateElement();
        }
        catch (Exception ex)
        {
            Debug.WriteError($"Tried to deactivate non existent element {elementID} with error {ex.Message}");
        }
    }

    public UiElement GetElement(string menuName, string searchPath)
    {
        return menus[menuName].GetChild(searchPath);
    }

    public void HandleWindowChange(object sender, EventArgs args)
    {
        //tell elements to update their sizes to match new window
        Rectangle bounds = gameWindowRef.ClientBounds;
        bounds.X = 0;
        bounds.Y = 0;
        foreach (UiElement element in menus.Values)
        {
            element.UpdatePos(bounds);
        }
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



/// <summary>
/// LLM generated via Google gemini due to simplicity and tedium:
/// Vector2 jsonconverter as it otherwise doesn't work due to 
/// </summary>
public class Vector2JsonConverter : JsonConverter<Vector2>
{
    public override Vector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        float x = 0;
        float y = 0;

        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected StartObject token for Vector2.");

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                return new Vector2(x, y);

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                string propertyName = reader.GetString();
                reader.Read(); // Move to the value

                if (string.Equals(propertyName, "X", StringComparison.OrdinalIgnoreCase))
                    x = reader.GetSingle();
                else if (string.Equals(propertyName, "Y", StringComparison.OrdinalIgnoreCase))
                    y = reader.GetSingle();
            }
        }

        throw new JsonException("Unexpected end of JSON while reading Vector2.");
    }

    public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("X", value.X);
        writer.WriteNumber("Y", value.Y);
        writer.WriteEndObject();
    }
}
