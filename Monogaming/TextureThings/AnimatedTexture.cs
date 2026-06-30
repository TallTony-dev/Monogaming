using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monogaming.Interfaces;

namespace Monogaming.TextureThings;

public class AnimatedTexture : ImUpdatable
{
    public bool IsActive { get; private set; } // determines if should be updated


    //TODO: define a set of things here that describe how the texture should change over time
    //have a list of textures and the delays that come after them and aan active one that rotates
    //via Update, also have a storage format for them to describe how they are

    
    double passedTime = 0;
    List<(string texid, float delay)> animation = new();
    int currentIndex = 0;

    public AnimatedTexture(List<(string texid, float delay)> list)
    {
        animation = list;

    }

    public static implicit operator Texture2D(AnimatedTexture animatedTexture)
    {
        return animatedTexture.GetCurrentTexture();
    }

    public Texture2D GetCurrentTexture()
    {
        return Textures.Get2DTex(animation[currentIndex].texid);
    }

    public void Update(GameTime gameTime)
    {
        passedTime += gameTime.ElapsedGameTime.TotalSeconds;
        if (passedTime > animation[currentIndex].delay)
        {
            if (currentIndex < animation.Count) 
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
