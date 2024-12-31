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
    private GameObject objective;
    public bool watchingyou = false;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
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
                        ZombiManager.zManager.AlertZombis();
                        agent.SetDestination(hit.transform.position);
                        playerDetected = true;
                        watchingyou = true;
                        objective = hit.collider.gameObject;
                    }
                }
            }
        }

        if (playerDetected == true)
        {
            if (objective != null)
            {
                agent.SetDestination(objective.transform.position);
            }
        }


    }

    public void ZombieScream()
    {
        playerDetected = true;
    }

    public void BlindZombie()
    {
        playerDetected = false;
    }


}
