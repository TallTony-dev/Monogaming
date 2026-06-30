using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Monogaming;

public static class InputManager
{
    public static bool isLeftMousePressed { get; private set; } = false;
    public static bool isLeftMouseDown { get; private set; } = false;

    public static Vector2 mousePos { get; private set; }


    public static void Update()
    {
        KeyboardState kState = Keyboard.GetState();
        MouseState mState = Mouse.GetState();

        mousePos = new Vector2(mState.Position.X, mState.Position.Y);

        if (mState.LeftButton == ButtonState.Pressed)
        {
            if (!isLeftMousePressed && !isLeftMouseDown)
            {
                isLeftMousePressed = true;
                isLeftMouseDown = true;
            }
            else if (isLeftMouseDown)
            {
                isLeftMousePressed = false;
            }
        }
        else
        {
            isLeftMouseDown = false;
            isLeftMousePressed = false;
        }
    }



}