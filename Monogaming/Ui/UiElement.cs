using System;
using Monogaming.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monogaming.Interfaces;
using Monogaming.TextureThings;
using Microsoft.Xna.Framework.Content;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using Apos.Input;
using Monogaming.PlayerThings;

namespace Monogaming.Ui;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "ElementType")]
[JsonDerivedType(typeof(UiElement), typeDiscriminator: "baseElement")]
[JsonDerivedType(typeof(TextureElement), typeDiscriminator: "textureElement")]
[JsonDerivedType(typeof(TextElement), typeDiscriminator: "textElement")]
[JsonDerivedType(typeof(ButtonElement), typeDiscriminator: "buttonElement")]

public class UiElement : ImUpdatable
{
    [JsonIgnore]
    public bool IsVisible { get; private set; } = true;
    [JsonIgnore]
    public bool IsActive { get; private set; } = true;
    
    public string ID { get; private set; }
    public Hitbox DrawBox { get; protected set; }
    public List<UiElement> Children { get; protected set; }


    [JsonConstructor]
    public UiElement(string ID, Hitbox DrawBox, List<UiElement> Children)
    {
        this.DrawBox = DrawBox;
        this.ID = ID;
        this.Children = Children != null ? Children : new List<UiElement>();
    }

    /// <summary>
    /// Recursively searches for a subelement, where path is dot seperated element IDs,
    /// eg "buttonPanel.startButton"
    /// </summary>
    public UiElement GetChild(string searchPath)
    {
        if (!string.IsNullOrEmpty(searchPath))
        {
            return Children.Find(t => t.ID == searchPath.Substring(0, searchPath.IndexOf('.')))
            .GetChild(searchPath.Substring(searchPath.IndexOf('.') + 1, searchPath.Length - 1));
        }
        else
        {
            return this;
        }
    }

    public void ActivateElement()
    {
        IsActive = true;
        IsVisible = true;
        foreach (UiElement child in Children)
        {
            child.ActivateElement();
        }
    }

    public void DeactivateElement()
    {
        IsActive = false;
        IsVisible = false;
        foreach (UiElement child in Children)
        {
            child.DeactivateElement();
        }
    }

    public virtual void Update(GameTime gameTime)
    {
        foreach(UiElement child in Children)
        {
            if (child.IsActive)
            {
                child.Update(gameTime);
            }
        }
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        foreach(UiElement child in Children)
        {
            if (child.IsVisible)
            {
                child.Draw(spriteBatch);
            }
        }
    }

}