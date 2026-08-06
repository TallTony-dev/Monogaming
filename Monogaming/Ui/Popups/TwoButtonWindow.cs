using System;
using Monogaming.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Monogaming.Interfaces;
using Monogaming.TextureThings;
using Microsoft.Xna.Framework.Content;
using Monogaming.Ui;
using System.Text.Json.Serialization;
using Monogaming.Items;
using Apos.Input;

namespace Monogaming.Ui.Windows;

public class TwoButtonWindow : WindowElement
{
    private const int ButtonPadding = 5;
    private const int ButtonHeight = 30;
    private const int TextPadding = 4;

    ButtonElement kid1;
    ButtonElement kid2;
    

    public TwoButtonWindow(Hitbox drawBox, string Text, string button1Text, string button2Text) : base("twoButtonElement", AnchorPoint.TopLeft, drawBox, null)
    {
        Hitbox textBox = new Hitbox(new Vector2(TextPadding, TopBarHeight + TextPadding), drawBox.Width, drawBox.Height - TopBarHeight - TextPadding * 2 - ButtonHeight - ButtonPadding * 2);
        Hitbox h1 = new Hitbox(new Vector2(ButtonPadding, drawBox.Height - ButtonPadding - ButtonHeight), drawBox.Width / 3 - ButtonPadding, ButtonHeight);
        Hitbox h2 = new Hitbox(new Vector2(ButtonPadding + drawBox.Width * 2 / 3 - ButtonPadding, drawBox.Height - ButtonPadding - ButtonHeight), drawBox.Width / 3 - ButtonPadding, ButtonHeight);

        //maybe having null uiactions is bad design but probably fine 
        kid1 = new ButtonElement("kid1", AnchorPoint.TopLeft, h1, new TextureHandler(TextureHandler.TextureType.Default, "buttonRock.Unpressed"), 
        null, button1Text, null);
        kid2 = new ButtonElement("kid2", AnchorPoint.TopLeft, h2, new TextureHandler(TextureHandler.TextureType.Default, "buttonRock.Unpressed"), 
        null, button2Text, null);

        Children = new List<UiElement>() { kid1, kid2, new TextElement("text", AnchorPoint.TopLeft, textBox, Text, null) };
        
    }

    public override void UpdatePos(Rectangle window)
    {
        base.UpdatePos(window);

    }

    public override void Update(GameTime gameTime)
    {        
        base.Update(gameTime);
        if (!ShouldRemove)
        {
            if (kid1.WasClicked)
            {
                
            }
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);

        
    }
}