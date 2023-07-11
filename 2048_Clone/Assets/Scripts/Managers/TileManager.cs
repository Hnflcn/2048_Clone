using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Base;
using Data_Scriptables;
using DG.Tweening;
using Lean.Touch;
using Tile;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Managers
{
    public class TileManager : MonoBehaviour
    {
        public static int GridSize = 4;
        protected readonly Transform[,] TilePos = new Transform[GridSize, GridSize];
        protected readonly TileObj[,] Tiles = new TileObj[GridSize, GridSize];

        private TileObj _tileScript;

        private bool _isMoving;

        [SerializeField] private TileData tileData;
       

        private void Start()
        {
            GetTilePositions();
            CanSpawn();
            CanSpawn();
            UpdateTilePosition(true);
        }

        private void GetTilePositions()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
            // bu sayede transformlar artık aynı görünmüyor
        
            var x = 0;
            var y = 0;

            foreach (Transform child in transform)
            {
                TilePos[x, y] = child;

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
                    if (Tiles[i,j] == null)
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
            

            Tiles[slot.x, slot.y] = _tileScript;

            return true;
        }

        private int GetRandomValue()
        {
            var randomValue = Random.Range(0f, 1f);
            return randomValue <= .8f ? 2 : 4;
        }

        private void UpdateTilePosition(bool firstIns)
        {
            if (!firstIns)
            {
                _isMoving = true;
                StartCoroutine(WaitForTileMove());
            }
            
            for (var i = 0; i < GridSize; i++)
                for (var j = 0; j < GridSize; j++)
                    if (Tiles[i, j] != null)
                        Tiles[i, j].SetPosition(TilePos[i, j].position, firstIns);
        }

        private IEnumerator WaitForTileMove()
        {
            yield return new WaitForSeconds(tileData.movingTime);
            _isMoving = false;
        }

        private bool _tilesUpdt;
        
        #region SwipeManager

        private Vector2 _startPosition;
        private const float SwipeThreshold = 50f;


        private void OnEnable()
        {
            LeanTouch.OnFingerDown += OnFingerDown;
            LeanTouch.OnFingerUp += OnFingerUp;
        }

        private void OnDisable()
        {
            LeanTouch.OnFingerDown -= OnFingerDown;
            LeanTouch.OnFingerUp -= OnFingerUp;
        }

        private void OnFingerDown(LeanFinger finger)
        {
            _startPosition = finger.ScreenPosition;
        }

        private void OnFingerUp(LeanFinger finger)
        {
            var currentPosition = finger.ScreenPosition;
            var direction = currentPosition - _startPosition;

            if (!(direction.magnitude >= SwipeThreshold)) return;
            direction.Normalize();

            if(!_isMoving)
                DefineFingerMoving(direction);
        }

        #endregion


        #region MoveDirection

        private void DefineFingerMoving(Vector2 direction)
        {
            _tilesUpdt = false;
            
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                if (direction.x > 0)
                    MoveRight();
                else
                    MoveLeft();
            }
            else
            {
                if (direction.y > 0)
                    MoveUp();
                else
                    MoveDown();
            }
            
            if(_tilesUpdt)
                UpdateTilePosition(false);
        }
        
        
        private void MoveRight()
        {
              for (var j = 0; j < GridSize; j++)
                  for (var i= GridSize - 1; i >= 0; i--)
                  {
                      if (Tiles[i,j] == null) continue; 
                      
                      for (var k = GridSize - 1; k > i; k--)
                      {
                          if (Tiles[k,j] != null) continue;

                          _tilesUpdt = true;
                          Tiles[k, j] = Tiles[i, j];
                          Tiles[i, j] = null;
                          break;
                      }
                  }
        }
        private void MoveLeft()
        {
            for (var j = 0; j < GridSize; j++)
                for (var i=0; i<GridSize; i++)
                {
                    if (Tiles[i,j] == null) continue;
                            for (var k = 0; k < i; k++)
                    {
                        if (Tiles[k,j] != null) continue;
                        
                        _tilesUpdt = true;
                        Tiles[k, j] = Tiles[i, j];
                        Tiles[i, j] = null;
                        break;
                    }
                }
        }
        private void MoveDown()
        {
            for (var i = 0; i < GridSize; i++)
                for (var j = GridSize-1; j>=0; j--)
                {
                    if(Tiles[i,j] == null) continue;
                    for (var k = GridSize-1; k >j; k--)
                    {
                        if(Tiles[i,k] != null) continue;
                        
                        _tilesUpdt = true;
                        Tiles[i, k] = Tiles[i, j];
                        Tiles[i, j] = null;
                        break;
                    }
                }
            
        }
        private void MoveUp()
        {
            for (var i = 0; i < GridSize; i++)
                for (var j =0; j<GridSize; j++)
                {
                    if(Tiles[i,j] == null) continue;
                    for (var k = 0; k < j; k++)
                    {
                        if(Tiles[i,k] != null) continue;
                        
                        _tilesUpdt = true;
                        Tiles[i, k] = Tiles[i, j];
                        Tiles[i, j] = null;
                        break;    
                    }
                }
        }

        #endregion
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
}


































