using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    [Header("References")]
    public GameObject enemyPrefab;

    [Header("Wave Settings")]
    public float timeBetweenSpawns = 1.5f;

    private bool battleActive          = false;
    private int  currentWaveEnemyCount = 0;
    private int  enemiesSpawned        = 0;
    private int  enemiesKilled         = 0;
    private bool[] columnOccupied;

    // The spawn list for current wave — built by BuildWaveComposition()
    private List<Enemy.EnemyType> spawnQueue = new List<Enemy.EnemyType>();

    void Awake() { Instance = this; }

    // ── Wave complete tracking 

    public void OnEnemyKilled()
    {
        enemiesKilled++;
        Debug.Log($"Killed/Escaped: {enemiesKilled} / {currentWaveEnemyCount}");
        CheckWaveComplete();
    }

    public void OnEnemyEscaped(int column)
    {
        FreeColumn(column);
        enemiesKilled++;
        CheckWaveComplete();
    }

    public void FreeColumn(int column)
    {
        if (columnOccupied != null && column >= 0
            && column < columnOccupied.Length)
            columnOccupied[column] = false;
    }

    void CheckWaveComplete()
    {
        bool spawnDone = enemiesSpawned >= currentWaveEnemyCount;
        bool allGone   = enemiesKilled  >= currentWaveEnemyCount;

        if (spawnDone && allGone && battleActive)
            WaveCompleted();
    }

    // ── Send button 

    public void OnSendButtonPressed()
    {
        if (battleActive) return;
        StartNextWave();
    }

    // ── Wave composition 

    // Builds a list of enemy types for this wave
    // Early waves = mostly Normal, later waves add Fast/Tank/Boss gradually
  List<Enemy.EnemyType> BuildWaveComposition(int wave)
{
    List<Enemy.EnemyType> queue = new List<Enemy.EnemyType>();

    // ── Waves 1-3: gentle intro ──────────────────────────
    // Player learns the game, merging feels rewarding
    if (wave <= 3)
    {
        int normalCount = wave == 1 ? 2 : wave == 2 ? 3 : 4;
        int fastCount   = wave == 1 ? 0 : wave == 2 ? 1 : 2;
        int tankCount   = wave == 3 ? 1 : 0;

        for (int i = 0; i < normalCount; i++) queue.Add(Enemy.EnemyType.Normal);
        for (int i = 0; i < fastCount;   i++) queue.Add(Enemy.EnemyType.Fast);
        for (int i = 0; i < tankCount;   i++) queue.Add(Enemy.EnemyType.Tank);
    }
    // ── Waves 4-6: ramp up ───────────────────────────────
    // Noticeably harder — more enemies, faster spawns
    else if (wave <= 6)
    {
        int normalCount = 3;
        int fastCount   = wave - 2;          // wave4=2, wave5=3, wave6=4
        int tankCount   = wave - 3;          // wave4=1, wave5=2, wave6=3
        int bossCount   = (wave == 5) ? 1 : 0;

        for (int i = 0; i < normalCount; i++) queue.Add(Enemy.EnemyType.Normal);
        for (int i = 0; i < fastCount;   i++) queue.Add(Enemy.EnemyType.Fast);
        for (int i = 0; i < tankCount;   i++) queue.Add(Enemy.EnemyType.Tank);
        for (int i = 0; i < bossCount;   i++) queue.Add(Enemy.EnemyType.Boss);
    }
    // ── Wave 7+: intense ─────────────────────────────────
    // Every wave gets meaningfully harder, boss every 5 waves
    else
    {
        int extra       = wave - 6;          // grows by 1 each wave after 6
        int normalCount = 3;
        int fastCount   = Mathf.Min(4 + extra, 8);
        int tankCount   = Mathf.Min(3 + extra, 6);
        int bossCount   = (wave % 5 == 0) ? 2 : 1; // 2 bosses on milestone waves

        for (int i = 0; i < normalCount; i++) queue.Add(Enemy.EnemyType.Normal);
        for (int i = 0; i < fastCount;   i++) queue.Add(Enemy.EnemyType.Fast);
        for (int i = 0; i < tankCount;   i++) queue.Add(Enemy.EnemyType.Tank);
        for (int i = 0; i < bossCount;   i++) queue.Add(Enemy.EnemyType.Boss);
    }

    // Shuffle so types appear in random order not clumped together
    for (int i = queue.Count - 1; i > 0; i--)
    {
        int j = Random.Range(0, i + 1);
        Enemy.EnemyType tmp = queue[i];
        queue[i] = queue[j];
        queue[j] = tmp;
    }

    Debug.Log($"Wave {wave}: {queue.Count} enemies — " +
        $"{queue.FindAll(e => e == Enemy.EnemyType.Normal).Count}N " +
        $"{queue.FindAll(e => e == Enemy.EnemyType.Fast).Count}F " +
        $"{queue.FindAll(e => e == Enemy.EnemyType.Tank).Count}T " +
        $"{queue.FindAll(e => e == Enemy.EnemyType.Boss).Count}B");

    return queue;
}
    // ── Spawning ───────────────────────────────────────────

    void StartNextWave()
    {
        GameManager.Instance.StartNextWave();
        int wave = GameManager.Instance.GetCurrentWave();

        // Build the mix for this wave
        spawnQueue            = BuildWaveComposition(wave);
        currentWaveEnemyCount = spawnQueue.Count;
        enemiesSpawned        = 0;
        enemiesKilled         = 0;
        battleActive          = true;

        int colCount   = GridManager.Instance.GetColumnCount();
        columnOccupied = new bool[colCount];

        UIManager.Instance.ShowMessage(
            $"Wave {wave} — {currentWaveEnemyCount} enemies incoming!", 3f);

        foreach (Cannon c in FindObjectsByType<Cannon>(FindObjectsSortMode.None))
            c.StartShooting();

        StartCoroutine(SpawnWaveEnemies());
    }

    IEnumerator SpawnWaveEnemies()
    {
        yield return new WaitForSeconds(1f);

        while (enemiesSpawned < spawnQueue.Count)
        {
            int col = GetFreeRandomColumn();

            if (col == -1)
            {
                yield return new WaitForSeconds(0.4f);
                continue;
            }

            // Spawn the next enemy type from the queue
            SpawnEnemyInColumn(col, spawnQueue[enemiesSpawned]);
            enemiesSpawned++;
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }

    int GetFreeRandomColumn()
    {
        int colCount = GridManager.Instance.GetColumnCount();
        List<int> free = new List<int>();
        for (int i = 0; i < colCount; i++)
            if (!columnOccupied[i]) free.Add(i);

        if (free.Count == 0) return -1;
        return free[Random.Range(0, free.Count)];
    }

void SpawnEnemyInColumn(int col, Enemy.EnemyType type)
{
    float spawnX = GridManager.Instance.GetColumnWorldX(col);
    float spawnY = GridManager.Instance.GetGridTopY();

    GameObject obj = Instantiate(enemyPrefab,
        new Vector3(spawnX, spawnY, 0), Quaternion.identity);

    int wave = GameManager.Instance.GetCurrentWave();

    // Base scale
    float scale = GridManager.Instance.cellSize * 0.80f;
    if (type == Enemy.EnemyType.Tank) scale *= 1.2f;
    if (type == Enemy.EnemyType.Boss) scale *= 1.5f;
    obj.transform.localScale = new Vector3(scale, scale, 1f);

    Enemy enemy       = obj.GetComponent<Enemy>();
    enemy.columnIndex = col;
    enemy.enemyType   = type;

    // Speed multiplier grows after wave 3 
    // Wave 1-3: 1.0x  Wave 4: 1.2x  Wave 5: 1.4x  Wave 6: 1.6x  Wave 7+: keeps growing
    float speedMult = 1f;
    if (wave > 3)
        speedMult = 1f + (wave - 3) * 0.2f; // +20% speed per wave after wave 3

    // Cap so game stays playable
    speedMult = Mathf.Min(speedMult, 2.5f);

    enemy.speedMultiplier = speedMult;
    enemy.StartMoving();

    // Spawn interval also shrinks after wave 3 — enemies come faster
    if (wave > 3)
    {
        float newInterval = Mathf.Max(
            timeBetweenSpawns - (wave - 3) * 0.15f,
            0.6f // minimum gap between spawns
        );
        timeBetweenSpawns = newInterval;
    }

    columnOccupied[col] = true;
}
    //  Wave complete 

    void WaveCompleted()
    {
        battleActive = false;

        foreach (Cannon c in FindObjectsByType<Cannon>(FindObjectsSortMode.None))
            c.StopShooting();

        GameManager.Instance.OnWaveCompleted();

        int nextWave       = GameManager.Instance.GetCurrentWave() + 1;
        var nextComposition = BuildWaveComposition(nextWave);

        UIManager.Instance.ShowMessage(
            $"Wave complete! Press SEND for Wave {nextWave} ({nextComposition.Count} enemies).", 4f);

        Debug.Log("WAVE COMPLETE");
    }
}