using Monogaming.Entities;
using Microsoft.Xna.Framework;
using Monogaming.Interfaces;
using Monogaming.TextureThings;
using Monogaming.Ui;
using System.Text.Json;
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
        Hitbox newBox = new Hitbox(pos, DrawBox.Width, DrawBox.Height);
        return new InteractableEntity(Name, ID, IsSolid, newBox, TextureHandler.Copy());
    }

    /// <summary>
    /// Player can call this to do something of some sort
    /// </summary>
    public virtual void Interact()
    {
        UI.LiveUI.AddPopup("oneButtonPopup.json");
    }


}