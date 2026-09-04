using Monogaming.Entities;

public class Camera
{
    GameWindow gameWindowRef;
    GraphicsDeviceManager graphicsRef;
    Vector2 prevTarget = Vector2.Zero;

    private float trackSpeed;

    public const int ScreenWidth = 640; // I think at this point its just ui width lol
    public const int ScreenHeight = ScreenWidth / 16 * 9;

    private float _scale = 1f;
    public Vector2 UiScale
    {
        get
        {
            return new Vector2((float)gameWindowRef.ClientBounds.Width / ScreenWidth, (float)gameWindowRef.ClientBounds.Height / ScreenHeight);
        }
    }
    public float Scale
    {
        get
        {
            return _scale + (float)gameWindowRef.ClientBounds.Width / ScreenWidth;
        }
    }


    public Matrix ScaleMatrix
    {
        get
        {
            return Matrix.CreateScale(Scale, Scale, 0);
        }
    }

    public Matrix TranslationMatrix
    {
        get
        {
            return Matrix.CreateTranslation(new Vector3(-prevTarget.X, -prevTarget.Y, 0));
        }
    }
    /// <summary>
    /// View matrix to be passed when starting spritebatch
    /// </summary>
    public Matrix ViewMatrix
    {
        get
        {
            Matrix m = TranslationMatrix;
            m *= ScaleMatrix;
            m *= Matrix.CreateTranslation(gameWindowRef.ClientBounds.Width / 2, gameWindowRef.ClientBounds.Height / 2, 0);


            // if (graphicsRef.GraphicsDevice.UseHalfPixelOffset)
            // {
            m.M41 += -0.5f * m.M11;
            m.M42 += -0.5f * m.M22;
            // }
            return m;
        }
    }

    /// <summary>
    /// View-Projection matrix to be passed into vertex shader
    /// </summary>
    public Matrix ViewProjMatrix
    {
        get
        {
            Matrix m = ViewMatrix;
            return m * Matrix.CreateOrthographicOffCenter(0, gameWindowRef.ClientBounds.Width,
            gameWindowRef.ClientBounds.Height, 0, 0, 1);
        }
    }

    public Hitbox Screen
    {
        get
        {
            int width = (int)gameWindowRef.ClientBounds.Width;
            int height = (int)gameWindowRef.ClientBounds.Height;
            return new Hitbox(new Vector2(prevTarget.X - width / 2 / Scale, prevTarget.Y - height / 2 / Scale), (int)(width / Scale), (int)(height / Scale));
        }
    }



    public Camera(GameWindow window, GraphicsDeviceManager graphics, float trackSpeed = 1f)
    {
        this.trackSpeed = trackSpeed;
        gameWindowRef = window;
        graphicsRef = graphics;
    }

    public void Update(GameTime t, Vector2 trackingPos, bool snapToPos = false)
    {
        //Scale = 1 + MathF.Sin((float)t.TotalGameTime.TotalSeconds) * 0.5f;
        if (!snapToPos)
        {
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