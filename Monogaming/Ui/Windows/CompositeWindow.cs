using Monogaming.Entities;
using System.Text.Json.Serialization;

namespace Monogaming.Ui.Windows;



public class CompositeWindow : WindowElement
{

    // Vector2 oldDims;

    [JsonConstructor]
    public CompositeWindow(Hitbox drawBox, List<UiElement> Children, string id = "compWindow") : base(AnchorPoint.TopLeft, drawBox, Children, id)
    {
        // oldDims = drawBox.Dims;
        foreach (WindowElement e in Children)
        {
            e.isChildOfOtherWindow = true;
        }
        UpdateChildPositions();
        // IsResizable = false;
    }

    private void UpdateChildPositions()
    {
        // int totalWidth = (int)(Children.Sum(t => t.DrawBox.Width));
        // float widthScale = (float)DrawBox.Width / totalwidth;

        // Vector2 dimScale = DrawBox.Dims / oldDims;

        Vector2 scale = UI.LiveUI.Scale;

        int runningTotal = 0;
        foreach (WindowElement e in Children)
        {
            e.SetRealPos(new Vector2(runningTotal, 0));
            int newWidth = (int)(e.DrawBox.Width / scale.X * UI.LiveUI.UiScale);
            runningTotal += newWidth;

            // e.SetRealDims(new Vector2(e.DrawBox.Width, DrawBox.Height) * dimScale * ((float)e.DrawBox.Width / totalWidth));
        }

        // oldDims = DrawBox.Dims;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        UpdateChildPositions();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {

        base.Draw(spriteBatch);
    }
}