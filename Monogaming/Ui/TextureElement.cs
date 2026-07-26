using System;
using Monogaming.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monogaming.Interfaces;
using Monogaming.TextureThings;
using Microsoft.Xna.Framework.Content;
using Apos.Input;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace Monogaming.Ui;

public class TextureElement : UiElement
{

    protected TextureHandler textureHandler { get; private set; }

    [JsonConstructor]
    public TextureElement(string ID, Hitbox drawBox, TextureHandler textureHandler, List<UiElement> Children) : base(drawBox, ID, Children)
    {
        this.drawBox = drawBox;
        this.textureHandler = textureHandler;
    }


    public override void Update(GameTime gameTime)
    {
        if (drawBox.Intersects(InputHelper.NewMouse.Position))
        {
            if (!(InputHelper.NewMouse.LeftButton == ButtonState.Pressed))
            {
                textureHandler.SetTexture(TextureHandler.TextureType.OnHover);
            }
            else
            {
                textureHandler.SetTexture(TextureHandler.TextureType.OnClick);
            }
        }

        textureHandler.Update(gameTime);
        
        base.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        textureHandler.Draw(spriteBatch, drawBox.AsRect());
        base.Draw(spriteBatch);
    }
}