using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monogaming.Entities;
using Monogaming.TextureThings;
using Monogaming.Ui;
using Apos.Input;
using Monogaming.Interfaces;
using System.Diagnostics.Metrics;
using Monogaming.Items;

namespace Monogaming;

public class MeowGame : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch SpriteBatch;

    Map map;
    Camera camera;
    SpriteFont font;

    Effect testEffect;

    //TEMP:
    Entity testEntity;
    Entity testEntity2;

    List<ImUpdatable> activeUpdatables = new();
    List<ImDrawable> activeDrawables = new();
    float timeSinceCleared = 0;
    RenderTarget2D testTarget;
    //:TEMP


    public MeowGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        camera = new(_graphics, Window);
        Window.AllowUserResizing = true;
        Window.Title = "Fun awesome game :)";
        //Window.ClientSizeChanged += some ui update function

        testEntity = new(new Vector2(100, 100), 100, 50, "Icon");
        testEntity2 = new(new Vector2(100, 100), 100, 400, "Icon");
        activeUpdatables.Add(testEntity);
        activeUpdatables.Add(testEntity2);
        activeDrawables.Add(testEntity);
        activeDrawables.Add(testEntity2);

        testEffect = Content.Load<Effect>("testshader");
        testTarget = new RenderTarget2D(GraphicsDevice, 1000, 1000, 
        false, SurfaceFormat.Color, DepthFormat.None, 0 ,RenderTargetUsage.PreserveContents);


        base.Initialize();
    }

    protected override void LoadContent()
    {
        InputHelper.Setup(this);
        SpriteBatch = new SpriteBatch(GraphicsDevice);
        font = Content.Load<SpriteFont>("default");
        map = new();
        ItemDict.LoadFromFile("coreItems");
        RecipeBook.LoadFromFile("coreRecipes");
        Textures.LoadTextures(Content);
    }

    ICondition escape = new AnyCondition(
        new KeyboardCondition(Keys.Escape),
        new GamePadCondition(GamePadButton.Back, 0)
    );
    ICondition click = new AnyCondition(
        new KeyboardCondition(Keys.Space),
        new GamePadCondition(GamePadButton.A, 0)
    );

    string lastPerformance = "";

    protected override void Update(GameTime gameTime)
    {
        if (escape.Pressed())
            Exit();
        InputHelper.UpdateSetup();
        float s = (float)gameTime.TotalGameTime.TotalSeconds;
        testEntity.SetPos(new Vector2(400/*200 * MathF.Cos(s)*/, 
        200 * MathF.Sin(s + 10) + 100 * MathF.Cos(s / 2)));

        foreach (ImUpdatable e in activeUpdatables)
        {
            if (e.IsActive)
            {
                e.Update(gameTime);
            }
        }


        if (click.Pressed())
        {

        }

        camera.Update(gameTime, testEntity.hitbox.Center);

        InputHelper.UpdateCleanup();
        base.Update(gameTime);
    }


    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.SetRenderTarget(testTarget);
        if (timeSinceCleared >= 0)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            timeSinceCleared = 0;
        } else timeSinceCleared += (float)gameTime.ElapsedGameTime.TotalSeconds;

        testEffect.Parameters["view_projection"].SetValue(camera.ViewProjMatrix);        
        testEffect.Parameters["time"].SetValue((float)gameTime.TotalGameTime.TotalSeconds);
        SpriteBatch.Begin(transformMatrix: camera.ViewProjMatrix, effect: testEffect);

        //https://github.com/SuRGeoNix/Flyleaf
        //try implementing this for videos
        Hitbox screen = camera.Screen;
        foreach (ImDrawable e in activeDrawables)
        {
            if (e.IsVisible)
            {
                e.Draw(SpriteBatch, screen);
            }
        }
        SpriteBatch.End();
        

        SpriteBatch.Begin();
        //ui pass?
        SpriteBatch.DrawString(font, lastPerformance, new Vector2(0,0), Color.Black);
        SpriteBatch.End();
        

        GraphicsDevice.SetRenderTarget(null);
        
        SpriteBatch.Begin();
        SpriteBatch.Draw(testTarget, new Rectangle(0,0,Window.ClientBounds.Width, Window.ClientBounds.Height),
        null, Color.White);
        SpriteBatch.End();

        base.Draw(gameTime);
    }
}
