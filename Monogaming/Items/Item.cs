using System;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Monogaming.Items;

public class Item
{
    public string Name { get; private set; } //what this item is called
    public string TypeID { get; private set; } //describes the type of item this is
    public string Category { get; private set; }
    public int Count { get; private set; } = 1;

    public Item(string name, string typeID, string category, int count = 1)
    {
        Name = name;
        TypeID = typeID;
        Category = category;
        Count = count;
    }

    public Item(JsonProperty obj)
    {
        Name = obj.Name;
        TypeID = obj.Value.GetProperty("ID").GetString();
        Category = obj.Value.GetProperty("Category").GetString();
    }

    public Item Copy()
    {
        return new Item(Name, TypeID, Category, Count);
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
        return i.TypeID == TypeID;
    }
    public bool IDEqual(string s)
    {        
        return s == TypeID;
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
