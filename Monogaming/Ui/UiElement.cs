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
    [JsonIgnore]
    public Hitbox ScaledDrawBox { get; protected set; } //scaled to fit screen 
    [JsonIgnore]
    public Hitbox ParentOffset { get; set; }

    public string ID { get; private set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AnchorPoint Anchor { get; private set; }
    public List<UiElement> Children { get; protected set; }
    [JsonInclude]
    [JsonPropertyName("DrawBox")]
    public Hitbox DrawBox { get; private set; } // the actual pos (offset from anchor) as defined by Camera.ScreenWidth/Height



    [JsonConstructor]
    public UiElement(string ID, AnchorPoint Anchor, Hitbox DrawBox, List<UiElement> Children)
    {
        this.Anchor = Anchor;
        this.DrawBox = DrawBox;
        this.ID = ID;
        this.Children = Children != null ? Children : new List<UiElement>();
        this.ScaledDrawBox = DrawBox;
    }
    

    public static Hitbox OffsetHitbox(AnchorPoint anchor, Hitbox child, Rectangle parent) 
    {
        Vector2 retPos = Vector2.Zero;
        if (anchor == AnchorPoint.Center)
        {
            retPos.X = parent.X + (parent.Width - child.Width) / 2 + child.X;
            retPos.Y = parent.Y + (parent.Height - child.Height) / 2 + child.Y;
        }
        else
        {
            if (((int)anchor & 1) == 1)
            {
                //right anchored
                retPos.X = parent.Width + parent.X - child.X - child.Width;
            }
            else
            {
                //left anchored
                retPos.X = parent.X + child.X;
            }
            if ((int)anchor >= 2)
            {
                //bottom anchored
                retPos.Y = parent.Height + parent.Y - child.Y - child.Height;
            }
            else
            {
                retPos.Y = parent.Y + child.Y;
            }
        }

        return new Hitbox(retPos, child.Width, child.Height);
    }

    public void UpdatePos(GameWindow window)
    {
        if (DrawBox != null) {
            int ScreenWidth = Camera.ScreenWidth;
            int ScreenHeight = Camera.ScreenHeight;
            Rectangle realWindow = window.ClientBounds;
            realWindow.X = 0;
            realWindow.Y = 0;

            Vector2 scale = new Vector2(realWindow.Width / (float)ScreenWidth, realWindow.Height / (float)ScreenHeight);

            Hitbox scaled = new Hitbox(DrawBox.Pos * scale, (int)(DrawBox.Width * scale.X), (int)(DrawBox.Height * scale.Y));

            if (ParentOffset == null) {
                ScaledDrawBox = OffsetHitbox(Anchor, scaled, realWindow);
            }
            else
            {
                ScaledDrawBox = OffsetHitbox(Anchor, scaled, ParentOffset.AsRect());
            }
        }
        // else
        // {
        //     ScaledDrawBox = new Hitbox(window.ClientBounds);
        // }

        

        foreach (UiElement child in this.Children)
        {
            child.ParentOffset = this.ScaledDrawBox;
        }
        foreach (UiElement child in Children)
        {
            child.UpdatePos(window);
        }

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
            .GetChild(searchPath.Substring(searchPath.IndexOf('.') + 1, searchPath.Length - searchPath.IndexOf('.') - 1));
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

public enum AnchorPoint { TopLeft, TopRight, BottomLeft, BottomRight, Center }