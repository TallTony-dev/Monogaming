using Monogaming.Entities;
using System.Text.Json.Serialization;

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
        ScaledDrawBox.DrawStringInto(spriteBatch, Text, Color.Indigo);
        base.Draw(spriteBatch);
    }
}