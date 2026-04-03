using UnityEngine;

public class FlowCell : MonoBehaviour
{
    public int x, y;
    public Color cellColor = Color.clear;
    public bool isNode = false;

    [HideInInspector] public bool isOccupied = false;

    private Renderer rend;
    private Color defaultColor = Color.white;

    void Awake()
    {
        if (isNode)
        {
            isOccupied = true;
        }

        rend = GetComponent<Renderer>();
        if (isNode && cellColor != Color.clear)
        {
            rend.material.color = cellColor;
        }
        else
        {
            rend.material.color = defaultColor;
        }
    }

    public void SetColor(Color newColor)
    {
        cellColor = newColor;
        rend.material.color = newColor;
        isOccupied = (newColor != Color.clear);
    }

    public void ClearCell()
    {
        if (!isNode)
        {
            cellColor = Color.clear;
            rend.material.color = defaultColor;
            isOccupied = false;
        }
    }
}