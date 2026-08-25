using Apos.Input;
using Monogaming.Entities;
using Monogaming.TextureThings;
using System.Text.Json.Serialization;

namespace Monogaming.Ui;

public class ButtonElement : TextureElement
{
    [JsonIgnore]
    public bool WasClicked { get; private set; }

    public List<Command> OnClickActions { get; private set; }
    public string Text { get; set; }

    [JsonConstructor]
    public ButtonElement(string ID, AnchorPoint Anchor, Hitbox drawBox, TextureHandler textureHandler, List<Command> OnClickActions, string Text, List<UiElement> Children) :
    base(ID, Anchor, drawBox, textureHandler, Children)
    {
        this.OnClickActions = OnClickActions == null ? new List<Command>() : OnClickActions;
        this.Text = Text;
    }

    public override void Update(GameTime gameTime)
    {
        WasClicked = false;
        if (ScaledDrawBox.Intersects(InputHelper.NewMouse.Position))
        {
            if (InputConditions.click.Pressed())
            {
                foreach (Command action in OnClickActions)
                {
                    action.DispatchCommand();
                }
                WasClicked = true;
            }
        }

        base.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        if (Text != null)
        {
            ScaledDrawBox.DrawStringInto(spriteBatch, Text, Color.Black);
        }
    }

}