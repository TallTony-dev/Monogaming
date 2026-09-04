namespace Monogaming.Ui.Transitions;

public static class TransitionManager
{
    static Transition activeTransition;

    public static bool IsTransitioning => activeTransition != null;
    
    public static void SetTransition(Transition transition)
    {
        activeTransition = transition;
    }

    public static void Update(GameTime gameTime)
    {
        activeTransition?.Update(gameTime);
        if (activeTransition?.CurrentState == TransitionState.Complete)
        {
            activeTransition = null;
        }
    }

    /// <summary>
    /// IMPORTANT: DO NOT CALL WITH STARTED SPRITEBATCH
    /// </summary>
    public static void DrawTransitionUninit(SpriteBatch uninitedBatch)
    {
        activeTransition?.DrawUninit(uninitedBatch);
    }

}