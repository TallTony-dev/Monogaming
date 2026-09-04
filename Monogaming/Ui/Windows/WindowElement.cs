using Apos.Input;
using Microsoft.Xna.Framework.Input;
using Monogaming.Entities;
using Monogaming.Interfaces;
using Monogaming.TextureThings;
using System.Linq;
using System.Text.Json.Serialization;

namespace Monogaming.Ui.Windows;



public class WindowElement : UiElement
{
    protected const int TopBarHeight = 20;
    const int CloseButtonPadding = 1;
    const int CloseButtonWidth = 30;
    protected int MinWidth = 140;
    protected int MinHeight = 100;


    private ImTexture backTex = Textures.GetTexture("buttonRock.Unpressed");
    private Hitbox topBar;
    private Hitbox bottomRight;
    private Hitbox closeButton;

    [JsonIgnore]
    public bool ShouldRemove { get; protected set; } = false;
    [JsonIgnore]
    public bool ShouldMoveToFront { get; set; } = false;
    private bool isDragged = false;
    private bool isBottomRightDragged = false;

    protected bool IsResizable = true;
    public bool isChildOfOtherWindow = false;

    [JsonConstructor]
    public WindowElement(AnchorPoint Anchor, Hitbox drawBox, List<UiElement> Children, string id = "windowElement") : base(id, Anchor, drawBox, Children)
    {
        ResetBounds();
    }

    public override UiElement GetTopCollision(Vector2 pos)
    {
        foreach (UiElement child in Children)
        {
            UiElement element = child.GetTopCollision(pos);
            if (element != null)
            {
                return element;
            }
        }
        if (ScaledDrawBox != null && (ScaledDrawBox.Intersects(pos) || topBar.Intersects(pos)))
        {
            return this;
        }
        return null;
    }

    private void ResetBounds()
    {
        //putting the window border above scaledbox
        Vector2 off = new Vector2(0, TopBarHeight);
        bottomRight = new Hitbox(ScaledDrawBox.Pos + ScaledDrawBox.Dims - new Vector2(6, 6), 18, 18);
        topBar = new Hitbox(ScaledDrawBox.Pos - off, (int)ScaledDrawBox.Width, TopBarHeight);
        closeButton = new Hitbox(ScaledDrawBox.Pos - off + new Vector2(CloseButtonPadding), CloseButtonWidth, TopBarHeight - CloseButtonPadding * 2);
    }

    private void MoveWindow(Vector2 deltaPos)
    {
        var realWindow = UI.LiveUI.gameWindowRef.ClientBounds with { X = 0, Y = 0 };
        Vector2 val = deltaPos / UI.LiveUI.Scale;
        AdjustRealPos(val);
        ResetBounds();
    }

    private void ResizeWindow(Vector2 deltaDim)
    {
        Vector2 oldDims = DrawBox.Dims;
        var windowKids = Children.OfType<WindowElement>();
        Vector2 minDim;
        if (windowKids.Count() > 0)
        {
            minDim = new Vector2(windowKids.Sum(t => t.MinWidth * (oldDims.X + deltaDim.X) / t.DrawBox.Width), windowKids.Max(t => t.MinHeight));
        }
        else
        {
            minDim = new Vector2(MinWidth, MinHeight);
        }
        Vector2 overage = Vector2.Min(oldDims + deltaDim - minDim, Vector2.Zero);
        deltaDim -= overage;

        AdjustRealDims(deltaDim);

        foreach (var e in Children)
        {
            //tiny bit of LLM: fix the way i was calculating delta for children
            Vector2 dimRatio = e.DrawBox.Dims / oldDims;
            Vector2 posRatio = e.DrawBox.Pos / oldDims;

            e.SetRealPos(posRatio * DrawBox.Dims);

            Vector2 childDelta = dimRatio * DrawBox.Dims - e.DrawBox.Dims;
            if (e is WindowElement w)
            {
                w.ResizeWindow(childDelta);
            }
            else
            {
                e.AdjustRealDims(childDelta);
            }
        }
    }

    public override void UpdatePos(Rectangle window)
    {
        base.UpdatePos(window);
        ResetBounds();
    }

    public override void Update(GameTime gameTime)
    {
        if (!isChildOfOtherWindow)
        {
            if (isBottomRightDragged)
            {
                if (InputConditions.click.Held())
                {
                    ResizeWindow(MouseCondition.PointerDelta.ToVector2() / UI.LiveUI.UiScale);
                }
                else
                {
                    isBottomRightDragged = false;
                }
            }
            if (isDragged)
            {
                if (InputConditions.click.Held(true))
                {
                    MoveWindow(MouseCondition.PointerDelta.ToVector2());
                }
                else
                {
                    isDragged = false;
                }
            }
            if (IsResizable)
            {
                if (bottomRight.Intersects(InputHelper.NewMouse.Position))
                {
                    Mouse.SetCursor(MouseCursor.SizeNWSE);
                }
                else
                {
                    Mouse.SetCursor(MouseCursor.Arrow);
                }
            }
            
            if (InputConditions.click.Pressed(false))
            {
                if (closeButton.Intersects(InputHelper.NewMouse.Position))
                {
                    ShouldRemove = true;
                    return;
                }
                if (topBar.Intersects(InputHelper.NewMouse.Position))
                {
                    isDragged = true;
                    InputConditions.click.Consume();
                }
                if (GetTopCollision(InputHelper.NewMouse.Position.ToVector2()) != null)
                {
                    ShouldMoveToFront = true;
                }
                if (IsResizable && bottomRight.Intersects(InputHelper.NewMouse.Position))
                {
                    isBottomRightDragged = true;
                    InputConditions.click.Consume();
                }
            }


        }

        base.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        backTex.Draw(spriteBatch, ScaledDrawBox.ToRect());

        if (!isChildOfOtherWindow)
        {
            backTex.Draw(spriteBatch, topBar.ToRect());
            backTex.Draw(spriteBatch, closeButton.ToRect());
        }

        base.Draw(spriteBatch);
    }

    public override void DrawTop(SpriteBatch spriteBatch)
    {
        base.DrawTop(spriteBatch);
    }
}