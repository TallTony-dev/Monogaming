using System;
using Monogaming.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monogaming.Interfaces;
using Monogaming.TextureThings;
using Microsoft.Xna.Framework.Content;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using Apos.Input;
using Monogaming.PlayerThings;
using Monogaming;
using System.Linq;

namespace Monogaming.Ui;

public class ButtonElement : TextureElement
{
    public List<UiAction> OnClickActions { get; private set; }
    public string Text { get; set; }
    
    [JsonConstructor]
    public ButtonElement(string ID, AnchorPoint Anchor, Hitbox drawBox, TextureHandler textureHandler, List<UiAction> OnClickActions, string Text, List<UiElement> Children) : 
    base(ID, Anchor, drawBox, textureHandler, Children)
    {
        this.OnClickActions = OnClickActions == null? new List<UiAction>() : OnClickActions;
        this.Text = Text;
    }

    public override void Update(GameTime gameTime)
    {
        if (ScaledDrawBox.Intersects(InputHelper.NewMouse.Position))
        {
            if (InputConditions.click.Pressed())
            {
                foreach (UiAction action in OnClickActions)
                {
                    action.DispatchCommand();
                }
            }
        }

        base.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        if (Text != null) {
            ScaledDrawBox.DrawStringInto(spriteBatch, Text);
        }
    }

}