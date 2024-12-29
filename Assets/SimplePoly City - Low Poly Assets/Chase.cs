using UnityEngine;
using UnityEngine.AI;

public class Seek : MonoBehaviour
{
    private NavMeshAgent agent;
    public Transform DestPositions; // Array de posiciones destino

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError("No se encontró el componente NavMeshAgent en el jugador.");
            return;
        }
    }

    void Update()
    {
        // Verificar si el agente ha llegado al destino
        agent.SetDestination(DestPositions.position);
    }
}
