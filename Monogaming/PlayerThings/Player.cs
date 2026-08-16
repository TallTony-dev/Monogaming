using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Apos.Input;
using Monogaming.Entities;
using Monogaming.MapThings;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Monogaming.TextureThings;
using Monogaming.Items;
using System.Text.Json;
using System.Text.Json.Serialization;
using Monogaming.Ui;
using Monogaming.Ui.Windows;
using System.Linq;

namespace Monogaming.PlayerThings;


public class Player
{
    private const int InteractDistance = 120;
    private const int ColliderDiff = 2;

    public Body body { get; private set; } = new(Vector2.Zero, Map.TileDims - ColliderDiff * 2, Map.TileDims - ColliderDiff * 2, 10f);
    private InputManager inputManager = new();
    public TextureHandler TextureHandler { get; private set; }

    public Inventory Inventory { get; private set; }


    private InventoryWindow inventoryWindow = null;
    private bool isPlacedItemValid = false;
    private Vector2 placedItemPos;
    private (int x, int y) placedItemTilePos;

    public float MoveSpeed { get; private set; }

    [JsonConstructor]
    public Player(float moveSpeed, TextureHandler textureHandler, Inventory inventory)
    {
        Inventory = inventory;
        MoveSpeed = moveSpeed;
        TextureHandler = textureHandler;
    }

    private List<string> GetNearbyWorkbenches()
    {
        //TODO: implement this lol
        return new List<string>();
    }

    private void UpdateSaveWithCurrentInv()
    {
        foreach (Item i in Inventory.Items)
        {
            //this is about as cheap/cheaper than checking and then setting
            GameState.ActiveSave.SetItemFound(i.ID);
        }
    }

    private bool IsWithinReach(Vector2 worldPos)
    {
        return (worldPos - body.Center).LengthSquared() < InteractDistance * InteractDistance;
    }

    public void Update(GameTime gameTime, Chunk currentChunk, Map map)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Vector2 moveDir = inputManager.GetMovementDir();
        body.AddVelocity(50 * moveDir * dt * MoveSpeed / (body.velocity.Length() * 0.002f + 0.5f));
        body.Update(gameTime, currentChunk);

        TextureHandler.Update(gameTime);
        if (body.IsWalking)
        {
            TextureHandler.SetTexture(TextureHandler.TextureType.Walking);
        }
        else if (!body.IsWalking)
        {
            TextureHandler.SetTexture(TextureHandler.TextureType.Idle);
        }


        Vector2 mPos = InputHelper.NewMouse.Position.ToVector2();
        Vector2 mWorldPos = map.ScreenPosToWorldPos(mPos);
        (int x, int y) mTilePos = Map.WorldPosToAbsTilePos(mWorldPos - new Vector2(20, 16));

        if (inventoryWindow != null)
        {
            Item item;
            if ((item = inventoryWindow.SelectedItem) != null && !string.IsNullOrEmpty(item.PlacedEntityID))
            {
                Vector2 tileWorldPos = Map.TilePosToWorldPos(mTilePos.x, mTilePos.y);
                isPlacedItemValid = currentChunk.CanAddTileEntityWithoutCollision
                ((TileEntity)EntityDict.GetReadOnlyEntity(item.PlacedEntityID), mTilePos.x, mTilePos.y)
                && IsWithinReach(tileWorldPos);

                placedItemPos = tileWorldPos;
                placedItemTilePos = mTilePos;
            }
        }

        if (IsWithinReach(mPos))
        {
            
            if (InputConditions.click.Pressed(false))
            {
                InteractableEntity clickedOn = currentChunk.GetTopCollision<InteractableEntity>(mWorldPos);

                if (clickedOn != null)// && IsWithinReach(clickedOn.DrawBox.Center))
                {
                    clickedOn.Interact();
                    InputConditions.click.Consume();
                }

                currentChunk.AddEntity("core_testTileEntity", body.Pos);
            }

        }

        

        if (InputConditions.openInventory.Pressed() && !UI.LiveUI.ContainsPopupWithID("playerInventory"))
        {
            UpdateSaveWithCurrentInv(); //adds items to discovered

            UI.LiveUI.AddPopup(new CompositeWindow(new Hitbox(140, 120, 460, 180),
            new List<UiElement>() { 
                (inventoryWindow = new InventoryWindow(Inventory, new Hitbox(0,0, 260, 180), "inv")),
                new CraftingMenuWindow(Inventory, GetNearbyWorkbenches(), new Hitbox(0,0, 200, 180)),
            }, "playerInventory"));
        }

        if (Inventory.HasChanged && GameState.ActiveSave != null)
        {
            UpdateSaveWithCurrentInv();
        }

        Inventory.Update(gameTime);
    }

    public void Draw(SpriteBatch batch)
    {
        if (inventoryWindow != null)
        {
            Item item;
            if ((item = inventoryWindow.SelectedItem) != null && !string.IsNullOrEmpty(item.PlacedEntityID))
            {
                TileEntity entity = (TileEntity)EntityDict.GetReadOnlyEntity(item.PlacedEntityID);
                Color color = isPlacedItemValid ? new Color((byte)0, (byte)255, (byte)0, (byte)150) : new Color((byte)255, (byte)0, (byte)0, (byte)190);

                for (int x = placedItemTilePos.x; x < placedItemTilePos.x + entity.Width; x++)
                {
                    for (int y = placedItemTilePos.y; y < placedItemTilePos.y + entity.Height; y++)
                    {
                        Vector2 pos = Map.TilePosToWorldPos(x,y);
                        item.DrawBackground(batch, new Hitbox(pos, Map.TileDims, Map.TileDims), color, 0);
                    }
                }

                entity.DrawAtPos(batch, placedItemPos, color);


            }
        }


        Rectangle rect = body.ToRect();
        rect.X -= ColliderDiff;
        rect.Y -= ColliderDiff;
        rect.Width += ColliderDiff * 2;
        rect.Height += ColliderDiff * 2;
        TextureHandler.Draw(batch, rect);
    }

}