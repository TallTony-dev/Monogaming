using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Monogaming.Items;


public class Inventory
{
    List<Item> items = new();

    public Inventory(string fileName)
    {
        throw new NotImplementedException();
        //should populate the inventory with items based off saved file
    }

    public bool AddItem(Item item)
    {
        //No duplicates currently
        if (item.Count == 0)
        {
            return false;
        }

        int ind;
        if ((ind = items.FindIndex(i => i.IDEqual(item))) != -1)
        {
            items[ind].AddTo(item.Count);
            return true;
        }
        else
        {
            items.Add(item);
            return true;
        }
    }
    public bool RemoveItem(string id, int count)
    {
        //return true if items were all removed
        //can handle duplicates
        int toRemove = count;
        foreach (Item i in items)
        {
            if (i.IDEqual(id))
            {
                int amt = i.Count >= toRemove ? toRemove : i.Count;
                i.RemoveFrom(amt);
                toRemove -= amt;
                if (i.Count == 0)
                {
                    items.Remove(i);
                    continue;
                }
            }
        }
        if (toRemove == 0)
        {
            return true;
        }
        return false;
    }

    public int GetCountOfItem(string id)
    {
        int count = 0;
        foreach (Item i in items)
        {
            if (i.IDEqual(id))
            {
                count += i.Count;
            }
        }
        return count;
    }

    public List<Item> GetItems()
    {
        return items;
    }

}