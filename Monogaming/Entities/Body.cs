using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using Monogaming.MapThings;

namespace Monogaming.Entities;

public class Body : Hitbox
{

    private static readonly float PhysicsTimeStep = 0.05f;
    private static readonly float MovingEpsilon = 1f;

    public Vector2 Velocity { get; private set; }= Vector2.Zero;

    public float Damping { get; private set; } = 0.7f;

    /// <summary>
    /// True if Velocity length > MovingEpsilon
    /// </summary>
    public bool IsMoving { get; private set; }
    /// <summary>
    /// True if collided last frame
    /// </summary>
    public bool Collided { get; private set; }

    public Body(Vector2 startPos, int width, int height, float damping) : base(startPos, width, height)
    {
        Pos = startPos;
        Width = width;
        Height = height;
        Damping = damping;
    }
    [JsonConstructor] 
    public Body(int width, int height) : base(width, height)
    {
        Width = width;
        Height = height;
        Pos = Vector2.Zero;
    }

    public void Update(GameTime gameTime, Chunk chunk)
    {
        Collided = false;
        float i = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        Velocity -= Velocity * Damping * i;

        if (Velocity.Length() > MovingEpsilon)
        {
            IsMoving = true;
        }
        else
        {
            IsMoving = false;
        }

        do
        {
            float dt = i >= PhysicsTimeStep ? PhysicsTimeStep : i;

            Pos += Velocity * dt;
            // Vector2 collision = chunk.Collides(this);
            // Pos += collision;
            // if (collision.LengthSquared() > 0)
            // {
            //     Collided = true;
            // }
            i -= dt;
        } while (i > 0);

    }

    public void AddVelocity(Vector2 vel)
    {
        Velocity += vel;
    }


}