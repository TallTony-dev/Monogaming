using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monogaming.Entities;
using Monogaming.TextureThings;
using Monogaming.Ui;
using Apos.Input;
using Monogaming.Interfaces;
using System.IO;
using System.Text.Json;
using Monogaming.MapThings;
using Monogaming.PlayerThings;
using System.Text.Json.Serialization;

namespace Monogaming;


public class Save
{
    [JsonInclude]
    private Dictionary<string, bool> flags;
    [JsonInclude]
    private Dictionary<string, bool> itemsFound;
    public string Name { get; private set; }
    public PlayerPos PlayerSavePos { get; private set; }


    [JsonConstructor]
    public Save(string Name, PlayerPos PlayerSavePos, Dictionary<string, bool> flags, Dictionary<string, bool> itemsFound)
    {
        this.Name = Name;
        this.flags = flags;
        this.itemsFound = itemsFound;
        this.PlayerSavePos = PlayerSavePos;
        if (flags == null)
        {
            this.flags = new();
        }
        if (itemsFound == null)
        {
            this.itemsFound = new();
        }
    }

    public void SetPlayerSavePos(PlayerPos pos)
    {
        PlayerSavePos = pos;
    }
    public bool IsItemFound(string itemID)
    {
        if (itemsFound.ContainsKey(itemID))
        {
            return itemsFound[itemID];
        }
        return false;
    }
    public void SetItemFound(string itemID)
    {
        if (itemsFound.ContainsKey(itemID))
        {
            itemsFound[itemID] = true;
        }
        else
        {
            itemsFound.Add(itemID, true);
        }
    }
    
    public void SetFlag(string flag, bool value)
    {
        if (flags.ContainsKey(flag))
        {
            flags[flag] = value;
        }
        else
        {
            Debug.WriteWarn($"Set new flag {flag} to value {value}");
            flags.Add(flag, value);
        }
    }
    public struct PlayerPos
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string ChunkName { get; set; }

        [JsonConstructor]
        public PlayerPos(int x, int y, string chunkName)
        {
            X = x;
            Y = y;
            ChunkName = chunkName;
        }
    }
}