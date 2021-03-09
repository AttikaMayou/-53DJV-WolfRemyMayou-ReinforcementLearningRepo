using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridWorldController : MonoBehaviour
{
    [SerializeField] public GameObject player;
    [SerializeField] public Grid grid;
    
    public enum Intents
    {
        Down,
        Up,
        Left,
        Right
    }
    
    private void Start()
    {
        grid.InitGridWorld();
        player = Instantiate(player, grid.startPos,Quaternion.identity);
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
