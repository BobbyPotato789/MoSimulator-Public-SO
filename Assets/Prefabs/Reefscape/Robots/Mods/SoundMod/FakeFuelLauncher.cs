using UnityEngine;

public class FakeFuelLauncher : MonoBehaviour
{
    [Header("Spawner Setup")]
    public GameObject fuelPrefab;
    public Transform[] spawnPoints; 

    [Header("Launch Physics Baseline")]
    public float baseLaunchVelocity = 8.0f; 
    public float baseUpwardAngle = 35.0f; 

    [Header("Randomization Settings (Realism)")]
    [Tooltip("How many degrees up or down the angle can deviate.")]
    public float pitchVariance = 2.0f; 
    [Tooltip("How many degrees left or right the ball can veer.")]
    public float spreadVariance = 1.5f; 
    [Tooltip("How much faster or slower (m/s) the ball can exit.")]
    public float velocityVariance = 0.5f; 

    [Header("Firing Logic")]
    public float minBPS = 28.0f; 
    public float maxBPS = 34.0f; 
    
    [Header("Hopper Settings")]
    public int hopperCapacity = 60; 
    
    private float nextFireTime = 0f;
    private int lastSlotIndex = -1;
    private int ballsFired = 0; 

    void Update()
    {
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
            // Calculate randomized physics variables for this specific ball
            float randomSpeed = baseLaunchVelocity + Random.Range(-velocityVariance, velocityVariance);
            float randomPitch = baseUpwardAngle + Random.Range(-pitchVariance, pitchVariance);
            float randomYaw = Random.Range(-spreadVariance, spreadVariance);

            // Apply the randomized Pitch (Up/Down) and Yaw (Left/Right)
            Quaternion pitchRotation = Quaternion.AngleAxis(-randomPitch, spawnLoc.right);
            Quaternion yawRotation = Quaternion.AngleAxis(randomYaw, spawnLoc.up);
            
            // Combine rotations with the spawn point's forward facing direction
            Vector3 launchDirection = yawRotation * pitchRotation * spawnLoc.forward;

            // Fire the ball
            rb.AddForce(launchDirection * randomSpeed, ForceMode.VelocityChange);
        }
    }
}