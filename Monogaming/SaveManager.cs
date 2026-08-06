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
using System.Diagnostics.Metrics;
using Monogaming.MapThings;
using Monogaming.PlayerThings;

namespace Monogaming;


public static class SaveManager
{
    private static Dictionary<string, bool> flags = new();

    public static void SetFlag(string flag, bool value)
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


    public static void LoadSave(string path)
    {
        
    }

    public static void Save(string path, GameState gameState)
    {

        string text = JsonSerializer.Serialize(flags);
    }

}