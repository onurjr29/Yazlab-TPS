using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public enum State { Idle, Patrol, Chase, Dead }
    public State currentState = State.Idle;

    public Transform player;
    public Transform[] waypoints;
    private int waypointIndex = 0;

    public float detectionRange = 10f;
    public float chaseSpeed = 4f;
    public float patrolSpeed = 2f;
    public float respawnDelay = 5f;

    private NavMeshAgent agent;
    private float waitTime = 2f;
    private float waitCounter = 0f;
    private EnemyHealth health;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<EnemyHealth>();

        if (waypoints.Length > 0)
        {
            currentState = State.Patrol;
            agent.SetDestination(waypoints[waypointIndex].position);
        }
    }

    void Update()
    {
        if (currentState == State.Dead) return;

        switch (currentState)
        {
            case State.Idle:
                Idle();
                break;
            case State.Patrol:
                Patrol();
                break;
            case State.Chase:
                Chase();
                break;
        }

        // Saðlýk kontrolü
        if (health != null && health.currentHealth <= 0 && currentState != State.Dead)
        {
            StartCoroutine(HandleDeath());
        }
    }

    void Idle()
    {
        waitCounter += Time.deltaTime;
        if (waitCounter >= waitTime)
        {
            waitCounter = 0f;
            currentState = State.Patrol;
            agent.SetDestination(waypoints[waypointIndex].position);
        }

        // Oyuncuyu görür görmez kovalamaya baþla
        if (Vector3.Distance(transform.position, player.position) <= detectionRange)
        {
            currentState = State.Chase;
        }
    }

    void Patrol()
    {
        agent.speed = patrolSpeed;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            waypointIndex = (waypointIndex + 1) % waypoints.Length;
            agent.SetDestination(waypoints[waypointIndex].position);
        }

        if (Vector3.Distance(transform.position, player.position) <= detectionRange)
        {
            currentState = State.Chase;
        }
    }

    void Chase()
    {
        agent.speed = chaseSpeed;

        // Sürekli oyuncunun pozisyonunu güncelle
        agent.SetDestination(player.position);

        //  Artýk kovalamayý asla býrakmýyor
        // Eskiden burada "mesafe artarsa Patrol'a dön" vardý, onu kaldýrdýk.
    }

    IEnumerator HandleDeath()
    {
        currentState = State.Dead;
        agent.isStopped = true;

        Debug.Log($"{name} öldü!");
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        yield return new WaitForSeconds(respawnDelay);

        // Respawn
        health.currentHealth = health.maxHealth;
        health.healthSlider.value = health.maxHealth;

        GetComponent<MeshRenderer>().enabled = true;
        GetComponent<Collider>().enabled = true;

        currentState = State.Patrol;
        agent.isStopped = false;
        agent.SetDestination(waypoints[waypointIndex].position);
    }
}
