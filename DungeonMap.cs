using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project;

public class DungeonMap
{
    public const int TileSize = 32;
    public const int Cols = 25;
    public const int Rows = 15;

    // 0 = floor, 1 = wall
    private static readonly int[,] MapData = new int[,]
    {
        { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }, // doorway
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }, // doorway
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
        { 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1 }, 
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }, // doorway
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }, // doorway
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
        { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
    };

    private Texture2D _wallTexture;
    private Texture2D _floorTexture;

    public void LoadContent(GraphicsDevice graphicsDevice)
    {
        _wallTexture = CreateTexture(graphicsDevice, new Color(55, 50, 60));
        _floorTexture = CreateTexture(graphicsDevice, new Color(120, 100, 75));
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Cols; col++)
            {
                var texture = MapData[row, col] == 1 ? _wallTexture : _floorTexture;
                var dest = new Rectangle(col * TileSize, row * TileSize, TileSize, TileSize);
                spriteBatch.Draw(texture, dest, Color.White);
            }
        }
    }

    public bool IsWall(int col, int row)
    {
        if (col < 0 || col >= Cols || row < 0 || row >= Rows) return true;
        return MapData[row, col] == 1;
    }

    public bool CollidesWithWall(float x, float y, float radius)
    {
        int leftCol = (int)((x - radius) / TileSize);
        int rightCol = (int)((x + radius - 0.001f) / TileSize);
        int topRow = (int)((y - radius) / TileSize);
        int bottomRow = (int)((y + radius - 0.001f) / TileSize);

        for (int row = topRow; row <= bottomRow; row++)
            for (int col = leftCol; col <= rightCol; col++)
                if (IsWall(col, row)) return true;

        return false;
    }

    private static Texture2D CreateTexture(GraphicsDevice gd, Color color)
    {
        var tex = new Texture2D(gd, 1, 1);
        tex.SetData(new[] { color });
        return tex;
    }
}
