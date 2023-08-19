using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastWave : MonoBehaviour
{
    public int pointsCount = 100;   // Number of points drawn for the blast wave radius
    public float maxRadius = 50;    // Max radius of blast wave
    public float speed = 5;         // Speed of blast wave
    public float startWidth = 5;    // Starting width of blast wave
    public float force = 5;         // Force of blast wave

    public float damage = 10;

    public bool waveActive = true;

    // Renderer for blast wave
    private LineRenderer lineRenderer;

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

        for (int i = 0; i < hittingObjects.Length; i++)
        {
            Rigidbody rb = hittingObjects[i].GetComponent<Rigidbody>();

            if (!rb)
            {
                continue;
            }

            // Adds force to the objects the blast hits
            Vector3 direction = (hittingObjects[i].transform.position - transform.position).normalized;
            rb.AddForce(direction * force, ForceMode.Impulse);

            // Check if damageable then apply
            Damageable damaged = hittingObjects[i].GetComponent<Damageable>();
            if (!damaged)
            {
                continue;
            }
            Debug.Log("Explosion damage");
            float modDamage = damage / currentRadius;
            damaged.Damage(modDamage);
        }
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

    // test controls
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.A)) 
    //    {
    //        StartCoroutine(Blast());
    //    }
    //}
}
