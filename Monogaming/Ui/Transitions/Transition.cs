namespace Monogaming.Ui.Transitions;

public class Transition
{
    
    protected bool IsLoadComplete { get; set; }
    public TransitionState CurrentState { get; protected set; }

    private Func<bool> _checkLoadComplete;

    public Transition(Func<bool> checkLoadComplete)
    {
        _checkLoadComplete = checkLoadComplete;
    }

    public virtual void Update(GameTime gameTime)
    {
        IsLoadComplete = _checkLoadComplete.Invoke();
    }

    /// <summary>
    /// IMPORTANT: DO NOT CALL WITH STARTED SPRITEBATCH
    /// </summary>
    public virtual void DrawUninit(SpriteBatch uninitedBatch)
    {

    }
}

public enum TransitionState { Beginning, Middle, End, Complete }