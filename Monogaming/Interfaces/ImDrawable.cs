using Monogaming.Entities;

namespace Monogaming.Interfaces;

public interface ImDrawable
{
    bool IsVisible { get; } //controls if should be drawn
    void Draw(SpriteBatch spriteBatch, Hitbox screen);
    void TryDrawLight(SpriteBatch spriteBatch, Hitbox screen);

}
