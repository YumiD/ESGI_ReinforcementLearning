using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private ScriptableGrid _grid;
    private Grid2D _grid2D;
    private Vector2 _currentPosition;

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
                if (horizontal < 0) // Aller a gauche
                {
                    if (_currentPosition.x - 1 >= 0)
                    {
                        _currentPosition.x--;
                        transform.position = new Vector2(transform1.x - 1, transform1.y);
                    }
                }
                else if (horizontal > 0) // Aller a droite
                {
                    if (_currentPosition.x + 1 < _grid.Width)
                    {
                        _currentPosition.x++;
                        transform.position = new Vector2(transform1.x + 1, transform1.y);
                    }
                }
                else if (vertical != 0)
                {
                    if (vertical < 0) // Aller en bas
                    {
                        if (_currentPosition.y - 1 >= 0)
                        {
                            _currentPosition.y--;
                            transform.position = new Vector2(transform1.x, transform1.y - 1);
                        }
                    }
                    else if (vertical > 0) // Aller en haut
                    {
                        if (_currentPosition.y + 1 < _grid.Height)
                        {
                            _currentPosition.y++;
                            transform.position = new Vector2(transform1.x, transform1.y + 1);
                        }
                    }
                }

                CheckCurrentTile(_grid.State.Grid[_grid.Height - 1 - (int)_currentPosition.y, (int)_currentPosition.x],
                    _currentPosition, oldPosition, _grid);
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
                    Debug.Log("Right");
                    newPos = new Vector2(grid.Height - 1 - (int)curr.y, (int)curr.x + 1);
                }
                else if (old.x > curr.x) // Pousser vers la gauche
                {
                    Debug.Log("Gauche");
                    newPos = new Vector2(grid.Height - 1 - (int)curr.y, (int)curr.x - 1);
                }
                else if (old.y < curr.y) // Pousser vers le haut
                {
                    Debug.Log("Up");
                    newPos = new Vector2(grid.Height - 1 - (int)curr.y, (int)curr.x - 1);
                }
                else if (old.y > curr.y) // Pousser vers le bas
                {
                    Debug.Log("Down");
                    newPos = new Vector2(grid.Height - 1 - (int)curr.y, (int)curr.x - 1);
                }
                grid.State.Grid[(int)newPos.y, (int)newPos.x] =
                    grid.State.Grid[grid.Height - 1 - (int)curr.y, (int)curr.x];
                break;
            default:
                return;
        }

        var tileNb = (int)((grid.Height - 1 - curr.y) * grid.Width) + (int)curr.x+1;
        Debug.Log(tileNb);
        Debug.Log(newPos);
        _grid2D.MoveTile(tileNb, newPos);
    }
}