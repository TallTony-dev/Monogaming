using System;
using Monogaming.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monogaming.Interfaces;
using Monogaming.TextureThings;
using System.Text.Json;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Linq;

namespace Monogaming.TextureThings;

public class TextureHandler
{
    private ImTexture currentTexture;
    public TextureType CurrentTextureType { get; private set; }

    public Dictionary<TextureType, string> textures { get; private set; } //mix of keys to animated and static textures

    
    [JsonConstructor]
    public TextureHandler(Dictionary<TextureType, string> textures)
    {
        this.textures = textures;
        SetTexture(TextureType.Default);
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
    }



    public TextureHandler Copy()
    {
        return new TextureHandler(new Dictionary<TextureType, string>(textures));
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

    public void SetTexture(TextureType textureType)
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


    public enum TextureType { Default, Idle, Walking, Running, OnHover, OnClick }
}