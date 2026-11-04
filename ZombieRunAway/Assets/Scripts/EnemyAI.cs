
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    [Header("Target")]
    public string playerTag = "Player";
    public float detectionRange = 20f;

    [Header("Movement")]
    public float moveSpeed = 3.5f;
    public float stoppingDistance = 1.5f;

    [Header("Attack")]
    public float attackRange = 2f;
    public float attackDamage = 10f;
    public float attackCooldown = 1.5f;

    private NavMeshAgent agent;
    private Transform player;
    private float nextAttackTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.speed = moveSpeed;
        agent.stoppingDistance = stoppingDistance;

        FindPlayer();
    }

    void Update()
    {
        if (player == null)
        {
            FindPlayer();
            return;
        }

        if (!agent.isOnNavMesh)
            return;

        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        if (distance <= detectionRange)
        {
            ChasePlayer(distance);
        }
        else
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
    }

    void FindPlayer()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag(playerTag);

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    void ChasePlayer(float distance)
    {
        if (distance > attackRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        else
        {
            agent.isStopped = true;
            AttackPlayer();
        }
    }

    void AttackPlayer()
    {
        if (Time.time < nextAttackTime)
            return;

        nextAttackTime = Time.time + attackCooldown;

        PlayerHealth playerHealth =
            player.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
        }
        else
        {
            Debug.LogWarning(
                "На игроке отсутствует PlayerHealth!"
            );
        }
    }
}
