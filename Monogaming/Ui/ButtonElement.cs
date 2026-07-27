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
using System.Linq;

namespace Monogaming.Ui;

public class ButtonElement : TextureElement
{
    [JsonIgnore]
    static ICondition click = new MouseCondition(MouseButton.LeftButton);
    public List<UiAction> OnClickActions { get; private set; }
    
    [JsonConstructor]
    public ButtonElement(string ID, Hitbox drawBox, TextureHandler textureHandler, List<UiAction> OnClickActions, List<UiElement> Children) : 
    base(ID, drawBox, textureHandler, Children)
    {
        this.OnClickActions = OnClickActions == null? new List<UiAction>() : OnClickActions;
    }

    public override void Update(GameTime gameTime)
    {
        if (DrawBox.Intersects(MeowGame.ScaledMousePos))
        {
            if (click.Pressed())
            {
                foreach (UiAction action in OnClickActions)
                {
                    action.DispatchCommand();
                }
            }
        }

        base.Update(gameTime);
    }

}