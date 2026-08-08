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
using Monogaming.Items;
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
    private TileEntity[,] _tileEntities; // should store things

    [JsonIgnore]
    private List<ImUpdatable> Updatables = new();
    [JsonIgnore]
    private List<ImDrawable> Drawables = new();
    [JsonIgnore]
    private List<ImCollidable> Collidables = new();
    

    private const float TileUpdateTickLength = 0.1f;
    private float timeSinceUpdatedTiles = 0;


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
        _tileEntities = new TileEntity[Width, Height];
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                _tiles[x,y] = TileDict.GetTileCopy(Tiles[x][y]);
            }
        }
        SettleTiles();

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
        _tileEntities = new TileEntity[width, height];
    }

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

    /// <summary>
    /// To be used after moving entities after they spawn in 
    /// </summary>
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
    /// <summary>
    /// Gets the top item in collidables of type T that intersects with a pos
    /// </summary>
    public T GetTopCollision<T>(Vector2 worldPos) {
        return (T)Collidables.Find(t => (t.DrawBox.Intersects(worldPos)) && t is T);
    }
    

    public void AddEntity(string EntityID, int tileX, int tileY)
    {
        AddEntity(EntityID, Map.TilePosToWorldPos(tileX,tileY));
    }
    public void AddEntity(string EntityID, Vector2 worldPos)
    {
        if (EntityDict.Contains(EntityID)) {
            JsonEntity entity = new JsonEntity(EntityID, worldPos);
            Entities.Add(entity);

            Entity e = EntityDict.CreateEntity(EntityID, worldPos);
            Updatables.Add(e);
            Drawables.Add(e);
            Collidables.Add(e);
            if (e is TileEntity te)
            {
                var tp = Map.WorldPosToAbsTilePos(worldPos);
                e.SetPos(Map.TilePosToWorldPos(tp.x, tp.y));

                if (tp.x >= 0 && tp.x < Width && tp.y >= 0 && tp.y < Height)
                {
                    if (_tileEntities[tp.x,tp.y] != null)
                    {
                        BreakTileEntity(tp.x, tp.y);
                    }
                    _tileEntities[tp.x,tp.y] = te;
                }

            }
        }
    }

    public void BreakTileEntity(int x, int y, Inventory toAddTo = null)
    {
        var prev = _tileEntities[x,y];
        if (toAddTo != null)
        {
            toAddTo.TryTransferAllItemsFrom(prev.Inventory);
        }
        Hitbox pos = new (prev.DrawBox.Center + new Vector2(-6,-6), 12, 12);
        var dropped = prev.Inventory.GetDroppedEntities(pos);
        Updatables.AddRange(dropped);
        Drawables.AddRange(dropped);
        prev.Break();

        Collidables.Remove(prev);
        Drawables.Remove(prev);
        Updatables.Remove(prev);
        
        _tileEntities[x,y] = null;
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
        //actually its pretty chill theres like an extra two layers being drawn because abstilepos rounds but thats ok
        (int leftX, int topY) = Map.WorldPosToAbsTilePos(screen.Pos);
        (int tileWidth, int tileHeight) = Map.WorldPosToAbsTilePos(new Vector2(screen.Width, screen.Height));
        leftX = Math.Clamp(leftX - 1, 0, _tiles.GetLength(0));
        topY = Math.Clamp(topY - 1, 0, _tiles.GetLength(1));
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
        timeSinceUpdatedTiles += (float)gameTime.ElapsedGameTime.TotalSeconds;
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
        if (timeSinceUpdatedTiles > TileUpdateTickLength)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    TileEntity e = _tileEntities[x,y];
                    if (e != null && e.IsActive)
                    {
                        e.UpdateTile(_tileEntities, x, y, timeSinceUpdatedTiles);
                    }
                }
            }
            timeSinceUpdatedTiles = 0;
        }
    }

    
    public Vector2 GetCollision(Hitbox box)
    {
        Vector2 boxTilePos = Map.WorldPosToTilePos(box.Pos);
        boxTilePos.Round();
        Vector2 closest = Vector2.Zero;
        Vector2 closestPen = Vector2.Zero;

        int maxX = _tiles.GetLength(0);
        int maxY = _tiles.GetLength(1);

        for (int x = (int)boxTilePos.X - 1; x <= (int)boxTilePos.X + 1; x++)
        {
            for (int y = (int)boxTilePos.Y - 1; y <= (int)boxTilePos.Y + 1; y++)
            {
                if (x >= 0 && x < maxX && y >= 0 && y < maxY
                && (!_tiles[x,y].Walkable || (_tileEntities[x,y] != null && _tileEntities[x,y].IsSolid)))
                {
                    Hitbox tileBox = new Hitbox(Map.TilePosToWorldPos(new(x,y)), Map.TileDims, Map.TileDims);
                    if (tileBox.Intersects(box)) {
                        Vector2 pt = tileBox.GetClosestPointTo(box);

                        Vector2 pen = box.GetClosestPointTo(tileBox) - pt;

                        if (closest.X == 0 || Math.Abs(pen.X) > Math.Abs(closestPen.X))
                        {
                            closest.X = pt.X;
                            closestPen.X = pen.X;
                        }
                        if (closest.Y == 0 || Math.Abs(pen.Y) > Math.Abs(closestPen.Y))
                        {
                            closest.Y = pt.Y;
                            closestPen.Y = pen.Y;
                        }
                    }
                }
            }
        }

        //maybe check if theres any collidable entities here too!!
        // Vector2 delta = box.Center - closest;
        // if (delta.X > delta.Y)
        // {
        //     closest.X
        // }
        return closest;
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