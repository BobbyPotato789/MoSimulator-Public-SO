using UnityEngine;
using FMODUnity;

public class FuelAudioCollision : MonoBehaviour
{
    [Header("FMOD Settings")]
    public EventReference fuelImpactEvent;
    
    [Header("Physics Settings")]
    public float minImpactVelocity = 1.0f; 
    
    [Tooltip("The Unity Tag applied to your ground/carpet object.")]
    public string floorTag = "Floor"; 

    [Header("Spam Prevention")]
    public float audioCooldown = 0.1f; 
    private float lastPlayTime = 0f;

    private void OnCollisionEnter(Collision collision)
    {
        // ONLY calculate the vertical (Y-axis) impact speed, ignoring forward launch momentum
        float verticalVelocity = Mathf.Abs(collision.relativeVelocity.y);

        // Check cooldown, minimum speed, AND make sure it is ONLY hitting the designated floor
        if (Time.time >= lastPlayTime + audioCooldown && verticalVelocity >= minImpactVelocity)
        {
            if (collision.gameObject.CompareTag(floorTag))
            {
                lastPlayTime = Time.time;

                // Math to convert m/s to IRL feet
                float dropHeightMeters = (verticalVelocity * verticalVelocity) / 19.62f;
                float dropHeightFeet = dropHeightMeters * 3.28084f;
                dropHeightFeet = Mathf.Clamp(dropHeightFeet, 0f, 10.0f);

                // Play the FMOD Event
                FMOD.Studio.EventInstance impactInstance = RuntimeManager.CreateInstance(fuelImpactEvent);
                impactInstance.set3DAttributes(RuntimeUtils.To3DAttributes(collision.GetContact(0).point));
                impactInstance.setParameterByName("Drop_Height", dropHeightFeet);
                impactInstance.start();
                impactInstance.release();
            }
            // FUTURE PROOFING: You can easily add an "else if" here later for the Driver Station tag
        }
    }
}