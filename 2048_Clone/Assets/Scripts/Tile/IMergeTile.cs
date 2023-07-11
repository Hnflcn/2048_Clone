using UnityEngine;

namespace Tile
{
    public interface IMergeTile
    {
        public bool Merge(TileObj otherTile);
    }
}