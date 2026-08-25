namespace Monogaming.PlayerThings;


public class InputManager
{

    public Vector2 GetMovementDir()
    {
        bool leftPressed = InputConditions.Left.Held();
        bool rightPressed = InputConditions.Right.Held();
        bool upPressed = InputConditions.Up.Held();
        bool downPressed = InputConditions.Down.Held();
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