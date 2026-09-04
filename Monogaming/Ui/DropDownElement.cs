using Apos.Input;
using Monogaming.Entities;
using Monogaming.TextureThings;
using System.Linq;
using System.Text.Json.Serialization;

namespace Monogaming.Ui;


/// <summary>
/// Children under this only become activated after a targeted activation of the element
/// Or after being clicked on
/// </summary>
public class DropDownElement : TextureElement
{
    [JsonIgnore]
    public bool IsMenuOpen { get; private set; } = false;

    public string Text { get; private set; }

    [JsonConstructor]
    public DropDownElement(string ID, AnchorPoint Anchor, Hitbox drawBox, TextureHandler textureHandler, string text, List<UiElement> Children) : base(ID, Anchor, drawBox, textureHandler, Children)
    {
        Text = text;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (InputConditions.click.Pressed(false))
        {
            if (ScaledDrawBox.Intersects(InputHelper.NewMouse.Position))
            {
                InputConditions.click.Consume();
                foreach (UiElement element in Children)
                {
                    element.ActivateElement();
                }
                IsMenuOpen = true;
            }
            else if (!Children.Any(t => t.ScaledDrawBox.Intersects(InputHelper.NewMouse.Position)))
            {
                foreach (UiElement element in Children)
                {
                    element.DeactivateElement();
                }
                IsMenuOpen = false;
            }
        }

    }
    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        if (Text != null)
        {
            ScaledDrawBox.DrawStringInto(spriteBatch, Text, Color.Indigo);
        }
    }

    public override void DeactivateElement()
    {
        base.DeactivateElement();
    }

    public override void ActivateElement(bool force = false)
    {
        IsActive = true;
        IsVisible = true;
        if (force)
        {
            foreach (var child in Children)
            {
                child.ActivateElement(force);
            }
        }
    }

}