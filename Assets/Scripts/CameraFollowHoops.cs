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
    public Vector3 offset = new Vector3(0, 5, -15); 
    public Vector3 rotationOffset = new Vector3(15, 0, 0);

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
        desiredPos.z -= distance * 0.3f;

        transform.position = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);

        transform.LookAt(smoothedMiddle);
        transform.rotation *= Quaternion.Euler(rotationOffset);

        float targetFOV = Mathf.Clamp(distance * 8f + extraPadding, 50f, 70f);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, smoothSpeed * Time.deltaTime);
    }
}
