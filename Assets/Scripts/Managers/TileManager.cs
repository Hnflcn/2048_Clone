using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using Enums;
using InputProcess;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
    public class TileManager : MonoBehaviour
    {
        public static readonly int GridSize = 4;
        
        private readonly Transform[,] _tilePositions = new Transform[GridSize, GridSize];
        private readonly Tile.Tile[,] _tiles = new Tile.Tile[GridSize, GridSize];
        
        [SerializeField] private Tile.Tile tilePrefab;
        [SerializeField] private TileData tileData;
    
        [SerializeField] private UnityEvent<int> scoreUpdated;
        [SerializeField] private UnityEvent<int> bestScoreUpdated;
        [SerializeField] private UnityEvent<int> moveCountUpdated;
        [SerializeField] private UnityEvent<System.TimeSpan> gameTimeUpdated;
    
        [SerializeField] private GameOverScreen gameOverScreen;
    
        private readonly Stack<GameState> _gameStates = new Stack<GameState>();
        private readonly System.Diagnostics.Stopwatch _gameStop = new System.Diagnostics.Stopwatch();
    
        private IInputManager _inputManager = new MultipleInputManager(new KeyboardInputManager(), new SwipeInputManager());

        private bool _isMoving;
        private int _score;
        private int _bestScore;
        private int _moveCount;

        private void Start()
        {
            GetTilePositions();
            SpawnProcess();
            SpawnProcess();
            UpdateTilePositions(true);

            _gameStop.Start();
            _bestScore = PlayerPrefs.GetInt(ConstantVariables.BestScore, 0);
            bestScoreUpdated.Invoke(_bestScore);
        }



        private void Update()
        {
            gameTimeUpdated.Invoke(_gameStop.Elapsed);

            var input = _inputManager.GetInput();

            if (!_isMoving)
                MoveProcess(input.XInp, input.YInp);

        }

        public void AddScore(int value)
        {
            _score += value;
            scoreUpdated.Invoke(_score);
            if (_score <= _bestScore) return;
            _bestScore = _score;
            bestScoreUpdated.Invoke(_bestScore);
            PlayerPrefs.SetInt(ConstantVariables.BestScore, _bestScore);
        }

        public void RestartGame()
        {
            var activeScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(activeScene.name);
        }

        private void GetTilePositions()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
            var index = 0;
            foreach (Transform trans in transform)
            {
                var x = index % GridSize;
                var y = index / GridSize;
                _tilePositions[x, y] = trans;
                index++;
            }
        }

        private bool SpawnProcess()
        {
            var availableSlots = new List<Vector2Int>();

            for (var x = 0; x < GridSize; x++)
            for (var y = 0; y < GridSize; y++)
            {
                if (_tiles[x, y] == null)
                    availableSlots.Add(new Vector2Int(x, y));
            }

            if (!availableSlots.Any())
                return false;

            var randomIndex = Random.Range(0, availableSlots.Count);
            var slot = availableSlots[randomIndex];

            var tile = Instantiate(tilePrefab, transform.parent);
            tile.SetValue(GetRandomValue());
            _tiles[slot.x, slot.y] = tile;

            return true;
        }

        private int GetRandomValue()
        {
            return Random.value <= 0.8f ? 2 : 4;
        }

        private void UpdateTilePositions(bool firstIns)
        {
            if (!firstIns)
            {
                _isMoving = true;
                StartCoroutine(WaitForTileAnimation());
            }

            for (var x = 0; x < GridSize; x++)
            for (var y = 0; y < GridSize; y++)
                if (_tiles[x, y] != null)
                    _tiles[x, y].SetPosition(_tilePositions[x, y].position, firstIns);
        }

        private IEnumerator WaitForTileAnimation()
        {
            yield return new WaitForSeconds(tileData.animTime);
            
            if (!SpawnProcess())
            {
                Debug.LogError("error");
            }
            UpdateTilePositions(true);

            if (!AnyMovesLeft())
            {
                gameOverScreen.SetGameOver(true);
            }

            _isMoving = false;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private bool AnyMovesLeft()
        {
            return CanMoveLeft() || CanMoveUp() || CanMoveRight() || CanMoveDown();
        }

        private bool _tilesUpdated;
    //  private void MoveProcess(int x, int y)
    //  {
    //      if (x == 0 && y == 0)
    //          return;

    //      if (Mathf.Abs(x) == 1 && Mathf.Abs(y) == 1)
    //      {
    //          Debug.LogWarning($"Invalid move {x}, {y}");
    //          return;
    //      }

    //      _tilesUpdated = false;
    //      var previousMoveTileValues = GetCurrentTileValues();

    //      switch (x)
    //      {
    //          case 0 when y > 0:
    //              MoveUp();
    //              break;
    //          case 0:
    //              MoveDown();
    //              break;
    //          case < 0:
    //              MoveLeft();
    //              break;
    //          default:
    //              MoveRight();
    //              break;
    //      }

    //      if (!_tilesUpdated) return;
    //      _gameStates.Push(new GameState { TileValues = previousMoveTileValues, Score = _score, MoveCount = _moveCount });
    //      _moveCount++;
    //      moveCountUpdated.Invoke(_moveCount);
    //      UpdateTilePositions(false);
    //  }
private void MoveProcess(int x, int y)
{
    if (x == 0 && y == 0)
    {
        return;
    }

    if (Mathf.Abs(x) == 1 && Mathf.Abs(y) == 1)
    {
        Debug.LogWarning($"Invalid move {x}, {y}");
        return;
    }

    var direction = GetDirection(x, y);
    if (CanMoveInDirection(direction))
    {
        _tilesUpdated = false;
        var previousMoveTileValues = GetCurrentTileValues();

        MoveTilesInDirection(direction);

        if (_tilesUpdated)
        {
            _gameStates.Push(new GameState { TileValues = previousMoveTileValues, Score = _score, MoveCount = _moveCount });
            _moveCount++;
            moveCountUpdated.Invoke(_moveCount);
            UpdateTilePositions(false);
        }
    }
}

private Direction GetDirection(int x, int y)
{
    if (x == 0)
    {
        return y > 0 ? Direction.Up : Direction.Down;
    }
    else
    {
        return x > 0 ? Direction.Right : Direction.Left;
    }
}

private bool CanMoveInDirection(Direction direction)
{
    switch (direction)
    {
        case Direction.Up:
            return CanMoveUp();
        case Direction.Down:
            return CanMoveDown();
        case Direction.Left:
            return CanMoveLeft();
        case Direction.Right:
            return CanMoveRight();
        default:
            return false;
    }
}

private void MoveTilesInDirection(Direction direction)
{
    for (var x = 0; x < GridSize; x++)
    {
        for (var y = 0; y < GridSize; y++)
        {
            var cell = _tiles[x, y];
            if (cell != null)
            {
                switch (direction)
                {
                    case Direction.Up:
                        MoveUp();
                        break;
                    case Direction.Down:
                        MoveDown();
                        break;
                    case Direction.Left:
                        MoveLeft();
                        break;
                    case Direction.Right:
                        MoveRight();
                        break;
                }
            }
        }
    }
}
        private int[,] GetCurrentTileValues()
        {
            var result = new int[GridSize, GridSize];
            for (var x = 0; x < GridSize; x++)
            for (var y = 0; y < GridSize; y++)
                if (_tiles[x, y] != null)
                    result[x, y] = _tiles[x, y].GetValue();

            return result;
        }

        public void LoadLastGameState()
        {
            if (_isMoving || !_gameStates.Any())
                return;

            var prevGameState = _gameStates.Pop();

            gameOverScreen.SetGameOver(false);

            _score = prevGameState.Score;
            scoreUpdated.Invoke(_score);

            _moveCount = prevGameState.MoveCount;
            moveCountUpdated.Invoke(_moveCount);
            
            ClearTiles();

            for (var x = 0; x < GridSize; x++)
                for (var y = 0; y < GridSize; y++)
                {
                    if (prevGameState.TileValues[x, y] == 0) continue;
                    var tile = Instantiate(tilePrefab, transform.parent);
                    tile.SetValue(prevGameState.TileValues[x, y]);
                    _tiles[x, y] = tile;
                }

            UpdateTilePositions(true);
        }
        
        private void ClearTiles()
        {
            for (var x = 0; x < GridSize; x++)
            {
                for (var y = 0; y < GridSize; y++)
                {
                    if (_tiles[x, y] == null) continue;
                    Destroy(_tiles[x, y].gameObject);
                    _tiles[x, y] = null;
                }
            }
        }

        private bool TileExistsBetween(int x, int y, int x2, int y2)
        {
            if (x == x2)
                return TileExistsBetweenHorizontal(x, y, y2);
            else if (y == y2)
                return TileExistsBetweenVertical(x, x2, y);

            Debug.LogError("error");
            return false;

        }

        private bool TileExistsBetweenVertical(int x, int x2, int y)
        {
            var minX = Mathf.Min(x, x2);
            var maxX = Mathf.Max(x, x2);
            for (var xIndex = minX + 1; xIndex < maxX; xIndex++)
                if (_tiles[xIndex, y] != null)
                    return true;
            return false;
        }

        private bool TileExistsBetweenHorizontal(int x, int y, int y2)
        {
            var minY = Mathf.Min(y, y2);
            var maxY = Mathf.Max(y, y2);
            for (var yIndex = minY + 1; yIndex < maxY; yIndex++)
                if (_tiles[x, yIndex] != null)
                    return true;
            return false;
        }

        private void MoveRight()
        {
            for (var y = 0; y < GridSize; y++)
            for (var x = GridSize - 1; x >= 0; x--)
            {
                if (_tiles[x, y] == null) continue;

                for (var x2 = GridSize - 1; x2 > x; x2--)
                {
                    if (_tiles[x2, y] != null)
                    {
                        if (TileExistsBetween(x, y, x2, y))
                            continue;
                        if (_tiles[x2, y].Merge(_tiles[x, y]))
                        {
                            _tiles[x, y] = null;
                            _tilesUpdated = true;
                            break;
                        }

                        continue;
                    }

                    _tilesUpdated = true;
                    _tiles[x2, y] = _tiles[x, y];
                    _tiles[x, y] = null;
                    break;
                }
            }
        }

        private void MoveLeft()
        {
            for (var y = 0; y < GridSize; y++)
            for (var x = 0; x < GridSize; x++)
            {
                if (_tiles[x, y] == null) continue;
                for (var x2 = 0; x2 < x; x2++)
                {
                    if (_tiles[x2, y] != null)
                    {
                        if (TileExistsBetween(x, y, x2, y))
                            continue;
                        if (_tiles[x2, y].Merge(_tiles[x, y]))
                        {
                            _tiles[x, y] = null;
                            _tilesUpdated = true;
                            break;
                        }

                        continue;
                    }

                    _tilesUpdated = true;
                    _tiles[x2, y] = _tiles[x, y];
                    _tiles[x, y] = null;
                    break;
                }
            }
        }

        private void MoveDown()
        {
            for (var x = 0; x < GridSize; x++)
            for (var y = GridSize - 1; y >= 0; y--)
            {
                if (_tiles[x, y] == null) continue;
                for (var y2 = GridSize - 1; y2 > y; y2--)
                {
                    if (_tiles[x, y2] != null)
                    {
                        if (TileExistsBetween(x, y, x, y2))
                            continue;
                        if (_tiles[x, y2].Merge(_tiles[x, y]))
                        {
                            _tiles[x, y] = null;
                            _tilesUpdated = true;
                            break;
                        }

                        continue;
                    }

                    _tilesUpdated = true;
                    _tiles[x, y2] = _tiles[x, y];
                    _tiles[x, y] = null;
                    break;
                }
            }
        }

        private void MoveUp()
        {
            for (var x = 0; x < GridSize; x++)
            for (var y = 0; y < GridSize; y++)
            {
                if (_tiles[x, y] == null) continue;
                for (var y2 = 0; y2 < y; y2++)
                {
                    if (_tiles[x, y2] != null)
                    {
                        if (TileExistsBetween(x, y, x, y2))
                            continue;
                        if (_tiles[x, y2].Merge(_tiles[x, y]))
                        {
                            _tiles[x, y] = null;
                            _tilesUpdated = true;
                            break;
                        }

                        continue;
                    }

                    _tilesUpdated = true;
                    _tiles[x, y2] = _tiles[x, y];
                    _tiles[x, y] = null;
                    break;
                }
            }
        }


        private bool CanMoveRight()
        {
            for (var y = 0; y < GridSize; y++)
            for (var x = GridSize - 1; x >= 0; x--)
            {
                if (_tiles[x, y] == null) continue;

                for (var x2 = GridSize - 1; x2 > x; x2--)
                {
                    if (_tiles[x2, y] == null) return true;
                    if (TileExistsBetween(x, y, x2, y))
                        continue;
                    if (_tiles[x2, y].CanMerge(_tiles[x, y]))
                    {
                        return true;
                    }

                }
            }
            return false;
        }

        private bool CanMoveLeft()
        {
            for (var y = 0; y < GridSize; y++)
            for (var x = 0; x < GridSize; x++)
            {
                if (_tiles[x, y] == null) continue;
                for (var x2 = 0; x2 < x; x2++)
                {
                    if (_tiles[x2, y] == null) return true;
                    if (TileExistsBetween(x, y, x2, y))
                        continue;
                    if (_tiles[x2, y].CanMerge(_tiles[x, y]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool CanMoveDown()
        {
            for (var x = 0; x < GridSize; x++)
            for (var y = GridSize - 1; y >= 0; y--)
            {
                if (_tiles[x, y] == null) continue;
                for (var y2 = GridSize - 1; y2 > y; y2--)
                {
                    if (_tiles[x, y2] == null) return true;
                    if (TileExistsBetween(x, y, x, y2))
                        continue;
                    if (_tiles[x, y2].CanMerge(_tiles[x, y]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool CanMoveUp()
        {
            for (var x = 0; x < GridSize; x++)
            for (var y = 0; y < GridSize; y++)
            {
                if (_tiles[x, y] == null) continue;
                for (var y2 = 0; y2 < y; y2++)
                {
                    if (_tiles[x, y2] == null) return true;
                    if (TileExistsBetween(x, y, x, y2))
                        continue;
                    if (_tiles[x, y2].CanMerge(_tiles[x, y]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
