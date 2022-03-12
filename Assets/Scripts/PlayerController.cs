using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public enum Movement
{
    Up = 1,
    Down = -1,
    Right = 1,
    Left = -1
}
public enum PossibleMovement{Up, Down, Right, Left};

public class PlayerController : MonoBehaviour
{
    private ScriptableGrid _grid;
    private Grid2D _grid2D;
    private Vector2 _currentPosition;
    private TileType _oldTile = TileType.Ground;
    private List<Vector2> _listOfMoves = new List<Vector2>();

    public void Spawn(ScriptableGrid grid, Vector2 currentPos, Grid2D grid2D, List<Vector2> listOfMoves)
    {
        _grid = grid;
        _grid2D = grid2D;
        _currentPosition = currentPos;
        _listOfMoves = listOfMoves;
        
        // // Example of policy in input (raw input but will be replaced by list on parameter
        // _listOfMoves.Add(new Vector2(1, 0));
        // _listOfMoves.Add(new Vector2(0, 1));
        // _listOfMoves.Add(new Vector2(1, 0));
        //
        // StartCoroutine(PlayMoves(_listOfMoves, _listOfMoves[0]));
    }

    private void Update()
    {
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");
        if (Input.anyKeyDown)
        {
            Move(new Vector2(horizontal, vertical));
        }
    }

    private void Move(Vector2 movement)
    {
        var horizontal = movement.x;
        var vertical = movement.y;

        // if (!Input.anyKeyDown) return;
        var transform1 = transform.position;
        var oldPosition = _currentPosition;
        if (horizontal == 0 && vertical == 0) return;
        bool crateInFront;
        if (horizontal != 0)
        {
            crateInFront = _grid.State.Grid[_grid.Height - 1 - (int)_currentPosition.y,
                (int)_currentPosition.x - (horizontal > 0
                    ? (int)Movement.Right
                    : (int)Movement.Left)] == (int)TileType.Crate;
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
        
        // DebugDisplay.DisplayGrid(_grid);
    }

    private void CheckCurrentTile(int tileValue, Vector2 curr, Vector2 old, ScriptableGrid grid)
    {
        var newPos = Vector2.zero;
        switch (tileValue)
        {
            case (int)TileType.Goal:
                Debug.Log("Win");
                break;
            case (int)TileType.Void:
                Debug.Log("Lose");
                break;
            case (int)TileType.Crate:
                if (old.x < curr.x) // Push to the right
                {
                    newPos = new Vector2(_currentPosition.x + 1, _currentPosition.y);
                }
                else if (old.x > curr.x) // Push to the left
                {
                    newPos = new Vector2(_currentPosition.x - 1, _currentPosition.y);
                }
                else if (old.y < curr.y) // Push toward the top
                {
                    newPos = new Vector2(_currentPosition.x, _currentPosition.y + 1);
                }
                else if (old.y > curr.y) // Push toward the bottom
                {
                    newPos = new Vector2(_currentPosition.x, _currentPosition.y - 1);
                }

                _grid2D.VerifyCrateOnGoal(newPos, _currentPosition);
                grid.State.Grid[grid.Height - 1 - (int)newPos.y, (int)newPos.x] = tileValue;
                var tileNb = (int)((grid.Height - 1 - curr.y) * grid.Width) + (int)curr.x;
                _grid2D.MoveTile(tileNb, newPos);
                break;
            default:
                return;
        }
    }

    private IEnumerator PlayMoves(List<Vector2> listOfMoves, Vector2 move)
    {
        yield return new WaitForSeconds(.5f);
        Move(move);
        listOfMoves.RemoveAt(0);
        if (listOfMoves.Count == 0)
        {
            Debug.Log("End of Moves.");
            yield break;
        }
        StartCoroutine(PlayMoves(listOfMoves, listOfMoves[0]));
    }
}