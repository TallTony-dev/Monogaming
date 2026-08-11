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
    public static Save ActiveSave { get; private set; }
    public UI Ui { get; private set; }
    public Map Map { get; private set; }
    public Player Player { get; private set; }

    public Camera cameraRef { get; private set; }


    public UiEditor uiEditor { get; private set; }
    public MapEditor mapEditor { get; private set; }
    
    State state = State.InMenus;
    bool isGameLogicPaused = false;


    public GameState(GameWindow window, ref Camera cameraRef)
    {
        this.cameraRef = cameraRef;
        Map = new Map(cameraRef, this);
        Ui = new UI(window);

        uiEditor = new (this, window);
        mapEditor = new (this);

        window.ClientSizeChanged += Ui.HandleWindowChange;
        var stream = File.OpenRead("./GameData/corePlayerInfo.json");
        Player = JsonSerializer.Deserialize<Player>(stream);
        
        Command.GlobalInit(this);
    }


    public void LoadSave(string saveName)
    {
        string path = "./GameData/" + saveName + ".json";
        if (File.Exists(path)) {
            Stream textStream = File.OpenRead(path);
            JsonDocument document = JsonDocument.Parse(textStream);
            JsonElement root = document.RootElement;
            ActiveSave = JsonSerializer.Deserialize<Save>(root);
        }
        else
        {
            Debug.WriteInfo($"No save file found at {path}, creating new save");
            ActiveSave = new Save(saveName, new Save.PlayerPos(0, 0, "baseMap"), null, null);
        }
        Player.body.SetPos(new Vector2(ActiveSave.PlayerSavePos.X, ActiveSave.PlayerSavePos.Y));
        _ = Map.LoadChunkAsync("./GameData/saves/" + saveName + "/chunks/" + ActiveSave.PlayerSavePos.ChunkName + ".json");
    }

    public void Save(string saveName)
    {
        string path = "./GameData/saves/" + saveName + $"/{saveName}.json";
        if (File.Exists(path))
        {
            File.Move(path, path + ".bckp", true);
        }
        var options = new JsonSerializerOptions { WriteIndented = true };
        options.Converters.Add(new Vector2JsonConverter());
        File.WriteAllText(path, JsonSerializer.Serialize(ActiveSave, options));

        SaveLoadedChunk("./GameData/saves/" + saveName + "/chunks/" + Map.GetLoadedChunk().MapName + ".json");
        ActiveSave.SetPlayerSavePos(new Save.PlayerPos((int)MathF.Round(Player.body.Pos.X), (int)MathF.Round(Player.body.Pos.X), Map.GetLoadedChunk().MapName));
        Debug.WriteInfo($"Saved loaded save to {path}");
    }

    private void SaveLoadedChunk(string path)
    {
        Chunk chunk = Map.GetLoadedChunk();
        if (chunk != null) {
            if (File.Exists(path))
            {
                File.Move(path, path + ".bckp", true);
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            options.Converters.Add(new Vector2JsonConverter());
            File.WriteAllText(path, JsonSerializer.Serialize(chunk, options));

            Debug.WriteInfo($"Saved loaded chunk to {path}");
        }
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
    public void EnterLoadMenu()
    {
        state = State.InMenus;
        isGameLogicPaused = true;
        Ui.ActivateElement("loadSaveMenu");
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

        Chunk currentChunk = Map.GetLoadedChunk();
        
        //you should have something here that stops if theres a transition in progress
        if (!isGameLogicPaused && currentChunk != null)
        {
            Player.Update(gameTime, currentChunk, Map);
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