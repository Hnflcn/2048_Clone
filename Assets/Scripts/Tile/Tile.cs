using System.Linq;
using Data;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Tile
{
    public class Tile : MonoBehaviour
    {
        private int _value = 2;

        private Vector3 _startPosition;
        private Vector3 _endPosition;
        
        private bool _isTwening;
        private float _count;
        
        [SerializeField] private TileData tileData;
        [SerializeField] private TMP_Text text;
        
        private TileManager _tileManager;
        private Image _tileImage;
        private Tile _mergeTile;
        
        private Animator _animator;
        private static readonly int Merge1 = Animator.StringToHash(ConstantVariables.Merge);

        public void SetValue(int value)
        {
            _value = value;
            text.text = value.ToString();
            var newColor = tileData.tileColors.FirstOrDefault(color => color.value == _value) ?? new TileColor();
            text.color = newColor.fgColor;
            _tileImage.color = newColor.bgColor;
        }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _tileManager = FindObjectOfType<TileManager>();
            _tileImage = GetComponent<Image>();
        }

        private void Update()
        {
            if (!_isTwening)
                return;
            _count += Time.deltaTime;

            var t = _count / tileData.animTime;
            t = tileData.animCurve.Evaluate(t);

            var newPosition = Vector3.Lerp(_startPosition, _endPosition, t);

            transform.position = newPosition;

            if (!(_count >= tileData.animTime)) return;
            _isTwening = false;
            if (_mergeTile == null) return;
            var newValue = _value + _mergeTile._value;
            _tileManager.AddScore(newValue);
            SetValue(newValue);
            Destroy(_mergeTile.gameObject);
            _animator.SetTrigger(Merge1);
            _mergeTile = null;
        }

        public bool Merge(Tile otherTile)
        {
            if (!CanMerge(otherTile))
                return false;

            _mergeTile = otherTile;

            return true;
        }

        public bool CanMerge(Tile otherTile)
        {
            if (_value != otherTile._value)
                return false;

            return _mergeTile == null && otherTile._mergeTile == null;
        }

        public void SetPosition(Vector3 newPosition, bool firstIns)
        {
            if (firstIns)
            {
                transform.position = newPosition;
                return;
            }
            _startPosition = transform.position;
            _endPosition = newPosition;
            _count = 0;
            _isTwening = true;

            if (_mergeTile != null)
            {
                _mergeTile.SetPosition(newPosition, false);
            }
        }

        public int GetValue()
        {
            return _value;
        }
    }
}
