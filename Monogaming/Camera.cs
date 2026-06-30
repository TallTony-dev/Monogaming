
using System;
using Microsoft.Xna.Framework;
using Monogaming;
using Monogaming.Entities;

public class Camera
{
    GraphicsDeviceManager graphicsRef;
    GameWindow gameWindowRef;
    Vector2 pos;

    // const int screenDim = 1000;

    float scale = 1;

    public Matrix CameraMatrix { get; private set; } = new Matrix();

    public Camera(GraphicsDeviceManager graphics, GameWindow window)
    {
        graphicsRef = graphics;
        gameWindowRef = window;
        UpdateScaling();
    }

    public void Update(GameTime t, Vector2 playerPos)
    {
        pos = playerPos;
        scale = 1 + MathF.Sin((float)t.TotalGameTime.TotalSeconds) * 0.5f;
        UpdateScaling();
    }
    
    public void UpdateScaling()
    {
        Matrix m = Matrix.CreateTranslation(new Vector3(-pos.X, -pos.Y, 0));
        // float xScale = (float)gameWindowRef.ClientBounds.Width / screenDim;
        // float yScale = (float)gameWindowRef.ClientBounds.Height / screenDim;
        // m *= Matrix.CreateScale(xScale, yScale, 1);
        m *= Matrix.CreateScale(scale, scale, 0);
        m *= Matrix.CreateTranslation((float)gameWindowRef.ClientBounds.Width / 2, (float)gameWindowRef.ClientBounds.Height / 2, 0);
        CameraMatrix = m;
    }

}