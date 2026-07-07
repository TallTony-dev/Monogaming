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

namespace Monogaming;

public class MeowGame : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch SpriteBatch;

    Map map;
    Camera camera;
    SpriteFont font;

    Effect testEffect;
    Entity testEntity;
    Entity testEntity2;



    Wave testWave = new(new Vector2(0, 0), 300, 100, 10, 1);
    Wave testWave2 = new(new Vector2(0, 100), 300, 100, 10, 1, MathF.PI * 4);
    Wave testWavesum;

    List<ImUpdatable> activeUpdatables = new();
    List<ImDrawable> activeDrawables = new();

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
        //Window.ClientSizeChanged += some ui update function

        testEntity = new(new Vector2(100, 100), 100, 50, "Icon");
        testEntity2 = new(new Vector2(100, 100), 100, 400, "Icon");
        activeUpdatables.Add(testEntity);
        activeUpdatables.Add(testEntity2);
        activeDrawables.Add(testEntity);
        activeDrawables.Add(testEntity2);

        activeUpdatables.Add(testWave);
        activeUpdatables.Add(testWave2);

        testEffect = Content.Load<Effect>("testshader");

        Wave.Initialize(Content, GraphicsDevice);


        base.Initialize();
    }

    protected override void LoadContent()
    {
        InputHelper.Setup(this);
        SpriteBatch = new SpriteBatch(GraphicsDevice);
        font = Content.Load<SpriteFont>("default");
        map = new();


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
        testEntity.SetPos(new Vector2(200 * MathF.Cos(s), 
        200 * MathF.Sin(s + 10) + 100 * MathF.Cos(s / 2)));

        foreach (ImUpdatable e in activeUpdatables)
        {
            if (e.IsActive)
            {
                e.Update(gameTime);
            }
        }

        testWave.PhaseDiff += (float)(4 * gameTime.ElapsedGameTime.TotalSeconds);
        testWavesum = testWave.Sum(testWave2);
        testWavesum.SetPos(new Vector2(0, 200));
        testWavesum.Update(gameTime);
        if (click.Pressed())
        {
            float score = testWavesum.Amplitude;
            switch (score)
            {
                case < 0.3f : lastPerformance = "Perfect!"; break;
                case < 0.5f : lastPerformance = "Good"; break;
                default: lastPerformance = "bad"; break;
            }
        }

        camera.Update(gameTime, testEntity.hitbox.Center);

        InputHelper.UpdateCleanup();
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        testEffect.Parameters["view_projection"].SetValue(camera.ViewProjMatrix);
        double f = gameTime.TotalGameTime.TotalSeconds;
        
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


        testWave.Draw(SpriteBatch, camera.ViewProjMatrix);
        testWave2.Draw(SpriteBatch, camera.ViewProjMatrix);
        testWavesum.Draw(SpriteBatch, camera.ViewProjMatrix);


        base.Draw(gameTime);
    }
}
