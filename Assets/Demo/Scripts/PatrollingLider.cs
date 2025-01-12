using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;

public class PatrollingLider : MonoBehaviour
{
    public Transform[] waypoints;  // Array de puntos de patrullaje
    public float moveSpeed = 3f;
    public float waitTime = 1f;
    public float radiusWayPoint = 0.2f;
    public float rotationSpeed = 0.2f;

    private int currentWaypointIndex = 0;
    Transform targetWaypoint;

    public bool debug = true;

    void Start()
    {
        targetWaypoint = waypoints[currentWaypointIndex];
    }

    private void Update()
    {
        // Obtener la direcci�n hacia el waypoint objetivo
        Vector3 targetDirection = (targetWaypoint.position - transform.position).normalized;

        // Suavizar la transici�n de direcci�n
        Vector3 currentDirection = Vector3.Lerp(transform.forward, targetDirection, rotationSpeed * Time.deltaTime);

        // Mover en la direcci�n suavizada
        transform.position += currentDirection * moveSpeed * Time.deltaTime;

        // Rotar el agente para mirar hacia la direcci�n de movimiento
        transform.rotation = Quaternion.LookRotation(currentDirection);

        if (debug)
        {
            //debug current direction
            Debug.DrawLine(transform.position, transform.position + (currentDirection * 5), new Color(255, 0, 0, 255));

            //debug current direction
            Debug.DrawLine(transform.position, transform.position + (targetDirection * 5), new Color(0, 255, 0, 255));
        }

        if((targetWaypoint.position - transform.position).magnitude < radiusWayPoint)
        {
            MoveToNextPosition();
        }
    }

    void MoveToNextPosition()
    {
        if (waypoints.Length == 0) return;

        // Actualizar al siguiente �ndice, volver al inicio si es el �ltimo
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;

        // Establecer el destino al siguiente punto en el array
        targetWaypoint = waypoints[currentWaypointIndex];
    }
}
