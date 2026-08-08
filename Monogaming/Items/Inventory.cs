using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monogaming.Entities;

namespace Monogaming.Items;


public class Inventory
{
    public int MaxStackSize { get; private set; }
    public int MaxStackCount { get; private set; }
    [JsonIgnore]
    public List<Item> Items { get; private set; }
    [JsonInclude]
    [JsonPropertyName("Items")]
    public JsonItem[] SerializedItems
    {
        get => Items.Select(t => new JsonItem(t.ID, t.Count)).ToArray();
    }

    [JsonConstructor]
    public Inventory(int MaxStackSize, int MaxStackCount, JsonItem[] SerializedItems)
    {
        this.MaxStackSize = MaxStackSize;
        this.MaxStackCount = MaxStackCount;
        this.Items = SerializedItems.Select(t => ItemDict.GetNewItem(t.ItemID, t.Count)).ToList();
    }

    public Inventory Copy()
    {
        JsonItem[] cpy = new JsonItem[SerializedItems.Length];
        for (int i = 0; i < SerializedItems.Length; i++)
        {
            cpy[i] = new JsonItem(SerializedItems[i].ItemID, SerializedItems[i].Count);
        }
        return new Inventory(MaxStackSize, MaxStackCount, cpy);
    }

    public struct JsonItem
    {
        [JsonConstructor]
        public JsonItem(string ItemID, int Count)
        {
            this.ItemID = ItemID;
            this.Count = Count;
        }
        [JsonInclude]
        public string ItemID;
        [JsonInclude]
        public int Count;
    }

    public void TryTransferAllItemsFrom(Inventory toTakeFrom)
    {
        for (int i = toTakeFrom.Items.Count() - 1; i >= 0; i--)
        {
            AddItem(toTakeFrom.Items[i]);
        }
        toTakeFrom.CheckEmpty();
    }

    /// <summary>
    /// Adds items to inventory, note that the count of the item object added
    /// can be greater than MaxStackCount. Returns false on fail to add all
    /// WARNING: MODIFIES STATE OF ITEM
    /// </summary>
    public bool AddItem(Item item)
    {
        //No duplicates currently, and unordered set
        if (item.Count == 0)
        {
            return false;
        }
        //try adding to the existing item stacks first
        List<Item> matches = Items.FindAll(i => i.IDEqual(item));

        int matchesInd = 0;
        while (item.Count != 0 && matchesInd < matches.Count)
        {
            int diff = MaxStackSize - matches[matchesInd].Count;
            diff = diff > item.Count ? item.Count : diff;

            matches[matchesInd].AddTo(diff);
            item.RemoveFrom(diff);

            matchesInd++;
        }
        //make new stacks otherwise :P
        //the below if is implied by the following but its useful to think of as that
        // if (item.Count != 0)
        // {
            while (item.Count != 0 && Items.Count < MaxStackCount)
            {
                int stackCnt = MaxStackSize < item.Count ? MaxStackSize : item.Count;
                Items.Add(item.Copy(stackCnt));
                item.RemoveFrom(stackCnt);
            }
        // }

        if (item.Count != 0)
        {
            return false;
        }

        return true;
    }
    public bool RemoveItem(string id, int count)
    {
        //return true if items were all removed
        //can handle duplicates
        int toRemove = count;
        foreach (Item i in Items)
        {
            if (i.IDEqual(id))
            {
                int amt = i.Count >= toRemove ? toRemove : i.Count;
                i.RemoveFrom(amt);
                toRemove -= amt;
                if (i.Count == 0)
                {
                    Items.Remove(i);
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

    public List<ItemEntity> GetDroppedEntities(Hitbox pos)
    {
        //wowow linq so pretty
        return Items.Select(t => new ItemEntity(new Hitbox(pos), t, true)).ToList();
    }

    public void CheckEmpty()
    {
        for (int i = Items.Count() - 1; i >= 0; i--)
        {
            if (Items[i].Count == 0)
            {
                Items.RemoveAt(i);
            }
        }
    }
 
    public int GetCountOfItem(string id)
    {
        int count = 0;
        foreach (Item i in Items)
        {
            if (i.IDEqual(id))
            {
                count += i.Count;
            }
        }
        return count;
    }

    // public void Update(GameTime gameTime)
    // {
    //     //update items here in case any are animated?


    // }

    const int ItemPadding = 10;
    const int MaxItemDisplaySize = 40;
    const int MinItemDisplaySize = 20;
    // public void Draw(SpriteBatch batch, Rectangle pos)
    // {
    //     //determine best representation and draw it here
    //     int stackCount = Items.Count;
    //     float aspectRatio = pos.Width / pos.Height;



    //     int itemDisplaySize = Math.Clamp( pref, MinItemDisplaySize, MaxItemDisplaySize);

    //     //stackCount = ciel(rows * columns)

    //     int itemWidth = 

    //     throw new NotImplementedException();
    // }
}