using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Json.Nodes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Monogaming.Interfaces;
using Monogaming.Items;
using Microsoft.VisualBasic;

namespace Monogaming.TextureThings;

public static class Textures
{

    //wow this was all unnessecary and i couldve just used https://github.com/AristurtleDev/monogame-aseprite );

    public const string DefaultTextureID = "Sprite-0006.Idle";

    public static SpriteFont MainFont { get; private set; }
    public static Texture2D BlankTexture { get; private set; }

    /// <summary>
    /// Has actual loaded texture handles
    /// </summary>
    private static Dictionary<string, Texture2D> loadedTextures2D = new();
    /// <summary>
    /// Holds references to loadedTextures2D along with json rect data
    /// </summary>
    private static Dictionary<string, AtlasTex> loadedAtlasTexs = new();
    
    private static Dictionary<string, TextureAnimation> loadedAnimatedTexs = new();
    private static Dictionary<string, TileTex> loadedTileTexs = new();

    public static bool TextureExists(string textureName)
    {
        return loadedAtlasTexs.ContainsKey(textureName) || 
        loadedAnimatedTexs.ContainsKey(textureName);
    }
    /// <summary>
    /// Gets a reference to a loaded textureatlastex or tile tex, or a copy of an animated texture
    /// </summary>
    public static ImTexture GetTexture(string textureName)
    {
        try
        {
            if (loadedAtlasTexs.ContainsKey(textureName))
            {
                return loadedAtlasTexs[textureName].Copy();
            }
            else if (loadedAnimatedTexs.ContainsKey(textureName))
            {
                return loadedAnimatedTexs[textureName].Copy();
            }
            else if (loadedTileTexs.ContainsKey(textureName))
            {
                return loadedTileTexs[textureName].Copy();
            }
            else
            {
                throw new ArgumentException($"Texture with name '{textureName}' not found in loadedImTexs.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteError($"Failed to load texture {textureName} with exception {ex.Message}");
        }

        return loadedAnimatedTexs[DefaultTextureID];
    }
    private static AtlasTex Get2DTex(string textureName)
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
    private static TextureAnimation GetAnimatedTexture(string textureName)
    {
        try {
            if (!loadedAnimatedTexs.ContainsKey(textureName))
            {
                throw new ArgumentException($"Texture with name '{textureName}' not found in loadedImTexs.");
            }
            return (TextureAnimation)loadedAnimatedTexs[textureName].Copy();
        }
        catch (Exception ex)
        {
            Debug.WriteError($"Failed to load animTexture {textureName} with exception {ex.Message}");
        }
        return null;
    }
    public static Texture2D GetRawTex(string textureID)
    {
        if (loadedTextures2D.ContainsKey(textureID))
        {
            return loadedTextures2D[textureID];
        }
        else
        {
            Debug.WriteError($"Tried to get raw texture {textureID} but failed");
            return null;
        }
    }
    private static AtlasTex ParseTex(FrameInfo element, string texture2DKey)
    {
        Rectangle rect = ConvertRect(element.Frame);
        AtlasTex tex = new AtlasTex(loadedTextures2D[texture2DKey], rect);
        return tex;
    }
    private static Rectangle ConvertRect(Rect badRect)
    {
        return new Rectangle(
            badRect.X,
            badRect.Y,
            badRect.W,
            badRect.H
        );
    }


    const string DefaultPath = "./GameData/Textures/";
    public static void Initialize(ContentManager content, GraphicsDevice graphics)
    {
        MainFont = content.Load<SpriteFont>("default");
        BlankTexture = new Texture2D(graphics, 1, 1);
        BlankTexture.SetData(new[] { Color.White });
        
        foreach(string path in Directory.EnumerateFiles(DefaultPath, "*.json"))
        {
            LoadTextures(content, path);
        }
        foreach(string path in Directory.EnumerateFiles(DefaultPath + "Tiles/", "*.json"))
        {
            LoadTextures(content, path, true);
        }
    }

    private static void LoadTextures(ContentManager content, string path, bool isTiles = false)
    {
        try
        {
            Stream textStream = File.OpenRead(path);
            JsonDocument document = JsonDocument.Parse(textStream);
            JsonElement root = document.RootElement;

            JsonElement meta = root.GetProperty("meta");
            string title = meta.GetProperty("image").GetString();
            title = title.Split('.')[0]; //trim extension

            int loadCount = 0;


            JsonElement frames = root.GetProperty("frames");

            if (!loadedTextures2D.ContainsKey(title))
            {
                try {
                    loadedTextures2D.Add(title, content.Load<Texture2D>(title));
                }
                catch (Exception ex)
                {
                    Debug.WriteError($"Failed to load file {title} from contentloader in file {path} with err {ex.Message}");
                }
            }

            Dictionary<(string texType, int frameNum), FrameInfo> frameInfos = new();
            //each texture property within the texture file object
            foreach (JsonProperty frame in frames.EnumerateObject()) {
 
                string[] texNameSplit = frame.Name.Split('.');
                string texName = texNameSplit[0];
                string texType = texNameSplit[1];
                int frameNum = 0;
                if (texNameSplit.Length > 2)
                {
                    frameNum = Convert.ToInt32(texNameSplit[2]);
                }

                FrameInfo frameInfo = JsonSerializer.Deserialize<FrameInfo>(frame.Value);
                
                //if is animated, loop through subobjs again to get rects
                frameInfos.Add((texType, frameNum), frameInfo);

            }

            var groups = frameInfos.GroupBy(t => t.Key.texType);
            //group them by type
            foreach (var group in groups)
            {
                group.OrderBy(t => t.Key.frameNum);

                if (group.Count() > 1)
                {
                    if (!isTiles) {
                        List<(AtlasTex tex, float delay)> texs = new();
                        
                        foreach (var item in group)
                        {
                            //is ascending from previous ordering
                            texs.Add((ParseTex(item.Value, title), item.Value.Duration));
                        }

                        loadedAnimatedTexs.Add($"{title}.{group.First().Key.texType}", 
                        new TextureAnimation(texs));
                        loadCount++;
                    }
                    else
                    {
                        List<Rectangle> sourceRects = new();
                        foreach (var item in group)
                        {
                            sourceRects.Add(ConvertRect(item.Value.Frame));
                        }
                        loadedTileTexs.Add($"{group.First().Key.texType}", 
                        new TileTex(loadedTextures2D[title], sourceRects));
                        loadCount++;
                    }

                }
                else
                {
                    loadedAtlasTexs.Add($"{title}.{group.First().Key.texType}", 
                    ParseTex(group.First().Value, title));
                    loadCount++;
                }
            }

            Debug.WriteInfo($"Parsed/Loaded {loadCount} textures from TextureBook {title}");
        }
        catch (Exception ex)
        {
            Debug.WriteError($"Failed to load textureFile with name {path} with exception {ex.Message}");
        }

    }



    private class FrameInfo
    {
        [JsonPropertyName("frame")]
        public Rect Frame { get; set; }

        [JsonPropertyName("rotated")]
        public bool Rotated { get; set; }

        [JsonPropertyName("trimmed")]
        public bool Trimmed { get; set; }

        [JsonPropertyName("spriteSourceSize")]
        public Rect SpriteSourceSize { get; set; }

        [JsonPropertyName("sourceSize")]
        public Size SourceSize { get; set; }

        [JsonPropertyName("duration")]
        public int Duration { get; set; }
    }

    private class Rect
    {
        [JsonPropertyName("x")] public int X { get; set; }
        [JsonPropertyName("y")] public int Y { get; set; }
        [JsonPropertyName("w")] public int W { get; set; }
        [JsonPropertyName("h")] public int H { get; set; }
    }

    private class Size
    {
        [JsonPropertyName("w")] public int W { get; set; }
        [JsonPropertyName("h")] public int H { get; set; }
    }
}
