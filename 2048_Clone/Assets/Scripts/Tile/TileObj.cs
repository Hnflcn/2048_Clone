using TMPro;
using UnityEngine;

namespace Tile
{
    public class TileObj : MonoBehaviour
    {
        private int _value = 2;

        [SerializeField] private TMP_Text text;

        public void SetValue(int value)
        {
            _value = value;
            text.text = value.ToString();
        }
    }
}

































