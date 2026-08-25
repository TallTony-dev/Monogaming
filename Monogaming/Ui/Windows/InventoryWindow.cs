using Apos.Input;
using Monogaming.Entities;
using Monogaming.Interfaces;
using Monogaming.Items;
using Monogaming.MapThings;
using Monogaming.PlayerThings;
using Monogaming.TextureThings;
using System.Linq;

namespace Monogaming.Ui.Windows;

public class InventoryWindow : WindowElement
{
    public Inventory BoundInventory { get; private set; }
    public Item SelectedItem { get => (Item)sonion.SelectedItem; }

    private Item prevSelected = null;
    private bool isDraggingItem = false;
    private (Item item, Hitbox box) draggedItem;
    private bool isPlacedItemValid = false;
    private Vector2 placedItemPos;
    private (int x, int y) placedItemTilePos;

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


    private void TryAddHeldItemToOtherInventory()
    {
        UiElement element;
        if ((element = UI.LiveUI.GetWindowElements().FirstOrDefault(t =>
        (t is InventoryWindow || (t is CompositeWindow c && c.Children.Any(t => t is InventoryWindow)))
        && t.ScaledDrawBox.Intersects(draggedItem.box))) != default)
        {
            InventoryWindow window;
            if (element is InventoryWindow)
            {
                window = (InventoryWindow)element;
            }
            else
            {
                window = (InventoryWindow)element.Children.Find(t => t is InventoryWindow);
            }

            bool allMoved = window.BoundInventory.AddItem(draggedItem.item);
            if (allMoved)
            {
                BoundInventory.ClearEmpties();
            }
        }
    }
    public override void Update(GameTime gameTime)
    {
        if (prevSelected != SelectedItem && SelectedItem != null)
        {
            isDraggingItem = true;
            draggedItem.item = SelectedItem;
            draggedItem.box = sonion.GetNewHitboxForItem(SelectedItem);
            draggedItem.box.ChangePos(new Vector2(draggedItem.box.Width / 2, 0));
        }

        Vector2 mPos = InputHelper.NewMouse.Position.ToVector2();
        Vector2 mWorldPos = Map.LiveMap.ScreenPosToWorldPos(mPos);
        (int x, int y) mTilePos = Map.WorldPosToAbsTilePos(mWorldPos - new Vector2(20, 16));
        Vector2 tileWorldPos = Map.TilePosToWorldPos(mTilePos.x, mTilePos.y);
        //check if placed item is in valid spot
        if (SelectedItem != null && isDraggingItem && !string.IsNullOrEmpty(SelectedItem.PlacedEntityID))
        {
            draggedItem.box.ChangePos(MouseCondition.PointerDelta.ToVector2());

            isPlacedItemValid = Map.LiveChunk.CanAddTileEntityWithoutCollision
            ((TileEntity)EntityDict.GetReadOnlyEntity(SelectedItem.PlacedEntityID), mTilePos.x, mTilePos.y)
            && Player.LivePlayer.IsWithinReach(tileWorldPos);

            placedItemPos = tileWorldPos;
            placedItemTilePos = mTilePos;
        }
        else
        {
            isPlacedItemValid = false;
            isDraggingItem = false;
        }


        if (isDraggingItem)
        {
            if (InputConditions.click.Pressed())
            {
                if (isPlacedItemValid)
                {
                    Map.LiveChunk.AddEntity(SelectedItem.PlacedEntityID, tileWorldPos);
                    SelectedItem.RemoveFrom(1);
                    BoundInventory.ClearEmpties();
                    InputConditions.click.Consume();
                }
                else
                {
                    TryAddHeldItemToOtherInventory();
                }

            }
        }


        if (BoundInventory.HasChanged)
        {
            sonion.SetItems(BoundInventory.Items.ConvertAll(t => ((ImItem)t, false)));
        }
        prevSelected = SelectedItem;
        base.Update(gameTime);


    }



    /// <summary>
    /// Must be called outside of this class (opt in) as it isn't drawn in standard ui batch
    /// </summary>
    public void TryDrawSelectedItem(SpriteBatch batch)
    {
        if (isDraggingItem && SelectedItem != null && !string.IsNullOrEmpty(SelectedItem.PlacedEntityID))
        {
            TileEntity entity = (TileEntity)EntityDict.GetReadOnlyEntity(SelectedItem.PlacedEntityID);
            Color color = isPlacedItemValid ? new Color((byte)0, (byte)255, (byte)0, (byte)150) : new Color((byte)255, (byte)0, (byte)0, (byte)190);

            for (int x = placedItemTilePos.x; x < placedItemTilePos.x + entity.Width; x++)
            {
                for (int y = placedItemTilePos.y; y < placedItemTilePos.y + entity.Height; y++)
                {
                    Vector2 pos = Map.TilePosToWorldPos(x, y);
                    SelectedItem.DrawBackground(batch, new Hitbox(pos, Map.TileDims, Map.TileDims), color, 0);
                }
            }

            entity.DrawAtPos(batch, placedItemPos, color);
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (isDraggingItem)
        {
            draggedItem.item.Draw(spriteBatch, draggedItem.box, false);
        }
        base.Draw(spriteBatch);
    }
}