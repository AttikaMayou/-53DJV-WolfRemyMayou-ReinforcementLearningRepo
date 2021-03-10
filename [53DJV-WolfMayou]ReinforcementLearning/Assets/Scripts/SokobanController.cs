using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SokobanController : MonoBehaviour
{
    [SerializeField] public Grid grid;

    [SerializeField] public GameObject playerPrefab;
    public GameObject _player;

    [Header("Grid Color")]
    [SerializeField] public Material emptyGridMaterial;
    [SerializeField] public Material wallGridMaterial;
    [SerializeField] public Material crateGridMaterial;
    [SerializeField] public Material cratePlacedGridMaterial;
    [SerializeField] public Material targetBoxGridMaterial;

    [Header("GameObject")]
    [SerializeField] public GameObject crate;
    [SerializeField] public GameObject wall;

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

    // Check Collision With Player
    private bool checkCollision(Vector3 currentPosition, Vector3 direction)
    {
        /*Vector3 targetPosition = currentPosition + direction;
        int x = (int)targetPosition.x;
        int z = (int)targetPosition.z;*/
        float maxDistance = 1.0f;
        RaycastHit hit;

        // What we hit
        if (Physics.Raycast(currentPosition, direction, out hit, maxDistance))
        {
            Debug.Log("Hit Something.");

            // If Wall
            if (hit.collider.gameObject.tag == "Wall")
            {
                Debug.Log("Un mur bloque le chemin pour avancer.");
                return false;
            }

            // If Crate
            if (hit.collider.gameObject.tag == "Crate")
            {
                Debug.Log("Hit a le tag Crate.");
                GameObject crate = hit.collider.gameObject;

                // Can i move the crate
                if (Physics.Raycast(crate.transform.position, direction, out hit, maxDistance))
                {
                    if (hit.collider.gameObject.tag == "Wall")
                    {
                        Debug.Log("Hit a le tag Wall.");
                        Debug.Log("Impossible de déplacer la caisse dans un mur.");
                        Debug.Log(hit.collider.gameObject);
                        return false;
                    }
                    else
                    {
                        Debug.Log("Pas de mur en obstacle je déplace la caisse.");
                        if ((crate.transform.position += direction).z < grid.gridHeight ||
                            (crate.transform.position += direction).z > 1 ||
                            (crate.transform.position += direction).x > 1 ||
                            (crate.transform.position += direction).x < grid.gridWidth)
                        {
                            Debug.Log("Caisse déplacé.");
                            crate.transform.position += direction;
                            return true;
                        }
                        else
                        {
                            Debug.Log("Impossible de déplacer la caisse hors de la carte.");
                            return false;
                        }
                    }
                }
                else
                {
                    Debug.Log("Pas d'obstacle je déplace la caisse.");
                    crate.transform.position += direction;
                    return true;
                }
            }

        }

        return true;
    }

    public void UpIntent()
    {
        if (_player.transform.position.z < grid.gridHeight - 1 && checkCollision(_player.transform.position, Vector3.forward))
        {
            _player.transform.position += Vector3.forward;
        }
    }

    public void DownIntent()
    {
        if (_player.transform.position.z > 0 && checkCollision(_player.transform.position, - Vector3.forward))
        {
            _player.transform.position -= Vector3.forward;
        }
    }

    public void LeftIntent()
    {
        if (_player.transform.position.x > 0 && checkCollision(_player.transform.position, Vector3.left))
        {
            _player.transform.position += Vector3.left;
        }
    }

    public void RightIntent()
    {
        if (_player.transform.position.x < grid.gridWidth - 1 && checkCollision(_player.transform.position, -Vector3.left))
        {
            _player.transform.position -= Vector3.left;
        }
    }
}