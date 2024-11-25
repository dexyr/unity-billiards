using UnityEngine;

public class CueStickController : MonoBehaviour {
    [SerializeField] GameObject target;
    [SerializeField] GameObject camera;
    [SerializeField] float sensitivity = 2;

    CueStick cueStick;

    Vector3 stickOffset = new Vector3(0, 0, -aimDistanceMin);
    float stickSlide = 0f;

    float offsetMax = 0.02f;
    Vector3 aimOffset = new Vector3(-0.02f, 0.02f, -(aimDistanceMax + aimDistanceMin) / 2);

    static float aimDistanceMin = 0.1f;
    static float aimDistanceMax = 0.4f;
    static float scrollSensitivity = 0.01f;

    static int angleMin = 15;
    static int angleMax = 40;

    public enum ShotState { SHOT, AIM, LOOK };

    void Awake() {
        transform.localEulerAngles = new Vector3((angleMax + angleMin) / 2, 0, 0);
        cueStick = GetComponentInChildren<CueStick>();
    }

    void Update() {
        stickSlide = 0;

        var state = ShotState.AIM;
        if (Input.GetMouseButton(0))
            state = ShotState.SHOT;
        if (Input.GetMouseButton(1))
            state = ShotState.LOOK;

        switch (state) {
        case ShotState.SHOT:
            Shot();
            break;

        case ShotState.AIM:
            Aim();
            Zoom();
            break;

        case ShotState.LOOK:
            Look();
            Zoom();
            break;
        }

        transform.position = target.transform.position;
        transform.Translate(stickOffset);

        camera.transform.rotation = transform.rotation;
        camera.transform.position = transform.position;
        camera.transform.Translate(aimOffset);

        // transform.Translate(new Vector3(0, 0, stickSlide));
    }

    public void FixedUpdate() {
        var rigidbody = cueStick.GetComponent<Rigidbody>();

        if (rigidbody.isKinematic)
            return;

        if (stickSlide == 0)
            rigidbody.velocity *= 0.5f;

        if (rigidbody.velocity.magnitude < 0.01f)
            rigidbody.velocity = Vector3.zero;

        rigidbody.AddRelativeForce(new Vector3(0, stickSlide, 0), ForceMode.Impulse);
    }

    public void StickCollided() {
        ResetAim();
    }

    public void ResetAim() {
        aimOffset = new Vector3(-0.02f, 0.02f, -(aimDistanceMax + aimDistanceMin) / 2);

        cueStick.ResetPosition();
    }

    void Shot() {
        float z = Input.GetAxis("Mouse Y") * sensitivity / 2;
        stickSlide = z;
        
        // stickSlide += z;
        // stickSlide = Mathf.Clamp(stickSlide, -(aimDistanceMin + aimDistanceMax) / 2, aimDistanceMin * 2);
    }

    void Aim() {
        float x = Input.GetAxis("Mouse X") * sensitivity / 1000;
        float y = Input.GetAxis("Mouse Y") * sensitivity / 1000;

        stickOffset.x += x;
        stickOffset.y += y;

        stickOffset.x = Mathf.Clamp(stickOffset.x, -offsetMax, offsetMax);
        stickOffset.y = Mathf.Clamp(stickOffset.y, -offsetMax, offsetMax);
    }

    void Look() {
        float yaw = Input.GetAxis("Mouse X") * sensitivity;
        float pitch = -Input.GetAxis("Mouse Y") * sensitivity;

        Vector3 newRotation = transform.localEulerAngles + new Vector3(pitch, yaw, 0);
        newRotation.x = Mathf.Clamp(newRotation.x, angleMin, angleMax);

        transform.localEulerAngles = newRotation;
    }

    void Zoom() {
        aimOffset.z += Input.mouseScrollDelta.y * scrollSensitivity;
        aimOffset.z = -Mathf.Clamp(-aimOffset.z, aimDistanceMin, aimDistanceMax);
    }
}
