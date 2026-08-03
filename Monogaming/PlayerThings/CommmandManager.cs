using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Apos.Input;

namespace Monogaming.PlayerThings;

/// <summary>
/// TODO: Implement this class, a way to send commands to do assorted things and call assorted functions
/// Using reflection is acceptable here as this is mostly debug stuff
/// </summary>
public class CommandManager
{
    public bool IsActive;

    List<Command> Commands = new();
    public void Update(GameTime gameTime, ref GameState state)
    {
        
    }

    public void AddCommand(string command)
    {
        Commands.Add(new Command(command));
    }


    private class Command
    {
        string command;
        
        Action<GameState> action;

        public Command(string cmd)
        {
            
        }
    }
}