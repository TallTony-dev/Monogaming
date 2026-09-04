using Monogaming.Entities;
using Monogaming.Interfaces;
using Monogaming.Ui.Windows;
using System.Linq;
using System.Text.Json.Serialization;

namespace Monogaming.Ui;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "ElementType")]
[JsonDerivedType(typeof(UiElement), typeDiscriminator: "baseElement")]
[JsonDerivedType(typeof(TextureElement), typeDiscriminator: "textureElement")]
[JsonDerivedType(typeof(TextElement), typeDiscriminator: "textElement")]
[JsonDerivedType(typeof(ButtonElement), typeDiscriminator: "buttonElement")]
[JsonDerivedType(typeof(TextInputElement), typeDiscriminator: "textInputElement")]
[JsonDerivedType(typeof(DropDownElement), typeDiscriminator: "dropDownElement")]
[JsonDerivedType(typeof(WindowElement), typeDiscriminator: "windowElement")]

public class UiElement : ImUpdatable
{
    [JsonIgnore]
    public bool IsVisible { get; protected set; } = false;
    [JsonIgnore]
    public bool IsActive { get; protected set; } = false;
    [JsonIgnore]
    public Hitbox ScaledDrawBox { get; protected set; } //scaled to fit screen 
    [JsonIgnore]
    public Hitbox ParentOffset { get; set; }
    [JsonIgnore]
    public Color DrawColor { get; set; } = Color.White;


    public string ID { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AnchorPoint Anchor { get; private set; }
    public List<UiElement> Children { get; protected set; }
    [JsonInclude]
    [JsonPropertyName("DrawBox")]
    public Hitbox DrawBox { get; set; } // the actual pos (offset from anchor) as defined by Camera.ScreenWidth/Height



    [JsonConstructor]
    public UiElement(string ID, AnchorPoint Anchor, Hitbox DrawBox, List<UiElement> Children)
    {
        this.Anchor = Anchor;
        this.DrawBox = DrawBox;
        this.ID = ID;
        this.Children = Children != null ? Children : new List<UiElement>();
        this.ScaledDrawBox = DrawBox;
    }

    // public UiElement(UiElement e)
    // {
    //     Anchor = e.Anchor;
    //     DrawBox = e.DrawBox;
    //     ID = e.ID;
    //     Children = e.Children.Select(t => new UiElement(t)).ToList();
    //     ScaledDrawBox = DrawBox;
    // }

    public bool DeleteElement(UiElement element)
    {
        foreach (UiElement child in Children)
        {
            if (UiElement.ReferenceEquals(element, child))
            {
                Children.Remove(child);
                return true;
            }
            else
            {
                child.DeleteElement(element);
            }
        }
        return false;
    }

    private static Hitbox OffsetHitbox(AnchorPoint anchor, Hitbox child, Rectangle parent)
    {
        Vector2 retPos = Vector2.Zero;
        Vector2 scale = UI.LiveUI.Scale;

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
                retPos.X = parent.Width + parent.X + child.X - child.Width;
            }
            else
            {
                //left anchored
                retPos.X = parent.X + child.X;
            }
            if ((int)anchor >= 2)
            {
                //bottom anchored
                retPos.Y = parent.Height + parent.Y + child.Y - child.Height;
            }
            else
            {
                retPos.Y = parent.Y + child.Y;
            }
        }

