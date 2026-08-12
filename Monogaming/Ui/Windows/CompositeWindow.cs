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
using System.Linq;

namespace Monogaming.Ui.Windows;



public class CompositeWindow : WindowElement
{

    [JsonConstructor]
    public CompositeWindow(Hitbox drawBox, List<UiElement> Children, string id = "compWindow") : base(AnchorPoint.TopLeft, drawBox, Children, id)
    {
        foreach (WindowElement e in Children)
        {
            e.isChildOfOtherWindow = true;
        }
        UpdateChildPositions();
        IsResizable = false;
    }


    private void UpdateChildPositions()
    {
        int totalwidth = (int)(Children.Sum(t => t.DrawBox.Width) * UI.LiveUI.UiScale);
        float widthScale = (float)DrawBox.Width / totalwidth;

        int runningTotal = 0;
        foreach (WindowElement e in Children)
        {
            e.SetRealPos(new Vector2(runningTotal, 0));

            // e.SetRealDims(new Vector2(e.DrawBox.Width, DrawBox.Height));
            runningTotal += (int)(e.DrawBox.Width * UI.LiveUI.UiScale * widthScale);
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