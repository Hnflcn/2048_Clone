using TMPro;
using UnityEngine;

namespace UI
{
    public class ScoreDisplay : MonoBehaviour
    {
        private TMP_Text _text;
        private Animator _animator;
        private static readonly int ScoreUpdated = Animator.StringToHash("ScoreUpdated");

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
            _animator = GetComponent<Animator>();
        }

        public void UpdateScore(int score)
        {
            _text.text = score.ToString();
            if(_animator != null)
                _animator.SetTrigger(ScoreUpdated);
        }

    }
}
