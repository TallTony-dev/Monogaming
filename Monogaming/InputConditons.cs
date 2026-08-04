using Apos.Input;
using Microsoft.Xna.Framework.Input;

namespace Monogaming;

public static class InputConditions
{
    
    public static ICondition click = new MouseCondition(MouseButton.LeftButton);
    public static ICondition copy = new AllCondition(
        new KeyboardCondition(Keys.C), 
        new KeyboardCondition(Keys.LeftControl)
    );
    public static ICondition paste = new AllCondition(
        new KeyboardCondition(Keys.V), 
        new KeyboardCondition(Keys.LeftControl)
    );
    public static ICondition delete = new AllCondition(
        new KeyboardCondition(Keys.Back), 
        new KeyboardCondition(Keys.LeftControl)
    );

    public static ICondition Left = new AnyCondition(
        new KeyboardCondition(Keys.A),
        new KeyboardCondition(Keys.Left),
        new GamePadCondition(GamePadButton.Left, 0)
    );
    public static ICondition Right = new AnyCondition(
        new KeyboardCondition(Keys.D),
        new KeyboardCondition(Keys.Right),
        new GamePadCondition(GamePadButton.Right, 0)
    );
    public static ICondition Up = new AnyCondition(
        new KeyboardCondition(Keys.W),
        new KeyboardCondition(Keys.Up),
        new GamePadCondition(GamePadButton.Up, 0)
    );
    public static ICondition Down = new AnyCondition(
        new KeyboardCondition(Keys.S),
        new KeyboardCondition(Keys.Down),
        new GamePadCondition(GamePadButton.Down, 0)
    );


}