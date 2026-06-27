using System;

namespace Monogaming.Interfaces;

public interface ImDrawable
{
    bool IsVisible { get; } //controls if should be drawn

    void Draw(); //should use the static spritebatch in implementation
}
