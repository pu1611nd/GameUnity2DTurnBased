using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraFollowTilemap : MonoBehaviour
{
    [Header("Target")]
    public Transform target; // Nhân vật cần camera theo dõi

    [Header("Camera Settings")]
    public Vector3 offset = new Vector3(0, 0, -10);
    public float smoothSpeed = 0.15f;

    [Header("Tilemap")]
    public Tilemap tilemap; // Kéo tilemap vào đây trong Inspector

    private Vector2 minBounds;
    private Vector2 maxBounds;
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;

        if (tilemap != null)
        {
            // Lấy giới hạn tilemap trong không gian thế giới
            Bounds bounds = tilemap.localBounds;

            // Chuyển sang Vector2 vì ta chỉ cần x và y
            minBounds = new Vector2(bounds.min.x, bounds.min.y);
            maxBounds = new Vector2(bounds.max.x, bounds.max.y);
        }
        else
        {
            Debug.LogWarning("⚠️ Chưa gán Tilemap cho CameraFollowTilemap!");
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Vị trí mong muốn của camera
        Vector3 desiredPosition = target.position + offset;

        if (tilemap != null)
        {
            // Tính nửa kích thước camera theo chiều cao/rộng
            float camHalfHeight = cam.orthographicSize;
            float camHalfWidth = cam.aspect * camHalfHeight;

            // Giới hạn camera không ra khỏi tilemap
            desiredPosition.x = Mathf.Clamp(desiredPosition.x,
                minBounds.x + camHalfWidth,
                maxBounds.x - camHalfWidth);

            desiredPosition.y = Mathf.Clamp(desiredPosition.y,
                minBounds.y + camHalfHeight,
                maxBounds.y - camHalfHeight);
        }

        // Làm mượt chuyển động camera
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
