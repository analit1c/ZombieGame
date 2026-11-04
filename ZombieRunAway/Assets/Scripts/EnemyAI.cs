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

    [Header("Health")]
    public float maxHealth = 100f;

    private float currentHealth;
    private NavMeshAgent agent;
    private Transform player;
    private float nextAttackTime;
    private bool isDead;

    void Start()
    {
        currentHealth = maxHealth;

        agent = GetComponent<NavMeshAgent>();

        agent.speed = moveSpeed;
        agent.stoppingDistance = stoppingDistance;

        FindPlayer();
    }

    void Update()
    {
        if (isDead)
            return;

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
            player = playerObject.transform;
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

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;

        Debug.Log(
            gameObject.name + " получил урон: " +
            damage + ". HP: " +
            Mathf.Max(0, currentHealth) + "/" + maxHealth
        );

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        if (agent != null)
            agent.enabled = false;

        Debug.Log(gameObject.name + " уничтожен!");

        Destroy(gameObject);
    }
}
