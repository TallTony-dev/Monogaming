using Apos.Input;
using Monogaming.Entities;
using Monogaming.TextureThings;
using System.Text.Json.Serialization;

namespace Monogaming.Ui;

public class TextureElement : UiElement
{
    [JsonInclude]
    protected TextureHandler TextureHandler { get; private set; }

    [JsonConstructor]
    public TextureElement(string ID, AnchorPoint Anchor, Hitbox drawBox, TextureHandler TextureHandler, List<UiElement> Children) : base(ID, Anchor, drawBox, Children)
    {
        if (TextureHandler != null)
        {
            this.TextureHandler = TextureHandler;
        }
        else
        {
            this.TextureHandler = new(TextureHandler.TextureType.Default, Textures.DefaultTextureID);
        }
    }


    public override void Update(GameTime gameTime)
    {
        if (ScaledDrawBox.Intersects(InputHelper.NewMouse.Position))
        {
            if (InputConditions.click.Pressed(false))
            {
                TextureHandler.SetTexture(TextureHandler.TextureType.OnClick);
            }
            else
            {
                TextureHandler.SetTexture(TextureHandler.TextureType.OnHover);
            }
        }
        else
        {
            TextureHandler.SetTexture(TextureHandler.TextureType.Default);
        }

        TextureHandler.Update(gameTime);

        base.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        TextureHandler.Draw(spriteBatch, ScaledDrawBox.ToRect(), DrawColor);
        base.Draw(spriteBatch);
    }
}