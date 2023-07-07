using System;
using Lean.Touch;
using UnityEngine;

namespace Managers
{
    public class SwipeManager : TileManager
    {
        #region Variables

        private Vector2 _startPosition;
        private const float SwipeThreshold = 50f;

        #endregion

        #region LeanTouch Functions

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

            DefineFingerMoving(direction);
        }

        #endregion
       
        


        #region MoveDirection

        private void DefineFingerMoving(Vector2 direction)
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                if (direction.x > 0)
                {
                    MoveRight();
                }
                else
                {
                    MoveLeft();
                }
            }
            else
            {
                if (direction.y > 0)
                {
                    MoveUp();
                }
                else
                {
                    MoveDown();
                }
            }
        }
        
        
        private void MoveRight()
        {
            Debug.Log("sag");
            for (var j = 0; j < GridSize; j++)
                for (var i= GridSize - 1; i >= 0; i--)
                {
                    if (Tiles[i,j] == null) continue;

                    for (var k = GridSize - 1; k > i; k--)
                    {
                        if (Tiles[k,j] != null) continue;

                        Tiles[k, j] = Tiles[i, j];
                        Tiles[i, j] = null;
                        break;
                    }
                }
        }

        private void MoveLeft()
        {
            Debug.Log("sol");
            for (var j = 0; j < GridSize; j++)
                for (var i=0; i<GridSize; i++)
                {
                    if (Tiles[i,j] == null) continue;

                    for (var k = 0; k < i; k++)
                    {
                        if (Tiles[k,j] != null) continue;

                        Tiles[k, j] = Tiles[i, j];
                        Tiles[i, j] = null;
                        break;
                    }
                }
        }

        private void MoveDown()
        {
            Debug.Log("aşağı");
            for (var i = 0; i < GridSize; i++)
                for (var j = GridSize-1; j>=0; j--)
                {
                    if(Tiles[i,j] == null) continue;
                    for (var k = GridSize-1; k >j; k--)
                    {
                        if(Tiles[i,k] != null) continue;

                        Tiles[i, k] = Tiles[i, j];
                        Tiles[i, j] = null;
                        break;
                    }
                }
            
        }

        private void MoveUp()
        {
            Debug.Log("yukarı");
            for (var i = 0; i < GridSize; i++)
                for (var j =0; j<GridSize; j++)
                {
                    if(Tiles[i,j] == null) continue;
                    for (var k = 0; k < j; k++)
                    {
                        if(Tiles[i,k] != null) continue;

                        Tiles[i, k] = Tiles[i, j];
                        Tiles[i, j] = null;
                        break;    
                    }
                }
        }

        #endregion

       
    }

    
}
