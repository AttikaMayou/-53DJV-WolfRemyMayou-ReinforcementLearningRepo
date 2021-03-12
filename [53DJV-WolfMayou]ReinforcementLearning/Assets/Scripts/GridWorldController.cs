using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GridWorldController : MonoBehaviour
{
    [SerializeField] public GameObject playerPrefab;
    public GameObject player;
    [SerializeField] public Grid grid;
    public GameObject downArrow;
    public GameObject upArrow;
    public GameObject leftArrow;
    public GameObject rightArrow;
    
    public enum GridWorldIntent
    {
        Down,
        Up,
        Left,
        Right
    }
    
    public void InitGridWorldGame()
    {
        grid.gridWidth = 4;
        grid.gridHeight = 4;
        grid.GridWorld();
        player = Instantiate(playerPrefab, grid.startPos + new Vector3(0, 0.5f, 0), Quaternion.identity);
        player.transform.SetParent(this.transform);
    }

    public void UpIntent()
    {
        if (player.transform.position.z < grid.gridHeight-1)
        {
            player.transform.position += Vector3.forward;
        }
    }
    
    public void DownIntent()
    {
        if (player.transform.position.z > 0)
        {
            player.transform.position -= Vector3.forward;
        }
    }

    public void LeftIntent()
    {
        if (player.transform.position.x > 0)
        {
            player.transform.position += Vector3.left;
        }
    }

    public void RightIntent()
    {
        if (player.transform.position.x < grid.gridWidth-1)
        {
            player.transform.position -= Vector3.left;
        }
    }

    private void Update()
    {
        if (GameSelector.type != GameSelector.GameType.GridWorld) return;
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