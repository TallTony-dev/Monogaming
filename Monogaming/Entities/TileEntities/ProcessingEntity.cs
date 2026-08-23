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
using System.Linq;


namespace Monogaming.Entities;

public class ProcessingEntity : TileEntity
{
    public string WorkbenchName { get; private set; }
    public float WorkbenchSpeed { get; private set; }

    [JsonIgnore]
    public float Progress { get; private set; } = 0;
    private Recipe currentCraft;


    public virtual Inventory Outputs { get; protected set; }
    public virtual Inventory Inputs { get; protected set; }

    [JsonConstructor]
    public ProcessingEntity(string name, string WorkbenchName, float WorkbenchSpeed, string id, int Width, int Height, bool IsSolid, Inventory Inputs, Inventory Outputs, Inventory inventory, TextureHandler textureHandler) :
    base(name, id, Width, Height, IsSolid, inventory, textureHandler)
    {
        this.Outputs = Outputs;
        this.Inputs = Inputs;
        this.WorkbenchName = WorkbenchName;
        this.WorkbenchSpeed = WorkbenchSpeed;
    }

    public override Entity CopyToPos(Vector2 worldPos)
    {
        var e = new ProcessingEntity(Name, WorkbenchName, WorkbenchSpeed, ID, Width, Height, IsSolid, Inputs.Copy(), Outputs.Copy(), Inventory, TextureHandler.Copy());
        e.SetPos(worldPos);
        return e;
    }

    protected virtual void OnCraft(UpdateData data)
    {
        
    }

    /// <summary>
    /// Updates this tile with a 'tick', maybe doesn't need to be called every frame
    /// </summary>
    public override void UpdateTile(UpdateData data)
    {

        if (currentCraft == null)
        {
            Recipe recipe;
            if ((recipe = RecipeBook.GetRecipesForWorkbench(WorkbenchName).FirstOrDefault(t => t.CanCraft(Inputs)))
            != default)
            {
                currentCraft = recipe;
            }
        }
        else if (Progress > 0 || Outputs.CanStoreItems(currentCraft.Outputs))
        {
            Progress += data.passedSecs * WorkbenchSpeed * currentCraft.RecipeDifficulty;

            if (Progress >= 1)
            {
                currentCraft.TryCraft(Inputs, new List<string>() { WorkbenchName });
                OnCraft(data);
                Progress = 0;
            }
        }
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

}

