using System;
using Microsoft.Xna.Framework;
using Monogaming;
using Monogaming.Entities;

public class Camera
{
    GraphicsDeviceManager graphicsRef;
    GameWindow gameWindowRef;
    Vector2 prevTarget = Vector2.Zero;

    // const int screenDim = 1000;
    private float trackSpeed;

    private float scale = 1;

    /// <summary>
    /// View matrix to be passed when starting spritebatch
    /// </summary>
    public Matrix ViewMatrix 
    { get
        {
            Matrix m = Matrix.CreateTranslation(new Vector3(-prevTarget.X, -prevTarget.Y, 0));
            m *= Matrix.CreateScale(scale, scale, 0);
            m *= Matrix.CreateTranslation((float)gameWindowRef.ClientBounds.Width / 2, (float)gameWindowRef.ClientBounds.Height / 2, 0);
            return m;
        }
    }

    /// <summary>
    /// View-Projection matrix to be passed into vertex shader
    /// </summary>
    public Matrix ViewProjMatrix 
    { get
        {
            Matrix m = ViewMatrix;
            return m * Matrix.CreateOrthographicOffCenter(0, graphicsRef.GraphicsDevice.Viewport.Width, 
            graphicsRef.GraphicsDevice.Viewport.Height, 0, 0, 1);
        }
    }

    public Camera(GraphicsDeviceManager graphics, GameWindow window, 
    float trackSpeed = 0.2f)
    {
        this.trackSpeed = trackSpeed;
        graphicsRef = graphics;
        gameWindowRef = window;
    }

    public void Update(GameTime t, Vector2 trackingPos, bool snapToPos = false)
    {
        //Scale = 1 + MathF.Sin((float)t.TotalGameTime.TotalSeconds) * 0.5f;
        if (!snapToPos) {
            float dt = (float)t.ElapsedGameTime.TotalSeconds;
            Vector2 deltaPos = trackingPos - prevTarget;
            Vector2 newTarget = deltaPos * trackSpeed * dt * deltaPos.LengthSquared() * 0.0005f;
            prevTarget += newTarget;
        }
        else
        {
            prevTarget = trackingPos;
        }
    }

}