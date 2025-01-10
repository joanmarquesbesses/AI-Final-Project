using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombiManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static ZombiManager zManager;

    // The zombi prefab to instantiate
    public GameObject zombiPrefab;

    // Number of zombis
    public int numZombis = 20;

    // Array to store all instantiated fish
    public GameObject[] allZombis;

    public int neighbourDistance = 2;

    public Transform target;

    void Start()
    {
        // Initialize the array to hold all the zombis
        allZombis = new GameObject[numZombis];

        for (int i = 0; i < numZombis; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * 20;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, 20, NavMesh.AllAreas))
            {
                allZombis[i] = Instantiate(zombiPrefab, hit.position + transform.position, Quaternion.identity);
                allZombis[i].transform.SetParent(transform);
            }

        }

        zManager = this;
    }

    // Update is called once per frame
    void Update()
    {
        bool playerSeen = false;
        for (int i = 0; i < numZombis; i++)
        {
            if (allZombis[i].GetComponent<Frustrum>().watchingyou)
            {
                playerSeen = true;
                break;
            }
        }

        if (!playerSeen)
        {
            StopZombis();
        }
    }

    public void AlertZombis()
    {
        BroadcastMessage("ZombieScream", SendMessageOptions.DontRequireReceiver); // Notificar a otros zombis
    }

    public void StopZombis()
    {
        BroadcastMessage("BlindZombie", SendMessageOptions.DontRequireReceiver); // Notificar que se ha perdido de vista
    }

    public void SetObjective(Transform transform)
    {
        target = transform;
    }

    public Transform GetObjective()
    {
        return target;
    }
}
