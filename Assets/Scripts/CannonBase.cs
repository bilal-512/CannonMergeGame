using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CannonBase : MonoBehaviour
{
    public static CannonBase Instance;

    [Header("Health Settings")]
    public int maxHealth = 100;

    private int currentHealth;

    // UI references — assign in Inspector
    [Header("UI References")]
    public Slider healthBar;        // drag a UI Slider here
    public TextMeshProUGUI healthText; // drag a TMP text here

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        currentHealth  = Mathf.Max(0, currentHealth);
        UpdateHealthUI();

        Debug.Log($"Cannon base hit! HP: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
            GameOver();
    }

    void UpdateHealthUI()
    {
        if (healthBar != null)
            healthBar.value = (float)currentHealth / maxHealth;

        if (healthText != null)
            healthText.text = $"Base HP: {currentHealth}/{maxHealth}";
    }

    void GameOver()
    {
        Debug.Log("GAME OVER — cannon base destroyed!");
        UIManager.Instance.ShowMessage("GAME OVER! Your base was destroyed!", 10f);

        // Stop everything
        Time.timeScale = 0f; // Pause the game
    }

    // Call this to resume after game over screen
    public void RestartGame()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}