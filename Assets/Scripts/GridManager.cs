using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width = 9;
    public int height = 9;
    public int mines = 10;
    public GameObject[,] grid;

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
                PlaceMines();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PlaceMines()
    {
        int x = UnityEngine.Random.Range(0, width);
        int y = UnityEngine.Random.Range(0, height);

        if (grid[x, y] == null)
        {
            GameObject empty = new GameObject();
            grid[x, y] = empty;

            Debug.Log(x + " " + y);
        }
        else
        {
            PlaceMines();
        }
    }
}
