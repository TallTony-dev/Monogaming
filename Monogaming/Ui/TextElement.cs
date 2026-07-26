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
    [JsonInclude]
    protected string Text { get; private set; }

    [JsonConstructor]
    public TextElement(string ID, Hitbox drawBox, string Text, List<UiElement> Children) : base(ID, drawBox, Children)
    {
        this.Text = Text;
    }


    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        float scaleX = DrawBox.Width / Textures.MainFont.MeasureString(Text).X;
        spriteBatch.DrawString(Textures.MainFont, Text, DrawBox.Pos, Color.BlueViolet, 0, Vector2.Zero, scaleX, SpriteEffects.None, 0);
        base.Draw(spriteBatch);
    }
}