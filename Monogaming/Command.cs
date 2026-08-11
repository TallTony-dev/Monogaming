using System;
using Monogaming.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monogaming.Interfaces;
using Monogaming.TextureThings;
using Microsoft.Xna.Framework.Content;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using Apos.Input;
using Monogaming.PlayerThings;
using System.Text.Json;
using System.IO;
using Monogaming.MapThings;

namespace Monogaming;

public class Command
{
    [JsonIgnore]
    public static GameState gameStateRef { get; private set; }
    public static void GlobalInit(GameState gameState)
    {
        gameStateRef = gameState;
    }


    public string CommandName { get; private set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public JsonElement Args { get; private set; } //maybe just one int lolol

    [JsonConstructor]
    public Command(string CommandName, JsonElement Args)
    {
        this.CommandName = CommandName;
        this.Args = Args;
    }


    public void DispatchCommand()
    {
        if (!string.IsNullOrEmpty(CommandName)) {
            switch(CommandName)
            {
                case ("ActivateElement"):
                    gameStateRef.Ui.ActivateElement(Args.GetString());
                break;
                case ("DeactivateElement"):
                    gameStateRef.Ui.DeactivateElement(Args.GetString());
                break;
                case ("EnterMainGame"):
                    gameStateRef.EnterMainGame();
                break;
                case ("EnterUiEditor"):
                    gameStateRef.EnterUiEditor();
                break;
                case ("EnterMapEditor"):
                    gameStateRef.EnterMapEditor();
                break;
                case ("EnterLoadMenu"):
                    gameStateRef.EnterLoadMenu();
                break;
                case ("LoadSave"):
                    gameStateRef.LoadSave(Args.GetProperty("SaveName").GetString());
                break;
                case ("UiEditor"):
                    gameStateRef.uiEditor.HandleCommand(Args);
                break;
                case ("Map"):
                    gameStateRef.mapEditor.HandleCommand(Args);
                break;
                default:
                Debug.WriteWarn($"Fell out of dispatch ui command with name {CommandName}");
                break;
            }
        }
        else
        {
            Debug.WriteError($"Encountered null string when dispatching ui command");
        }
    }

}