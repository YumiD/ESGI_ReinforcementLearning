using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private ScriptableGrid _grid;
    private Vector2 _currentPosition;

    public void Spawn(ScriptableGrid grid, Vector2 currentPos)
    {
        _grid = grid;
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
                else if (vertical > 0) // aller en haut
                {
                    if (_currentPosition.y + 1 < _grid.Height)
                    {
                        _currentPosition.y++;
                        transform.position = new Vector2(transform1.x, transform1.y + 1);
                    }
                }
            }
        }
    }
}