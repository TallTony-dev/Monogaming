using Apos.Input;
using Monogaming.Entities;
using Monogaming.Interfaces;
using Monogaming.Items;
using Monogaming.TextureThings;
using System.Linq;

namespace Monogaming.Ui;

public class ScrollBoxElement : UiElement
{
    public const int ScrollBarWidth = 20;
    private const int ScrollBarHeight = 50;
    public const int ScrollBarPadding = 5;

    private const int MinItemWidth = 30;
    private const int MaxItemWidth = 60;
    private const int PrefItemsPerRow = 6;

    private List<(ImItem item, bool isGrey)> Items;
    private List<(Hitbox box, ImItem item, bool isGrey)> ItemBoxes = new();

    //Gives the item clicked this frame
    public ImItem SelectedItem { get; private set; }

    bool isBeingDragged = false;
    TextureHandler scollTex;
    Hitbox scrollBar;
    float scrollPosNorm = 0;

    public ScrollBoxElement(string ID, AnchorPoint Anchor, TextureHandler scollTex, Hitbox drawBox, List<(ImItem, bool)> items) :
    base(ID, Anchor, drawBox, null)
    {
        Items = items;
        this.scollTex = scollTex;
        UpdateHitboxes();
    }

    public void SetItems(List<(ImItem, bool)> items)
    {
        Items = items;
        UpdateHitboxes();
    }

    private int GetItemWidth()
    {
        return Math.Clamp(ScaledDrawBox.Width / PrefItemsPerRow + Item.ItemPadding * PrefItemsPerRow, MinItemWidth, MaxItemWidth);
    }

    public Hitbox GetNewHitboxForItem(ImItem item)
    {
        return new Hitbox(ItemBoxes.Find(t => t.item == item).box);
    }

    private void UpdateHitboxes()
    {
        int x = ScaledDrawBox.Width - ScrollBarWidth - ScrollBarPadding;
        int y = (int)(scrollPosNorm * (float)(ScaledDrawBox.Height - ScrollBarPadding - ScrollBarHeight));
        scrollBar = new Hitbox(new Vector2(x, y) + ScaledDrawBox.Pos, ScrollBarWidth, ScrollBarHeight);

        ItemBoxes.Clear();
        Rectangle toDrawIn = ScaledDrawBox.ToRect();
        toDrawIn.Width -= scrollBar.Width + ScrollBarPadding * 2 + 1;
        int itemWidth = GetItemWidth();
        int itemsPerRow = toDrawIn.Width / itemWidth;
        if (itemsPerRow == 0) { itemsPerRow = 1; }

        int xPerItem = itemWidth + Item.ItemPadding;
        int itemCount = Items.Count();
        int rowCount = itemCount / itemsPerRow + 1;

        int maxScrollY = Math.Max(0, rowCount * xPerItem - ScaledDrawBox.Height);

        float iOff = maxScrollY / (float)xPerItem * scrollPosNorm;

        for (int i = 0; i < rowCount; i++)
        {
            for (int j = 0; j < itemsPerRow; j++)
            {
                int ind = j + i * itemsPerRow;
                if (ind >= itemCount)
                {
                    break;
                }
                Hitbox hitbox = new Hitbox(ScaledDrawBox.Pos + new Vector2(Item.ItemPadding + xPerItem * j, (i - iOff) * xPerItem + Item.ItemPadding),
                itemWidth, itemWidth);
                ItemBoxes.Add((hitbox, Items[ind].item, Items[ind].isGrey));
            }
        }


    }

    public override void UpdatePos(Rectangle window)
    {
        base.UpdatePos(window);
        UpdateHitboxes();
    }

    public override void Update(GameTime gameTime)
    {
        if (isBeingDragged)
        {
            if (!InputConditions.click.Held())
            {
                isBeingDragged = false;
            }
            else
            {
                scrollPosNorm += (float)MouseCondition.PointerDelta.Y / (ScaledDrawBox.Height - ScrollBarPadding * 2);
                scrollPosNorm = Math.Clamp(scrollPosNorm, 0, 1);
                UpdateHitboxes();
            }
        }
        if (scrollBar.Intersects(InputHelper.NewMouse.Position))
        {
            if (InputConditions.click.Pressed())
            {
                isBeingDragged = true;
            }
        }

        //check for item selection
        if (InputConditions.rightClick.Pressed())
        {
            isBeingDragged = false;
            SelectedItem = null;
        }
        else if (InputConditions.click.Pressed(false))
        {
            (Hitbox box, ImItem item, bool isGrey) intersect;
            if ((intersect = ItemBoxes.FirstOrDefault(t => t.box.Intersects(InputHelper.NewMouse.Position))) != default)
            {
                InputConditions.click.Consume();
                SelectedItem = intersect.item;
            }
            else
            {
                SelectedItem = null;
            }
        }


        base.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);

        Rectangle toDrawIn = ScaledDrawBox.ToRect();
        toDrawIn.Width -= ScrollBarWidth + ScrollBarPadding * 2;

        GraphicsDevice graphicsDevice = spriteBatch.GraphicsDevice;
        Rectangle previousScissor = graphicsDevice.ScissorRectangle;

        spriteBatch.End();
        //LLM: use scissor state here (it did it wrong and i had to debug anyways)
        graphicsDevice.ScissorRectangle = toDrawIn;
        spriteBatch.Begin(
            samplerState: SamplerState.PointClamp,
            blendState: BlendState.AlphaBlend,
            rasterizerState: new RasterizerState { ScissorTestEnable = true });

        spriteBatch.Draw(Textures.BlankTexture, toDrawIn, Color.BlanchedAlmond);

        int itemWidth = GetItemWidth();
        int itemsPerRow = toDrawIn.Width / itemWidth;

        int xPerItem = itemWidth + Item.ItemPadding;
        int itemCount = Items.Count();
        int rowCount = itemCount / itemsPerRow + 1;

        int maxScrollY = Math.Max(0, rowCount * xPerItem - ScaledDrawBox.Height);

        int firstRowInd = (int)(maxScrollY / (float)xPerItem * scrollPosNorm);
        int lastRowInd = Math.Min(rowCount, (int)MathF.Ceiling(ScaledDrawBox.Height / xPerItem) + firstRowInd + 2);
        for (int i = firstRowInd; i < lastRowInd; i++)
        {
            for (int j = 0; j < itemsPerRow; j++)
            {
                int ind = j + i * itemsPerRow;
                if (ind >= itemCount)
                {
                    break;
                }
                (Hitbox box, ImItem item, bool isGrey) = ItemBoxes[ind];

                item.Draw(spriteBatch, box, isGrey ? Color.DimGray : Color.White, true);
            }
        }

        spriteBatch.End();
        graphicsDevice.ScissorRectangle = previousScissor;
        spriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: null);

        scollTex.Draw(spriteBatch, scrollBar.ToRect());

    }

}