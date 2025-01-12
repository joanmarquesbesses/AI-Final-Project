using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Frustrum : MonoBehaviour
{
    public Camera frustum;          // Cámara para el campo de visión del zombi
    public LayerMask mask;          // Máscara de capas para detectar al jugador
    private NavMeshAgent agent;      // Componente NavMeshAgent para moverse
    private bool playerDetected = false;  // Si el zombi ha detectado al jugador
    public bool watchingyou = false;
    public float wanderRadius;  // Radio del área donde el zombi puede vagar
    private float wanderTimer;
    private float timer = 0;

    // Start is called before the first frame update
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

        DetectPlayer();

        if (!playerDetected)
        {
            if (agent.velocity.sqrMagnitude <= 0.1f)
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

    }

    public void ZombieScream()
    {
        Transform auxTransform = ZombiManager.zManager.GetObjective();
        if (Vector3.Distance(auxTransform.position, transform.position) < 20)
        {
            agent.SetDestination(auxTransform.position);
            playerDetected = true;
        }
        else
        {
            playerDetected = false;
        }
    }

    public void BlindZombie()
    {
        playerDetected = false;
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

    void DetectPlayer()
    {
        watchingyou = false;
        Collider[] colliders = Physics.OverlapSphere(frustum.transform.position, frustum.farClipPlane, mask);
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(frustum);

        foreach (Collider col in colliders)
        {
            if (col.gameObject != gameObject && GeometryUtility.TestPlanesAABB(planes, col.bounds))
            {
                RaycastHit hit;

                if (Physics.Raycast(this.transform.position, (col.transform.position - this.transform.position).normalized,
                    out hit, frustum.farClipPlane, mask))
                {
                    if (hit.collider.gameObject.CompareTag("NPC"))
                    {
                        ZombiManager.zManager.SetObjective(hit.transform);
                        ZombiManager.zManager.AlertZombis();
                        watchingyou = true;
                        hit.collider.gameObject.GetComponent<Evade>().zombi = this.transform;
                    }
                }
            }
        }
    }

}
