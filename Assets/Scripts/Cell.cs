using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public Vector3Int cellValue;
    public bool isBomb;

    [SerializeField] private SpriteRenderer renderer;

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
        if (Input.GetMouseButtonUp(0)) Destroy(this.gameObject);
        if (Input.GetMouseButtonUp(1)) Debug.Log(this.name);
    }
}
