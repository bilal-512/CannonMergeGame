using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int bulletDamage;
    private float bulletSpeed = 8f;
    private bool hasHit = false;

    public void Initialize(int dmg)
    {
        bulletDamage = dmg;
    }

    public void Initialize(Enemy target, int dmg)
    {
        bulletDamage = dmg;
    }

    void Update()
    {
        if (hasHit) return;

        // Move straight UP every frame
        transform.Translate(Vector2.up * bulletSpeed * Time.deltaTime);

        // Destroy if it goes off the top of the screen
        if (transform.position.y > 8f)
            Destroy(gameObject);
    }

    // Unity calls this when bullet hits the enemy
    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            hasHit = true;
            enemy.TakeDamage(bulletDamage);
            Destroy(gameObject);
        }
    }
}