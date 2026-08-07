using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Monogaming.Interfaces;
using Monogaming.TextureThings;

namespace Monogaming.Items;

public class Item
{
    public string Name { get; private set; } //what this item is called
    public string ID { get; private set; } //describes the type of item this is
    public string Category { get; private set; }
    public int Count { get; private set; } = 1;

    public string TextureID { get; private set; }
    ImTexture Texture;

    [JsonConstructor]
    public Item(string name, string ID, string category, string textureID, int count = 1)
    {
        TextureID = textureID;
        Texture = Textures.GetTexture(textureID);
        Name = name;
        this.ID = ID;
        Category = category;
        Count = count;
    }
    
    public Item Copy()
    {
        return new Item(Name, ID, Category, TextureID, Count);
    }
    public Item Copy(int count)
    {
        return new Item(Name, ID, Category, TextureID, count);
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 pos)
    {
        Texture.Draw(spriteBatch, pos);
    }
    public void Draw(SpriteBatch spriteBatch, Rectangle rect)
    {
        Texture.Draw(spriteBatch, rect);
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
