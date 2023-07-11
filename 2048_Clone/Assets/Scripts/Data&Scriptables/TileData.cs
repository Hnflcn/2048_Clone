using UnityEngine;

namespace Data_Scriptables
{
    [CreateAssetMenu(fileName = "TileData", menuName = "2048_Clone/Tile Data", order = 0)]
    public class TileData : ScriptableObject
    {
        public float movingTime = .3f;
        public AnimationCurve animCurve;
    }
}
