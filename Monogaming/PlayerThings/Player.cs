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
        if (InputConditions.click.Pressed())
        {
            InteractableEntity clickedOn = currentChunk.GetTopCollision<InteractableEntity>(mWorldPos);
            if (clickedOn != null && 
            (clickedOn.DrawBox.Center - body.Center).LengthSquared() < InteractDistance * InteractDistance)
            {
                clickedOn.Interact();
            }

            currentChunk.AddEntity("core_testTileEntity", body.Pos);

        }

        if (InputConditions.openInventory.Pressed())
        {
            UI.LiveUI.AddPopup(new InventoryWindow(Inventory, 
            new Hitbox(new Vector2 (Camera.ScreenWidth / 3, Camera.ScreenHeight / 3), Camera.ScreenWidth / 3, Camera.ScreenHeight / 3)));
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