using UnityEngine;

public class FakeFuelLauncher : MonoBehaviour
{
    [Header("Spawner Setup")]
    public GameObject fuelPrefab;
    public Transform[] spawnPoints; 

    [Header("Launch Physics")]
    public float launchVelocity = 8.0f; 
    public float upwardAngle = 35.0f; 

    [Header("Firing Logic")]
    [Tooltip("Minimum Balls Per Second (e.g., 20)")]
    public float minBPS = 28.0f; 
    [Tooltip("Maximum Balls Per Second (e.g., 34)")]
    public float maxBPS = 34.0f; 
    
    [Header("Hopper Settings")]
    public int hopperCapacity = 60; 
    
    private float nextFireTime = 0f;
    private int lastSlotIndex = -1;
    private int ballsFired = 0; 

    void Update()
    {
        // Reload logic
        if (Input.GetKeyDown(KeyCode.R))
        {
            ballsFired = 0;
            nextFireTime = Time.time; 
            Debug.Log("Hopper Reloaded!");
        }

        while (Time.time >= nextFireTime && spawnPoints.Length > 0 && ballsFired < hopperCapacity)
        {
            FireSingleBall();
            ballsFired++; 

            // Calculate the delay for the NEXT ball based on a randomized BPS
            float currentBPS = Random.Range(minBPS, maxBPS);
            float fireInterval = 1.0f / currentBPS;
            
            nextFireTime += fireInterval;
        }
    }

    void FireSingleBall()
    {
        int chosenSlot = Random.Range(0, spawnPoints.Length);
        
        if (spawnPoints.Length > 1 && chosenSlot == lastSlotIndex)
        {
            chosenSlot = (chosenSlot + 1) % spawnPoints.Length;
        }
        lastSlotIndex = chosenSlot;

        Transform spawnLoc = spawnPoints[chosenSlot];

        GameObject fuel = Instantiate(fuelPrefab, spawnLoc.position, spawnLoc.rotation);
        
        Rigidbody rb = fuel.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 launchDirection = Quaternion.AngleAxis(-upwardAngle, spawnLoc.right) * spawnLoc.forward;
            rb.AddForce(launchDirection * launchVelocity, ForceMode.VelocityChange);
        }
    }
}