using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Apos.Input;

namespace Monogaming.PlayerThings;

public class CommandManager
{
    public bool IsActive;

    List<Command> Commands = new();
    public void Update(GameTime gameTime)
    {
        
    }

    public void AddCommand(string command)
    {
        Commands.Add(new Command(command));
    }


    private class Command
    {
        string command;

        public Command(string cmd)
        {
            
        }
    }
}