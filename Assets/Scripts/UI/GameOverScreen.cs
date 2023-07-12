using UnityEngine;

namespace UI
{
    public class GameOverScreen : MonoBehaviour
    {
        private Animator _animator;
        private static readonly int IsGameOver = Animator.StringToHash("IsGameOver");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void SetGameOver(bool isGameOver)
        {
            _animator.SetBool(IsGameOver, isGameOver);
        }

    }
}
