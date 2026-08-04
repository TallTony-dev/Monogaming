using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monogaming.Entities;
using Monogaming.Interfaces;
using Monogaming.TextureThings;

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


    //TODO: Fix how these are stored seperately cause it sucks lol, make a custom jsonConverter or something!
    [JsonInclude]
    [JsonPropertyName("Entities")]
    List <JsonEntity> Entities = new();

    [JsonIgnore]
    private Tile[,] _tiles;

    [JsonIgnore]
    private List<ImUpdatable> Updatables = new();
    [JsonIgnore]
    private List<ImDrawable> Drawables = new();
    [JsonIgnore]
    private List<ImCollidable> Collidables = new();
    
    [JsonConstructor]
    public Chunk(string MapName, int Width, int Height, string[][] Tiles, List<JsonEntity> Entities)
    {
        this.MapName = MapName;
        this.Width = Width;
        this.Height = Height;
        this.Tiles = Tiles;

        if (Tiles.GetLength(0) != Width || Tiles.Any(t => t.Length != Height))
        {
            throw new JsonException($"Bad tile length, width: {Width} height: {Height} but got array of width {Tiles.GetLength(0)} and max height {Tiles.Max(t => t.Length)}");
        }

        _tiles = new Tile[Width,Height];
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                _tiles[x,y] = TileDict.GetTileCopy(Tiles[x][y]);
            }
        }

        foreach (JsonEntity je in Entities)
        {
            AddEntity(je.ID, je.Pos);
        }
    }

    /// <summary>
    /// Creates a new blank chunk
    /// </summary>
    public Chunk(int width, int height)
    {
        _tiles = new Tile[width, height];
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

    public void SettleTiles()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {                
                TileTex.TileTexType texTyp = GetTextureTypeForNeighbors(x,y);
                _tiles[x,y].SetTileTex(texTyp);
            }
        }
    }

    public void SettleEntities() 
    {
        Entities.Clear();
        foreach (Entity e in Updatables) {
            Entities.Add(new JsonEntity(e.ID, e.DrawBox.Pos));
        }
    }

    private TileTex.TileTexType GetTextureTypeForNeighbors(int x, int y)
    {
        Tile tile = _tiles[x, y];
        string id = tile.ID;

        bool top = y == 0 || _tiles[x, y - 1].ID != id;
        bool right = x == Width - 1 || _tiles[x + 1, y].ID != id;
        bool bottom = y == Height - 1 || _tiles[x, y + 1].ID != id;
        bool left = x == 0 || _tiles[x - 1, y].ID != id;

        int mask = (top ? 1 : 0) | (right ? 2 : 0) | (bottom ? 4 : 0) | (left ? 8 : 0);
        //llm here for this switch :(((( guys i swaer i just didnt wanna write this again cause i did it in another game ok sorey
        TileTex.TileTexType t = mask switch
        {
            0 => TileTex.TileTexType.None,
            1 => TileTex.TileTexType.Top,
            2 => TileTex.TileTexType.Right,
            3 => TileTex.TileTexType.TopRight,
            4 => TileTex.TileTexType.Bottom,
            5 => TileTex.TileTexType.TopBottom,
            6 => TileTex.TileTexType.BottomRight,
            7 => TileTex.TileTexType.TopRightBottom,
            8 => TileTex.TileTexType.Left,
            9 => TileTex.TileTexType.LeftTop,
            10 => TileTex.TileTexType.LeftRight,
            11 => TileTex.TileTexType.LeftRightTop,
            12 => TileTex.TileTexType.LeftBottom,
            13 => TileTex.TileTexType.LeftTopBottom,
            14 => TileTex.TileTexType.LeftRightBottom,
            _ => TileTex.TileTexType.AllSides,
        };
        //end llm lol it forgot to do the next part so i had to do it again anyways ;P
        if (t == TileTex.TileTexType.None) {
            bool tl = y > 0 && x > 0 && _tiles[x - 1, y - 1].ID != id;
            bool tr = y > 0 && x < Width - 1 && _tiles[x + 1, y - 1].ID != id;
            bool bl = y < Height - 1 && x > 0 && _tiles[x - 1, y + 1].ID != id;
            bool br = y < Height - 1 && x < Width - 1 && _tiles[x + 1, y + 1].ID != id;
            int mask2 = (tl ? 1 : 0) | (tr ? 2 : 0) | (bl ? 4 : 0) | (br ? 8 : 0);
            int sum = (tl ? 1 : 0) + (tr ? 1 : 0) + (bl ? 1 : 0) + (br ? 1 : 0);
            if (sum != 1) {
                return t;
            }
            else {
                switch (mask2) {
                    case(1): return TileTex.TileTexType.ExTopLeft;
                    case(2): return TileTex.TileTexType.ExTopRight;
                    case(4): return TileTex.TileTexType.ExBottomLeft;
                    case(8): return TileTex.TileTexType.ExBottomRight;
                }
            }
        }

        return t;
    }

    public bool ChangeTile(int x, int y, Tile tile)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
        {
            _tiles[x,y] = tile;
            Tiles[x][y] = tile.ID;
            return true;
        }
        return false;
    }
    // public Tile GetTile(int x, int y)
    // {
    //     if (x > 0 && x < Width && y > 0 && y < Height)
    //     {
    //         return _tiles[x,y];
    //     }
    //     return null;
    // }
    public Entity GetTopEntityCollision(Vector2 worldPos) {
        return (Entity)Collidables.Find(t => (t.DrawBox.Intersects(worldPos)) && t is Entity);
    }
    
    public void AddEntity(string EntityID, Vector2 pos)
    {
        if (EntityDict.Contains(EntityID)) {
            JsonEntity entity = new JsonEntity(EntityID, pos);
            Entities.Add(entity);

            Entity e = EntityDict.CreateEntity(EntityID, pos);
            Updatables.Add(e);
            Drawables.Add(e);
            Collidables.Add(e);
        }
    }
    public void RemoveEntity(Entity e) {
        Updatables.Remove(e);
        Drawables.Remove(e);
        Collidables.Remove(e);
    }

    public void Draw(SpriteBatch spriteBatch, Hitbox screen)
    {
        //decode which tiles are on screen here :p
        //topY might actuaclly be the bottom of the screen but whatevs lol

        //TODO: make this more optimized cause i lowkey just bandaided it with a couple +2s
        (int leftX, int topY) = Map.WorldPosToAbsTilePos(screen.Pos);
        (int tileWidth, int tileHeight) = Map.WorldPosToAbsTilePos(new Vector2(screen.Width, screen.Height));
        leftX = Math.Clamp(leftX, 0, _tiles.GetLength(0));
        topY = Math.Clamp(topY, 0, _tiles.GetLength(1));
        int rightX = Math.Clamp(leftX + tileWidth + 2, 0, _tiles.GetLength(0));
        int bottomY = Math.Clamp(topY + tileHeight + 2, 0, _tiles.GetLength(1));

        for (int x = leftX; x < rightX; x++)
        {
            for (int y = topY; y < bottomY; y++)
            {
                _tiles[x,y].Draw(spriteBatch, Map.TilePosToWorldPos(new Vector2(x,y)));
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
        foreach (Tile t in _tiles)
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
                if (x > 0 && x < _tiles.Length && y > 0 && y < _tiles.Length 
                && !_tiles[x,y].Walkable)
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
        public string ID { get; set; }
        [JsonInclude]
        public Vector2 Pos { get; set; }


        [JsonConstructor]
        public JsonEntity(string ID, Vector2 Pos)
        {
            this.ID = ID;
            this.Pos = Pos;
        }
    }
}