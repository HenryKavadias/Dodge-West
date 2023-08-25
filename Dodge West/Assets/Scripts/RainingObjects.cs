using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainingObjects : MonoBehaviour
{
    [Header("Spawn Positioning and Amount")]
    public Transform spawnZoneReference = null;
    public Vector3 sizeOfZone = new Vector3(50f, 1f, 50f);
    public float distanceOfZone = 10;

    public int numberOfSpawnsPerUpdate = 2;

    [Header("Spawn Timer")]
    public float startTimer = 60f;
    private bool allowSpawning = false;

    public float delayTime = 5f;

    [Header("Rapid Spawn")]
    public bool rapidSpawn = true;
    public float rapidSpawnDuration = 5f;
    public int rapidSpawnCountTrigger = 5;
    private int rapidSpawnTracker = 0;
    private bool triggerRapidSpawn = false;
    private bool startRapidSpawn = false;


    //private float currentTime = 0f;

    public List<GameObject> objects = new List<GameObject>();

    private System.Random random = new System.Random();

    private void Start()
    {
        Invoke(nameof(ResetSpawnerAllowance), startTimer);
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnZoneReference && 
            objects.Count > 0 &&
            allowSpawning)
        {
            for (int i = 0; i < numberOfSpawnsPerUpdate; i++)
            {
                int objectCount = objects.Count;

                int randomiseObject = random.Next(0, objects.Count);

                float randomX = Random.Range(-(sizeOfZone.x / 2), (sizeOfZone.x / 2));
                float randomZ = Random.Range(-(sizeOfZone.z / 2), (sizeOfZone.z / 2));

                Vector3 spawn = new Vector3(randomX, 0f, randomZ);

                Instantiate(
                    objects[randomiseObject],
                    new Vector3(
                        randomX + spawnZoneReference.position.x,
                        spawnZoneReference.position.y + distanceOfZone,
                        randomZ + spawnZoneReference.position.z),
                    Quaternion.identity);
            }

            if (startRapidSpawn)
            {
                startRapidSpawn = false;
                triggerRapidSpawn = true;
                rapidSpawnTracker = 0;
                //allowSpawning = false;
                Invoke(nameof(ResetRapidSpawn), rapidSpawnDuration);

                return;
            }
            else if (triggerRapidSpawn)
            {
                return;
            }

            if (rapidSpawn)
            {
                rapidSpawnTracker++;

                if (rapidSpawnTracker >= rapidSpawnCountTrigger)
                {
                    startRapidSpawn = true;

                    allowSpawning = false;
                    Invoke(nameof(ResetSpawnerAllowance), delayTime);
                }
                else
                {
                    allowSpawning = false;
                    Invoke(nameof(ResetSpawnerAllowance), delayTime);
                }
            }
            else
            {
                allowSpawning = false;
                Invoke(nameof(ResetSpawnerAllowance), delayTime);
            }
        }

        //else if (currentTime >= startTimer)
        //{
        //    allowSpawning = true;
        //}

        //currentTime += Time.deltaTime;
    }

    void ResetSpawnerAllowance()
    {
        allowSpawning = true;
    }

    void ResetRapidSpawn()
    {
        triggerRapidSpawn = false;
    }

    private void OnDrawGizmos()
    {
        //float distanceOfZone = 0;

        Gizmos.color = Color.red;

        if (spawnZoneReference != null)
        {
            Gizmos.DrawWireCube(
                        spawnZoneReference.position + 
                        (Vector3.up * distanceOfZone),
                        sizeOfZone);
        }
    }
}
