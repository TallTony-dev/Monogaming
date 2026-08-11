using System;
using Monogaming.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Monogaming.Interfaces;
using Monogaming.TextureThings;
using Microsoft.Xna.Framework.Content;
using System.Text.Json.Serialization;
using Monogaming.Items;
using Monogaming.Ui;
using Apos.Input;
using Microsoft.Xna.Framework.Input;

namespace Monogaming.Ui.Windows;



public class CompositeWindow : WindowElement
{

    [JsonConstructor]
    public CompositeWindow(Hitbox drawBox, List<UiElement> Children) : base(AnchorPoint.TopLeft, drawBox, Children)
    {
        foreach (WindowElement e in Children)
        {
            e.isChildOfOtherWindow = true;
        }
        UpdateChildPositions();
    }


    private void UpdateChildPositions()
    {
        int totalwidth = 0;
        foreach (WindowElement e in Children)
        {
            e.SetRealPos(new Vector2(totalwidth, DrawBox.Y));
            totalwidth += e.DrawBox.Width;
        }
        float widthScale = DrawBox.Width / totalwidth;
        foreach (WindowElement e in Children)
        {
            e.SetRealDims(new Vector2(e.DrawBox.Width * widthScale, DrawBox.Height));
        }
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        UpdateChildPositions();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
   
        base.Draw(spriteBatch);
    }
}