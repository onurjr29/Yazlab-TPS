using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float life = 3f;
    public float damage = 25f;

    void Start()
    {
        Destroy(gameObject, life);
    }

    void OnTriggerEnter(Collider col)
    {
        // Düşmana çarptı mı?
        var enemyHp = col.GetComponentInParent<EnemyHealth>() ?? col.GetComponent<EnemyHealth>();
        if (enemyHp != null)
        {
            enemyHp.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // Oyuncuya çarptı mı?
        var playerHp = col.GetComponentInParent<PlayerHealth>() ?? col.GetComponent<PlayerHealth>();
        if (playerHp != null)
        {
            playerHp.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // Başka bir şeye çarptıysa
        Destroy(gameObject);
    }
}
