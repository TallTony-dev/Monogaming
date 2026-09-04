using Monogaming.MapThings;
using System.Text.Json.Serialization;

namespace Monogaming.Entities;

public class Body : Hitbox
{

    private static readonly float PhysicsTimeStep = 0.05f;
    private static readonly float WalkingEpsilon = 0.1f;
    [JsonIgnore]
    public Vector2 velocity = Vector2.Zero;

    public float Damping { get; private set; } = 0.7f;

    /// <summary>
    /// True if Velocity length > WalkingEpsilon
    /// </summary>
    public bool IsWalking { get; private set; }
    /// <summary>
    /// True if collided last frame
    /// </summary>
    public bool Collided { get; private set; }

    public Body(Vector2 startPos, int width, int height, float damping) : base(startPos, width, height)
    {
        Pos = startPos;
        SetDims(new Vector2(width, height));
        Damping = damping;
    }
    [JsonConstructor]
    public Body(int width, int height) : base(width, height)
    {
        SetDims(new Vector2(width, height));
        Pos = Vector2.Zero;
    }

    public void Update(GameTime gameTime, Chunk chunk)
    {
        Collided = false;
        float i = (float)gameTime.ElapsedGameTime.TotalSeconds;

        velocity -= velocity * Damping * i;

        if (velocity.Length() > WalkingEpsilon)
        {
            IsWalking = true;
        }
        else
        {
            IsWalking = false;
        }

        do
        {
            float dt = i >= PhysicsTimeStep ? PhysicsTimeStep : i;

            Pos += velocity * dt;
            Vector2 collision = chunk.GetCollision(this);

            if (collision.LengthSquared() > 0)
            {
                Vector2 delta = Pos - collision;
                Pos = collision;

                if (delta.LengthSquared() > 0)
                {
                    Vector2 normal = Vector2.Normalize(delta);
                    if (Math.Abs(normal.X) > Math.Abs(normal.Y))
                    {
                        velocity.X = 0f;
                    }
                    else
                    {
                        velocity.Y = 0f;
                    }
                }

                Collided = true;
            }
            i -= dt;
        } while (i > 0);

    }

    public void AddVelocity(Vector2 vel)
    {
        velocity += vel;
    }


}