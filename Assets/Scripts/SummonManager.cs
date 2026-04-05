using UnityEngine;

public class SummonManager : MonoBehaviour
{
    [Header("References")]
    public GameObject cannonPrefab;

    public void OnSummonButtonPressed()
    {
        // Check if player can afford it
        if (!GameManager.Instance.HasEnoughCoins(GameManager.Instance.cannonCost))
        {
            UIManager.Instance.ShowMessage("Not enough coins! Need 10.");
            Debug.Log("Not enough coins to summon!");
            return;
        }

        // Bottom-left cell is always grid position (0, 0)
        // Find any random empty cell on the grid
        Cell randomCell = GridManager.Instance.GetRandomEmptyCell();

        if (randomCell == null)
        {
            UIManager.Instance.ShowMessage("Grid is full! Merge cannons to make space.");
            Debug.Log("No empty cells available!");
            return;
        }

        // Spend the coins
        GameManager.Instance.SpendCoins(GameManager.Instance.cannonCost);

        // Spawn cannon
        GameObject cannonObj = Instantiate(cannonPrefab);
        Cannon cannon = cannonObj.GetComponent<Cannon>();

        // Scale to match grid
        float cannonScale = GridManager.Instance.cellSize * 0.80f;
        cannonObj.transform.localScale = new Vector3(cannonScale, cannonScale, 1f);

        // Place on random empty cell
        randomCell.PlaceCannon(cannon);

        UIManager.Instance.ShowMessage("Cannon summoned!");
        Debug.Log($"Cannon summoned at cell ({randomCell.gridX},{randomCell.gridY})");
    }
}