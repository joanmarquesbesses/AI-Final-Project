using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieIA : MonoBehaviour
{
    public Camera frustum;          // Cámara para el campo de visión del zombi
    private NavMeshAgent agent;      // Componente NavMeshAgent para moverse
    private GameObject player;       // Referencia al jugador
    private bool playerDetected = false;  // Si el zombi ha detectado al jugador
    public bool playerInSight = false;   // Si el jugador está en la vista de algun zombi actualmente
    float speed;
    public float wanderRadius;  // Radio del área donde el zombi puede vagar
    private float wanderTimer;
    private float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        // Configuración del NavMeshAgent para evitar a otros zombis
        agent.radius = 1.0f; // Ajusta el radio del agente para evitar aglomeraciones entre zombis
        agent.avoidancePriority = 40; // Prioridades aleatorias para evitar comportamientos iguales
        // Evitación de alta calidad para evitar entre zombis
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        wanderTimer = Random.Range(2,4);
        wanderRadius = Random.Range(10, 20);
    }

    // Update is called once per frame
    void Update()
    {
        DetectPlayer();

        if (playerDetected)
        {
            // Si el jugador está en la vista, seguirlo
            agent.SetDestination(player.transform.position);
            agent.speed = 3;
        }
        else
        {
            if (agent.velocity.sqrMagnitude <= 0.1f)
            {
                agent.speed = 2.0f;
                timer += Time.deltaTime; // Contar tiempo
                if (timer >= wanderTimer)
                {
                    Vector3 newPos = RandomNavMeshLocation(wanderRadius);
                    agent.SetDestination(newPos);
                    timer = 0; // Reiniciar el temporizador
                    wanderTimer = Random.Range(2, 4);
                    wanderRadius = Random.Range(10, 20);
                }
            }
        }
    }

    private void DetectPlayer()
    {
        playerInSight = false;
        // Obtener la posición del objeto en coordenadas del Viewport
        Vector3 posicionViewport = frustum.WorldToViewportPoint(player.gameObject.transform.position);

        // Verificar si el objeto está dentro del campo de visión horizontal, vertical, y dentro de los planos de clipping
        bool estaEnCampoDeVision = (posicionViewport.x >= 0 && posicionViewport.x <= 1) &&
                                    (posicionViewport.y >= 0 && posicionViewport.y <= 1) &&
                                    (posicionViewport.z >= frustum.nearClipPlane &&
                                    posicionViewport.z <= frustum.farClipPlane); // Comprobar plano cercano y lejano

        if (estaEnCampoDeVision)
        {
            agent.SetDestination(player.gameObject.transform.position);
            Debug.DrawRay(transform.position, player.gameObject.transform.position - transform.position, new Color(255, 0, 0, 255));
            playerInSight = true;
            if (!playerDetected)
            {
                playerDetected = true; // El zombi detecta al jugador
                ZombiManager.zManager.AlertZombis();
            }
        }
    }

    // Método que será llamado por otros zombis cuando uno detecte al jugador
    void OnPlayerDetected()
    {
        agent.stoppingDistance = 4.0f; // Mantener distancia del jugador
        if (!playerDetected)  // Si este zombi aún no ha detectado al jugador
        {
            playerDetected = true;
        }
    }

    // Método que será llamado por otros zombis cuando pierden de vista al jugador
    void OnPlayerLost()
    {
        if (playerDetected)
        {
            playerDetected = false; // Dejar de seguir al jugador si no está en la vista
            agent.stoppingDistance = 0.0f; // Mantener distancia del jugador
        }
    }

    // Función para encontrar una posición aleatoria válida en el NavMesh
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
