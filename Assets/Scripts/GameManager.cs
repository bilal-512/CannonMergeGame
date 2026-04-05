using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Starting Values")]
    public int startingCoins = 20;
    public int cannonCost = 10;
    public int powerUpCost = 5;
    public int waveRewardCoins = 8; // Coins given after each wave

    // Current game state
    private int currentCoins;
    private int currentWave = 0;

    // Events / UIManager listens to these to update the display
    public Action<int> OnCoinsChanged;   // fires when coins change
    public Action<int> OnWaveChanged;    // fires when wave changes

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentCoins = startingCoins;
        OnCoinsChanged?.Invoke(currentCoins);
    }

    public int GetCoins() => currentCoins;

    public bool HasEnoughCoins(int amount) => currentCoins >= amount;

    public void AddCoins(int amount)
    {
        currentCoins += amount;
        OnCoinsChanged?.Invoke(currentCoins);
        Debug.Log($"Coins added: +{amount} | Total: {currentCoins}");
    }

    public bool SpendCoins(int amount)
    {
        if (!HasEnoughCoins(amount))
        {
            Debug.Log($"Not enough coins! Need {amount}, have {currentCoins}");
            return false;
        }

        currentCoins -= amount;
        OnCoinsChanged?.Invoke(currentCoins);
        Debug.Log($"Coins spent: -{amount} | Remaining: {currentCoins}");
        return true;
    }

    public int GetCurrentWave() => currentWave;

    public void StartNextWave()
    {
        currentWave++;
        OnWaveChanged?.Invoke(currentWave);
        Debug.Log($"Wave {currentWave} starting!");
    }

    public void OnWaveCompleted()
    {
        // Reward increases with wave number
        int reward = waveRewardCoins + (currentWave * 2);
        AddCoins(reward);
        Debug.Log($"Wave {currentWave} complete! Rewarded {reward} coins.");
    }
}