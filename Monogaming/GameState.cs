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
using Monogaming.MapThings;
using Monogaming.PlayerThings;

namespace Monogaming;

public class GameState
{
    //TODO: implement a game state system here to update things based off current state

    UI ui = new();
    Map map = new();
    Player player = new();

    

    public GameState(GameWindow window)
    {
        window.ClientSizeChanged += ui.HandleWindowChange;
    }

    public void Update(GameTime gameTime)
    {
        
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        
    }
}