using System;
using Monogaming.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Monogaming.Interfaces;
using Monogaming.TextureThings;
using Microsoft.Xna.Framework.Content;
using Monogaming.Ui;
using System.Text.Json.Serialization;
using Monogaming.Items;
using Apos.Input;
using System.Linq;

namespace Monogaming.Ui.Windows;

public class InventoryWindow : WindowElement
{
    public Inventory BoundInventory { get; private set; }
    private Item SelectedItem { get => (Item)sonion.SelectedItem; }

    private Item prevSelected = null;
    private bool isDraggingItem = false;
    private (Item item, Hitbox box) draggedItem;

    ScrollBoxElement sonion;

    //can be used to facilitate other things like a sort button or smth
    private const int TopOffset = 20;
    private const int ScrollBoxPadding = 3;

    public InventoryWindow(Inventory inventory, Hitbox drawBox, string id = "inventoryElement") : base(AnchorPoint.TopLeft, drawBox, null, id)
    {
        BoundInventory = inventory;

        sonion = new ScrollBoxElement("scrollBox", AnchorPoint.TopLeft, new TextureHandler(TextureHandler.TextureType.Default, "buttonRock.Unpressed"),
         new Hitbox(ScrollBoxPadding, 0, drawBox.Width - ScrollBoxPadding * 2, drawBox.Height - ScrollBoxPadding), inventory.Items.ConvertAll(t => ((ImItem)t, false)));

        Children.Add(sonion);
    }



    public override void Update(GameTime gameTime)
    {
        if (BoundInventory.HasChanged)
        {
            sonion.SetItems(BoundInventory.Items.ConvertAll(t => ((ImItem)t, false)));
        }
        
        base.Update(gameTime);


        if (isDraggingItem)
        {
            if (InputConditions.click.Held(false))
            {
                draggedItem.box.ChangePos(MouseCondition.PointerDelta.ToVector2());
            }
            else
            {
                isDraggingItem = false;
                
                InventoryWindow element;
                if ((element = (InventoryWindow)UI.LiveUI.GetWindowElements().FirstOrDefault(t => 
                t is InventoryWindow && !(t == this) && t.ScaledDrawBox.Intersects(draggedItem.box))) != default)
                {
                    bool allMoved = element.BoundInventory.AddItem(draggedItem.item);
                    if (allMoved) 
                    {
                        BoundInventory.ClearEmpties();
                    }
                }
            }
        }
        
        if (prevSelected != SelectedItem && SelectedItem != null)
        {
            isDraggingItem = true;
            draggedItem.item = SelectedItem;
            draggedItem.box = sonion.GetNewHitboxForItem(SelectedItem);
        }

        prevSelected = SelectedItem;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        if (isDraggingItem)
        {
            draggedItem.item.Draw(spriteBatch, draggedItem.box);
        }
    }
}