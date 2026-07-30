global using System;
global using System.Collections.Generic;
global using Microsoft.Xna.Framework;
global using Microsoft.Xna.Framework.Graphics;
global using System.Text.Json;
using Monogaming.Entities;
using Monogaming.TextureThings;
using Monogaming.Ui;
using Apos.Input;
using Monogaming.Interfaces;
using System.Diagnostics.Metrics;
using Monogaming.MapThings;
using Monogaming.PlayerThings;
using System.IO;
using Monogaming.Utils;

namespace Monogaming;

public class GameState
{
    //TODO: implement a game state system here to update things based off current state

    public UI Ui { get; private set; }
    public Map Map { get; private set; } = new();
    public Player Player { get; private set; }
    CommandManager commandManager;


    
    
    State state = State.InMenus;
    bool isGameLogicPaused = false;

  
    bool isInLevelEditor = false;

    public GameState(GameWindow window)
    {
        Ui = new UI(window);
        window.ClientSizeChanged += Ui.HandleWindowChange;
        var stream = File.OpenRead("./GameData/corePlayerInfo.json");
        Player = JsonSerializer.Deserialize<Player>(stream);
        
        UiAction.GlobalInit(this);

        _ = Map.LoadChunkAsync("baseMap");
    }

    public void EnterMainGame()
    {
        state = State.InGame;
        isGameLogicPaused = false;
        Ui.DeactivateElement("mainMenu");
    }
    public void ExitMainGame()
    {
        state = State.InMenus;
        isGameLogicPaused = true;
        Ui.ActivateElement("mainMenu");
    }

    public void EnterEditor()
    {
        state = State.InEditor;
        isGameLogicPaused = true;
        //Ui.DeactivateElement("mainMenu");
        // Ui.ActivateElement("editorMenu");
    }

    public void Update(GameTime gameTime)
    {
        Ui.Update(gameTime);
        
        //you should have something here that stops if theres a transition in progress
        if (!isGameLogicPaused)
        {
            Player.Update(gameTime, this);
            Map.Update(gameTime);
        }
        switch (state)
        {
            case State.InEditor:
                LevelEditor.Update(gameTime);
            break;

            default:
            break;
        }
    }

    public void Draw(SpriteBatch spriteBatch, Hitbox screen)
    {
        switch (state)
        {
            case State.InEditor:
                LevelEditor.Draw(spriteBatch);
                Map.Draw(spriteBatch, screen);
            break;
            case State.InGame:
                Map.Draw(spriteBatch, screen);
                Player.Draw(spriteBatch);
            break;
            default:
            break;
        }
    }

    public void DrawUI(SpriteBatch spriteBatch)
    {
        Ui.Draw(spriteBatch);
    }


    private enum State { InMenus, InGame, InEditor }
}