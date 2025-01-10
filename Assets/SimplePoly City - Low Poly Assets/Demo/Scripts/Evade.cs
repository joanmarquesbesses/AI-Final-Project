using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Evade : MonoBehaviour
{
    // Start is called before the first frame update
    private NavMeshAgent agent;
    public float wanderRadius;  // Radio del área donde el zombi puede vagar
    private float wanderTimer;
    private float timer = 0;
    public Transform zombi = null;
    public bool evade = false;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.radius = 1.0f;
        agent.avoidancePriority = 40;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        wanderTimer = Random.Range(2, 4);
        wanderRadius = Random.Range(10, 20);
    }

    // Update is called once per frame
    void Update()
    {
        if(zombi != null)
        {
            Vector3 direction = (zombi.position - transform.position).normalized *-1;
            Debug.Log(direction);
            agent.SetDestination(transform.position + (direction * 10));
            agent.speed = 4;
            zombi = null;
        }
        else if (agent.velocity.sqrMagnitude <= 0.1f)
        {
            agent.speed = 2.0f;
            timer += Time.deltaTime;
            if (timer >= wanderTimer)
            {
                Vector3 newPos = RandomNavMeshLocation(wanderRadius);
                agent.SetDestination(newPos);
                timer = 0;
                wanderTimer = Random.Range(2, 4);
                wanderRadius = Random.Range(10, 20);
            }
        }
    }

    Vector3 RandomNavMeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius; // Crear una dirección aleatoria dentro de un radio
        randomDirection += transform.position; // Aplicar la posición actual como origen

        NavMeshHit hit;
        // Encontrar la posición más cercana dentro del NavMesh desde el punto aleatorio
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas))
        {
            return hit.position; // Si es válida, devolver la posición en el NavMesh
        }

        return transform.position; // Si no es válida, quedarse en la misma posición
    }
}
