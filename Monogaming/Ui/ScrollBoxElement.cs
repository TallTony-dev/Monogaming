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
using Monogaming;
using System.Linq;
using Monogaming.Items;

namespace Monogaming.Ui;

public class ScrollBoxElement : UiElement
{
    private const int ScrollBarWidth = 20;
    private const int ScrollBarHeight = 50;
    private const int ScrollBarPadding = 5;

    private const int ItemPadding = 3;
    private const int MinItemWidth = 30;
    private const int MaxItemWidth = 60;
    private const int PrefItemsPerRow = 6;

    private List<ImDrawable> Items;
    private List<(Hitbox box, ImDrawable item)> ItemBoxes = new();
    [JsonIgnore]
    public ImDrawable SelectedItem { get; private set; }

    bool isBeingDragged = false;
    TextureHandler scollTex;
    Hitbox scrollBar;
    float scrollPosNorm = 0;
    
    public ScrollBoxElement(string ID, AnchorPoint Anchor, TextureHandler scollTex, Hitbox drawBox, List<ImDrawable> items) : 
    base(ID, Anchor, drawBox, null)
    {
        Items = items;
        this.scollTex = scollTex;
        UpdateHitboxes();
    }

    public void SetItems(List<ImDrawable> items)
    {
        Items = items;
        UpdateHitboxes();
    }

    private void UpdateHitboxes()
    {
        ItemBoxes.Clear();
        Rectangle toDrawIn = ScaledDrawBox.ToRect();
        int itemWidth = Math.Clamp(toDrawIn.Width / PrefItemsPerRow + ItemPadding * PrefItemsPerRow, MinItemWidth, MaxItemWidth);
        int itemsPerRow = toDrawIn.Width / itemWidth;

        int xPerItem = itemWidth + ItemPadding;
        int itemCount = Items.Count();
        int rowCount = itemCount / itemsPerRow + 1;

        int maxScrollY = Math.Max(0, rowCount * xPerItem - ScaledDrawBox.Height);

        int iOff = (int)(maxScrollY / (float)xPerItem * scrollPosNorm);

        for (int i = 0; i < rowCount; i++) 
        {
            for (int j = 0; j < itemsPerRow; j++)
            {
                int ind = j + i * itemsPerRow;
                if (ind >= itemCount)
                {
                    break;
                }
                Hitbox hitbox = new Hitbox(ScaledDrawBox.Pos + new Vector2(ItemPadding + xPerItem * j, (i - iOff) * xPerItem + ItemPadding),
                itemWidth, itemWidth);
                ItemBoxes.Add((hitbox, Items[ind]));
            }
        }

        int x = ScaledDrawBox.Width - ScrollBarWidth - ScrollBarPadding;
        int y = (int)(scrollPosNorm * (float)(ScaledDrawBox.Height - ScrollBarPadding - ScrollBarHeight));
        scrollBar = new Hitbox(new Vector2(x, y) + ScaledDrawBox.Pos, ScrollBarWidth, ScrollBarHeight);
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
                InputConditions.click.Consume();
            }
        }

        //check for item selection

        if (InputConditions.click.Pressed())
        {
            InputConditions.click.Consume();
            SelectedItem = null;

            (Hitbox box, ImDrawable item) intersect;
            if ((intersect = ItemBoxes.FirstOrDefault(t => t.box.Intersects(InputHelper.NewMouse.Position))) != default)
            {
                SelectedItem = intersect.item;
            }
        }


        base.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);

        //TODO: DONT REPEAT YOURSELF DINGUS
        Rectangle toDrawIn = ScaledDrawBox.ToRect();
        int itemWidth = Math.Clamp(toDrawIn.Width / PrefItemsPerRow + ItemPadding * PrefItemsPerRow, MinItemWidth, MaxItemWidth);
        int itemsPerRow = toDrawIn.Width / itemWidth;

        int xPerItem = itemWidth + ItemPadding;
        int itemCount = Items.Count();
        int rowCount = itemCount / itemsPerRow + 1;

        int maxScrollY = Math.Max(0, rowCount * xPerItem - ScaledDrawBox.Height);

        int firstRowInd = (int)(maxScrollY / (float)xPerItem * scrollPosNorm);
        int lastRowInd = Math.Min(rowCount, (int)MathF.Ceiling(ScaledDrawBox.Height / xPerItem) + firstRowInd);
        for (int i = firstRowInd; i < lastRowInd; i++) 
        {
            for (int j = 0; j < itemsPerRow; j++)
            {
                int ind = j + i * itemsPerRow;
                if (ind >= itemCount)
                {
                    break;
                }
                (Hitbox box, ImDrawable item) = ItemBoxes[ind];
                item.Draw(spriteBatch, box);
            }
        }

        scollTex.Draw(spriteBatch, scrollBar.ToRect());
    }

}