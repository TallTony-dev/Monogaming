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

namespace Monogaming.PlayerThings;


public class Player
{
    private const int InteractDistance = 40;
    private const int ColliderDiff = 2;

    public Body body { get; private set; } = new(Vector2.Zero, Map.TileDims - ColliderDiff * 2, Map.TileDims - ColliderDiff * 2, 10f);
    private InputManager inputManager = new();
    public TextureHandler TextureHandler { get; private set; }

    public Inventory Inventory { get; private set; }

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
        if (InputConditions.click.Pressed(false))
        {
            InteractableEntity clickedOn = currentChunk.GetTopCollision<InteractableEntity>(mWorldPos);

            if (clickedOn != null && 
            (clickedOn.DrawBox.Center - body.Center).LengthSquared() < InteractDistance * InteractDistance)
            {
                clickedOn.Interact();
                InputConditions.click.Consume();
            }

            currentChunk.AddEntity("core_testTileEntity", body.Pos);
        }

        if (InputConditions.openInventory.Pressed() && !UI.LiveUI.ContainsPopupWithID("playerInventory"))
        {
            UpdateSaveWithCurrentInv();

            UI.LiveUI.AddPopup(new CompositeWindow(new Hitbox(140, 120, 460, 180),
            new List<UiElement>() { 
                new InventoryWindow(Inventory, new Hitbox(0,0, 260, 180)),
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
        Rectangle rect = body.ToRect();
        rect.X -= ColliderDiff;
        rect.Y -= ColliderDiff;
        rect.Width += ColliderDiff * 2;
        rect.Height += ColliderDiff * 2;
        TextureHandler.Draw(batch, rect);
    }

}