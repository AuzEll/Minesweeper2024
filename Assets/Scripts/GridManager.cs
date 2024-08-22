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

        if (toBeRevealed < 0)
        {
            Debug.Log("ERROR: There are more mines than cells in the grid");
        }

        GenerateGridLayer(true);
        //GenerateContents();
        GenerateGridLayer(false);

        cam.transform.position = new Vector3((float)width / 2 - 0.5f, (float)height / 2 - 0.5f, -10);
    }

    void firstClick(int x, int y)
    {
        if (toBeRevealed >= 1) for (int i = 0; i < mines; i++) PlaceMine(new Vector2(x, y));
        //else if 
        GenerateContents();
        Debug.Log("true");

        firstTileRemoved = true;
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
                    if (!firstTileRemoved) firstClick(x, y);

                    if (gridArray[x, y] == null) ClearSurroundingTiles(x, y, gridArray.GetLength(0), gridArray.GetLength(1));

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

    void GenerateContents()
    {
        int mineCount;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                mineCount = 0;
                if (gridArray[x, y] == "Mine")
                {
                    var spawnedMine = Instantiate(minePrefab, new Vector3(x, y), Quaternion.identity);
                    spawnedMine.name = "Mine";
                    spawnedMine.transform.parent = transform;
                }
                else
                {
                    if (0 <= x - 1 && gridArray[x - 1, y] == "Mine") mineCount++;
                    if (0 <= x - 1 && 0 <= y - 1 && gridArray[x - 1, y - 1] == "Mine") mineCount++;
                    if (0 <= x - 1 && y + 1 <= height - 1 && gridArray[x - 1, y + 1] == "Mine") mineCount++;
                    if (0 <= y - 1 && gridArray[x, y - 1] == "Mine") mineCount++;
                    if (y + 1 <= height - 1 && gridArray[x, y + 1] == "Mine") mineCount++;
                    if (x + 1 <= width - 1 && gridArray[x + 1, y] == "Mine") mineCount++;
                    if (x + 1 <= width - 1 && 0 <= y - 1 && gridArray[x + 1, y - 1] == "Mine") mineCount++;
                    if (x + 1 <= width - 1 && y + 1 <= height - 1 && gridArray[x + 1, y + 1] == "Mine") mineCount++;

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
    }

    void PlaceMine(Vector2 cellToAvoid)
    {
        int x = UnityEngine.Random.Range(0, gridArray.GetLength(0));
        int y = UnityEngine.Random.Range(0, gridArray.GetLength(1));
        Vector2 thisCell = new Vector2(x, y);

        if (gridArray[x, y] == null && thisCell != cellToAvoid) gridArray[x, y] = "Mine";
        else PlaceMine(cellToAvoid);
    }

    void ClearSurroundingTiles(int x, int y, int gridWidth, int gridHeight)
    {
        GameObject coverTile = GameObject.Find("Cover (" + (x - 1) + ", " + y + ")");
        if (0 <= x - 1 && gridArray[x - 1, y] != "Mine" && coverTile != null) Destroy(coverTile);

        coverTile = GameObject.Find("Cover (" + (x - 1) + ", " + (y - 1) + ")");
        if (0 <= x - 1 && 0 <= y - 1 && gridArray[x - 1, y - 1] != "Mine" && coverTile != null) Destroy(coverTile);

        coverTile = GameObject.Find("Cover (" + (x - 1) + ", " + (y + 1) + ")");
        if (0 <= x - 1 && y + 1 <= height - 1 && gridArray[x - 1, y + 1] != "Mine" && coverTile != null) Destroy(coverTile);

        coverTile = GameObject.Find("Cover (" + x + ", " + (y - 1) + ")");
        if (0 <= y - 1 && gridArray[x, y - 1] != "Mine" && coverTile != null) Destroy(coverTile);

        coverTile = GameObject.Find("Cover (" + x + ", " + (y + 1) + ")");
        if (y + 1 <= gridHeight - 1 && gridArray[x, y + 1] != "Mine" && coverTile != null) Destroy(coverTile);

        coverTile = GameObject.Find("Cover (" + (x + 1) + ", " + y + ")");
        if (x + 1 <= gridWidth - 1 && gridArray[x + 1, y] != "Mine" && coverTile != null) Destroy(coverTile);

        coverTile = GameObject.Find("Cover (" + (x + 1) + ", " + (y - 1) + ")");
        if (x + 1 <= width - 1 && 0 <= y - 1 && gridArray[x + 1, y - 1] != "Mine" && coverTile != null) Destroy(coverTile);

        coverTile = GameObject.Find("Cover (" + (x + 1) + ", " + (y + 1) + ")");
        if (x + 1 <= width - 1 && y + 1 <= height - 1 && gridArray[x + 1, y + 1] != "Mine" && coverTile != null) Destroy(coverTile);
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
}
