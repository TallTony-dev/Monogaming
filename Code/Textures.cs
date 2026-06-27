using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Monogaming;

public static class Textures
{
    private static Dictionary<string, Texture2D> loadedTextures2D = new();

    private static bool are2DTexturesLoaded;

    public static Texture2D Get2DTex(string textureName)
    {
        if (!loadedTextures2D.ContainsKey(textureName))
        {
            throw new ArgumentException($"Texture with name '{textureName}' not found in loadedTextures.");
        }

        return loadedTextures2D[textureName];
    }


    private static readonly string[] texturesToLoad =
    {
        "Icon"
    };

    public static void LoadTextures(ContentManager content)
    {
        if (!are2DTexturesLoaded)
        {
            foreach (string s in texturesToLoad)
            {
                loadedTextures2D.Add(s, content.Load<Texture2D>(s));
            }
            are2DTexturesLoaded = true;
        }
        else
        {
            Console.WriteLine("Attempted to load textures multiple times");
        }
    }
}
