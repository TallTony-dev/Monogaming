using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monogaming.Interfaces;

namespace Monogaming.TextureThings;

public class AnimatedTexture : ImUpdatable, ImTexture
{
    public bool IsActive { get; private set; } // determines if should be updated
    public bool IsAnimated { get; } = true;

    //TODO: define a set of things here that describe how the texture should change over time
    //have a list of textures and the delays that come after them and aan active one that rotates
    //via Update, also have a storage format for them to describe how they are

    
    double passedTime = 0;
    List<(AtlasTex tex, float delay)> animation = new();
    int currentIndex = 0;

    public AnimatedTexture(List<(AtlasTex tex, float delay)> list)
    {
        animation = list;
    }

    public AnimatedTexture Copy()
    {
        return new AnimatedTexture(animation);
    }

    public void Draw(SpriteBatch batch, Vector2 pos)
    {
        animation[currentIndex].tex.Draw(batch, pos);
    }
    public void Draw(SpriteBatch batch, Rectangle dest)
    {
        animation[currentIndex].tex.Draw(batch, dest);
    }
    
    public void Update(GameTime gameTime)
    {
        passedTime += gameTime.ElapsedGameTime.TotalSeconds;
        if (passedTime > animation[currentIndex].delay)
        {
            if (currentIndex < animation.Count - 1) 
            {
                currentIndex++;
            }
            else
            {
                currentIndex = 0;
            }

            passedTime = 0;
        }
    }

}
