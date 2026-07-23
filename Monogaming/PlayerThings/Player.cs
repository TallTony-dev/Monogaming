using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Apos.Input;
using Monogaming.Entities;
using Monogaming.MapThings;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Monogaming.TextureThings;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Monogaming.PlayerThings;


public class Player
{
    public Body body { get; private set; } = new(Vector2.Zero, Map.TileDims, Map.TileDims, 10f);
    InputManager inputManager = new();
    public TextureHandler TextureHandler { get; private set; }

    public float MoveSpeed { get; private set; }

    [JsonConstructor]
    public Player(float moveSpeed, TextureHandler textureHandler)
    {
        MoveSpeed = moveSpeed;
        TextureHandler = textureHandler;
    }

    public void Update(GameTime gameTime, GameState gameState)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Vector2 moveDir = inputManager.GetMovementDir();
        body.AddVelocity(50 * moveDir * dt * MoveSpeed / (body.Velocity.Length() * 0.002f + 0.5f));
        body.Update(gameTime, null);

        TextureHandler.Update(gameTime);
        if (body.IsWalking && (TextureHandler.CurrentTextureType != TextureHandler.TextureType.Walking))
        {
            TextureHandler.SetTexture(TextureHandler.TextureType.Walking);
        }
        else if (!body.IsWalking && TextureHandler.CurrentTextureType == TextureHandler.TextureType.Walking)
        {
            TextureHandler.SetTexture(TextureHandler.TextureType.Idle);
        }

    }

    public void Draw(SpriteBatch batch)
    {
        TextureHandler.Draw(batch, body.AsRect());
    }

}