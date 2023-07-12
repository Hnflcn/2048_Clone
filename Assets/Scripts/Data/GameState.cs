using Managers;

namespace Data
{
    public class GameState
    {
        public int[,] TileValues = new int[TileManager.GridSize, TileManager.GridSize];
        public int Score;
        public int MoveCount;
    }
}