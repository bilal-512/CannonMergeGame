using UnityEngine;

public class Cell : MonoBehaviour
{
    [HideInInspector] public int gridX;
    [HideInInspector] public int gridY;

    private Cannon occupyingCannon = null;

    // Returns true if nothing is on this cell
    public bool IsEmpty()
    {
        return occupyingCannon == null;
    }

    // Place a cannon ON this cell
    public void PlaceCannon(Cannon cannon)
    {
        occupyingCannon = cannon;
        cannon.transform.position = transform.position;
        cannon.currentCell = this;
    }

    // Remove cannon reference (cannon moved away)
    public void ClearCannon()
    {
        occupyingCannon = null;
    }

    // Get the cannon sitting on this cell
    public Cannon GetCannon()
    {
        return occupyingCannon;
    }
}