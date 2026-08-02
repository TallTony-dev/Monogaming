using System;
using Monogaming.Entities;
using Monogaming.TextureThings;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using Monogaming.Ui;
using Apos.Input;
using Monogaming;

public class TextInputElement : UiElement
{
    [JsonIgnore]
    static ICondition click = new MouseCondition(MouseButton.LeftButton);
    [JsonIgnore]
    private Action<string> setVal;
    [JsonIgnore]
    private bool isSelected;
    [JsonIgnore]
    private string buffer;
    
    [JsonConstructor]
    public TextInputElement(string ID, AnchorPoint Anchor, Hitbox drawBox, List<UiElement> Children) : base(ID, Anchor, drawBox, Children)
    {
        
    }

    public void BindString(Action<string> setter)
    {
        setVal = setter;
    }

    public override void Update(GameTime gameTime)
    {
        if (click.Pressed())
        {
            if (ScaledDrawBox.Intersects(InputHelper.NewMouse.Position))
            {
                isSelected = true;
            }
            else
            {
                isSelected = false;
            }
        }

        if (isSelected)
        {
            foreach (var c in InputHelper.TextEvents)
            {
                if (c.Key == Keys.Back && !string.IsNullOrEmpty(buffer))
                {
                    buffer = buffer.Remove(buffer.Length - 1);
                }
                else if (c.Key == Keys.Enter)
                {
                    if (setVal != null && setVal.Target != null)
                    {
                        setVal(buffer);
                    }
                }
                else if (c.Key != Keys.Back)
                {
                    buffer += c.Character;
                }
            }
        }


        base.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Textures.BlankTexture, ScaledDrawBox.AsRect(), Color.White);

        if (!string.IsNullOrEmpty(buffer)) {
            float scaleX = ScaledDrawBox.Width / Textures.MainFont.MeasureString(buffer).X;
            spriteBatch.DrawString(Textures.MainFont, buffer, ScaledDrawBox.Pos, Color.BlueViolet, 0, Vector2.Zero, scaleX, SpriteEffects.None, 0);
        }
        base.Draw(spriteBatch);
    }


}
