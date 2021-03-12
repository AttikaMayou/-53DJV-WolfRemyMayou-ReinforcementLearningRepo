using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GameSelector : MonoBehaviour
{
    [SerializeField] private GridWorldController gridWorldController;
    [SerializeField] private TicTacToeController ticTacToeController;
    [SerializeField] private SokobanController sokobanController;

    [SerializeField] private GameObject gridWorldLegends;
    [SerializeField] private GameObject ticTacToeLegends;
    [SerializeField] private GameObject sokobanLegends;
    
    public enum GameType
    {
        GridWorld,
        TicTacToe,
        Sokoban
    }

    public static GameType type;
    
    public void ChangeGameType(int value)
    {
        type = (GameType) value;
    }
    
    public void InitGameGrid()
    {
        switch (type)
        {
            case GameType.GridWorld:
                gridWorldController.InitGridWorldGame();
                
                gridWorldLegends.SetActive(true);
                ticTacToeLegends.SetActive(false);
                sokobanLegends.SetActive(false);
                break;
            
            case GameType.TicTacToe:
                ticTacToeController.InitTicTacToeGame();
                
                gridWorldLegends.SetActive(false);
                ticTacToeLegends.SetActive(true);
                sokobanLegends.SetActive(false);
                break;
            
            case GameType.Sokoban:
                sokobanController.InitSokobanGrid();
                
                gridWorldLegends.SetActive(false);
                ticTacToeLegends.SetActive(false);
                sokobanLegends.SetActive(true);
                break;
        }
    }
}
