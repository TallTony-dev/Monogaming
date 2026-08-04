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
    public Map Map { get; private set; }
    public Player Player { get; private set; }
    CommandManager commandManager;

    public Camera cameraRef { get; private set; }


    public UiEditor uiEditor { get; private set; }
    public MapEditor mapEditor { get; private set; }
    
    State state = State.InMenus;
    bool isGameLogicPaused = false;


    public GameState(GameWindow window, ref Camera cameraRef)
    {
        this.cameraRef = cameraRef;
        Map = new Map(cameraRef);
        Ui = new UI(window);

        uiEditor = new (this, window);
        mapEditor = new (this);

        window.ClientSizeChanged += Ui.HandleWindowChange;
        var stream = File.OpenRead("./GameData/corePlayerInfo.json");
        Player = JsonSerializer.Deserialize<Player>(stream);
        
        UiAction.GlobalInit(this);

        _ = Map.LoadChunkAsync("./GameData/Chunks/baseMap.json");
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

    public void EnterUiEditor()
    {
        state = State.InUiEditor;
        isGameLogicPaused = true;
        Ui.ActivateElement("uiEditorMenu");
    }
    
    public void EnterMapEditor()
    {
        state = State.InMapEditor;
        isGameLogicPaused = false;
        Ui.ActivateElement("tileMapEditorMenu");
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
            case State.InUiEditor:
                uiEditor.Update(gameTime);
            break;
            case State.InMapEditor:
                mapEditor.Update(gameTime);
            break;
            default:
            break;
        }
    }

    public void Draw(SpriteBatch spriteBatch, Hitbox screen)
    {
        switch (state)
        {
            case State.InUiEditor:
                Map.Draw(spriteBatch, screen);
            break;
            case State.InMapEditor:
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
        if (state == State.InUiEditor)
        {
            uiEditor.Draw(spriteBatch);
        }
        Ui.Draw(spriteBatch);
    }


    private enum State { InMenus, InGame, InUiEditor, InMapEditor }
}