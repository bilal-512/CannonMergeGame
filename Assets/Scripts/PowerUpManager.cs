using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    [Header("Power-Up Values")]
    public float rapidFireMultiplier = 2f;
    public int firePowerBonus = 20;

    public void ActivateRapidFire()
    {
        if (!GameManager.Instance.SpendCoins(GameManager.Instance.powerUpCost))
        {
            UIManager.Instance.ShowMessage("Not enough coins for Rapid Fire! Need 5.");
            return;
        }

        Cannon[] cannons = FindObjectsByType<Cannon>(FindObjectsSortMode.None);
        foreach (Cannon c in cannons)
            c.fireRate *= rapidFireMultiplier;

        UIManager.Instance.ShowMessage("Rapid Fire activated!");
    }

    public void ActivateShield()
    {
        if (!GameManager.Instance.SpendCoins(GameManager.Instance.powerUpCost))
        {
            UIManager.Instance.ShowMessage("Not enough coins for Shield! Need 5.");
            return;
        }

        UIManager.Instance.ShowMessage("Shield activated!");
        Debug.Log("Shield activated!");
    }

    public void ActivateFirePower()
    {
        if (!GameManager.Instance.SpendCoins(GameManager.Instance.powerUpCost))
        {
            UIManager.Instance.ShowMessage("Not enough coins for Fire Power! Need 5.");
            return;
        }

        Cannon[] cannons = FindObjectsByType<Cannon>(FindObjectsSortMode.None);
        foreach (Cannon c in cannons)
            c.damage += firePowerBonus;

        UIManager.Instance.ShowMessage("Fire Power activated!");
    }
}
