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
    public string Text { get; set; }

    [JsonConstructor]
    public TextElement(string ID, AnchorPoint Anchor, Hitbox drawBox, string Text, List<UiElement> Children) : base(ID, Anchor, drawBox, Children)
    {
        this.Text = Text;
    }


    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        ScaledDrawBox.DrawStringInto(spriteBatch, Text);
        base.Draw(spriteBatch);
    }
}