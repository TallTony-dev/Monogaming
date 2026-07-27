using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
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
    public string MapName { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }

    [JsonInclude]
    private string[][] Tiles;


    //TODO: Fix how these are stored seperately, make a custom jsonConverter or something!
    [JsonInclude]
    [JsonPropertyName("Entities")]
    Dictionary<string, JsonEntity> Entities;

    [JsonIgnore]
    private Tile[,] tiles;

    [JsonIgnore]
    private List<ImUpdatable> Updatables = new();
    [JsonIgnore]
    private List<ImDrawable> Drawables = new();

    
    [JsonConstructor]
    public Chunk(string MapName, int Width, int Height, Dictionary<string, JsonEntity> Entities, string[][] Tiles)
    {
        this.MapName = MapName;
        this.Width = Width;
        this.Height = Height;
        this.Tiles = Tiles;
        this.Entities = Entities;

        if (Tiles.GetLength(0) != Width || Tiles.Any(t => t.Length != Height))
        {
            throw new JsonException($"Bad tile length, width: {Width} height: {Height} but got array of width {Tiles.GetLength(0)} and max height {Tiles.Max(t => t.Length)}");
        }

        tiles = new Tile[Width,Height];
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                tiles[x,y] = TileDict.GetTileCopy(Tiles[x][y]);
            }
        }

        foreach (JsonEntity je in Entities.Values)
        {
            Vector2 tilePos = Map.TilePosToWorldPos(new Vector2(je.Pos[0], je.Pos[1]));
            Entity e = EntityDict.CreateEntity(je.ID, tilePos);
            Updatables.Add(e);
            Drawables.Add(e);
        }
    }

    /// <summary>
    /// Creates a new blank chunk
    /// </summary>
    public Chunk(int width, int height)
    {
        tiles = new Tile[width, height];
    }

    // public void Serialize()
    // {
    //     string path = fileRoot + fileName + ".json";
    //     string bckupPath = fileRoot + fileName + "_bckup.json";
    //     //dont delete current backup yet
    //     File.Copy(path, bckupPath + "_temp");
    //     //then delete current
    //     File.Delete(bckupPath);
    //     //then move new to old spot
    //     File.Move(bckupPath + "_temp", bckupPath);
    //     //at this point, the last chunk file is in bckup file so safe to delete old
    //     File.Delete(path);
    //     //open new file for writing!
    //     FileStream stream = File.OpenWrite(path);

    //     throw new NotImplementedException();
    // }

    public void AddEntity(string EntityID, Vector2 pos)
    {
        (int x, int y) tilePos = Map.WorldPosToAbsTilePos(pos);
        JsonEntity entity = new JsonEntity(EntityID, new int[]{tilePos.x, tilePos.y});
        Entities.Add(EntityID, entity);

        Entity e = EntityDict.CreateEntity(EntityID, pos);
        Updatables.Add(e);
        Drawables.Add(e);
    }

    public void Draw(SpriteBatch spriteBatch, Hitbox screen)
    {
        //decode which tiles are on screen here :p
        //topY might actuaclly be the bottom of the screen but whatevs lol
        (int leftX, int topY) = Map.WorldPosToAbsTilePos(screen.Pos);
        (int tileWidth, int tileHeight) = Map.WorldPosToAbsTilePos(new Vector2(screen.Width, screen.Height));
        leftX = Math.Clamp(leftX, 0, tiles.GetLength(0));
        topY = Math.Clamp(topY, 0, tiles.GetLength(1));
        int rightX = Math.Clamp(leftX + tileWidth + 1, 0, tiles.GetLength(0));
        int bottomY = Math.Clamp(topY + tileHeight, 0, tiles.GetLength(1));

        for (int x = leftX; x < rightX; x++)
        {
            for (int y = topY; y < bottomY; y++)
            {
                tiles[x,y].Draw(spriteBatch, Map.TilePosToWorldPos(new Vector2(x,y)));
            }
        }
        

        foreach (ImDrawable i in Drawables)
        {
            if (i.IsVisible) 
            {
                i.Draw(spriteBatch, screen);
            }
        }

    }

    public void Update(GameTime gameTime)
    {
        foreach (Tile t in tiles)
        {
            if (t.IsActive)
            {
                t.Update(gameTime);
            }
        }
        foreach (ImUpdatable i in Updatables)
        {
            if (i.IsActive)
            {
                i.Update(gameTime);    
            }
        }
    }

    /// <summary>
    /// Returns the vector out if collides, otherwise returns Vector2.Zero
    /// </summary>
    public Vector2 GetCollision(Hitbox box)
    {
        Vector2 boxTilePos = Map.WorldPosToTilePos(box.Pos);
        Vector2 collision = Vector2.Zero;

        for (int x = (int)boxTilePos.X - 1; x < boxTilePos.X + 1; x++)
        {
            for (int y = (int)boxTilePos.Y - 1; y < boxTilePos.Y + 1; y++)
            {
                if (x > 0 && x < tiles.Length && y > 0 && y < tiles.Length 
                && !tiles[x,y].Walkable)
                {
                    Hitbox tileBox = new Hitbox(Map.TilePosToWorldPos(new(x,y)), Map.TileDims, Map.TileDims);                    
                    collision += tileBox.GetCollision(box);
                }
            }
        }

        //maybe check if theres any collidable entities here too!!

        return collision;
    }

    public struct JsonEntity
    {
        [JsonInclude]
        public string ID;
        [JsonInclude]
        public int[] Pos;


        [JsonConstructor]
        public JsonEntity(string ID, int[] Pos)
        {
            this.ID = ID;
            this.Pos = Pos;
        }
    }
}