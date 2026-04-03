using UnityEngine;
using System.Collections.Generic;

public class FlowManager : MonoBehaviour
{
    [Header("Puzzle Settings")]
    public int totalPairsToConnect;
    public GameObject linePrefab;
    public PuzzleTrigger puzzleTrigger;

    public float lineOffset = 0.6f;

    // We now use strings (Hex Codes) instead of Colors as dictionary keys to prevent floating-point mismatch bugs
    private Dictionary<string, GameObject> activeLineObjects = new Dictionary<string, GameObject>();
    private Dictionary<string, List<FlowCell>> activePaths = new Dictionary<string, List<FlowCell>>();
    private HashSet<string> completedColors = new HashSet<string>();

    private Color currentColor;
    private List<FlowCell> currentPath;
    private LineRenderer activeLine;

    // Helper method to convert Unity Colors into safe, exact string keys
    private string GetColorKey(Color color)
    {
        return ColorUtility.ToHtmlStringRGBA(color);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartDrawing();
        }
        if (Input.GetMouseButton(0) && activeLine != null && currentPath != null)
        {
            ContinueDrawing();
        }
        if (Input.GetMouseButtonUp(0))
        {
            EndDrawing();
        }
    }

    void StartDrawing()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            FlowCell cell = hit.collider.GetComponent<FlowCell>();
            if (cell == null) return;

            string cellColorKey = GetColorKey(cell.cellColor);

            if (cell.isNode)
            {
                currentColor = cell.cellColor;
                ClearLine(currentColor);
                StartNewLine(cell);
            }
            else if (cell.isOccupied && activeLineObjects.ContainsKey(cellColorKey))
            {
                currentColor = cell.cellColor;
                string colorKey = GetColorKey(currentColor);

                currentPath = activePaths[colorKey];
                activeLine = activeLineObjects[colorKey].GetComponent<LineRenderer>();

                int index = currentPath.IndexOf(cell);
                if (index != -1)
                {
                    if (completedColors.Contains(colorKey)) completedColors.Remove(colorKey);

                    for (int i = currentPath.Count - 1; i > index; i--)
                    {
                        if (!currentPath[i].isNode)
                        {
                            currentPath[i].ClearCell();
                        }
                        currentPath.RemoveAt(i);
                    }
                    activeLine.positionCount = currentPath.Count;
                }
            }
        }
    }

    void StartNewLine(FlowCell startCell)
    {
        currentColor = startCell.cellColor;
        string colorKey = GetColorKey(currentColor);

        GameObject lineObj = Instantiate(linePrefab, transform);
        activeLine = lineObj.GetComponent<LineRenderer>();
        activeLine.startColor = activeLine.endColor = currentColor;

        currentPath = new List<FlowCell>();
        activePaths[colorKey] = currentPath;
        activeLineObjects[colorKey] = lineObj;

        AddCellToPath(startCell);
    }

    void ContinueDrawing()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            FlowCell cell = hit.collider.GetComponent<FlowCell>();
            string colorKey = GetColorKey(currentColor);

            if (cell != null && IsAdjacent(cell, currentPath[currentPath.Count - 1]))
            {
                if (currentPath.Contains(cell))
                {
                    int index = currentPath.IndexOf(cell);
                    if (completedColors.Contains(colorKey)) completedColors.Remove(colorKey);

                    for (int i = currentPath.Count - 1; i > index; i--)
                    {
                        if (!currentPath[i].isNode) currentPath[i].ClearCell();
                        currentPath.RemoveAt(i);
                    }
                    activeLine.positionCount = currentPath.Count;
                    return;
                }

                if (cell.isNode && GetColorKey(cell.cellColor) != colorKey)
                {
                    return;
                }

                if (cell.isOccupied && GetColorKey(cell.cellColor) != colorKey)
                {
                    ClearLine(cell.cellColor);
                }

                FlowCell lastCell = currentPath[currentPath.Count - 1];
                if (lastCell.isNode && GetColorKey(lastCell.cellColor) == colorKey && currentPath.Count > 1)
                {
                    return;
                }

                if (!cell.isOccupied || (cell.isNode && GetColorKey(cell.cellColor) == colorKey))
                {
                    AddCellToPath(cell);

                    if (cell.isNode && GetColorKey(cell.cellColor) == colorKey)
                    {
                        completedColors.Add(colorKey);
                        CheckWinCondition();
                    }
                }
            }
        }
    }

    void AddCellToPath(FlowCell cell)
    {
        currentPath.Add(cell);
        cell.SetColor(currentColor);
        activeLine.positionCount = currentPath.Count;

        Vector3 linePos = cell.transform.position - (transform.forward * lineOffset);
        activeLine.SetPosition(currentPath.Count - 1, linePos);
    }

    void EndDrawing()
    {
        activeLine = null;
        currentPath = null;
    }

    void ClearLine(Color colorToClear)
    {
        string colorKey = GetColorKey(colorToClear);

        if (activeLineObjects.ContainsKey(colorKey))
        {
            Destroy(activeLineObjects[colorKey]);
            activeLineObjects.Remove(colorKey);
        }

        if (activePaths.ContainsKey(colorKey))
        {
            activePaths.Remove(colorKey);
        }

        completedColors.Remove(colorKey);

        FlowCell[] allCells = GetComponentsInChildren<FlowCell>();
        foreach (FlowCell cell in allCells)
        {
            // Checks using the precise Hex string instead of float comparison
            if (GetColorKey(cell.cellColor) == colorKey && !cell.isNode)
            {
                cell.ClearCell();
            }
        }
    }

    bool IsAdjacent(FlowCell a, FlowCell b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) == 1;
    }

    void CheckWinCondition()
    {
        if (completedColors.Count >= totalPairsToConnect)
        {
            Debug.Log("Puzzle Complete!");
            Invoke("FinishGame", 1.5f);
        }
    }

    void FinishGame()
    {
        if (puzzleTrigger != null)
        {
            puzzleTrigger.MarkAsSolved();
        }
    }

    public void CancelActiveDrawing()
    {
        if (activeLine != null && currentPath != null)
        {
            foreach (FlowCell cell in currentPath)
            {
                if (!cell.isNode) cell.ClearCell();
            }
            Destroy(activeLine.gameObject);

            activeLine = null;
            currentPath = null;
        }
    }
}