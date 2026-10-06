using UnityEngine;
using FMODUnity;

public class DriverStationAcoustics : MonoBehaviour
{
    [Header("FMOD Snapshot")]
    [Tooltip("Select your DriverStation_Muffle snapshot here")]
    public EventReference muffleSnapshot;

    private FMOD.Studio.EventInstance snapshotInstance;

    void Start()
    {
        // Pre-create the snapshot instance
        snapshotInstance = RuntimeManager.CreateInstance(muffleSnapshot);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the entering object is the Player / Main Camera
        if (other.CompareTag("MainCamera") || other.GetComponent<Camera>() != null || other.GetComponent<StudioListener>() != null)
        {
            snapshotInstance.start();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera") || other.GetComponent<Camera>() != null || other.GetComponent<StudioListener>() != null)
        {
            // STOP_MODE.ALLOWFADEOUT lets the AHDSR envelope smoothly fade the crisp field audio back in
            snapshotInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    private void OnDestroy()
    {
        snapshotInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        snapshotInstance.release();
    }
}