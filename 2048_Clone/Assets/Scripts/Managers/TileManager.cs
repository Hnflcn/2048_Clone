using System;
using System.Collections.Generic;
using System.Linq;
using Base;
using DG.Tweening;
using Tile;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Managers
{
    public class TileManager : MonoBehaviour
    {
        public static int GridSize = 4;
        private readonly Transform[,] _tilePos = new Transform[GridSize, GridSize];
        private readonly TileObj[,] _tiles = new TileObj[GridSize, GridSize];

       private TileObj _tileScript;

        private void Start()
        {
            GetTilePositions();
            CanSpawn();
            CanSpawn();
            UpdateTilePosition();
        }

        private void GetTilePositions()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
            // bu sayede transformlar artık aynı görünmüyor
        
            var x = 0;
            var y = 0;

            foreach (Transform child in transform)
            {
                _tilePos[x, y] = child;

                x ++;
                if (x < GridSize) continue;
                x = 0;
                y++;
            }
        }

        private bool CanSpawn()
        {
            var availableSlots = new List<Vector2Int>();
            for (var i = 0; i < GridSize; i++)
            {
                for (var j = 0; j < GridSize; j++)
                {
                    if (_tiles[i,j] == null)
                    {
                        availableSlots.Add(new Vector2Int(i,j));
                    }
                }
            }

            if (!availableSlots.Any())
            {
                return false;
            }

            var randomIndex = Random.Range(0, availableSlots.Count);
            var slot = availableSlots[randomIndex];

            var tile = Pool.Current.GetPooledObject(ConstantVariables.TileOrder);
            tile.transform.parent = transform.parent;
            tile.transform.localScale = Vector3.one /1.2f;
            tile.SetActive(true);
            tile.transform.DOScale(Vector3.one, .2f);
            _tileScript = tile.GetComponent<TileObj>();
            _tileScript.SetValue(GetRandomValue());
            

            _tiles[slot.x, slot.y] = _tileScript;

            return true;
        }

        private int GetRandomValue()
        {
            var randomValue = Random.Range(0f, 1f);
            return randomValue <= .8f ? 2 : 4;
        }

        private void UpdateTilePosition()
        {
            for (var i = 0; i < GridSize; i++)
            {
                for (var j = 0; j < GridSize; j++)
                {
                    if (_tiles[i,j] != null)
                        _tiles[i, j].transform.position = _tilePos[i, j].position;
                }
            }
        }
    }
}


































