using Monogaming.Items;
using Monogaming.MapThings;
using Monogaming.TextureThings;
using Monogaming.Ui;
using Monogaming.Ui.Windows;
using System.Text.Json.Serialization;


namespace Monogaming.Entities;

public class TileEntity : InventoryEntity
{
    /// <summary>
    /// Width in terms of tiles
    /// </summary>
    public int Width { get => DrawBox.Width / Map.TileDims; }
    /// <summary>
    /// Height in terms of tiles
    /// </summary>
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

    }


    /// <summary>
    /// Updates this tile with a 'tick', maybe doesn't need to be called every frame
    /// </summary>
    public virtual void UpdateTile(UpdateData data)
    {

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

    protected enum Direction { Left, Right, Up, Down }

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
    public struct UpdateData
    {
        public UpdateData(TileEntity[,] state, int myx, int myy, float passedSecs)
        {
            this.state = state;
            this.myx = myx;
            this.myy = myy;
            this.passedSecs = passedSecs;
        }
        public TileEntity[,] state;
        public int myx;
        public int myy;
        public float passedSecs;
    }
}

