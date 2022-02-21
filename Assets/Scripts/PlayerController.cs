using System;
using UnityEngine;

public enum Movement
{
    Up = 1,
    Down = -1,
    Right = 1,
    Left = -1
}

public class PlayerController : MonoBehaviour
{
    private ScriptableGrid _grid;
    private Grid2D _grid2D;
    private Vector2 _currentPosition;
    private TileType _oldTile = TileType.Ground;

    public void Spawn(ScriptableGrid grid, Vector2 currentPos, Grid2D grid2D)
    {
        _grid = grid;
        _grid2D = grid2D;
        _currentPosition = currentPos;
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");

        if (Input.anyKeyDown)
        {
            var transform1 = transform.position;
            var oldPosition = _currentPosition;
            if (horizontal != 0 || vertical != 0)
            {
                bool crateInFront;
                if (horizontal != 0)
                {
                    crateInFront = _grid.State.Grid[_grid.Height - 1 - (int)_currentPosition.y,
                                       (int)_currentPosition.x - (horizontal > 0
                                           ? (int)Movement.Right
                                           : (int)Movement.Left)] ==
                                   (int)TileType.Crate;
                    if (_grid2D.IsActionPossible(horizontal > 0 ? Movement.Right : Movement.Left,
                            new Vector2Int((int)_currentPosition.x + (int)horizontal, (int)_currentPosition.y),
                            crateInFront, horizontal != 0))
                    {
                        _currentPosition.x += (int)horizontal;
                        transform.position = new Vector2(transform1.x + horizontal, transform1.y);
                    }
                }
                else
                {
                    crateInFront =
                        _grid.State.Grid[
                            _grid.Height - 1 - (int)_currentPosition.y - (vertical > 0
                                ? (int)Movement.Up
                                : (int)Movement.Down), (int)_currentPosition.x] == (int)TileType.Crate;
                    if (_grid2D.IsActionPossible(vertical > 0 ? Movement.Up : Movement.Down,
                            new Vector2Int((int)_currentPosition.x, (int)_currentPosition.y + (int)vertical),
                            crateInFront, horizontal != 0))
                    {
                        _currentPosition.y += (int)vertical;
                        transform.position = new Vector2(transform1.x, transform1.y + vertical);
                    }
                }

                CheckCurrentTile(_grid.State.Grid[_grid.Height - 1 - (int)_currentPosition.y, (int)_currentPosition.x],
                    _currentPosition, oldPosition, _grid);
                // Set what was behind the player
                _grid.State.Grid[_grid.Height - 1 - (int)oldPosition.y, (int)oldPosition.x] =
                    (int)_oldTile;
                // Save the old tile, if crate, become ground
                if ((TileType)_grid.State.Grid[_grid.Height - 1 - (int)_currentPosition.y,
                        (int)_currentPosition.x] == TileType.Crate)
                {
                    _oldTile = TileType.Ground;
                }
                else
                {
                    _oldTile = (TileType)_grid.State.Grid[_grid.Height - 1 - (int)_currentPosition.y,
                        (int)_currentPosition.x];
                }

                // Set player position on grid
                _grid.State.Grid[_grid.Height - 1 - (int)_currentPosition.y, (int)_currentPosition.x] =
                    (int)TileType.Player;

                // // Display player position on grid map
                // for (int i = 0; i < _grid.Height; i++)
                // {
                //     string t = String.Empty;
                //     for (int j = 0; j < _grid.Width; j++)
                //     {
                //         t += _grid.State.Grid[i, j].ToString();
                //     }
                //
                //     print(t);
                // }
            }
        }
    }

    private void CheckCurrentTile(int tileValue, Vector2 curr, Vector2 old, ScriptableGrid grid)
    {
        Vector2 newPos = Vector2.zero;
        switch (tileValue)
        {
            case (int)TileType.Goal:
                Debug.Log("Win");
                break;
            case (int)TileType.Void:
                Debug.Log("Lose");
                break;
            case (int)TileType.Crate:
                if (old.x < curr.x) // Pousser vers la droite
                {
                    newPos = new Vector2(_currentPosition.x + 1, _currentPosition.y);
                }
                else if (old.x > curr.x) // Pousser vers la gauche
                {
                    newPos = new Vector2(_currentPosition.x - 1, _currentPosition.y);
                }
                else if (old.y < curr.y) // Pousser vers le haut
                {
                    newPos = new Vector2(_currentPosition.x, _currentPosition.y + 1);
                }
                else if (old.y > curr.y) // Pousser vers le bas
                {
                    newPos = new Vector2(_currentPosition.x, _currentPosition.y - 1);
                }

                grid.State.Grid[grid.Height - 1 - (int)newPos.y, (int)newPos.x] = tileValue;
                var tileNb = (int)((grid.Height - 1 - curr.y) * grid.Width) + (int)curr.x;
                _grid2D.MoveTile(tileNb, newPos);
                break;
            default:
                return;
        }
    }
}