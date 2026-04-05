using UnityEngine;
using System.Collections;
using TMPro;

public class Cannon : MonoBehaviour
{
    [Header("Cannon Stats")]
    public int level = 1;
    public float fireRate = 1f;
    public int damage = 10;

    [Header("References")]
    public GameObject bulletPrefab;

    [HideInInspector] public Cell currentCell;

    private bool isShooting = false;
    private Coroutine shootCoroutine;
    private TextMeshPro levelLabel;

    void Start()
    {
        CreateLevelLabel();
    }

    void CreateLevelLabel()
    {
        // Create label as child of cannon
        GameObject labelObj = new GameObject("LevelLabel");
        labelObj.transform.SetParent(transform, false);

        // Center it on the cannon, slightly in front
        labelObj.transform.localPosition = new Vector3(0, 0, -0.1f);

        levelLabel = labelObj.AddComponent<TextMeshPro>();
        levelLabel.text = $"Lv{level}";
        levelLabel.fontSize = 3f;
        levelLabel.alignment = TextAlignmentOptions.Center;
        levelLabel.color = Color.white;
        levelLabel.fontStyle = FontStyles.Bold;
        levelLabel.sortingOrder = 5;
    }

    void UpdateLevelLabel()
    {
        if (levelLabel != null)
            levelLabel.text = $"Lv{level}";
    }

    public void StartShooting()
    {
        if (isShooting) return;
        isShooting = true;
        shootCoroutine = StartCoroutine(ShootingLoop());
    }

    public void StopShooting()
    {
        isShooting = false;
        if (shootCoroutine != null)
            StopCoroutine(shootCoroutine);
    }

    IEnumerator ShootingLoop()
    {
        while (isShooting)
        {
            // Fire straight up — no target needed
            FireUp();
            yield return new WaitForSeconds(1f / fireRate);
        }
    }

    void FireUp()
    {
        if (bulletPrefab == null)
        {
            Debug.LogError("Bullet Prefab is NULL on: " + gameObject.name);
            return;
        }

        GameObject bulletObj = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        float bulletScale = GridManager.Instance.cellSize * 0.25f;
        bulletObj.transform.localScale = new Vector3(bulletScale, bulletScale, 1f);

        Bullet bullet = bulletObj.GetComponent<Bullet>();
        if (bullet != null)
            bullet.Initialize(damage);
    }

    // Called when two same-level cannons merge
    public void Upgrade()
    {
        level++;
        damage += 10;
        fireRate += 0.3f;
        gameObject.name = $"Cannon Lv{level}";
        UpdateLevelLabel();
        Debug.Log($"Merged! Now Level {level}");
    }

    // Keep old FindNearestEnemy for reference — not used anymore
    Enemy FindNearestEnemy()
    {
        Enemy[] all = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        Enemy nearest = null;
        float minDist = Mathf.Infinity;
        foreach (Enemy e in all)
        {
            float d = Vector3.Distance(transform.position, e.transform.position);
            if (d < minDist) { minDist = d; nearest = e; }
        }
        return nearest;
    }
}