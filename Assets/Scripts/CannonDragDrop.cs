using UnityEngine;
using UnityEngine.EventSystems;

public class CannonDragDrop : MonoBehaviour,
    IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private Cannon cannon;
    private Cell originalCell;
    private Camera mainCam;
    private SpriteRenderer spriteRenderer;
    private Collider2D myCollider;
    private bool isDragging = false;

    void Start()
    {
        cannon = GetComponent<Cannon>();
        mainCam = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
        myCollider = GetComponent<Collider2D>();

        if (myCollider == null)
            myCollider = gameObject.AddComponent<BoxCollider2D>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        originalCell = cannon.currentCell;

        if (originalCell != null)
            originalCell.ClearCannon();

        // Disable own collider so it does NOT block raycasts while dragging
        myCollider.enabled = false;

        spriteRenderer.sortingOrder = 10;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        Vector3 worldPos = mainCam.ScreenToWorldPoint(eventData.position);
        worldPos.z = 0;
        transform.position = worldPos;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDragging) return;
        isDragging = false;
        spriteRenderer.sortingOrder = 1;

        // Re-enable collider before finding cell
        myCollider.enabled = true;

        // Find the closest cell to where we dropped
        Cell targetCell = GetClosestCell(eventData.position);

        if (targetCell == null)
        {
            Debug.Log("No cell found nearby — returning home");
            ReturnHome();
            return;
        }

        if (targetCell.IsEmpty())
        {
            // Cell is free — place cannon here
            Debug.Log($"Placed on empty cell ({targetCell.gridX},{targetCell.gridY})");
            targetCell.PlaceCannon(cannon);
        }
        else
        {
            Cannon otherCannon = targetCell.GetCannon();

            // Make sure it is a different cannon and same level
            if (otherCannon != null
                && otherCannon.gameObject != gameObject
                && otherCannon.level == cannon.level)
            {
                // MERGE — upgrade the one on the cell
                Debug.Log($"Merging! Level {cannon.level} + Level {otherCannon.level} = Level {cannon.level + 1}");
                otherCannon.Upgrade();

                // Clear original cell and destroy the dragged cannon
                if (originalCell != null)
                    originalCell.ClearCannon();

                Destroy(gameObject);
            }
            else
            {
                // Different level or same cannon — go back
                Debug.Log("Cannot place here — returning home");
                ReturnHome();
            }
        }
    }

    void ReturnHome()
    {
        if (originalCell != null)
            originalCell.PlaceCannon(cannon);
        else
            Destroy(gameObject);
    }

    // Instead of raycast, find the closest cell by distance
    // This is much more reliable than raycasting
    Cell GetClosestCell(Vector2 screenPos)
    {
        // Convert drop position to world position
        Vector3 worldPos = mainCam.ScreenToWorldPoint(screenPos);
        worldPos.z = 0;

        // Find all cells in the scene
        Cell[] allCells = FindObjectsByType<Cell>(FindObjectsSortMode.None);

        Cell closestCell = null;
        float closestDistance = Mathf.Infinity;

        foreach (Cell cell in allCells)
        {
            float dist = Vector2.Distance(worldPos, cell.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestCell = cell;
            }
        }

        float acceptRadius = GridManager.Instance.cellSize * 0.6f;

        if (closestDistance <= acceptRadius)
        {
            Debug.Log($"Closest cell: ({closestCell.gridX},{closestCell.gridY}) dist={closestDistance:F2} radius={acceptRadius:F2}");
            return closestCell;
        }

        Debug.Log($"Drop too far from any cell. Closest dist={closestDistance:F2} needed<{acceptRadius:F2}");
        return null;
    }
}