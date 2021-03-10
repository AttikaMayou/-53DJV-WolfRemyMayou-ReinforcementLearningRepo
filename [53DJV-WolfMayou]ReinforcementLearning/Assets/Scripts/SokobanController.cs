using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SokobanController : MonoBehaviour
{
    [SerializeField] public Grid grid;

    [SerializeField] public GameObject playerPrefab;
    public GameObject _player;

    private int playerStrokeNumber = 0;
    private bool gameIsFinished = false;

    public enum SokobanIntent
    {
        Down,
        Up,
        Left,
        Right
    }

    public void InitSokobanGrid()
    {
        // Set Sokoban Grid Size
        grid.gridWidth = 4;
        grid.gridHeight = 4;
        grid.Sokoban();
        _player = Instantiate(playerPrefab, grid.startPos + new Vector3(0, 0.5f, 0), Quaternion.identity);
        _player.transform.SetParent(this.transform);
    }

    public void UpIntent()
    {
        if (_player.transform.position.z < grid.gridHeight - 1)
        {
            _player.transform.position += Vector3.forward;
        }
    }

    public void DownIntent()
    {
        if (_player.transform.position.z > 0)
        {
            _player.transform.position -= Vector3.forward;
        }
    }

    public void LeftIntent()
    {
        if (_player.transform.position.x > 0)
        {
            _player.transform.position += Vector3.left;
        }
    }

    public void RightIntent()
    {
        if (_player.transform.position.x < grid.gridWidth - 1)
        {
            _player.transform.position -= Vector3.left;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            UpIntent();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            DownIntent();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            LeftIntent();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            RightIntent();
        }
    }
}