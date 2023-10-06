using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Rains objects from the sky after a timer delay
public class RainingObjects : MonoBehaviour
{
    [SerializeField]
    private Timer timer;
    
    [Header("Spawn Positioning and Amount")]
    public Transform spawnZoneReference = null;
    public Vector3 sizeOfZone = new Vector3(50f, 1f, 50f);
    public float distanceOfZone = 10;
    public int numberOfSpawnsPerUpdate = 2;
    public bool enableSpawnIncrease = true;
    public int spawnIncreaseTrigger = 5;
    public int spawnIncreaseAmount = 1;
    public int objectSpawnCap = 10;
    private int spawnIncrease = 0;
    private int spawnIncreaseTracker = 0;

    [Header("Spawn Timer")]
    public float startTimer = 60f;
    private bool allowSpawning = false;

    public float delayTime = 5f;

    public List<GameObject> standardObjects = new List<GameObject>();

    [Header("Rapid Spawn")]
    public bool enableRapidSpawn = true;
    public float rapidSpawnDuration = 5f;
    // Re-purposed for spawn increase if it is enabled but not rapid spawn
    public int rapidSpawnTrackerTrigger = 5;
    public bool altRapidSpawnObjects = true;
    private int rapidSpawnTracker = 0;       // See above comment
    private bool triggerRapidSpawn = false;
    private bool startRapidSpawn = false;

    public List<GameObject> rapidObjects = new List<GameObject>();

    [Header("Mega Bomb Spawn")]
    public bool enableMegaBomb = true;
    public GameObject megaBomb = null;
    public int megaBombTrigger = 100;

    private int spawnCycleCount = 0;

    private System.Random random = new System.Random();

    // Starts the intial countdown until objects will start raining
    public void BeginCountDown()
    {
        if(timer != null)
        {
            timer.SetAndStart(startTimer, true);
        }
        Invoke(nameof(ResetSpawnerAllowance), startTimer);
    }

    // Spawns objects in a random object in a given list in a random area inside a box area
    private void SpawnCycle(List<GameObject> objectList)
    {
        // Spawns 1 object in the centre of the box area. Overrides normal spawn
        if (megaBomb != null && enableMegaBomb && spawnCycleCount >= megaBombTrigger)
        {
            Instantiate(
                megaBomb,
            new Vector3(
                    spawnZoneReference.position.x,
                    spawnZoneReference.position.y + distanceOfZone,
                    spawnZoneReference.position.z), 
                Quaternion.identity);

            return;
        }
        
        // Spawns the set number of objects randomly in the space of a in a box area.
        // The object spawned is randomly selected from the given list
        for (int i = 0; i < (numberOfSpawnsPerUpdate + spawnIncrease); i++)
        {
            int randomiseObject = random.Next(0, objectList.Count);

            float randomX = UnityEngine.Random.Range(-(sizeOfZone.x / 2), (sizeOfZone.x / 2));
            float randomY = UnityEngine.Random.Range(-(sizeOfZone.y / 2), (sizeOfZone.y / 2));
            float randomZ = UnityEngine.Random.Range(-(sizeOfZone.z / 2), (sizeOfZone.z / 2));

            Instantiate(
                objectList[randomiseObject],
                new Vector3(
                    randomX + spawnZoneReference.position.x,
                    randomY + spawnZoneReference.position.y + distanceOfZone,
                    randomZ + spawnZoneReference.position.z),
                Quaternion.identity);
        }
    }

    // There are 3 types of spawns:
    // - Standard: spawns a set number of objects from the standard list after a delay. The amount increases over time
    // - Rapid: same as standard, but disables the delay for a time frame and uses the rapid spawn list of objects
    // - Mega Bomb: spawns one type of object until the game is over. Usually a bomb the kills all the players.
    // 
    // Standard is the normal spawn cycle used. After every few spawns the Rapid spawn will trigger.
    // After a set number of standard spawns the Mega bomb spawn will start and remain that way until the end of the game.
    void Update()
    {
        if (spawnZoneReference &&
            standardObjects.Count > 0 &&
            allowSpawning)
        {
            if (startRapidSpawn)
            {
                startRapidSpawn = false;
                triggerRapidSpawn = true;
                rapidSpawnTracker = 0;
                Invoke(nameof(ResetRapidSpawn), rapidSpawnDuration);

                return;
            }
            
            if (triggerRapidSpawn && 
                rapidObjects.Count > 0)
            {
                if (altRapidSpawnObjects)
                {
                    SpawnCycle(rapidObjects);
                }
                else
                {
                    SpawnCycle(standardObjects);
                }
                return;
            }
            else
            {
                SpawnCycle(standardObjects);
            }

            if (enableRapidSpawn && spawnCycleCount < megaBombTrigger)
            {
                rapidSpawnTracker++;

                if (rapidSpawnTracker >= rapidSpawnTrackerTrigger)
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
    }

    void ResetSpawnerAllowance()
    {
        allowSpawning = true;

        if (enableSpawnIncrease && 
            (spawnIncrease + numberOfSpawnsPerUpdate) < objectSpawnCap)
        {
            spawnIncreaseTracker++;

            if (spawnIncreaseTracker >= spawnIncreaseTrigger)
            {
                spawnIncrease += spawnIncreaseAmount;
                spawnIncreaseTrigger += spawnIncreaseTracker;
            }
        }

        if (enableMegaBomb)
        {
            spawnCycleCount++;
        }
    }

    void ResetRapidSpawn()
    {
        triggerRapidSpawn = false;
    }

    // Draws the wire frame of the spawn area in the editor
    private void OnDrawGizmos()
    {
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
