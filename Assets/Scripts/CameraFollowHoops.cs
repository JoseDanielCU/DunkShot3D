using UnityEngine;

public class CameraFollowHoops : MonoBehaviour
{
    public HoopSpawner spawner;
    public Camera cam;

    [Header("Movimiento")]
    public float smoothSpeed = 2f;

    [Header("Zoom")]
    public float minDistance = 8f;
    public float extraPadding = 2f;

    [Header("Ajustes Manuales")]
    public Vector3 offset = new Vector3(0, 2, -10);
    public Vector3 rotationOffset = Vector3.zero;

    private Vector3 smoothedMiddle;

    private void LateUpdate()
    {
        if (spawner == null || spawner.CurrentHoop == null || spawner.NextHoop == null || cam == null)
            return;

        Transform current = spawner.CurrentHoop.transform;
        Transform next = spawner.NextHoop.transform;

        //transición entre aros
        Vector3 middlePoint = (current.position + next.position) / 2f;
        smoothedMiddle = Vector3.Lerp(smoothedMiddle, middlePoint, smoothSpeed * Time.deltaTime);

        float distance = Vector3.Distance(current.position, next.position);

        Vector3 desiredPos = smoothedMiddle + offset;
        desiredPos.z -= distance * 0.5f;

        transform.position = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);

        transform.LookAt(smoothedMiddle);
        transform.rotation *= Quaternion.Euler(rotationOffset);

        float targetFOV = Mathf.Clamp(distance * 10f + extraPadding, 40f, 80f);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, smoothSpeed * Time.deltaTime);
    }
}
