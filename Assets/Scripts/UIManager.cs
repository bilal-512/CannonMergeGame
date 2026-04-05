using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI References — drag in Inspector")]
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI messageText;

    private float messageTimer = 0f;

    void Awake()
    {
        Instance = this;
    }

    // OnEnable runs after Awake on all objects — safer than Start
    void OnEnable()
    {
        // Wait until GameManager exists
        if (GameManager.Instance == null) return;

        GameManager.Instance.OnCoinsChanged += UpdateCoinDisplay;
        GameManager.Instance.OnWaveChanged  += UpdateWaveDisplay;
    }

    void OnDisable()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.OnCoinsChanged -= UpdateCoinDisplay;
        GameManager.Instance.OnWaveChanged  -= UpdateWaveDisplay;
    }

    void Start()
    {
        // Subscribe again in Start to be safe — no duplicate because
        // we unsubscribe first then subscribe
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnCoinsChanged -= UpdateCoinDisplay;
            GameManager.Instance.OnCoinsChanged += UpdateCoinDisplay;
            GameManager.Instance.OnWaveChanged  -= UpdateWaveDisplay;
            GameManager.Instance.OnWaveChanged  += UpdateWaveChanged;

            // Force refresh displays immediately
            UpdateCoinDisplay(GameManager.Instance.GetCoins());
            UpdateWaveDisplay(GameManager.Instance.GetCurrentWave());
        }

        if (messageText != null)
            messageText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (messageTimer > 0)
        {
            messageTimer -= Time.deltaTime;
            if (messageTimer <= 0 && messageText != null)
                messageText.gameObject.SetActive(false);
        }
    }

    void UpdateCoinDisplay(int coins)
    {
        if (coinText != null)
            coinText.text = $"Coins: {coins}";
        else
            Debug.LogWarning("coinText is not assigned in UIManager!");
    }

    void UpdateWaveDisplay(int wave)
    {
        if (waveText != null)
            waveText.text = wave == 0 ? "Press SEND to start!" : $"Wave: {wave}";
        else
            Debug.LogWarning("waveText is not assigned in UIManager!");
    }

    // Alias so both event names work
    void UpdateWaveChanged(int wave) => UpdateWaveDisplay(wave);

    public void ShowMessage(string msg, float duration = 2.5f)
    {
        if (messageText == null)
        {
            Debug.LogWarning("messageText not assigned!");
            return;
        }
        messageText.text = msg;
        messageText.gameObject.SetActive(true);
        messageTimer = duration;
    }
}