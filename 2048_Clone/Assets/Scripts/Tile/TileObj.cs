using System;
using Data_Scriptables;
using TMPro;
using UnityEngine;

namespace Tile
{
    public class TileObj : MonoBehaviour, IMergeTile
    {
        public int val = 2;

        private Vector3 _startPosition;
        private Vector3 _endPosition;
        
        private TileObj _mergeTile;
        private PoolParent _poolParent;
        
        private bool _isMoving;
        public bool firstIns = true;
        private float _count;

        [SerializeField] private TMP_Text text;
        [SerializeField] private TileData tileData;

        private void Start()
        {
            _poolParent = FindObjectOfType<PoolParent>();
        }

        private void OnEnable()
        {
            firstIns = true;
            val = 2;
            _startPosition = _endPosition;
        }


        private void Update()
        {
            if (!_isMoving)
                return;
            _count += Time.deltaTime;

            var t = _count / tileData.movingTime;
            t = tileData.animCurve.Evaluate(t);
            var newPosition = Vector3.Lerp(_startPosition, _endPosition, t);
            transform.position = newPosition;

            if (!(_count >= tileData.movingTime)) return;
            _isMoving = false;
            if (_mergeTile == null) return;
            SetValue(val + _mergeTile.val);
            _mergeTile.transform.parent = _poolParent.transform;
            _mergeTile.gameObject.SetActive(false);
            _mergeTile = null;
        }

        public void SetValue(int value)
        {
            val = value;
            text.text = value.ToString();
        }

        public void SetPosition(Vector3 newPosition)
        {
            if (firstIns)
            {
                firstIns = false;
                transform.position = newPosition;
                return;
            }

            _startPosition = transform.position;
            _endPosition = newPosition;
            _count = 0;
            _isMoving = true;

            if (_mergeTile != null)
            {
                _mergeTile.SetPosition(newPosition);
                //_mergeTile.transform.parent = _poolParent.transform; 
               // _mergeTile.val = 2;
            }
        }


        public bool Merge(TileObj otherTile)
        {
            if (val != otherTile.val)
                return false;

            if (_mergeTile != null || otherTile._mergeTile != null)
                return false;

            _mergeTile = otherTile;
            return true;
        }
    }
}

































