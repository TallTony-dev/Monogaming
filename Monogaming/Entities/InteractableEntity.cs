using Monogaming.TextureThings;
using Monogaming.Ui;
using Monogaming.Ui.Windows;
using System.Text.Json.Serialization;


namespace Monogaming.Entities;

public class InteractableEntity : Entity
{

    [JsonConstructor]
    public InteractableEntity(string name, string id, bool IsSolid, Hitbox drawBox, TextureHandler textureHandler) :
    base(name, id, IsSolid, drawBox, textureHandler)
    {

    }

    public override Entity CopyToPos(Vector2 pos)
    {
        Hitbox newBox = new Hitbox(pos, (int)DrawBox.Width, (int)DrawBox.Height);
        return new InteractableEntity(Name, ID, IsSolid, newBox, TextureHandler.Copy());
    }

    /// <summary>
    /// Player can call this to do something of some sort
    /// </summary>
    public virtual void Interact()
    {
        var popup = new TwoButtonWindow("TestTextlolol", "butt1", "butt2",
        null,
        () => Debug.WriteInfo("Pressed butt"));

        UI.LiveUI.AddPopup(popup);
    }


}