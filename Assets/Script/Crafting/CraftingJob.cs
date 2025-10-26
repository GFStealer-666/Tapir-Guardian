using UnityEngine;

public enum CraftState { InProgress, Completed }

[System.Serializable]
public class CraftingJob
{
    public int id;
    public CraftingRecipeSO recipe;
    public string stationId;
    public float startTime;  // Time.time when started
    public float endTime;    // startTime + craftSeconds
    public CraftState state;

    public float Remaining => Mathf.Max(0f, endTime - Time.time);
    public float Progress01 => Mathf.Clamp01(
        (Time.time - startTime) / Mathf.Max(0.0001f, endTime - startTime)
    );
    public bool IsDone => Time.time >= endTime;
}
