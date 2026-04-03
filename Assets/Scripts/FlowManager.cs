using UnityEngine;
using System.Collections.Generic;

public class FlowManager : MonoBehaviour
{
    public int totalPairsToConnect;
    public GameObject linePrefab;
    public PuzzleTrigger puzzleTrigger;
    public float lineOffset = 0.6f;

    private int pairsConnected = 0;
    private Color currentColor;
    private List<FlowCell> currentPath = new List<FlowCell>();
    private LineRenderer activeLine;
    private Dictionary<Color, GameObject> completedLines = new Dictionary<Color, GameObject>();

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) StartDrawing();
        if (Input.GetMouseButton(0) && activeLine != null) ContinueDrawing();
        if (Input.GetMouseButtonUp(0) && activeLine != null) EndDrawing();
    }

    void StartDrawing()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            FlowCell startCell = hit.collider.GetComponent<FlowCell>();

            if (startCell != null && startCell.isNode)
            {
                currentColor = startCell.cellColor;

                if (completedLines.ContainsKey(currentColor))
                {
                    Destroy(completedLines[currentColor]);
                    completedLines.Remove(currentColor);
                    pairsConnected--;
                }

                GameObject lineObj = Instantiate(linePrefab, transform);
                activeLine = lineObj.GetComponent<LineRenderer>();
                activeLine.startColor = activeLine.endColor = currentColor;

                AddCellToPath(startCell);
            }
        }
    }

    void ContinueDrawing()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            FlowCell cell = hit.collider.GetComponent<FlowCell>();

            if (cell != null && IsAdjacent(cell, currentPath[currentPath.Count - 1]) && !currentPath.Contains(cell) && !cell.isOccupied)
            {
                AddCellToPath(cell);
            }
        }
    }

    void AddCellToPath(FlowCell cell)
    {
        currentPath.Add(cell);
        cell.SetColor(currentColor);
        activeLine.positionCount = currentPath.Count;

        // Pushes the line slightly towards the camera so it doesn't hide inside the 3D cube
        Vector3 linePos = cell.transform.position - (Camera.main.transform.forward * lineOffset);
        activeLine.SetPosition(currentPath.Count - 1, linePos);
    }

    void EndDrawing()
    {
        FlowCell lastCell = currentPath[currentPath.Count - 1];

        if (lastCell.isNode && lastCell.cellColor == currentColor && currentPath.Count > 1)
        {
            completedLines.Add(currentColor, activeLine.gameObject);
            pairsConnected++;
            CheckWinCondition();
        }
        else
        {
            foreach (FlowCell cell in currentPath) cell.ClearCell();
            Destroy(activeLine.gameObject);
        }

        activeLine = null;
        currentPath.Clear();
    }

    bool IsAdjacent(FlowCell a, FlowCell b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) == 1;
    }

    void CheckWinCondition()
    {
        if (pairsConnected >= totalPairsToConnect)
        {
            Debug.Log("Puzzle Complete!");
            Invoke("FinishGame", 1.5f);
        }
    }

    void FinishGame()
    {
        puzzleTrigger.ExitPuzzle();
    }
}