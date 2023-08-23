using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastWave : MonoBehaviour
{
    public int pointsCount = 100;   // Number of points drawn for the blast wave radius
    public float maxRadius = 50;    // Max radius of blast wave
    public float speed = 5;         // Speed of blast wave
    public float startWidth = 5;    // Starting width of blast wave
    public float forceModifier = 5;         // Force of blast wave

    public float damage = 10;

    public bool waveActive = true;

    public bool disableOverlapForce = false;

    [SerializeField] private float minForce = 40f;

    // Renderer for blast wave
    private LineRenderer lineRenderer;

    private List<Collider> previousCollisions = new List<Collider>();

    // Get the renderer and set the total number of draw points for it (needs one more than point count)
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.positionCount = pointsCount + 1;
    }

    // Start Blast Wave
    private void Start()
    {
        StartCoroutine(TriggerBlast());
    }

    // Apply damage and force to all objects hit by the blast wave
    private void Damage(float currentRadius)
    {
        Collider[] hittingObjects = Physics.OverlapSphere(transform.position, currentRadius);

        if (hittingObjects.Length > 0)
        {
            Debug.Log("Hit Stuff");
        }

        for (int i = 0; i < hittingObjects.Length; i++)
        {
            if (disableOverlapForce)
            {
                bool cancel = false;
                foreach (Collider col in previousCollisions)
                {
                    if (col == hittingObjects[i])
                    {
                        cancel = true;
                        break;
                    }
                }

                if (cancel)
                {
                    continue;
                }
                else
                {
                    previousCollisions.Add(hittingObjects[i]);
                }
            }

            if (hittingObjects[i].isTrigger)
            {
                //Debug.Log("Trigger Collider");
                continue;
            }
            
            // Need to get the parent object with the rigid body.
            // (may need to rework some pickup able objects to only have one physics collider)
            //GameObject cObject = hittingObjects[i].transform.parent.gameObject;

            Transform objTransform = FindRidgidBody(hittingObjects[i].transform);

            if (objTransform == null)
            {
                continue;
            }

            Rigidbody rb = objTransform.gameObject.GetComponent<Rigidbody>();

            if (!rb)
            {
                //Debug.Log("No Force");
                continue;
            }

            // Adds force to the objects the blast hits (put the power function here)
            Vector3 direction = (hittingObjects[i].transform.position - transform.position).normalized;
            rb.AddForce(direction * DynamicForce(rb.mass), ForceMode.Impulse);
            //Debug.Log("Force applied");

            // Check if damageable then apply (fix this)
            Damageable damaged = objTransform.gameObject.GetComponent<Damageable>();

            if (!damaged)
            {
                //Debug.Log("No damage");
                continue;
            }
            //Debug.Log("Explosion damage");
            float modDamage = damage / currentRadius;
            damaged.Damage(modDamage);
        }
    }

    private Transform FindRidgidBody(Transform transform)
    {
        Rigidbody currentObject = transform.gameObject.GetComponent<Rigidbody>();
        
        if (currentObject)
        {
            return currentObject.transform;
        }

        if (transform.parent != null)
        {
            return FindRidgidBody(transform.parent);
        }

        return null;
    }
    private float DynamicForce(float objectMass)
    {
        // Throw power ideal guide:

        // Power per Units - ppu
        // CAP ppu beyond this point
        // mass 1 - 40 ppu
        // mass 5 - 25 ppu
        // mass 10 - 17 ppu
        // mass 20 - 15 ppu
        // mass 100 - 10 ppu
        // mass 500 - 8 ppu
        // mass 1000 - 5 ppu

        // Using all data points for a power function gives:
        // y = 36.904x^-0.277

        float result = 0;

        // Cap the force power for objects lighter than 1 mass
        if (objectMass < 1)
        {
            result = minForce * objectMass;
            return result;
        }
        else
        {
            float ppu = 36.904f * Mathf.Pow(objectMass, -0.277f);

            result = (ppu + forceModifier) * objectMass;
        }

        return result;
    }

    // Modifies the radius of the blast wave over time
    private IEnumerator TriggerBlast()
    {
        float currentRadius = 0f;

        while (currentRadius < maxRadius)
        {
            currentRadius += Time.deltaTime * speed;

            if (waveActive)
            {
                Draw(currentRadius);
            }

            Damage(currentRadius);

            yield return null;
        }
    }

    // Draws the visual effect of the blast wave with the line renderer
    private void Draw(float currentRadius)
    {
        float angleBetweenPoints = 360f / pointsCount;

        for (int i = 0; i <= pointsCount; i++)
        {
            float angle = i * angleBetweenPoints * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0f);
            Vector3 position = direction * currentRadius;

            lineRenderer.SetPosition(i, position);
        }

        lineRenderer.widthMultiplier = Mathf.Lerp(0f, startWidth, 1f - currentRadius / maxRadius);
    }

    //// test controls
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.A))
    //    {
    //        StartCoroutine(TriggerBlast());
    //    }
    //}
}
