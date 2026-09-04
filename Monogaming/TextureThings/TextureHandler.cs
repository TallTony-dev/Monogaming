using Monogaming.Interfaces;
using System.Text.Json.Serialization;

namespace Monogaming.TextureThings;

public class TextureHandler
{
    private ImTexture currentTexture;
    private ImTexture lightTexture;
    public bool ShouldDrawLight { get; set; } = true;
    public int LightRadius { get; set; } = 0;
    public TextureType CurrentTextureType { get; private set; }
    public Dictionary<TextureType, string> textures { get; private set; } //mix of keys to animated and static textures

    //TODO: clean this up a little lot to be more efficient with new texture loader

    [JsonConstructor]
    public TextureHandler(Dictionary<TextureType, string> textures, int lightRadius)
    {
        LightRadius = lightRadius;
        this.textures = textures;
        currentTexture = Textures.GetTexture(textures[TextureType.Default]);
        if (textures.ContainsKey(TextureType.Light))
        {
            lightTexture = Textures.GetTexture(textures[TextureType.Light]);
            if (lightTexture == null)
            {
                throw new Exception($"Tried to load {textures[TextureType.Light]} as light in texturehandler but couldnt find texture with name.");
            }
        }
    }

    public TextureHandler(TextureType type, string texID)
    {
        textures = new();
        if (!Textures.TextureExists(texID))
        {
            throw new JsonException($"Texture {texID} is not loaded");
        }
        textures.Add(type, texID);
        currentTexture = Textures.GetTexture(texID);
        if (textures.ContainsKey(TextureType.Light))
        {
            lightTexture = Textures.GetTexture(textures[TextureType.Light]);
            if (lightTexture == null)
            {
                throw new Exception($"Tried to load {textures[TextureType.Light]} as light in texturehandler but couldnt find texture with name.");
            }
        }
    }



    public TextureHandler Copy()
    {
        return new TextureHandler(new Dictionary<TextureType, string>(textures), LightRadius);
    }

    public void Update(GameTime gameTime)
    {
        if (currentTexture.IsAnimated)
        {
            ((TextureAnimation)currentTexture).Update(gameTime);
        }
    }


    public void Draw(SpriteBatch batch, Vector2 pos)
    {
        currentTexture.Draw(batch, pos);
    }
    public void Draw(SpriteBatch batch, Rectangle dest)
    {
        currentTexture.Draw(batch, dest);
    }
    public void Draw(SpriteBatch batch, Vector2 pos, Color color)
    {
        currentTexture.Draw(batch, pos, color);
    }
    public void Draw(SpriteBatch batch, Rectangle dest, Color color)
    {
        currentTexture.Draw(batch, dest, color);
    }

    public void TryDrawLight(SpriteBatch batch, Rectangle dest)
    {
        if (ShouldDrawLight)
        {
            dest.Inflate(LightRadius, LightRadius);
            lightTexture?.Draw(batch, dest);
        }
    }
    public void TryDrawLight(SpriteBatch batch, Rectangle dest, Color color)
    {
        if (ShouldDrawLight)
        {
            dest.Inflate(LightRadius, LightRadius);
            lightTexture?.Draw(batch, dest, color);
        }
    }

    public void SetTexture(TextureType textureType)
    {
        if (CurrentTextureType != textureType)
        {
            if (textures.ContainsKey(textureType))
            {
                //creates a copy if animated so works good
                currentTexture = Textures.GetTexture(textures[textureType]);
                CurrentTextureType = textureType;
            }
            else
            {
                Debug.WriteWarn($"Tried to set to nonexistent textureType {textureType}, leaving at current texture");
            }
        }
    }


    public enum TextureType { Default, Idle, Walking, Running, OnHover, OnClick, Light }
}