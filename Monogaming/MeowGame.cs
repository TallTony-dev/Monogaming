using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monogaming.Entities;
using Monogaming.TextureThings;
using Apos.Input;

namespace Monogaming;

public class MeowGame : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch SpriteBatch;

    Map map;
    Camera camera;
    Effect testEffect;
    Entity testEntity = new(new Vector2(100, 100), 100, 50, "Icon");
    Entity testEntity2 = new(new Vector2(100, 100), 100, 400, "Icon");
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

        testEffect = Content.Load<Effect>("testshader");

        base.Initialize();
    }

    protected override void LoadContent()
    {
        InputHelper.Setup(this);
        SpriteBatch = new SpriteBatch(GraphicsDevice);

        map = new();


        Textures.LoadTextures(Content);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        InputHelper.UpdateSetup();
        
        testEntity.SetPos(new Vector2(50 * MathF.Cos((float)gameTime.TotalGameTime.TotalSeconds), 
        50 * MathF.Sin((float)gameTime.TotalGameTime.TotalSeconds)));

        camera.Update(gameTime, testEntity.hitbox.Center);

        InputHelper.UpdateCleanup();
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        testEffect.Parameters["view_projection"].SetValue(camera.ViewProjMatrix);
        testEffect.Parameters["time"].SetValue((float)gameTime.TotalGameTime.TotalSeconds);
        SpriteBatch.Begin(transformMatrix: camera.ViewProjMatrix, effect: testEffect);


        //SpriteBatch.Draw(Textures.Get2DTex("Icon"), new Vector2(0,0), Color.White);
        testEntity2.Draw(SpriteBatch);
        testEntity.Draw(SpriteBatch);

        SpriteBatch.End();


        base.Draw(gameTime);
    }
}
