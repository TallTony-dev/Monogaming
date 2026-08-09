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
    private Func<string, bool> setVal;
    [JsonIgnore]
    private Func<string> getVal;
    [JsonIgnore]
    private bool isSelected;
    [JsonIgnore]
    private bool failedLastBind;
    [JsonIgnore]
    private string buffer;
    [JsonInclude]
    public string Text { get; set; } //Displays when nothing in buffer
    
    [JsonConstructor]
    public TextInputElement(string ID, AnchorPoint Anchor, Hitbox drawBox, string Text, List<UiElement> Children) : base(ID, Anchor, drawBox, Children)
    {
        this.Text = Text;
    }

    public void BindString(Func<string, bool> setter, Func<string> getter)
    {
        setVal = setter;
        getVal = getter;
    }

    public override void Update(GameTime gameTime)
    {
        if (InputConditions.click.Pressed())
        {
            if (ScaledDrawBox.Intersects(InputHelper.NewMouse.Position))
            {
                isSelected = true;
                InputConditions.click.Consume();
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
                    failedLastBind = false;
                }
                else if (c.Key == Keys.Enter)
                {
                    if (setVal != null && setVal.Target != null)
                    {
                        if (!setVal(buffer))
                        {
                            failedLastBind = true;
                        }
                    }
                    else
                    {
                        failedLastBind = true;
                    }
                }
                else if (c.Key != Keys.Back)
                {
                    buffer += c.Character;
                    failedLastBind = false;
                }
            }
        }
        else if (getVal != null)
        {
            buffer = getVal();
        }


        base.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        Color color = failedLastBind ? Color.Red : (isSelected ? Color.LightBlue : Color.White);
        spriteBatch.Draw(Textures.BlankTexture, ScaledDrawBox.ToRect(), color);

        if (!string.IsNullOrEmpty(buffer)) {
            ScaledDrawBox.DrawStringInto(spriteBatch, buffer);
        }
        else
        {
            ScaledDrawBox.DrawStringInto(spriteBatch, Text);
        }
        base.Draw(spriteBatch);
    }


}
