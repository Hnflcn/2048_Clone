using System;
using TMPro;
using UnityEngine;

namespace UI
{
    public class GameTimer : MonoBehaviour
    {
        private TMP_Text _text;
        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }

        public void UpdateTimer(TimeSpan gameTime)
        {
            var format = "";
            if (gameTime.Hours > 0)
                format = "h\\:";
            format += "mm\\:ss";
            _text.text = gameTime.ToString(format);
        }
    }
}
