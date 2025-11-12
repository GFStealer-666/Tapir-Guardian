using UnityEngine;

/// <summary>
/// Dynamically adjusts volume of an AudioSource based on distance to a listener (e.g., the Tapir).
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AudioDistanceController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The Transform that hears this sound (for example, the Tapir).")]
    public Transform listener;   // Assign your Tapir’s transform

    [Header("Distance Settings")]
    [Tooltip("Distance at which the sound is maximum volume.")]
    public float maxVolumeDistance = 1f;

    [Tooltip("Distance at which the sound fades to zero.")]
    public float maxHearingDistance = 10f;

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float maxVolume = 1f;

    private AudioSource _source;

    void Awake()
    {
        _source = GetComponent<AudioSource>();
        PlayerLocator.OnPlayerSet += AssignPlayer;
    }
    private void AssignPlayer(GameObject player)
    {
        listener = player.transform;
    }
    void Update()
    {
        if (!listener || !_source) return;

        float distance = Vector2.Distance(listener.position, transform.position);
        float volumeFactor = 1f - Mathf.InverseLerp(maxVolumeDistance, maxHearingDistance, distance);
        _source.volume = Mathf.Clamp01(volumeFactor) * maxVolume;
    }
}
