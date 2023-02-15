using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class BallSpawner : MonoBehaviour
{
    [SerializeField] private Ball ballPrefab;
    [SerializeField] private Transform ballParent;
    [SerializeField] private float ballSpawnOffset = 0.45f;

    [SerializeField] private Vector3 topLeftCorner;
    [SerializeField] private Vector3 bottomRightCorner;
    [SerializeField] private Vector3 cellSpawnOffset = new Vector3(3f, 0, 3f);
    [SerializeField] private GameCell cellPrefab;
    [SerializeField] private Transform cellParent;

    private List<GameCell> cells = new List<GameCell>();
    private BoxCollider bc;

    void Start()
    {
        bc = GetComponent<BoxCollider>();
        CreateGameField();
        InstantiateBallsOnStart();
        float colliderBounds = topLeftCorner.z + bottomRightCorner.x + cellSpawnOffset.x;
        bc.size = new Vector3(colliderBounds, 0.25f, colliderBounds);
    }

    private void CreateGameField()
    {
        GameCell cell;
        Vector3 cellSpawnPosition = topLeftCorner;
        int cellSpawnAmount = (int)((topLeftCorner.z - bottomRightCorner.z) / cellSpawnOffset.z + 1);

        for (int i = 0; i < cellSpawnAmount; i++)
        {
            if(i > 0)
            {
                cellSpawnPosition.z -= cellSpawnOffset.z;
            }
            cellSpawnPosition.x = topLeftCorner.x;

            for (int j = 0; j < cellSpawnAmount; j++)
            {
                if (j > 0)
                {
                    cellSpawnPosition.x += cellSpawnOffset.x;
                }
                cell = Instantiate(cellPrefab, cellSpawnPosition, Quaternion.identity, cellParent);
                cells.Add(cell);
            }
        }
    }

    private void InstantiateBallsOnStart()
    {
        foreach (var cell in cells)
        {
            Instantiate(ballPrefab, cell.transform.position + new Vector3(0, ballSpawnOffset), ballPrefab.transform.rotation, ballParent);
        }
    }
}
