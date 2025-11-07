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

    public float detectionRange = 30f;
    public float chaseSpeed = 30f;
    public float patrolSpeed = 3f;
    public float respawnDelay = 5f;

    [Header("Davranýþ Ayarý")]
    public bool alwaysChase = false;  //  Bu düþman sonsuza kadar kovalar mý?

    private NavMeshAgent agent;
    private float waitTime = 2f;
    private float waitCounter = 0f;
    private EnemyHealth health;

    private bool hasSpottedPlayer = false;

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

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Oyuncu görme kontrolü
        if (!hasSpottedPlayer && distanceToPlayer <= detectionRange)
        {
            hasSpottedPlayer = true;
            currentState = State.Chase;
        }

        switch (currentState)
        {
            case State.Idle:
                Idle(distanceToPlayer);
                break;
            case State.Patrol:
                Patrol(distanceToPlayer);
                break;
            case State.Chase:
                Chase(distanceToPlayer);
                break;
        }

        if (health != null && health.currentHealth <= 0 && currentState != State.Dead)
        {
            StartCoroutine(HandleDeath());
        }
    }

    void Idle(float distanceToPlayer)
    {
        waitCounter += Time.deltaTime;
        if (waitCounter >= waitTime)
        {
            waitCounter = 0f;
            currentState = State.Patrol;
            agent.SetDestination(waypoints[waypointIndex].position);
        }

        if (distanceToPlayer <= detectionRange)
            currentState = State.Chase;
    }

    void Patrol(float distanceToPlayer)
    {
        agent.speed = patrolSpeed;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            waypointIndex = (waypointIndex + 1) % waypoints.Length;
            agent.SetDestination(waypoints[waypointIndex].position);
        }

        if (distanceToPlayer <= detectionRange)
            currentState = State.Chase;
    }

    void Chase(float distanceToPlayer)
    {
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);

        //  Eðer alwaysChase false ise, mesafe çok açýlýrsa kovalamayý býrak
        if (!alwaysChase)
        {
            if (distanceToPlayer > detectionRange * 2f)
            {
                hasSpottedPlayer = false;
                currentState = State.Patrol;
                agent.SetDestination(waypoints[waypointIndex].position);
            }
        }
    }

    IEnumerator HandleDeath()
    {
        currentState = State.Dead;
        agent.isStopped = true;

        Debug.Log($"{name} öldü!");
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        yield return new WaitForSeconds(respawnDelay);

        health.currentHealth = health.maxHealth;
        health.healthSlider.value = health.maxHealth;

        GetComponent<MeshRenderer>().enabled = true;
        GetComponent<Collider>().enabled = true;

        // Respawn sonrasý sýfýrla
        hasSpottedPlayer = false;
        currentState = State.Patrol;
        agent.isStopped = false;
        agent.SetDestination(waypoints[waypointIndex].position);
    }
}
