using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monogaming.Interfaces;

namespace Monogaming.TextureThings;

public class TextureAnimation : ImUpdatable, ImTexture
{
    public bool IsActive { get; private set; } // determines if should be updated
    public bool IsAnimated { get; } = true;
    public bool IsLooping { get; private set; } = true;
    public int loopCount { get; private set; } = 0;

    //TODO: define a set of things here that describe how the texture should change over time
    //have a list of textures and the delays that come after them and aan active one that rotates
    //via Update, also have a storage format for them to describe how they are

    
    double passedTime = 0;
    List<(AtlasTex tex, float delay)> animation = new();
    int currentIndex = 0;

    public TextureAnimation(List<(AtlasTex tex, float delay)> list, bool isLooping)
    {
        IsLooping = isLooping;
        animation = list;
    }

    public TextureAnimation Copy()
    {
        return new TextureAnimation(animation, IsLooping);
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
        if (IsLooping || loopCount != 1) {
            passedTime += gameTime.ElapsedGameTime.TotalSeconds;
            if (passedTime > animation[currentIndex].delay)
            {
                if (currentIndex < animation.Count - 1) 
                {
                    currentIndex++;
                }
                else
                {
                    if (IsLooping)
                    {
                        currentIndex = 0;
                    }
                    loopCount++;
                }

                passedTime = 0;
            }
        }
    }

}
