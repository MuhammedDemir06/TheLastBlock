using DG.Tweening;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera Instance;

    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);

    [SerializeField] private Transform cameraHolder;

    private void Awake()
    {
        Instance = this;
    }
    public void Shake(float duration = 0.2f, float strength = 0.3f, int vibrato = 10)
    {
        cameraHolder.DOComplete();
        cameraHolder.DOShakePosition(duration, strength, vibrato);
    }
    private void FixedUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, offset.z);

        //desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
