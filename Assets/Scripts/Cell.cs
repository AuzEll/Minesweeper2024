using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField] private SpriteRenderer renderer;
    private GridManager gridObject;
    private Vector2Int cellValue;
    private bool hasFlag;
    public bool HasFlag
    {
        get { return hasFlag; }
        set { hasFlag = value; }
    }

    void Start()
    {
        gridObject = GameObject.Find("Grid").GetComponent<GridManager>();
        cellValue = new Vector2Int((int)this.transform.position.x, (int)this.transform.position.y);
        hasFlag = false;
    }

    public void IsBackground(bool background)
    {
        if (background)
        {
            renderer.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            renderer.sortingOrder = -1;
            this.GetComponent<BoxCollider2D>().enabled = false;
        }
        else renderer.sortingOrder = 1;
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(0) && !hasFlag)
        {
            Destroy(this.gameObject);
        }

        if (Input.GetMouseButtonUp(1))
        {
            int x = (int)this.transform.position.x;
            int y = (int)this.transform.position.y;
            gridObject.PlaceRemoveFlag(cellValue.x, cellValue.y, hasFlag);
        }
    }
}
