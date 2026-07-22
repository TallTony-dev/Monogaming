using System;
using System.IO;
using Microsoft.Xna.Framework;
using Monogaming.Entities;

namespace Monogaming.MapThings;

/// <summary>
/// Describes an area of a map
/// </summary>
public class Chunk
{
    static readonly string fileRoot = "./GameData/chunks/";
    string fileName;


    Tile[,] tiles;


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

    /// <summary>
    /// Returns the vector out if collides, otherwise returns Vector2.Zero
    /// </summary>
    public Vector2 Collides(Hitbox box)
    {
        throw new NotImplementedException();
        return Vector2.Zero;
    }



}