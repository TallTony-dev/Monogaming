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

namespace Monogaming.PlayerThings;


public class Player
{
    private const int InteractDistance = 40;

    public Body body { get; private set; } = new(Vector2.Zero, Map.TileDims, Map.TileDims, 10f);
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
        body.AddVelocity(50 * moveDir * dt * MoveSpeed / (body.Velocity.Length() * 0.002f + 0.5f));
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
            //omg wow so optimized that saved like 5 cpu cycles no way
            {
                clickedOn.Interact();
            }
        }
    }

    public void Draw(SpriteBatch batch)
    {
        TextureHandler.Draw(batch, body.AsRect());
    }

}