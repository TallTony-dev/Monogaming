using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Apos.Input;

namespace Monogaming.PlayerThings;


public class InputManager
{

    #region Conditons
    ICondition Left = new AnyCondition(
        new KeyboardCondition(Keys.A),
        new KeyboardCondition(Keys.Left),
        new GamePadCondition(GamePadButton.Left, 0)
    );
    ICondition Right = new AnyCondition(
        new KeyboardCondition(Keys.D),
        new KeyboardCondition(Keys.Right),
        new GamePadCondition(GamePadButton.Right, 0)
    );
    ICondition Up = new AnyCondition(
        new KeyboardCondition(Keys.W),
        new KeyboardCondition(Keys.Up),
        new GamePadCondition(GamePadButton.Up, 0)
    );
    ICondition Down = new AnyCondition(
        new KeyboardCondition(Keys.S),
        new KeyboardCondition(Keys.Down),
        new GamePadCondition(GamePadButton.Down, 0)
    );
    #endregion


    public Vector2 GetMovementDir()
    {
        bool leftPressed = Left.Held();
        bool rightPressed = Right.Held();
        bool upPressed = Up.Held();
        bool downPressed = Down.Held();
        Vector2 dir = new Vector2();
        dir.X += leftPressed ? -1 : (rightPressed ? 1 : 0);
        dir.Y += downPressed ? 1 : (upPressed ? -1 : 0);
        if (dir != Vector2.Zero) 
        {
            dir.Normalize();
        }
        return dir;
    }
}