using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monogaming.Entities;
using Monogaming.TextureThings;

namespace Monogaming;

public class MeowGame : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch SpriteBatch;

    Map map;
    Camera camera;

    Entity testEntity = new(new Vector2(100, 100), 100, 50, "Icon");

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

        base.Initialize();
    }

    protected override void LoadContent()
    {
        SpriteBatch = new SpriteBatch(GraphicsDevice);

        map = new();


        Textures.LoadTextures(Content);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        InputManager.Update();
        

        camera.Update(gameTime, testEntity.hitbox.Center);
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        SpriteBatch.Begin(transformMatrix: camera.CameraMatrix);

        //SpriteBatch.Draw(Textures.Get2DTex("Icon"), new Vector2(0,0), Color.White);
        testEntity.Draw(SpriteBatch);

        SpriteBatch.End();


        base.Draw(gameTime);
    }
}
