using Apos.Input;
using Assimp;
using Microsoft.Xna.Framework.Input;
using Monogaming.Entities;
using Monogaming.Interfaces;
using Monogaming.Items;
using Monogaming.MapThings;
using Monogaming.TextureThings;
using Monogaming.Ui;
using Monogaming.Ui.Transitions;

namespace Monogaming;

public class MeowGame : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch SpriteBatch;


    GameState gameState;

    Camera camera;
    RenderTarget2D lightingTarget;

    //Effect testEffect;

    //TEMP:
    Entity testEntity;
    Entity testEntity2;

    List<ImUpdatable> activeUpdatables = new();
    List<ImDrawable> activeDrawables = new();
    float timeSinceCleared = 0;

    public static Vector2 OldScaledMousePos { get; private set; }
    public static Vector2 ScaledMousePos { get; private set; }
    public static Vector2 ScaledMouseDelta { get; private set; }


    public MeowGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content\\Resources";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        camera = new(Window, _graphics);
        Window.AllowUserResizing = true;
        Window.Title = "Fun awesome game :)";

        base.Initialize();
    }

    protected override void LoadContent()
    {
        InputHelper.Setup(this);
        SpriteBatch = new SpriteBatch(GraphicsDevice);
        Textures.Initialize(Content, _graphics.GraphicsDevice);
        ItemDict.LoadFromFile("coreItems");
        RecipeBook.LoadFromFile("coreRecipes");

        EntityDict.LoadEntities("coreEntities");
        TileDict.LoadTiles("coreTiles");

        //testEffect = Content.Load<Effect>("testshader");
        lightingTarget = new RenderTarget2D(GraphicsDevice, Camera.ScreenWidth, Camera.ScreenHeight,
        false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);

        gameState = new(Window, ref camera);
        testEntity = EntityDict.CreateEntity("core_testEntity", Vector2.Zero);
        testEntity2 = EntityDict.CreateEntity("core_testEntity", Vector2.Zero);
        testEntity2.SetTexture(TextureHandler.TextureType.Walking);
        activeUpdatables.Add(testEntity);
        activeUpdatables.Add(testEntity2);
        activeDrawables.Add(testEntity);
        activeDrawables.Add(testEntity2);
    }

    ICondition escape = new AnyCondition(
        new KeyboardCondition(Keys.Escape),
        new GamePadCondition(GamePadButton.Back, 0)
    );

    protected override void Update(GameTime gameTime)
    {
        if (escape.Pressed())
            Exit();
        InputHelper.UpdateSetup(gameTime);

        OldScaledMousePos = ScaledMousePos;
        ScaledMousePos = InputHelper.NewMouse.Position.ToVector2() / camera.Scale;
        ScaledMouseDelta = ScaledMousePos - OldScaledMousePos;
        gameState.Update(gameTime);

        float s = (float)gameTime.TotalGameTime.TotalSeconds;
        testEntity.SetPos(new Vector2(200 * MathF.Sin(s + 10),
        100 * MathF.Sin(s + 10) + 100 * MathF.Cos(s / 2)));

        foreach (ImUpdatable e in activeUpdatables)
        {
            if (e.IsActive)
            {
                e.Update(gameTime);
            }
        }

        camera.Update(gameTime, gameState.Player.body.Center);

        if (TransitionManager.IsTransitioning)
        {
            TransitionManager.Update(gameTime);
        }

        InputHelper.UpdateCleanup();
        base.Update(gameTime);
    }


    protected override void Draw(GameTime gameTime)
    {
        Hitbox screen = camera.Screen;

        GraphicsDevice.SetRenderTarget(lightingTarget);
        GraphicsDevice.Clear(new Color(0.5f, 0.5f, 0.5f, 0.5f));
        SpriteBatch.Begin(blendState: BlendState.Additive, transformMatrix: camera.TranslationMatrix * camera.ScaleMatrix * Matrix.CreateScale(Camera.ScreenWidth / (float)Window.ClientBounds.Width, Camera.ScreenHeight / (float)Window.ClientBounds.Height, 1) * Matrix.CreateTranslation(Camera.ScreenWidth / 2, Camera.ScreenHeight / 2, 0));
        gameState.DrawLights(SpriteBatch, screen);
        SpriteBatch.End();
        GraphicsDevice.SetRenderTarget(null);


        // GraphicsDevice.SetRenderTarget(gameTarget);
        if (timeSinceCleared >= 0)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            timeSinceCleared = 0;

        }
        else timeSinceCleared += (float)gameTime.ElapsedGameTime.TotalSeconds;


        // testEffect.Parameters["view_projection"].SetValue(camera.ViewProjMatrix);        
        // testEffect.Parameters["time"].SetValue((float)gameTime.TotalGameTime.TotalSeconds);
        SpriteBatch.Begin(/*effect: testEffect, */samplerState: SamplerState.PointClamp, transformMatrix: camera.ViewMatrix);

        //https://github.com/SuRGeoNix/Flyleaf
        //try implementing this for videos

        foreach (ImDrawable e in activeDrawables)
        {
            if (e.IsVisible)
            {
                e.Draw(SpriteBatch, screen);
            }
        }
        gameState.Draw(SpriteBatch, screen);
        SpriteBatch.End();

        var multiplyBlend = new BlendState
        {
            ColorBlendFunction = BlendFunction.Add,
            ColorSourceBlend = Blend.DestinationColor,
            ColorDestinationBlend = Blend.Zero
        };

        SpriteBatch.Begin(blendState: multiplyBlend);
        SpriteBatch.Draw(lightingTarget, Window.ClientBounds with { X = 0, Y = 0 }, Color.White);
        SpriteBatch.End();

        SpriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: null);
        gameState.DrawUI(SpriteBatch);
        SpriteBatch.End();

        if (TransitionManager.IsTransitioning)
        {
            TransitionManager.DrawTransitionUninit(SpriteBatch);
        }

        // GraphicsDevice.SetRenderTarget(null);


        // SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        // // SpriteBatch.Draw(gameTarget, new Rectangle(0,0,Window.ClientBounds.Width, Window.ClientBounds.Height),
        // // null, Color.White);
        // SpriteBatch.Draw(gameTarget, new Rectangle(0,0,ScreenWidth, ScreenHeight),
        // null, Color.White);
        // SpriteBatch.End();

        //do ui stuff here probably



        base.Draw(gameTime);
    }
}
