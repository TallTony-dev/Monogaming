using System;
using Monogaming.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Monogaming.Interfaces;
using Monogaming.TextureThings;
using Microsoft.Xna.Framework.Content;
using System.Text.Json.Serialization;
using Monogaming.Items;
using Monogaming.Ui;
using Apos.Input;
using Microsoft.Xna.Framework.Input;

namespace Monogaming.Ui.Windows;



public class WindowElement : UiElement
{
    
    protected const int TopBarHeight = 20;
    const int CloseButtonPadding = 1;
    const int CloseButtonWidth = 30;
    const int MinWidth = 100;
    const int MinHeight = 60;


    private ImTexture backTex = Textures.GetTexture("buttonRock.Unpressed");
    private Hitbox topBar;
    private Hitbox bottomRight;
    private Hitbox closeButton;

    [JsonIgnore]
    public bool ShouldRemove { get; protected set; } = false;
    private bool isDragged = false;
    private bool isBottomRightDragged = false;


    [JsonConstructor]
    public WindowElement(AnchorPoint Anchor, Hitbox drawBox, List<UiElement> Children) : base("windowID", Anchor, drawBox, Children)
    {
        ResetBounds();
    }


    private void ResetBounds()
    {
        //putting the window border above scaledbox
        Vector2 off = new Vector2(0, TopBarHeight);
        bottomRight = new Hitbox(ScaledDrawBox.Pos + ScaledDrawBox.Dims, 20, 20);
        topBar = new Hitbox(ScaledDrawBox.Pos - off, ScaledDrawBox.Width, TopBarHeight);
        closeButton = new Hitbox(ScaledDrawBox.Pos - off + new Vector2(CloseButtonPadding), CloseButtonWidth, TopBarHeight - CloseButtonPadding * 2);
    }

    private void MoveWindow(Vector2 deltaPos)
    {
        var realWindow = UI.LiveUI.gameWindowRef.ClientBounds with { X = 0, Y = 0 };
        Vector2 scale = new Vector2((float)Camera.ScreenWidth / realWindow.Width, (float)Camera.ScreenHeight / realWindow.Height);
        Vector2 val = deltaPos * scale / UI.LiveUI.UiScale;
        AdjustRealPos(val);
        ResetBounds();
    }
    private void ResizeWindow(Vector2 deltaDim)
    {
        var realWindow = UI.LiveUI.gameWindowRef.ClientBounds with { X = 0, Y = 0 };
        Vector2 scale = new Vector2((float)Camera.ScreenWidth / realWindow.Width, (float)Camera.ScreenHeight / realWindow.Height);
        Vector2 val = deltaDim * scale / UI.LiveUI.UiScale;

        var current = DrawBox.Dims;
        Vector2 after = val + current;
        after = Vector2.Max(after, new Vector2(MinWidth, MinHeight));
        val = after - current;

        AdjustRealDims(val);

        foreach (var child in Children)
        {
            child.AdjustRealDims(val);
        }

        ResetBounds();
    }

    public override void UpdatePos(Rectangle window)
    {
        base.UpdatePos(window);
        ResetBounds();
    }

    public override void Update(GameTime gameTime)
    {
        if (bottomRight.Intersects(InputHelper.NewMouse.Position))
        {
            Mouse.SetCursor(MouseCursor.SizeNWSE);
        }
        else
        {
            Mouse.SetCursor(MouseCursor.Arrow);
        }
        //TODO: have drag logic here and make sure to update pos properly
        if (InputConditions.click.Pressed())
        {
            if (closeButton.Intersects(InputHelper.NewMouse.Position))
            {
                ShouldRemove = true;
                InputConditions.click.Consume();
                return;
            }
            if (topBar.Intersects(InputHelper.NewMouse.Position))
            {
                isDragged = true;
                InputConditions.click.Consume();
            }
            if (bottomRight.Intersects(InputHelper.NewMouse.Position))
            {
                isBottomRightDragged = true;
                InputConditions.click.Consume();
            }
        }

        if (isBottomRightDragged)
        {
            if (InputConditions.click.Held())
            {
                ResizeWindow(MouseCondition.PointerDelta.ToVector2());
            }
            else
            {
                isBottomRightDragged = false;
            }
        }
        if (isDragged)
        {
            if (InputConditions.click.Held())
            {
                MoveWindow(MouseCondition.PointerDelta.ToVector2());
            }
            else
            {
                isDragged = false;
            }
        }
        
        base.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        backTex.Draw(spriteBatch, ScaledDrawBox.ToRect());
        backTex.Draw(spriteBatch, topBar.ToRect());
        backTex.Draw(spriteBatch, closeButton.ToRect());
   
        base.Draw(spriteBatch);
    }
}