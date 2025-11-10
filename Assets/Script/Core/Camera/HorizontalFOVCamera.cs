using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class HorizontalFOVCamera : MonoBehaviour
{
    [SerializeField] private float horizontalWidth = 16f; // world units you want visible horizontally
    private Camera cam;

    void Awake() => cam = GetComponent<Camera>();

    void LateUpdate()
    {
        float aspect = cam.aspect; // width / height
        cam.orthographicSize = horizontalWidth / aspect / 2f;
    }
}
