using System;
using Monogaming.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Monogaming.Interfaces;
using Monogaming.TextureThings;
using Microsoft.Xna.Framework.Content;
using System.Text.Json.Serialization;
using Monogaming.Items;
using Apos.Input;

namespace Monogaming.Ui.Windows;



public class WindowElement : UiElement
{
    
    protected const int TopBarHeight = 20;
    const int CloseButtonPadding = 1;
    const int CloseButtonWidth = 30;

    [JsonIgnore]
    private ImTexture backTex = Textures.GetTexture("buttonRock.Unpressed");
    [JsonIgnore]
    private Hitbox topBar;
    [JsonIgnore]
    private Hitbox closeButton;

    [JsonIgnore]
    public bool ShouldRemove { get; protected set; } = false;


    [JsonConstructor]
    public WindowElement(string ID, AnchorPoint Anchor, Hitbox drawBox, List<UiElement> Children) : base(ID, Anchor, drawBox, Children)
    {
        topBar = new Hitbox(drawBox.Pos, drawBox.Width, TopBarHeight);
        closeButton = new Hitbox(drawBox.Pos + new Vector2(CloseButtonPadding), CloseButtonWidth, TopBarHeight - CloseButtonPadding * 2);
    }

    public override void UpdatePos(Rectangle window)
    {
        base.UpdatePos(window);
        topBar = new Hitbox(ScaledDrawBox.Pos, ScaledDrawBox.Width, TopBarHeight);
        closeButton = new Hitbox(ScaledDrawBox.Pos + new Vector2(CloseButtonPadding), CloseButtonWidth, TopBarHeight - CloseButtonPadding * 2);
    }

    public override void Update(GameTime gameTime)
    {
        //TODO: have drag logic here and make sure to update pos properly
        if (closeButton.Intersects(InputHelper.NewMouse.Position) && InputConditions.click.Pressed())
        {
            ShouldRemove = true;
            return;
        }
        
        

        base.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        backTex.Draw(spriteBatch, ScaledDrawBox.AsRect());
        backTex.Draw(spriteBatch, topBar.AsRect());
        backTex.Draw(spriteBatch, closeButton.AsRect());
   
        base.Draw(spriteBatch);
    }
}