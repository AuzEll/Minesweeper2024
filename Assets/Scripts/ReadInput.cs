using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadInput : MonoBehaviour
{
    private GridManager gridObject;
    private int widthInput;
    private int heightInput;
    private int minesInput;

    // Start is called before the first frame update
    void Start()
    {
        gridObject = this.GetComponent<GridManager>();
        widthInput = gridObject.width;
        heightInput = gridObject.height;
        minesInput = gridObject.mines;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ReadWidthInput(InputField input)
    {
        widthInput = int.Parse(input.text);
        gridObject.width = widthInput;
    }

    public void ReadHeightInput(InputField input)
    {
        heightInput = int.Parse(input.text);
        gridObject.height = heightInput;
    }

    public void ReadMinesInput(InputField input)
    {
        minesInput = int.Parse(input.text);
        gridObject.mines = minesInput;
    }
}
