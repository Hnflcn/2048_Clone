using TMPro;
using UnityEngine;

namespace UI
{
    public class MoveCounter : MonoBehaviour
    {
        private TMP_Text _text;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();   
        }

        public void UpdateCount(int moveCount)
        {
            var shouldDisplayPlural = moveCount != 1;
            _text.text = $"{moveCount} {(shouldDisplayPlural ? "moves" : "move")}";
        }
    }
}
