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
using System.Text.Json;
using System.IO;

namespace Monogaming;

public class GameState
{
    //TODO: implement a game state system here to update things based off current state

    public UI Ui { get; private set; } = new();
    public Map Map { get; private set; } = new();
    public Player Player { get; private set; }
    CommandManager commandManager;


    bool isInMainMenu = true;
    

    bool isInGame = false;
    bool isPaused = false;


    public GameState(GameWindow window)
    {
        window.ClientSizeChanged += Ui.HandleWindowChange;
        var stream = File.OpenRead("./GameData/corePlayerInfo.json");
        Player = JsonSerializer.Deserialize<Player>(stream);
        
        UiAction.GlobalInit(this);

        _ = Map.LoadChunkAsync("baseMap");
    }

    public void EnterMainGame()
    {
        isInGame = true;
        isPaused = false;
        isInMainMenu = false;
        Ui.DeactivateElement("mainMenu");
    }

    public void Update(GameTime gameTime)
    {
        Ui.Update(gameTime);

        //you should have something here that stops if theres a transition in progress
        if (isInGame)
        {
            Player.Update(gameTime, this);
            Map.Update(gameTime);
        }
    }

    public void Draw(SpriteBatch spriteBatch, Hitbox screen)
    {
        Map.Draw(spriteBatch, screen);
        Player.Draw(spriteBatch);
    }

    public void DrawUI(SpriteBatch spriteBatch)
    {
        Ui.Draw(spriteBatch);
    }
}