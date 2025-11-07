using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float life = 3f;
    public float damage = 25f;
    public GameObject owner; // mermiyi atan obje

    void Start()
    {
        Debug.Log($"[Bullet Start] owner={(owner ? owner.name : "NULL")} ownerTag={(owner ? owner.tag : "NULL")} layer={gameObject.layer}");
        Destroy(gameObject, life);
    }

    void OnTriggerEnter(Collider col)
    {
        Debug.Log($"[Bullet Hit] owner={(owner ? owner.name : "NULL")} ownerTag={(owner ? owner.tag : "NULL")} hit={col.gameObject.name} hitTag={col.gameObject.tag} root={col.transform.root.name}");

        // Eğer çarpılan obje mermiyi atanla aynıysa ya da atan objenin child'ıysa yok say
        if (owner != null && (col.gameObject == owner || col.transform.IsChildOf(owner.transform) || col.transform.root == owner.transform))
            return;

        // Düşmana çarptıysa
        var enemyHp = col.GetComponentInParent<EnemyHealth>() ?? col.GetComponent<EnemyHealth>();
        if (enemyHp != null)
        {
            // Eğer mermiyi atan bir Enemy ise diğer Enemy'lere zarar verme
            if (owner != null && owner.CompareTag("Enemy"))
            {
                Debug.Log("[Bullet] Hit Enemy but owner is Enemy -> ignoring damage");
                return;
            }

            // Aksi halde hasar ver
            enemyHp.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // Oyuncuya çarptıysa
        var playerHp = col.GetComponentInParent<PlayerHealth>() ?? col.GetComponent<PlayerHealth>();
        if (playerHp != null)
        {
            if (owner != null && owner.CompareTag("Enemy"))
            {
                // Enemy ateş etti -> player vurulabilir
                playerHp.TakeDamage(damage);
                Destroy(gameObject);
            }
            else
            {
                // Player'in kendi mermisi player'a zarar vermesin
                Debug.Log("[Bullet] Hit player but owner is not Enemy -> ignoring (friendly fire off)");
            }
            return;
        }

        // Diğer herşeyde yok et
        Destroy(gameObject);
    }
}
