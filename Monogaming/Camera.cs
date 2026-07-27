using System;
using Microsoft.Xna.Framework;
using Monogaming;
using Monogaming.Entities;

public class Camera
{
    GameWindow gameWindowRef;
    Vector2 prevTarget = Vector2.Zero;

    private float trackSpeed;

    private float scale = 1f;

    public Matrix ScaleMatrix
    { get
        {
            return Matrix.CreateScale(scale, scale, 0);
        }
    }

    /// <summary>
    /// View matrix to be passed when starting spritebatch
    /// </summary>
    public Matrix ViewMatrix 
    { get
        {
            Matrix m = Matrix.CreateScale(scale, scale, 0);
            m *= Matrix.CreateTranslation(new Vector3(-prevTarget.X + MeowGame.ScreenWidth / 2, 
            -prevTarget.Y + MeowGame.ScreenHeight / 2, 0));
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
            return m * Matrix.CreateOrthographicOffCenter(0, MeowGame.ScreenWidth, 
            MeowGame.ScreenHeight, 0, 0, 1);
        }
    }

    public Hitbox Screen
    {
        get
        {
            int width = MeowGame.ScreenWidth;
            int height = MeowGame.ScreenHeight;
            return new Hitbox(new Vector2(prevTarget.X - width/2, prevTarget.Y - height/2), width, height);
        }
    }

    public Camera(GameWindow window, 
    float trackSpeed = 1f)
    {
        this.trackSpeed = trackSpeed;
        gameWindowRef = window;
    }

    public void Update(GameTime t, Vector2 trackingPos, bool snapToPos = false)
    {
        //Scale = 1 + MathF.Sin((float)t.TotalGameTime.TotalSeconds) * 0.5f;
        if (!snapToPos) {
            float dt = (float)t.ElapsedGameTime.TotalSeconds;
            Vector2 deltaPos = trackingPos - prevTarget;
            Vector2 newTarget = deltaPos * trackSpeed * dt * deltaPos.LengthSquared() * 0.005f;
            newTarget.X += Math.Sign(deltaPos.X) * trackSpeed * dt * 0.0005f;
            newTarget.Y += Math.Sign(deltaPos.Y) * trackSpeed * dt * 0.0005f;
            prevTarget += newTarget;
        }
        else
        {
            prevTarget = trackingPos;
        }
    }

}