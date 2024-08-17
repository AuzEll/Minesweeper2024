using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour {
    //[SerializeField] private Color baseColor, offsetColor;
    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private GameObject highlight;
 
    public void IsBackground() {
        renderer.color = new Color(0.5f, 0.5f, 0.5f, 1f);
    }
 
    void OnMouseEnter() {
        highlight.SetActive(true);
        Debug.Log("yes");
    }
 
    void OnMouseExit()
    {
        highlight.SetActive(false);
        Debug.Log("no");
    }
}
