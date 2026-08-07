using Monogaming.Entities;
using Microsoft.Xna.Framework;
using Monogaming.Interfaces;
using Monogaming.TextureThings;
using Monogaming.Ui;
using System.Text.Json;
using System.Text.Json.Serialization;
using Monogaming.Ui.Windows;
using Monogaming.MapThings;
using Monogaming.Items;


namespace Monogaming.Entities;

public class TileEntity : InventoryEntity
{
    public int Width { get => DrawBox.Width / Map.TileDims; }
    public int Height { get => DrawBox.Height / Map.TileDims; }


    [JsonConstructor]
    public TileEntity(string name, string id, int Width, int Height, bool IsSolid, Inventory inventory, TextureHandler textureHandler) :
    base(name, id, IsSolid, new Hitbox(Width * Map.TileDims, Height * Map.TileDims), inventory, textureHandler)
    {
        
    }

    public override Entity CopyToPos(Vector2 worldPos)
    {
        var e = new TileEntity(Name, ID, Width, Height, IsSolid, Inventory.Copy(), TextureHandler.Copy());
        e.SetPos(worldPos);
        return e;
    }

    public virtual void Break()
    {
        //if multiblock then disable somehow probably
    }

    /// <summary>
    /// Player can call this to do something of some sort
    /// </summary>
    public override void Interact()
    {
        var popup = new TwoButtonWindow("TestTextlolol", "butt1", "butt2", 
        null, 
        () => Debug.WriteInfo("Pressed butt"));
        
        UI.LiveUI.AddPopup(popup);
    }
    public struct JsonItem
    {
        [JsonInclude]
        public string ID { get; set; }
        [JsonInclude]
        public int Count { get; set; }


        [JsonConstructor]
        public JsonItem(string ID, int Count)
        {
            this.ID = ID;
            this.Count = Count;
        }
    }

}

