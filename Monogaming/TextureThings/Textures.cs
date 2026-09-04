using AsepriteDotNet;
using AsepriteDotNet.Aseprite;
using AsepriteDotNet.Aseprite.Types;
using AsepriteDotNet.Common;
using AsepriteDotNet.IO;
using AsepriteDotNet.Processors;
using Assimp.Unmanaged;
using Microsoft.Xna.Framework.Content;
using MonoGame.Effect;
using Monogaming.Interfaces;
using Monogaming.Items;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;

namespace Monogaming.TextureThings;

public static class Textures
{

    //wow this was all unnessecary and i couldve just used https://github.com/AristurtleDev/monogame-aseprite );

    //hold the phone this is decoupled so I can make my ImTexture impl be this instead!!! HOPE RESTORED

    public const string DefaultTextureID = "Sprite-0006.Idle";

    public static SpriteFont MainFont { get; private set; }
    public static SpriteFont MainFontBold { get; private set; }

    public static Texture2D BlankTexture { get; private set; }

    private static GraphicsDevice graphicsRef;

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


    public static void SetRenderTarget(RenderTarget2D target)
    {
        graphicsRef.SetRenderTarget(target);
        graphicsRef.Clear(Color.Transparent);
    }
    public static void ResetRanderTarget()
    {
        graphicsRef.SetRenderTarget(null);
    }
    public static RenderTarget2D GetRenderTarget2D(int width, int height)
    {
        return new RenderTarget2D(graphicsRef, width, height);
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

        return new AtlasTex(BlankTexture, new Microsoft.Xna.Framework.Rectangle(0,0,1,1));
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
        try
        {
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
        Microsoft.Xna.Framework.Rectangle rect = ConvertRect(element.Frame);
        AtlasTex tex = new AtlasTex(loadedTextures2D[texture2DKey], rect);
        return tex;
    }
    private static Microsoft.Xna.Framework.Rectangle ConvertRect(Rect badRect)
    {
        return new Microsoft.Xna.Framework.Rectangle(
            badRect.X,
            badRect.Y,
            badRect.W,
            badRect.H
        );
    }
    private static Microsoft.Xna.Framework.Rectangle ConvertRect(AsepriteDotNet.Common.Rectangle badRect)
    {
        return new Microsoft.Xna.Framework.Rectangle(
            badRect.X,
            badRect.Y,
            badRect.Width,
            badRect.Height
        );
    }


    const string DefaultPath = "./Content/GameData/Textures/";
    public static void Initialize(ContentManager content, GraphicsDevice graphics)
    {
        graphicsRef = graphics;
        MainFont = content.Load<SpriteFont>("PixelFont");
        MainFontBold = content.Load<SpriteFont>("PixelFontBold");
        BlankTexture = new Texture2D(graphics, 1, 1);
        BlankTexture.SetData(new[] { Color.White });

        foreach (string path in Directory.EnumerateFiles(DefaultPath, "*.json"))
        {
            LoadTextures(content, path);
        }
        foreach (string path in Directory.EnumerateFiles(DefaultPath + "Tiles/", "*.json"))
        {
            LoadTextures(content, path, true);
        }
        foreach (string path in Directory.EnumerateFiles(DefaultPath, "*.aseprite"))
        {
            LoadTexturesAseprite(graphics, path, false);
        }
        foreach (string path in Directory.EnumerateFiles(DefaultPath + "Tiles/", "*.aseprite"))
        {
            LoadTexturesAseprite(graphics, path, true);
        }
    }
    private static void LoadTexturesAseprite(GraphicsDevice graphics, string path, bool isTiles)
    {
        try
        {
            int loadCount = 0;

            AsepriteFile file = AsepriteFileLoader.FromFile(path);
            TextureAtlas atlas = TextureAtlasProcessor.Process(file, true);
            SpriteSheet spriteSheet = SpriteSheetProcessor.Process(file, true);

            string title = file.Name;

            if (!loadedTextures2D.ContainsKey(title))
            {
                try
                {
                    ReadOnlySpan<Rgba32> pixels = atlas.Texture.Pixels;
                    Texture2D texture = new Texture2D(graphics, atlas.Texture.Size.Width, atlas.Texture.Size.Height);
                    texture.SetData(pixels.ToArray());
                    loadedTextures2D.Add(title, texture);
                }
                catch (Exception ex)
                {
                    Debug.WriteError($"Failed to load aseprite file {title} from contentloader in file {path} with err {ex.Message}");
                    return;
                }
            }


            foreach (var tag in spriteSheet.Tags)
            {
                if (!isTiles)
                {
                    if (tag.Frames.Length > 1)
                    {
                        List<(AtlasTex tex, float delay)> texs = new();
                        foreach (var frame in tag.Frames)
                        {
                            texs.Add((new AtlasTex(loadedTextures2D[title], ConvertRect(spriteSheet.TextureAtlas.Regions[frame.FrameIndex].Bounds)),
                                (float)frame.Duration.TotalMilliseconds));
                        }

                        loadedAnimatedTexs.Add($"{title}.{tag.Name}", new TextureAnimation(texs));
                        loadCount++;
                    }
                    else
                    {
                        var tex = new AtlasTex(loadedTextures2D[title], ConvertRect(spriteSheet.TextureAtlas.Regions[tag.Frames[0].FrameIndex].Bounds));
                        loadedAtlasTexs.Add($"{title}.{tag.Name}", tex);
                        loadCount++;
                    }
                }
                else
                {
                    if (tag.Frames.Length > 1)
                    {
                        Debug.WriteError($"Got tile tag with more than one frame which isnt supported currently. Found {tag.Frames.Length} tiles.");
                        continue;
                    }
                    //foreach as eventually this could maybe do animated tiles if wanted though tiletex would need changes too
                    foreach (var frame in tag.Frames)
                    {
                        List<Microsoft.Xna.Framework.Rectangle> sourceRects = new();
                        var tileSetRect = ConvertRect(spriteSheet.TextureAtlas.Regions[frame.FrameIndex].Bounds);
                        for (int row = 0; row < 4; row++)
                        {
                            for (int col = 0; col < 6; col++)
                            {
                                sourceRects.Add(new Microsoft.Xna.Framework.Rectangle(col * 16 + tileSetRect.X, row * 16 + tileSetRect.Y, 16, 16));
                            }
                        }

                        loadedTileTexs.Add($"{title}.{tag.Name}",
                            new TileTex(loadedTextures2D[title], sourceRects));
                        loadCount++;
                    }
                }
            }
            Debug.WriteInfo($"Loaded {loadCount} aseprite textures from {title}");
        }
        catch (Exception ex)
        {
            Debug.WriteError($"Failed to load aseprite textureFile with name {path} with exception {ex.Message}");
        }
    }

    /// <summary>
    /// Prefer usage of loading aseprite files instead of this method
    /// </summary>
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
                try
                {
                    loadedTextures2D.Add(title, content.Load<Texture2D>(title));
                }
                catch (Exception ex)
                {
                    Debug.WriteError($"Failed to load file {title} from contentloader in file {path} with err {ex.Message}");
                }
            }

            Dictionary<(string texType, int frameNum), FrameInfo> frameInfos = new();
            //each texture property within the texture file object
            foreach (JsonProperty frame in frames.EnumerateObject())
            {

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
                    if (!isTiles)
                    {
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
                        List<Microsoft.Xna.Framework.Rectangle> sourceRects = new();
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
