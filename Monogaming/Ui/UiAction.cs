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
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public JsonElement Args { get; private set; } //maybe just one int lolol

    [JsonConstructor]
    public UiAction(string CommandName, JsonElement Args)
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
                case ("DeserializeUi"):
                    string menuName = Args.GetProperty("MenuName").GetString();
                    string outputPath = Args.GetProperty("OutputPath").GetString();
                    if (File.Exists(outputPath)) {
                        File.Move(outputPath, outputPath + ".bckp");
                        Debug.WriteInfo($"Moved old file at {outputPath} to .bckp");
                    }
                    Debug.WriteInfo($"Serialized menu {menuName} to {outputPath}");
                    File.WriteAllText(outputPath, gameStateRef.Ui.SerializeMenu(menuName));
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