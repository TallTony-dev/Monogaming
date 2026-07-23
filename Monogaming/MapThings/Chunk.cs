using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monogaming.Entities;
using Monogaming.Interfaces;

namespace Monogaming.MapThings;

/// <summary>
/// Describes an area of a map
/// </summary>
public class Chunk
{
    static readonly string fileRoot = "./GameData/chunks/";
    string fileName;


    Tile[,] tiles;
    List<ImUpdatable> updatables;
    List<ImDrawable> drawables;
    
    /// <summary>
    /// Loads a chunk from a json file
    /// </summary>
    public Chunk(string fileName)
    {
        this.fileName = fileName;
        string json = File.ReadAllText(fileName);
        
        throw new NotImplementedException();
    }

    /// <summary>
    /// Creates a new blank chunk
    /// </summary>
    public Chunk(int width, int height)
    {
        throw new NotImplementedException();
    }

    public void Serialize()
    {
        string path = fileRoot + fileName + ".json";
        string bckupPath = fileRoot + fileName + "_bckup.json";
        //dont delete current backup yet
        File.Copy(path, bckupPath + "_temp");
        //then delete current
        File.Delete(bckupPath);
        //then move new to old spot
        File.Move(bckupPath + "_temp", bckupPath);
        //at this point, the last chunk file is in bckup file so safe to delete old
        File.Delete(path);
        //open new file for writing!
        FileStream stream = File.OpenWrite(path);

        throw new NotImplementedException();
    }

    public void Draw(SpriteBatch spriteBatch, Hitbox screen)
    {
        //decode which are on screen here :p
    }

    /// <summary>
    /// Returns the vector out if collides, otherwise returns Vector2.Zero
    /// </summary>
    public Vector2 Collides(Hitbox box)
    {
        Vector2 boxTilePos = Map.WorldPosToTilePos(box.Pos);

        for (int x = (int)boxTilePos.X - 1; x < boxTilePos.X + 1; x++)
        {
            for (int y = (int)boxTilePos.Y - 1; y < boxTilePos.Y + 1; y++)
            {
                if (x > 0 && x < tiles.Length && y > 0 && y < tiles.Length 
                && !tiles[x,y].Walkable)
                {
                    throw new NotImplementedException();
                }
            }
        }

        //maybe check if theres any collidable entities here too!!

        return Vector2.Zero;
    }



}