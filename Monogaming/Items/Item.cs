using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Monogaming.Interfaces;
using Monogaming.TextureThings;
using System.Security.Cryptography;
using Monogaming.Entities;

namespace Monogaming.Items;

public class Item : ImItem
{
    public const int TextOffset = 40;
    public const int TextHeightOffset = 20;
    public const int TextWidth = 30;
    public const int TextHeight = 20;
    public const int ItemPadding = 5;


    [JsonIgnore]
    public bool IsVisible { get; private set; } = false;
    public string Name { get; private set; } //what this item is called
    public string ID { get; private set; } //describes the type of item this is
    public string Category { get; private set; }
    public int Count { get; private set; } = 1;
    public int MaxStackSize { get; private set; } = 75;
    public string PlacedEntityID { get; private set; }

    public string TextureID { get; private set; }
    ImTexture ItemTexture;
    ImTexture BackTexture;

    [JsonConstructor]
    public Item(string name, string ID, string category, int MaxStackSize, string textureID, string PlacedEntityID, int count = 1)
    {
        this.PlacedEntityID = PlacedEntityID;
        TextureID = textureID;
        ItemTexture = Textures.GetTexture(textureID);
        BackTexture = Textures.GetTexture(GetBGTexureIDForCategory(category));
        Name = name;
        this.ID = ID;
        Category = category;
        Count = count;
    }
    
    public Item Copy()
    {
        return new Item(Name, ID, Category, MaxStackSize, TextureID, PlacedEntityID, Count);
    }
    public Item Copy(int count)
    {
        return new Item(Name, ID, Category, MaxStackSize, TextureID, PlacedEntityID, count);
    }

    public void DrawBackground(SpriteBatch spriteBatch, Hitbox box, Color color, int padding)
    {
        Rectangle itemBox = box.ToRect();
        itemBox.Inflate(padding, padding);
        BackTexture.Draw(spriteBatch, itemBox, color);
    }

    public void Draw(SpriteBatch spriteBatch, Hitbox box, bool withBg)
    {
        Draw(spriteBatch, box, Color.White, withBg);
    }
    public void Draw(SpriteBatch spriteBatch, Hitbox box, Color color, bool withBg)
    {
        if (withBg) {
            DrawBackground(spriteBatch, box, color, ItemPadding);
        }

        ItemTexture.Draw(spriteBatch, box.ToRect(), color);
        if (Count > 1) {
            Hitbox textBox = new Hitbox(box.Pos + new Vector2(box.Width - TextOffset, box.Height - TextHeightOffset),
            TextWidth, TextHeight);
            textBox.DrawStringInto(spriteBatch, Count.ToString(), Color.White);
        }
    }


    private static string GetBGTexureIDForCategory(string category)
    {
        return category switch
        {
            "mechanical" => "itemBackgrounds.Mechanical",
            _ => "itemBackgrounds.Basic"
        };
    }

    public void AddTo(int count)
    {
        Count += count;
        //could add max stack logic here
    }
    public void RemoveFrom(int count)
    {
        Count -= count;
    }
    #region EqualityHelpers
    public bool IDEqual(Item i)
    {        
        return i.ID == ID;
    }
    public bool IDEqual(string s)
    {        
        return s == ID;
    }
    /// <summary>
    /// Checks if the called on object has a greater count than the param object
    /// </summary>
    public bool CountGreaterThan(Item i)
    {
        return Count > i.Count;
    }
    public bool CountEqualTo(Item i)
    {
        return Count == i.Count;
    }
    public bool CountGreaterOrEq(Item i)
    {
        return Count >= i.Count;
    }
    public bool CountGreaterOrEq(int i)
    {
        return Count >= i;
    }
    #endregion
}