        return new Hitbox(retPos, (int)child.Width, (int)child.Height);
    }


    /// <summary>
    /// Scales the passed in box from ui coords to real coords
    /// </summary>
    public static Hitbox ScaleRealPos(Rectangle realWindow, Hitbox DrawBox)
    {
        Vector2 scale = UI.LiveUI.Scale;
        return new Hitbox(DrawBox.Pos * scale, (int)(DrawBox.Width * UI.LiveUI.UiScale), (int)(DrawBox.Height * UI.LiveUI.UiScale));
    }

    public virtual void UpdatePos(Rectangle window)
    {
        if (DrawBox != null)
        {
            Rectangle realWindow = window;

            Hitbox scaled = ScaleRealPos(realWindow, DrawBox);

            if (ParentOffset == null)
            {
                ScaledDrawBox = OffsetHitbox(Anchor, scaled, realWindow);
            }
            else
            {
                ScaledDrawBox = OffsetHitbox(Anchor, scaled, ParentOffset.ToRect());
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


    public void RoundRealPos()
    {
        DrawBox.CeilPos();
    }
    public void AdjustRealPos(Vector2 deltaOffset)
    {
        DrawBox.ChangePos(deltaOffset);
        //ScaledDrawBox.ChangePos(deltaOffset);
        UpdatePos(UI.LiveUI.gameWindowRef.ClientBounds with { X = 0, Y = 0 });
    }
    public void SetRealPos(Vector2 pos)
    {
        DrawBox.SetPos(pos);
        UpdatePos(UI.LiveUI.gameWindowRef.ClientBounds with { X = 0, Y = 0 });
    }
    public void AdjustRealDims(Vector2 delta)
    {
        DrawBox.ChangeDims(delta);
        UpdatePos(UI.LiveUI.gameWindowRef.ClientBounds with { X = 0, Y = 0 });
    }
    public void SetRealDims(Vector2 dims)
    {
        DrawBox.SetDims(dims);
        UpdatePos(UI.LiveUI.gameWindowRef.ClientBounds with { X = 0, Y = 0 });
    }

    /// <summary>
    /// Recursively searches for a subelement, where path is dot seperated element IDs,
    /// eg "buttonPanel.startButton"
    /// </summary>
    public UiElement GetChild(string searchPath)
    {
        if (!string.IsNullOrEmpty(searchPath))
        {
            int dotIndex = searchPath.IndexOf('.');

            UiElement found = null;
            try
            {
                if (dotIndex == -1)
                {
                    found = Children.Find(t => t.ID == searchPath.Substring(0, searchPath.Length));
                }
                else
                {
                    found = Children.Find(t => t.ID == searchPath.Substring(0, dotIndex))
                    .GetChild(searchPath.Substring(dotIndex + 1, searchPath.Length - dotIndex - 1));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteError($"Failed to find element at path {searchPath} with exception {ex.Message} and children {string.Join(',', Children.Select(t => t.ID))}");
            }

            if (found == null)
            {
                Debug.WriteError($"Failed to find element at path {searchPath}");
            }
            return found;
        }
        else
        {
            return this;
        }
    }

    /// <summary>
    /// Runs on the assumption of no overlapping non familially related elements
    /// Gets the element nearest the bottom of the tree that intersects a pos
    /// </summary>
    public virtual UiElement GetTopCollision(Vector2 pos)
    {
        foreach (UiElement child in Children)
        {
            UiElement element = child.GetTopCollision(pos);
            if (element != null)
            {
                return element;
            }
        }
        if (ScaledDrawBox != null && ScaledDrawBox.Intersects(pos))
        {
            return this;
        }
        return null;
    }
    public virtual void ActivateElement(bool force = false)
    {
        IsActive = true;
        IsVisible = true;
        foreach (UiElement child in Children)
        {
            child.ActivateElement(force);
        }
    }

    public virtual void DeactivateElement()
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
        foreach (UiElement child in Children)
        {
            if (child.IsActive)
            {
                child.Update(gameTime);
            }
        }
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        foreach (UiElement child in Children)
        {
            if (child.IsVisible)
            {
                child.Draw(spriteBatch);
            }
        }
    }

    /// <summary>
    /// Draw things on top of the ui ie held items and such
    /// </summary>
    public virtual void DrawTop(SpriteBatch spriteBatch)
    {
        foreach (UiElement child in Children)
        {
            if (child.IsVisible)
            {
                child.DrawTop(spriteBatch);
            }
        }
    }
}

public enum AnchorPoint { TopLeft, TopRight, BottomLeft, BottomRight, Center }

