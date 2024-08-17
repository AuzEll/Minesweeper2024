using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width = 9;
    public int height = 9;
    public int mines = 10;
    public GameObject[,] grid;

    [SerializeField] private Cell tilePrefab;
    [SerializeField] private Transform cam;
    private int toBeRevealed;

    // Start is called before the first frame update
    void Start()
    {
        grid = new GameObject[width, height];

        toBeRevealed = width * height - mines;

        if (toBeRevealed < 0)
        {
            Debug.Log("ERROR: There are more mines than cells in the grid");
        }
        else
        {
            for (int i = 0; i < mines; i++)
            {
                PlaceMine();
            }
        }

        GenerateGrid(true);
        GenerateGrid(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PlaceMine()
    {
        int x = UnityEngine.Random.Range(0, width);
        int y = UnityEngine.Random.Range(0, height);

        if (grid[x, y] == null)
        {
            GameObject mine = new GameObject();
            grid[x, y] = mine;
        }
        else
        {
            PlaceMine();
        }
    }

    void GenerateGrid(bool background)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var spawnedTile = Instantiate(tilePrefab, new Vector3(x, y), Quaternion.identity);
                spawnedTile.name = "Tile (" + x + ", " + y + ")";
                if (background) spawnedTile.IsBackground();
            }
        }

        cam.transform.position = new Vector3((float)width/2 - 0.5f, (float)height/2 - 0.5f, -10);
    }
}
