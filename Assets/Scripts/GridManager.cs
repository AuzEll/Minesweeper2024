using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GridManager : MonoBehaviour
{
    public int width = 9;
    public int height = 9;
    public int mines = 10;
    public string[,] gridArray;

    [SerializeField] private Cell tilePrefab;
    [SerializeField] private GameObject minePrefab;
    [SerializeField] private GameObject flagPrefab;
    [SerializeField] private Transform cam;
    private int toBeRevealed;
    private bool exploded;
    private bool firstTileRemoved;
    private int flags;
    public int Flags
    {
        get { return flags; }
        set { flags = value; }
    }

    private const float cameraZoomMultiplier = 11f / 16f;

    private Dictionary<int, Color> digitColours = new Dictionary<int, Color>()
    {
        { 1, new Color(0f, 0f, 1f, 1f) },
        { 2, new Color(0f, 1f, 0f, 1f) },
        { 3, new Color(1f, 0f, 0f, 1f) },
        { 4, new Color(0f, 0f, 0.5f, 1f) },
        { 5, new Color(0.5f, 0f, 0f, 1f) },
        { 6, new Color(0.7f, 0.85f, 0.9f, 1f) },
        { 7, new Color(0f, 0f, 0f, 1f) },
        { 8, new Color(1f, 1f, 1f, 0.8f) }
    };

    // Start is called before the first frame update
    public void Start()
    {
        gridArray = new string[width, height];

        toBeRevealed = width * height - mines;
        exploded = false;
        firstTileRemoved = false;
        flags = mines;

        foreach (GameObject tile in GameObject.FindGameObjectsWithTag("Tile")) Destroy(tile);
        foreach (GameObject tile in GameObject.FindGameObjectsWithTag("Mine")) Destroy(tile);
        foreach (GameObject tile in GameObject.FindGameObjectsWithTag("Flag")) Destroy(tile);

        GenerateGridLayer(true);
        GenerateGridLayer(false);

        if (toBeRevealed < 0)
        {
            Debug.Log("ERROR: There are more mines than cells in the grid");
            foreach (GameObject tile in GameObject.FindGameObjectsWithTag("Tile")) tile.GetComponent<BoxCollider2D>().enabled = false;
        }

        cam.transform.position = new Vector3((float)width / 2 - 0.5f, (float)height / 2, -10);
        cam.GetComponent<Camera>().orthographicSize = (float)height * cameraZoomMultiplier;
    }

    // Update is called once per frame
    void Update()
    {
        //Check if bomb cover tile is revealed
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                GameObject coverTile = GameObject.Find("Cover (" + x + ", " + y + ")");
                if (coverTile == null)
                {
                    if (!firstTileRemoved) GenerateContents(x, y);

                    if (gridArray[x, y] == null) ClearSurroundingTiles(x, y);

                    if (gridArray[x, y] == "Mine" && !exploded) RevealMines();
                }
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

    void GenerateContents(int firstX, int firstY)
    {
        Dictionary<Vector2Int, string> adjacentTilesToFirst = checkAdjacentTiles(new Vector2Int(firstX, firstY));

        for (int i = 0; i < mines; i++) PlaceMine(new Vector2Int(firstX, firstY), adjacentTilesToFirst);

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                Dictionary<Vector2Int, string> adjacentTiles = checkAdjacentTiles(new Vector2Int(x, y));
                int mineCount = 0;

                if (gridArray[x, y] == "Mine")
                {
                    var spawnedMine = Instantiate(minePrefab, new Vector3(x, y), Quaternion.identity);
                    spawnedMine.name = "Mine";
                    spawnedMine.transform.parent = transform;
                }
                else
                {
                    foreach (var adjacentTileValue in adjacentTiles.Values) if (adjacentTileValue == "Mine") mineCount++;

                    if (mineCount > 0)
                    {
                        gridArray[x, y] = mineCount.ToString();

                        GameObject backTile = GameObject.Find("Tile (" + x + ", " + y + ")");
                        TMP_Text backTileText = backTile.transform.FindChild("text").GetComponent<TMP_Text>();

                        backTileText.text = mineCount.ToString();
                        backTileText.color = digitColours[mineCount];
                    }
                }
            }
        }

        firstTileRemoved = true;
    }

    void PlaceMine(Vector2Int cellToAvoid, Dictionary<Vector2Int, string> adjacentsToAvoid)
    {
        int oobs = 0;
        int x = UnityEngine.Random.Range(0, gridArray.GetLength(0));
        int y = UnityEngine.Random.Range(0, gridArray.GetLength(1));
        Vector2Int thisCell = new Vector2Int(x, y);

        foreach (var value in adjacentsToAvoid.Values) if (value == "OOB") oobs++;

        if (toBeRevealed >= 9 || (toBeRevealed >= 6 && oobs == 3) || (toBeRevealed >= 4 && oobs == 5))
        {
            while (adjacentsToAvoid.ContainsKey(thisCell) || thisCell == cellToAvoid || gridArray[x, y] == "Mine")
            {
                x = UnityEngine.Random.Range(0, gridArray.GetLength(0));
                y = UnityEngine.Random.Range(0, gridArray.GetLength(1));
                thisCell = new Vector2Int(x, y);
            }
        }
        else
        {
            while (thisCell == cellToAvoid || gridArray[x, y] == "Mine")
            {
                x = UnityEngine.Random.Range(0, gridArray.GetLength(0));
                y = UnityEngine.Random.Range(0, gridArray.GetLength(1));
                thisCell = new Vector2Int(x, y);
            }
        }

        gridArray[x, y] = "Mine";
    }

    void ClearSurroundingTiles(int x, int y)
    {
        Dictionary<Vector2Int, string> adjacentTiles = checkAdjacentTiles(new Vector2Int(x, y));

        foreach (var adjacentTile in adjacentTiles)
        {
            GameObject coverTile = GameObject.Find("Cover (" + adjacentTile.Key.x + ", " + adjacentTile.Key.y + ")");
            if (adjacentTile.Value != "Mine" && coverTile != null) Destroy(coverTile);
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
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                GameObject coverTile = GameObject.Find("Cover (" + x + ", " + y + ")");

                if (coverTile != null)
                {
                    coverTile.GetComponent<BoxCollider2D>().enabled = false;
                    if (gridArray[x, y] == "Mine" && GameObject.Find("Flag (" + x + ", " + y + ")") == null) Destroy(coverTile);
                }
            }
        }
        exploded = true;
    }

    Dictionary<Vector2Int, string> checkAdjacentTiles(Vector2Int tile)
    {
        Dictionary<Vector2Int, string> adjacentTiles = new Dictionary<Vector2Int, string>();

        if (0 <= tile.x - 1) adjacentTiles[new Vector2Int(tile.x - 1, tile.y)] = gridArray[tile.x - 1, tile.y];
        else adjacentTiles[new Vector2Int(tile.x - 1, tile.y)] = "OOB";

        if (0 <= tile.x - 1 && 0 <= tile.y - 1) adjacentTiles[new Vector2Int(tile.x - 1, tile.y - 1)] = gridArray[tile.x - 1, tile.y - 1];
        else adjacentTiles[new Vector2Int(tile.x - 1, tile.y - 1)] = "OOB";

        if (0 <= tile.x - 1 && tile.y + 1 < gridArray.GetLength(1)) adjacentTiles[new Vector2Int(tile.x - 1, tile.y + 1)] = gridArray[tile.x - 1, tile.y + 1];
        else adjacentTiles[new Vector2Int(tile.x - 1, tile.y + 1)] = "OOB";

        if (0 <= tile.y - 1) adjacentTiles[new Vector2Int(tile.x, tile.y - 1)] = gridArray[tile.x, tile.y - 1];
        else adjacentTiles[new Vector2Int(tile.x, tile.y - 1)] = "OOB";

        if (tile.y + 1 < gridArray.GetLength(1)) adjacentTiles[new Vector2Int(tile.x, tile.y + 1)] = gridArray[tile.x, tile.y + 1];
        else adjacentTiles[new Vector2Int(tile.x, tile.y + 1)] = "OOB";

        if (tile.x + 1 < gridArray.GetLength(0)) adjacentTiles[new Vector2Int(tile.x + 1, tile.y)] = gridArray[tile.x + 1, tile.y];
        else adjacentTiles[new Vector2Int(tile.x + 1, tile.y)] = "OOB";

        if (tile.x + 1 < gridArray.GetLength(0) && 0 <= tile.y - 1) adjacentTiles[new Vector2Int(tile.x + 1, tile.y - 1)] = gridArray[tile.x + 1, tile.y - 1];
        else adjacentTiles[new Vector2Int(tile.x + 1, tile.y - 1)] = "OOB";

        if (tile.x + 1 < gridArray.GetLength(0) && tile.y + 1 < gridArray.GetLength(1)) adjacentTiles[new Vector2Int(tile.x + 1, tile.y + 1)] = gridArray[tile.x + 1, tile.y + 1];
        else adjacentTiles[new Vector2Int(tile.x + 1, tile.y + 1)] = "OOB";

        return adjacentTiles;
    }
}
