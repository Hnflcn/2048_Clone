using System;
using Data_Scriptables;
using TMPro;
using UnityEngine;

namespace Tile
{
    public class TileObj : MonoBehaviour
    {
        private int _value = 2;

        private Vector3 _startPosition;
        private Vector3 _endPosition;
        private bool _isMoving;
        private float _count;

        [SerializeField] private TMP_Text text;
        [SerializeField] private TileData tileData;

        private void Update()
        {
            if (!_isMoving)
                return;
            _count += Time.deltaTime;

            var t = _count / tileData.movingTime;
            t = tileData.animCurve.Evaluate(t);
            var newPosition = Vector3.Lerp(_startPosition, _endPosition, t);
            transform.position = newPosition;

            if (_count >= tileData.movingTime)
                _isMoving = false;
        }

        public void SetValue(int value)
        {
            _value = value;
            text.text = value.ToString();
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
            _isMoving = true;
        }
    }
}

































