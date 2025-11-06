using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 10f;
    public float fireRate = 1.2f;
    public float bulletSpeed = 20f;
    public GameObject bulletPrefab;
    public Transform firePoint;

    private float nextFireTime;

    [Header("References")]
    private Transform player;
    private EnemyHealth health;
    private Collider[] myColliders;

    void Awake()
    {
        // Kendi collider’larını önceden al (mermi ile çarpışmayı ignore edeceğiz)
        myColliders = GetComponentsInChildren<Collider>();
    }

    void Start()
    {
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj) player = playerObj.transform;

        health = GetComponent<EnemyHealth>();

        // Spawn olur olmaz ateş etmesin
        nextFireTime = Time.time + 1f;
    }

    void Update()
    {
        if (!player || !health) return;
        if (health.currentHealth <= 0f) return; // ölü ise ateş etme

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + (1f / fireRate);
            Shoot();
        }
    }

    // *** DİKKAT: Bu metod Update/Start'ın DIŞINDA ***
    void Shoot()
    {
        if (!bulletPrefab || !firePoint || !player) return;

        // Mermiyi üret
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Düşman kendi mermisiyle çarpışmasın
        var bulletCol = bullet.GetComponent<Collider>();
        if (bulletCol != null && myColliders != null)
        {
            foreach (var col in myColliders)
            {
                if (col) Physics.IgnoreCollision(bulletCol, col);
            }
        }

        // Mermiyi oyuncuya doğru hızlandır
        var rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 dir = (player.position - firePoint.position).normalized;
            rb.velocity = dir * bulletSpeed;
        }
    }
}
