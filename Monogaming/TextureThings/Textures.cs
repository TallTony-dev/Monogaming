using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Monogaming.Interfaces;

namespace Monogaming.TextureThings;

public static class Textures
{

    //TODO: Make these load from a given json file (coretextures) instead of being hardcoded!!

    //have 



    /// <summary>
    /// Has actual loaded texture handles
    /// </summary>
    private static Dictionary<string, Texture2D> loadedTextures2D = new();
    /// <summary>
    /// Holds references to loadedTextures2D along with json rect data
    /// </summary>
    private static Dictionary<string, AtlasTex> loadedAtlasTexs = new();
    
    private static Dictionary<string, AnimatedTexture> loadedAnimatedTexs = new();

    public static AtlasTex Get2DTex(string textureName)
    {
        try
        {
            if (!loadedAtlasTexs.ContainsKey(textureName))
            {
                throw new ArgumentException($"Texture with name '{textureName}' not found in loadedImTexs.");
            }
            return loadedAtlasTexs[textureName];
        }
        catch (Exception ex)
        {
            Debug.WriteError($"Failed to load texture {textureName} with exception {ex.Message}");
        }
        return null;
    }
    public static AnimatedTexture GetAnimatedTexture(string textureName)
    {
        try {
            if (!loadedAnimatedTexs.ContainsKey(textureName))
            {
                throw new ArgumentException($"Texture with name '{textureName}' not found in loadedImTexs.");
            }
            return loadedAnimatedTexs[textureName].Copy();
        }
        catch (Exception ex)
        {
            Debug.WriteError($"Failed to load animTexture {textureName} with exception {ex.Message}");
        }
        return null;
    }

    private static AtlasTex ParseTex(JsonElement element, string texture2DKey)
    {
        JsonElement rectElement = element.GetProperty("Rect");
        Rectangle rect = new Rectangle(
            rectElement.GetProperty("X").GetInt32(),
            rectElement.GetProperty("Y").GetInt32(),
            rectElement.GetProperty("Width").GetInt32(),
            rectElement.GetProperty("Height").GetInt32()
        );
        AtlasTex tex = new AtlasTex(loadedTextures2D[texture2DKey], rect);
        return tex;
    }

    public static void LoadTextures(ContentManager content, string fileName)
    {
        try
        {
            string path = "./GameData/" + fileName + ".json";

            Stream textStream = File.OpenRead(path);
            JsonDocument document = JsonDocument.Parse(textStream);
            JsonElement root = document.RootElement;

            string name = root.GetProperty("TextureBookName").GetString();


            int loadCount = 0;
            foreach (JsonProperty prop in root.EnumerateObject()) {
                if (!(prop.Value.ValueKind == JsonValueKind.Object))
                {
                    continue;
                }
                JsonElement texFileObj = prop.Value;

                string texFileName = texFileObj.GetProperty("FileName").GetString();
                if (!loadedTextures2D.ContainsKey(texFileName))
                {
                    loadedTextures2D.Add(texFileName, content.Load<Texture2D>(texFileName));
                }

                //each texture property within the texture file object
                foreach (JsonProperty texProp in texFileObj.EnumerateObject()) {
                    if (!(texProp.Value.ValueKind == JsonValueKind.Object))
                    {
                        continue;
                    }
                    JsonElement texObj = texProp.Value;
                    string imTexName = texProp.Name;
                    bool isAnimated = texObj.GetProperty("Animated").GetBoolean();

                    //if is animated, loop through subobjs again to get rects
                    if (isAnimated)
                    {
                        List<(AtlasTex tex, float delay, int order)> texs = new();

                        foreach (JsonProperty animTexProp in texObj.EnumerateObject())
                        {
                            if (animTexProp.Value.ValueKind == JsonValueKind.Object) {
                                texs.Add((ParseTex(animTexProp.Value, texFileName), animTexProp.Value.GetProperty("Delay").GetSingle(),
                                animTexProp.Value.GetProperty("Order").GetInt32()));
                            }
                        }
                        List<(AtlasTex tex, float delay)> textures = 
                        texs.OrderBy(t => t.order).Select(t => (t.tex, t.delay)).ToList();
                        loadedAnimatedTexs.Add(imTexName, new AnimatedTexture(textures));
                        loadCount++;
                    }
                    else
                    {
                        loadedAtlasTexs.Add(imTexName, ParseTex(texObj, texFileName));
                        loadCount++;
                    }

                }

            }
            Debug.WriteInfo($"Parsed/Loaded {loadCount} textures from TextureBook {name}");
        }
        catch (Exception ex)
        {
            Debug.WriteError($"Failed to load textureFile with name {fileName} with exception {ex.Message}");
        }

    }
}
