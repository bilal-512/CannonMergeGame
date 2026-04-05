using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    // Singleton — lets any script access GridManager.Instance
    public static GridManager Instance;

    [Header("Grid Settings")]
    public int columns = 5;
    public int rows = 4;
    public float cellSize = 1.6f;
    public GameObject cellPrefab; // Drag your Cell prefab here in Inspector

    private Cell[,] grid; // 2D array storing all cells

    void Awake()
    {
        Instance = this; // Register singleton
    }

    void Start()
    {
        BuildGrid();
    }

void BuildGrid()
{
    grid = new Cell[columns, rows];

    float screenWidth  = Camera.main.orthographicSize * 2f * Camera.main.aspect;
    float screenHeight = Camera.main.orthographicSize * 2f;

    // Auto calculate cell size to fit screen width
    float autoSize   = (screenWidth * 0.85f) / columns;
    cellSize         = autoSize;
    float visualScale = autoSize * 0.92f;
    float bottomMargin = screenHeight * 0.08f; // 8% from bottom edge
    float gridHeight   = (rows - 1) * cellSize;

    // Bottom of grid sits just above the margin
    float startY = -screenHeight / 2f + bottomMargin + cellSize * 0.5f;
    float startX = -(columns - 1) * cellSize / 2f;

    for (int x = 0; x < columns; x++)
    {
        for (int y = 0; y < rows; y++)
        {
            Vector3 pos = new Vector3(
                startX + x * cellSize,
                startY + y * cellSize,
                0
            );

            GameObject cellObj = Instantiate(cellPrefab, pos, Quaternion.identity);
            cellObj.transform.parent = transform;
            cellObj.name = $"Cell ({x},{y})";
            cellObj.transform.localScale = new Vector3(visualScale, visualScale, 1f);

            Cell cell = cellObj.GetComponent<Cell>();
            cell.gridX = x;
            cell.gridY = y;
            grid[x, y] = cell;
        }
    }

    Debug.Log($"Grid built — cellSize={cellSize:F2} startX={startX:F2} startY={startY:F2}");
}
    // Returns a random cell that has no cannon on it
    public Cell GetRandomEmptyCell()
    {
        List<Cell> emptyCells = new List<Cell>();

        foreach (Cell cell in grid)
        {
            if (cell.IsEmpty())
                emptyCells.Add(cell);
        }

        if (emptyCells.Count == 0)
        {
            Debug.Log("No empty cells available!");
            return null;
        }

        return emptyCells[Random.Range(0, emptyCells.Count)];
    }

    // Get a specific cell by its grid coordinates
    public Cell GetCellAt(int x, int y)
    {
        if (x < 0 || x >= columns || y < 0 || y >= rows)
            return null;
        return grid[x, y];
    }
    // Returns the world X position of a specific column
public float GetColumnWorldX(int columnIndex)
{
    if (grid == null) return 0f;
    return grid[columnIndex, 0].transform.position.x;
}

// Returns the total number of columns
public int GetColumnCount() => columns;

// Returns world Y position just above the top of the grid
// Enemies will spawn here
public float GetGridTopY()
{
    // Returns the very top edge of the screen in world coordinates
    // Camera.main.orthographicSize = half screen height in world units
    return Camera.main.orthographicSize + 1f;
}
}