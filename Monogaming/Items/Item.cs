using System;

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

    public Item Copy()
    {
        return new Item(Name, TypeID, Category, Count);
    }
}
