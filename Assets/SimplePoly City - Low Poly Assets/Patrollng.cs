using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Velocidad del movimiento
    private NavMeshAgent agent;
    public Transform[] DestPositions; // Array de posiciones destino
    private int currentTargetIndex = 0; // Índice del destino actual

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError("No se encontró el componente NavMeshAgent en el jugador.");
            return;
        }

        if (DestPositions.Length > 0)
        {
            // Establecer el primer destino
            MoveToNextPosition();
        }
        else
        {
            Debug.LogWarning("No se han asignado posiciones destino en el array.");
        }
    }

    void Update()
    {
        // Verificar si el agente ha llegado al destino
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            MoveToNextPosition();
        }
    }

    void MoveToNextPosition()
    {
        if (DestPositions.Length == 0) return;

        // Establecer el destino al siguiente punto en el array
        agent.SetDestination(DestPositions[currentTargetIndex].position);

        // Actualizar al siguiente índice, volver al inicio si es el último
        currentTargetIndex = (currentTargetIndex + 1) % DestPositions.Length;
    }
}
