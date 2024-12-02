using UnityEngine;

public class TargetCamera : MonoBehaviour {
    public GameObject Target;

    new Camera camera;

    [SerializeField] float sensitivity = 2;

    static int angleMin = 15;
    static int angleMax = 40;

    static float aimDistanceMin = 0.1f;
    static float aimDistanceMax = 0.4f;
    static float scrollSensitivity = 0.01f;

    static float distance = -(aimDistanceMax + aimDistanceMin) / 2;

    void Update() {
        if (!Target)
            return;

        float yaw = Input.GetAxis("Mouse X") * sensitivity;
        float pitch = -Input.GetAxis("Mouse Y") * sensitivity;

        Vector3 newRotation = transform.localEulerAngles + new Vector3(pitch, yaw, 0);
        newRotation.x = Mathf.Clamp(newRotation.x, angleMin, angleMax);

        distance += Input.mouseScrollDelta.y * scrollSensitivity;
        distance = -Mathf.Clamp(-distance, aimDistanceMin, aimDistanceMax);

        transform.position = Target.transform.position;
        transform.localEulerAngles = newRotation;
        transform.Translate(new Vector3(0, 0, distance));
    }
}
