using Monogaming.Entities;
using System.Linq;
using System.Text.Json.Serialization;

namespace Monogaming.Items;


public class Inventory
{
    public int MaxStackCount { get; private set; }
    [JsonIgnore]
    public List<Item> Items { get; private set; }

    [JsonIgnore]
    public bool HasChanged { get; private set; }
    private bool changedlastframe = false;


    [JsonInclude]
    [JsonPropertyName("Items")]
    public JsonItem[] SerializedItems
    {
        get => Items.Select(t => new JsonItem(t.ID, t.Count)).ToArray();
    }

    [JsonIgnore]
    public int StackCount { get => Items.Count(); }

    [JsonConstructor]
    public Inventory(int MaxStackCount, JsonItem[] SerializedItems)
    {
        this.MaxStackCount = MaxStackCount;
        this.Items = SerializedItems.Select(t => ItemDict.GetNewItem(t.ItemID, t.Count)).ToList();
    }

    private Inventory(List<Item> items, int MaxStackCount)
    {
        this.Items = items;
        this.MaxStackCount = MaxStackCount;
    }

    public static Inventory Union(Inventory i, Inventory j)
    {
        return new Inventory(i.Items.Union(j.Items).ToList(), i.MaxStackCount + j.MaxStackCount);
    }

    public Inventory Copy()
    {
        JsonItem[] cpy = new JsonItem[SerializedItems.Length];
        for (int i = 0; i < SerializedItems.Length; i++)
        {
            cpy[i] = new JsonItem(SerializedItems[i].ItemID, SerializedItems[i].Count);
        }
        return new Inventory(MaxStackCount, cpy);
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

    public bool CanStoreItems(List<Item> items)
    {
        int totalNewTaken = 0;

        foreach (Item item in items)
        {
            List<Item> matches = Items.FindAll(i => i.IDEqual(item));

            int matchesInd = 0;
            int itemCount = item.Count;
            while (itemCount != 0 && matchesInd < matches.Count)
            {
                int diff = item.MaxStackSize - matches[matchesInd].Count;
                diff = diff > item.Count ? item.Count : diff;

                itemCount -= diff;

                matchesInd++;
            }

            while (itemCount != 0 && Items.Count < MaxStackCount)
            {
                int stackCnt = item.MaxStackSize < itemCount ? item.MaxStackSize : itemCount;
                itemCount -= stackCnt;
                totalNewTaken++;
            }
        }

        return totalNewTaken <= (MaxStackCount - Items.Count);
    }


    /// <summary>
    /// Adds items to inventory, note that the count of the item object added
    /// can be greater than MaxStackCount. Returns false on fail to add all
    /// WARNING: MODIFIES STATE OF ITEM
    /// </summary>
    public bool AddItem(Item item)
    {
        if (item.Count == 0)
        {
            return false;
        }

        int removalCount = 0;

        //try adding to the existing item stacks first
        List<Item> matches = Items.FindAll(i => i.IDEqual(item) && i != item);

        int matchesInd = 0;
        while (item.Count - removalCount != 0 && matchesInd < matches.Count)
        {
            int diff = item.MaxStackSize - matches[matchesInd].Count;
            diff = diff > (item.Count - removalCount) ? (item.Count - removalCount) : diff;

            matches[matchesInd].AddTo(diff);
            removalCount += diff;

            matchesInd++;
        }
        //make new stacks otherwise :P
        //the below if is implied by the following but its useful to think of as that
        // if (item.Count != 0)
        // {
        while ((item.Count - removalCount) != 0 && Items.Count < MaxStackCount)
        {
            int stackCnt = item.MaxStackSize < (item.Count - removalCount) ? item.MaxStackSize : item.Count - removalCount;
            Items.Add(item.Copy(stackCnt));
            removalCount += stackCnt;
        }
        // }

        HasChanged = true;

        ClearEmpties();
        item.RemoveFrom(removalCount);

        if (item.Count != 0)
        {
            return false;
        }

        return true;
    }
    public void ClearEmpties()
    {
        for (int i = Items.Count - 1; i >= 0; i--)
        {
            if (Items[i].Count == 0)
            {
                Items.RemoveAt(i);
                HasChanged = true;
                continue;
            }
        }
    }

    public Item TakeItemFrom(int ind)
    {
        Item i = Items[ind];
        Items.Remove(i);
        return i;
    }

    public bool RemoveItem(string id, int count)
    {
        //return true if items were all removed
        //can handle duplicates
        int toRemove = count;
        for (int j = Items.Count - 1; j >= 0; j--)
        {
            Item i = Items[j];
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
        HasChanged = true;
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

    public void Update(GameTime gameTime)
    {
        //update items here in case any are animated?

        //keeping this true for one extra frame in case a window that needed it
        //was only updated before haschanged was set which would stink
        //i think this is a bad workaround and could probably be refactored bc its
        //inefficient as windows might get updated more then once (TODO) but its ok for now
        if (changedlastframe)
        {
            HasChanged = false;
            changedlastframe = false;
        }
        if (HasChanged)
        {
            changedlastframe = true;
        }


    }

}