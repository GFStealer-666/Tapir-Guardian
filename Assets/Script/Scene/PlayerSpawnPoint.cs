using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    public string spawnId = "Spawn_Default";
    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 0.25f);
    }
}
