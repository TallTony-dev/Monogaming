namespace Monogaming.Interfaces;

public interface ImUpdatable
{
    bool IsActive { get; } // determines if should be updated

    void Update(GameTime gameTime);
}
