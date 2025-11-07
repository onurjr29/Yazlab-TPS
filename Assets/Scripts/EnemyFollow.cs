using UnityEngine;
using UnityEngine.AI;

public class EnemyFollow : MonoBehaviour
{
    public Transform player;
    public float stopDistance = 1.5f;       // Oyuncuya çok yaklaşınca durma mesafesi
    public float chaseUpdateRate = 0.2f;    // Hedefi güncelleme sıklığı
    private NavMeshAgent agent;
    private float updateTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                player = p.transform;
        }

        // Düşmanın hız ve tepki ayarlarını biraz güçlendirelim
        agent.speed = 15f;             // daha hızlı koşsun
        agent.acceleration = 50f;     // ani tepki versin
        agent.angularSpeed = 1000f;    // dönüşleri hızlı olsun
        agent.stoppingDistance = stopDistance;
        agent.autoBraking = false;
    }

    void Update()
    {
        if (player == null) return;

        updateTimer -= Time.deltaTime;
        if (updateTimer <= 0f)
        {
            // Sürekli oyuncunun pozisyonuna hedef koy
            agent.isStopped = false;
            agent.SetDestination(player.position);
            updateTimer = chaseUpdateRate;  // Belirli aralıkla hedefi güncelle
        }

        // Eğer çok yaklaştıysa biraz durup yön değiştirsin
        if (agent.remainingDistance <= stopDistance)
        {
            agent.isStopped = true;
        }
    }
}
