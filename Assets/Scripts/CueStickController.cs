using UnityEngine;

public class CueStickController : MonoBehaviour {
    public GameObject Target;

    [SerializeField] new GameObject camera;
    public float Sensitivity;

    CueStick cueStick;

    Vector3 stickOffset = new Vector3(0, 0, -aimDistanceMin);
    float stickSlide = 0f;

    float offsetMax = 0.02f;
    Vector3 aimOffset = new Vector3(-0.02f, 0.02f, -(aimDistanceMax + aimDistanceMin) / 2);

    static int angleMin = 10;
    static int angleMax = 45;

    static float aimDistanceMin = 0.1f;
    static float aimDistanceMax = 0.4f;
    static float scrollSensitivity = 0.01f;

    public enum ShotState { SHOT, AIM, LOOK };

    float delay = 0;

    void Awake() {
        transform.localEulerAngles = new Vector3((angleMax + angleMin) / 2, 0, 0);
        cueStick = GetComponentInChildren<CueStick>();
        cueStick.StickCollided += CueStickHit;
    }

    void OnDestroy() {
        cueStick.StickCollided -= CueStickHit;
    }

    void OnEnable() {
        delay = 0;
    }

    void OnDisable() {
        aimOffset = new Vector3(-0.02f, 0.02f, -(aimDistanceMax + aimDistanceMin) / 2);
    }

    void Update() {
        delay += Time.deltaTime;

        if (delay < 0.2f)
            return;

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

        transform.position = Target.transform.position;
        transform.Translate(stickOffset);

        camera.transform.rotation = transform.rotation;
        camera.transform.position = transform.position;
        camera.transform.Translate(aimOffset);
    }

    public void FixedUpdate() {
        var rigidbody = cueStick.GetComponent<Rigidbody>();

        if (rigidbody.isKinematic)
            return;

        if (rigidbody.velocity.magnitude < 0.01f)
            rigidbody.velocity = Vector3.zero;

        float currentZ = cueStick.transform.localPosition.z;
        float originalZ = cueStick.OriginalPosition.z;

        if (currentZ > originalZ - aimDistanceMin - 0.0001f) {
            rigidbody.AddRelativeForce(new Vector3(0, stickSlide, 0), ForceMode.Impulse);
            stickSlide = 0;
        }
        else {
            rigidbody.velocity = Vector3.zero;
            cueStick.ResetPosition();
            cueStick.transform.Translate(new Vector3(0, -aimDistanceMin, 0), Space.Self);
        }
    }

    public void CueStickHit(Collision collision, CueBall cueBall) {
        ContactPoint contact = collision.GetContact(0);
        cueBall.GetComponent<Rigidbody>().AddForceAtPosition(transform.forward * collision.relativeVelocity.magnitude / 200, contact.point, ForceMode.Impulse);
    }

    void Shot() {
        var rigidbody = cueStick.GetComponent<Rigidbody>();

        float z = Input.GetAxis("Mouse Y") * Sensitivity * 2 * rigidbody.mass;
        stickSlide = z;
    }

    void Aim() {
        float x = Input.GetAxis("Mouse X") * Sensitivity / 1000;
        float y = Input.GetAxis("Mouse Y") * Sensitivity / 1000;

        stickOffset.x += x;
        stickOffset.y += y;

        stickOffset.x = Mathf.Clamp(stickOffset.x, -offsetMax, offsetMax);
        stickOffset.y = Mathf.Clamp(stickOffset.y, -offsetMax, offsetMax);
    }

    void Look() {
        float yaw = Input.GetAxis("Mouse X") * Sensitivity;
        float pitch = -Input.GetAxis("Mouse Y") * Sensitivity;

        Vector3 newRotation = transform.localEulerAngles + new Vector3(pitch, yaw, 0);
        newRotation.x = Mathf.Clamp(newRotation.x, angleMin, angleMax);

        transform.localEulerAngles = newRotation;
    }

    void Zoom() {
        aimOffset.z += Input.mouseScrollDelta.y * scrollSensitivity;
        aimOffset.z = -Mathf.Clamp(-aimOffset.z, aimDistanceMin, aimDistanceMax);
    }
}
