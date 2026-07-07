using System;
using Monogaming.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monogaming.Interfaces;
using Monogaming.TextureThings;
using System.Runtime.Intrinsics.X86;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace Monogaming.Ui;

public class Menu : ImDrawable, ImUpdatable
{
    public bool IsVisible { get; private set; } = true;
    public bool IsActive { get; private set; } = true;
    List<UiElement> elements = new();

    public Menu(List<UiElement> elements)
    {
        this.elements = elements;
    }

    public virtual void Update(GameTime gameTime)
    {
        foreach (UiElement e in elements)
        {
            if (e.IsActive)
            {
                e.Update(gameTime);
            }
        }
    }

    public virtual void Draw(SpriteBatch spriteBatch, Hitbox screen)
    {
        foreach (UiElement e in elements)
        {
            if (e.IsVisible)
            {
                e.Draw(spriteBatch, screen);
            }
        }
    }

}