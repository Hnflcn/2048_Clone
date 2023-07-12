using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "TileData", menuName = "2048Clone/TileData", order = 0)]
    public class TileData : ScriptableObject {
        
        public float animTime = .2f;
        public AnimationCurve animCurve;
        public TileColor[] tileColors;
    }
}