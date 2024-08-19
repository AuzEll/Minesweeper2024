using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width = 9;
    public int height = 9;
    public int mines = 10;
    public string[,] minesGrid;

    [SerializeField] private Cell tilePrefab;
    [SerializeField] private GameObject minePrefab;
    [SerializeField] private GameObject flagPrefab;
    [SerializeField] private Transform cam;
    private int toBeRevealed;
    private bool exploded;
    private int flags;
    public int Flags
    {
        get { return flags; }
        set { flags = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        minesGrid = new string[width, height];

        toBeRevealed = width * height - mines;
        exploded = false;
        flags = mines;

        if (toBeRevealed < 0)
        {
            Debug.Log("ERROR: There are more mines than cells in the grid");
        }
        else
        {
            for (int i = 0; i < mines; i++) PlaceMine();
        }

        GenerateGridLayer(true);
        GenerateContents();
        GenerateGridLayer(false);

        cam.transform.position = new Vector3((float)width / 2 - 0.5f, (float)height / 2 - 0.5f, -10);
    }

    // Update is called once per frame
    void Update()
    {
        //Check if bomb cover tile is revealed
        for (int x = 0; x < minesGrid.GetLength(0); x++)
        {
            for (int y = 0; y < minesGrid.GetLength(1); y++)
            {
                GameObject coverTile = GameObject.Find("Cover (" + x + ", " + y + ")");
                if (coverTile == null && minesGrid[x, y] == "Mine" && !exploded) RevealMines();
            }
        }
    }

    void GenerateGridLayer(bool background)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var spawnedTile = Instantiate(tilePrefab, new Vector3(x, y), Quaternion.identity);
                spawnedTile.IsBackground(background);
                spawnedTile.name = background ? "Tile (" + x + ", " + y + ")" : "Cover (" + x + ", " + y + ")";
                spawnedTile.transform.parent = transform;
            }
        }

    }

    void GenerateContents()
    {
        int mineCount;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                mineCount = 0;
                if (minesGrid[x, y] == "Mine")
                {
                    var spawnedMine = Instantiate(minePrefab, new Vector3(x, y), Quaternion.identity);
                    spawnedMine.name = "Mine";
                    spawnedMine.transform.parent = transform;
                }
                else
                {
                    if (0 <= x - 1 && x - 1 <= width - 1 && 0 <= y && y <= height - 1 && minesGrid[x - 1, y] == "Mine") mineCount++;
                    if (0 <= x - 1 && x - 1 <= width - 1 && 0 <= y - 1 && y - 1 <= height - 1 && minesGrid[x - 1, y - 1] == "Mine") mineCount++;
                    if (0 <= x - 1 && x - 1 <= width - 1 && 0 <= y + 1 && y + 1 <= height - 1 && minesGrid[x - 1, y + 1] == "Mine") mineCount++;
                    if (0 <= x && x <= width - 1 && 0 <= y - 1 && y - 1 <= height - 1 && minesGrid[x, y - 1] == "Mine") mineCount++;
                    if (0 <= x && x <= width - 1 && 0 <= y + 1 && y + 1 <= height - 1 && minesGrid[x, y + 1] == "Mine") mineCount++;
                    if (0 <= x + 1 && x + 1 <= width - 1 && 0 <= y && y <= height - 1 && minesGrid[x + 1, y] == "Mine") mineCount++;
                    if (0 <= x + 1 && x + 1 <= width - 1 && 0 <= y - 1 && y - 1 <= height - 1 && minesGrid[x + 1, y - 1] == "Mine") mineCount++;
                    if (0 <= x + 1 && x + 1 <= width - 1 && 0 <= y + 1 && y + 1 <= height - 1 && minesGrid[x + 1, y + 1] == "Mine") mineCount++;
                    minesGrid[x, y] = mineCount.ToString();
                }
            }
        }
    }

    void PlaceMine()
    {
        int x = UnityEngine.Random.Range(0, width);
        int y = UnityEngine.Random.Range(0, height);

        if (minesGrid[x, y] == null)
        {
            minesGrid[x, y] = "Mine";
        }
        else
        {
            PlaceMine();
        }
    }

    public void PlaceRemoveFlag(int x, int y, bool hasFlag)
    {
        Cell coverTileCell = GameObject.Find("Cover (" + x + ", " + y + ")").GetComponent<Cell>();

        if (!hasFlag && flags > 0)
        {
            var spawnedFlag = Instantiate(flagPrefab, new Vector3(x, y), Quaternion.identity);
            spawnedFlag.name = "Flag (" + x + ", " + y + ")";
            flags -= 1;
            coverTileCell.HasFlag = !hasFlag;
        }
        else if (hasFlag)
        {
            Destroy(GameObject.Find("Flag (" + x + ", " + y + ")"));
            flags += 1;
            coverTileCell.HasFlag = !hasFlag;
        }
    }

    void RevealMines()
    {
        for (int x = 0; x < minesGrid.GetLength(0); x++)
        {
            for (int y = 0; y < minesGrid.GetLength(1); y++)
            {
                GameObject coverTile = GameObject.Find("Cover (" + x + ", " + y + ")");

                if (coverTile != null)
                {
                    coverTile.GetComponent<BoxCollider2D>().enabled = false;
                    if (minesGrid[x, y] == "Mine" && GameObject.Find("Flag (" + x + ", " + y + ")") == null) Destroy(coverTile);
                }
            }
        }
        exploded = true;
    }
}
