using Apos.Input;
using Monogaming.Entities;
using Monogaming.Interfaces;
using Monogaming.Items;
using Monogaming.MapThings;
using Monogaming.TextureThings;
using Monogaming.Ui;
using Monogaming.Ui.Windows;
using System.Text.Json.Serialization;

namespace Monogaming.PlayerThings;


public class Player
{
    private const int InteractDistance = 120;
    private const int ColliderDiff = 2;
    private const int LightRadius = 20;
    public static Player LivePlayer { get; private set; }

    public Body body { get; private set; } = new(Vector2.Zero, Map.TileDims - ColliderDiff * 2, Map.TileDims - ColliderDiff * 2, 10f);
    private InputManager inputManager = new();
    public TextureHandler TextureHandler { get; private set; }
    
    public Inventory Inventory { get; private set; }


    private InventoryWindow inventoryWindow = null;


    public float MoveSpeed { get; private set; }

    [JsonConstructor]
    public Player(float moveSpeed, TextureHandler textureHandler, Inventory inventory)
    {
        Inventory = inventory;
        MoveSpeed = moveSpeed;
        TextureHandler = textureHandler;
        LivePlayer = this;
    }

    private List<string> GetNearbyWorkbenches()
    {
        //TODO: implement this lol
        return new List<string>();
    }

    private void UpdateSaveWithCurrentInv()
    {
        foreach (Item i in Inventory.Items)
        {
            //this is about as cheap/cheaper than checking and then setting
            GameState.ActiveSave.SetItemFound(i.ID);
        }
    }

    public bool IsWithinReach(Vector2 worldPos)
    {
        return (worldPos - body.Center).LengthSquared() < InteractDistance * InteractDistance;
    }

    public void Update(GameTime gameTime, Chunk currentChunk, Map map)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Vector2 moveDir = inputManager.GetMovementDir();
        body.AddVelocity(50 * moveDir * dt * MoveSpeed / (body.velocity.Length() * 0.002f + 0.5f));
        body.Update(gameTime, currentChunk);

        TextureHandler.Update(gameTime);
        if (body.IsWalking)
        {
            TextureHandler.SetTexture(TextureHandler.TextureType.Walking);
        }
        else if (!body.IsWalking)
        {
            TextureHandler.SetTexture(TextureHandler.TextureType.Idle);
        }


        Vector2 mPos = InputHelper.NewMouse.Position.ToVector2();
        Vector2 mWorldPos = map.ScreenPosToWorldPos(mPos);

        if (IsWithinReach(mWorldPos))
        {

            if (InputConditions.click.Pressed(false))
            {
                InteractableEntity clickedOn = currentChunk.GetTopCollision<InteractableEntity>(mWorldPos);

                if (clickedOn != null)// && IsWithinReach(clickedOn.DrawBox.Center))
                {
                    clickedOn.Interact();
                    InputConditions.click.Consume();
                }
            }

        }



        if (InputConditions.openInventory.Pressed() && !UI.LiveUI.ContainsPopupWithID("playerInventory"))
        {
            UpdateSaveWithCurrentInv(); //adds items to discovered

            UI.LiveUI.AddPopup(new CompositeWindow(new Hitbox(140, 120, 700, 250),
            new List<UiElement>() {
                (inventoryWindow = new InventoryWindow(Inventory, new Hitbox(0,0, 400, 250), "inv")),
                new CraftingMenuWindow(Inventory, GetNearbyWorkbenches(), new Hitbox(0,0, 300, 250)),
            }, "playerInventory"));
        }

        if (Inventory.HasChanged && GameState.ActiveSave != null)
        {
            UpdateSaveWithCurrentInv();
        }

        Inventory.Update(gameTime);
    }

    public void Draw(SpriteBatch batch)
    {
        if (inventoryWindow != null)
        {
            inventoryWindow.TryDrawSelectedItem(batch);
        }
        Rectangle rect = body.ToRect();
        rect.Inflate(ColliderDiff, ColliderDiff);
        TextureHandler.Draw(batch, rect);
    }

    public void DrawLight(SpriteBatch batch)
    {
        var rect = body.ToRect();
        rect.Inflate(LightRadius, LightRadius);
        TextureHandler.TryDrawLight(batch, rect);
    }
}