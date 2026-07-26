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

namespace Monogaming.Ui;

public class TextElement : UiElement
{

    protected string text { get; private set; }

    [JsonConstructor]
    public TextElement(string ID, Hitbox drawBox, string text, List<UiElement> Children) : base(drawBox, ID, Children)
    {
        this.drawBox = drawBox;
        this.text = text;
    }


    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        float scaleX = drawBox.Width / Textures.MainFont.MeasureString(text).X;
        spriteBatch.DrawString(Textures.MainFont, text, drawBox.Pos, Color.BlueViolet, 0, Vector2.Zero, scaleX, SpriteEffects.None, 0);
        base.Draw(spriteBatch);
    }
}