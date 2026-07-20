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

public class UI : ImDrawable, ImUpdatable
{
    public bool IsVisible { get; private set; } = true;
    public bool IsActive { get; private set; } = true;
    List<Menu> menus = new();

    public void Update(GameTime gameTime)
    {
        foreach (Menu menu in menus)
        {
            if (menu.IsActive)
            {
                menu.Update(gameTime);
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch, Hitbox screen)
    {
        foreach (Menu menu in menus)
        {
            if (menu.IsVisible)
            {
                menu.Draw(spriteBatch, screen);
            }
        }
    }
    //TODO: implement this wrapper and also an animation handler one too
}