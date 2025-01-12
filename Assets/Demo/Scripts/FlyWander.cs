using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyWander : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform cubeTransform;
    private Vector3 destPos;
    public int destDistance = 2;
    public int moveSpeed = 5;
    public float rotationSpeed = 5;
    private float xMin, xMax;
    private float yMin, yMax;
    private float zMin, zMax;

    void Start()
    {
        CalculateCubeLimits();
        destPos = new Vector3(Random.Range(xMin, xMax), Random.Range(yMin, yMax), Random.Range(zMin, zMax));
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(transform.position, destPos) < destDistance)
        {
            destPos = new Vector3(Random.Range(xMin, xMax), Random.Range(yMin, yMax), Random.Range(zMin, zMax));
        }

        // Obtener la dirección hacia el waypoint objetivo
        Vector3 targetDirection = (destPos - transform.position).normalized;

        // Suavizar la transición de dirección
        Vector3 currentDirection = Vector3.Lerp(transform.forward, targetDirection, rotationSpeed * Time.deltaTime);

        // Mover en la dirección suavizada
        transform.position += currentDirection * moveSpeed * Time.deltaTime;

        // Rotar el agente para mirar hacia la dirección de movimiento
        transform.rotation = Quaternion.LookRotation(currentDirection);

        //debug current direction
        //Debug.DrawLine(transform.position, transform.position + (targetDirection * Vector3.Distance(transform.position, destPos)), new Color(0, 255, 0, 255));

    }

    public void CalculateCubeLimits()
    {
        if (cubeTransform == null)
        {
            Debug.LogError("El Transform del cubo no está asignado.");
            return;
        }

        // Centro del cubo
        Vector3 cubeCenter = cubeTransform.position;

        // Tamaño del cubo (localScale representa las dimensiones del cubo)
        Vector3 cubeSize = cubeTransform.localScale;

        // Cálculo de los límites en cada eje
        xMin = cubeCenter.x - (cubeSize.x / 2f);
        xMax = cubeCenter.x + (cubeSize.x / 2f);

        yMin = cubeCenter.y - (cubeSize.y / 2f);
        yMax = cubeCenter.y + (cubeSize.y / 2f);

        zMin = cubeCenter.z - (cubeSize.z / 2f);
        zMax = cubeCenter.z + (cubeSize.z / 2f);
    }
}
