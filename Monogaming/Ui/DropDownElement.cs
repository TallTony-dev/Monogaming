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


/// <summary>
/// Children under this only become activated after a targeted activation of the element
/// Or after being clicked on
/// </summary>
public class DropDownElement : TextureElement
{
    [JsonIgnore]
    static ICondition click = new MouseCondition(MouseButton.LeftButton);
    [JsonIgnore]
    private bool isMenuOpen = false;

    public string Text { get; private set; }

    [JsonConstructor]
    public DropDownElement(string ID, AnchorPoint Anchor, Hitbox drawBox, TextureHandler textureHandler, string text, List<UiElement> Children) : base(ID, Anchor, drawBox, textureHandler, Children)
    {
        Text = text;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (click.Pressed())
        {
            if (ScaledDrawBox.Intersects(InputHelper.NewMouse.Position)) 
            {
                foreach (UiElement element in Children)
                {
                    element.ActivateElement();
                }
                isMenuOpen = true;
            }
            else// if (!Children.Any(t => t.ScaledDrawBox.Intersects(InputHelper.NewMouse.Position)))
            {
                foreach (UiElement element in Children)
                {
                    element.DeactivateElement();
                }
                isMenuOpen = false;
            }
        }

    }
    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        float scaleX = ScaledDrawBox.Width / Textures.MainFont.MeasureString(Text).X;
        spriteBatch.DrawString(Textures.MainFont, Text, ScaledDrawBox.Pos, Color.BlueViolet, 0, Vector2.Zero, scaleX, SpriteEffects.None, 0);
    }

    public override void DeactivateElement()
    {
        base.DeactivateElement();
    }

    public override void ActivateElement()
    {
        IsActive = true;
        IsVisible = true;
    }

}