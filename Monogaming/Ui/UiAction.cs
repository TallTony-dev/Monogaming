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

namespace Monogaming.Ui;

public class UiAction
{
    [JsonIgnore]
    public static GameState gameStateRef { get; private set; }
    public static void GlobalInit(GameState gameState)
    {
        gameStateRef = gameState;
    }


    public string CommandName { get; private set; }
    public JsonProperty Args { get; private set; } //maybe just one int lolol
    [JsonConstructor]
    public UiAction(string CommandName, JsonProperty Args)
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
                    gameStateRef.Ui.ActivateElement(Args.Value.GetString());
                break;
                case ("DeactivateElement"):
                    gameStateRef.Ui.DeactivateElement(Args.Value.GetString());
                break;
                case ("EnterMainGame"):
                    gameStateRef.EnterMainGame();
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