using UnityEngine;
using TMPro;

public class Enemy : MonoBehaviour
{
    // ── Enemy Types ────────────────────────────────────────
    public enum EnemyType { Normal, Fast, Tank, Boss }

    [Header("Type — set by BattleManager")]
    public EnemyType enemyType = EnemyType.Normal;

    [Header("Base Stats (Normal type)")]
    public int maxHP = 50;
    public float moveSpeed = 1.5f;
    public int damageToBase = 10;
    public int attackDamage = 5;

    [Header("Wobble")]
    public float wobbleSpeed = 3f;
    public float wobbleAmount = 15f;

    [HideInInspector] public int columnIndex = -1;
    [HideInInspector] public float speedMultiplier = 1f;
    private int currentHP;
    private bool isMoving = false;
    private bool isAttacking = false;
    private float attackTimer = 0f;
    private float wobbleOffset;
    private float stopY;

    private TextMeshPro hpLabel;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        wobbleOffset = Random.Range(0f, 100f);

        // Apply stats and color based on type
        ApplyEnemyType();

        currentHP = maxHP;
        CreateHPLabel();

        float screenBottom = -Camera.main.orthographicSize;
        stopY = screenBottom + Camera.main.orthographicSize * 0.35f;
    }

    void ApplyEnemyType()
    {
        switch (enemyType)
        {
            case EnemyType.Normal:
                // Normal — slow and easy to handle
                moveSpeed = 0.8f;
                if (sr != null) sr.color = new Color(0.9f, 0.2f, 0.2f);
                break;

            case EnemyType.Fast:
                maxHP = 80;
                moveSpeed = 1.6f;  // was 3f — now feels fast but not impossible
                damageToBase = 10;
                attackDamage = 8;
                if (sr != null) sr.color = new Color(1f, 0.85f, 0f);
                wobbleAmount = 25f;
                break;

            case EnemyType.Tank:
                maxHP = 200;
                moveSpeed = 0.4f;  // was 0.7f — tanks are very slow
                damageToBase = 25;
                attackDamage = 15;
                if (sr != null) sr.color = new Color(1f, 0.45f, 0f);
                wobbleAmount = 8f;
                wobbleSpeed = 1.5f;
                break;

            case EnemyType.Boss:
                maxHP = 500;
                moveSpeed = 0.5f;  // was 1f — boss is slow but powerful
                damageToBase = 40;
                attackDamage = 20;
                if (sr != null) sr.color = new Color(0.6f, 0.1f, 0.9f);
                wobbleAmount = 5f;
                wobbleSpeed = 1f;
                break;
        }
        // Applied wave-based speed scaling set by BattleManager
        moveSpeed *= speedMultiplier;
    }

    void CreateHPLabel()
    {
        GameObject labelObj = new GameObject("HPLabel");
        labelObj.transform.SetParent(transform, false);
        labelObj.transform.localPosition = new Vector3(0, 0, -0.1f);

        hpLabel = labelObj.AddComponent<TextMeshPro>();
        hpLabel.fontSize = enemyType == EnemyType.Boss ? 4f : 3f;
        hpLabel.alignment = TextAlignmentOptions.Center;
        hpLabel.color = Color.white;
        hpLabel.fontStyle = FontStyles.Bold;
        hpLabel.sortingOrder = 5;

        UpdateHPLabel();
    }

    void UpdateHPLabel()
    {
        if (hpLabel != null)
            hpLabel.text = currentHP.ToString();
    }

    public void StartMoving() => isMoving = true;

    // Called by BattleManager to override HP (used for wave scaling if needed)
    public void SetHP(int hp)
    {
        maxHP = hp;
        currentHP = hp;
        UpdateHPLabel();
    }

    void Update()
    {
        if (isMoving)
        {
            MoveDown();
            Wobble();
        }
        else if (isAttacking)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= 1f)
            {
                attackTimer = 0f;
                AttackBase();
            }
            Wobble();
        }
    }

    void MoveDown()
    {
        transform.Translate(Vector2.down * moveSpeed * Time.deltaTime);

        if (transform.position.y <= stopY)
        {
            transform.position = new Vector3(transform.position.x, stopY, 0);
            isMoving = false;
            isAttacking = true;

            BattleManager.Instance.FreeColumn(columnIndex);

            if (CannonBase.Instance != null)
                CannonBase.Instance.TakeDamage(damageToBase);

            Debug.Log($"{enemyType} enemy reached base!");
        }
    }

    void Wobble()
    {
        float angle = Mathf.Sin(
            (Time.time + wobbleOffset) * wobbleSpeed) * wobbleAmount;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void AttackBase()
    {
        if (CannonBase.Instance != null)
            CannonBase.Instance.TakeDamage(attackDamage);
    }

    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        UpdateHPLabel();
        if (currentHP <= 0) Die();
    }

    void Die()
    {
        if (isMoving)
            BattleManager.Instance.FreeColumn(columnIndex);

        BattleManager.Instance.OnEnemyKilled();
        Destroy(gameObject);
    }
}