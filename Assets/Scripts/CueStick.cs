using UnityEngine;

public class CueStick : MonoBehaviour {
    GameController gameController;
    CueStickController cueStickController;
    [SerializeField] DebugUI debugUI;

    public delegate void StickCollisionHandler(float hitVelocity);
    public event StickCollisionHandler StickCollided;

    CapsuleCollider capsuleCollider;
    BoxCollider boxCollider;
    Vector3 originalPosition;
    Vector3 originalRotation;

    // Žè“®‚É‘¬“x‚ðŒvŽZ‚·‚é

    void Awake() {
        var controllerObject = GameObject.FindGameObjectWithTag("GameController");
        gameController = controllerObject.GetComponent<GameController>();

        cueStickController = GetComponentInParent<CueStickController>();

        StickCollided += gameController.StickCollided;
        StickCollided += cueStickController.StickCollided;
        StickCollided += debugUI.UpdateHitVelocity;

        capsuleCollider = GetComponent<CapsuleCollider>();
        boxCollider = GetComponent<BoxCollider>();

        originalPosition = transform.localPosition;
        originalRotation = transform.localEulerAngles;
    }

    void OnDestroy() {
        StickCollided -= gameController.StickCollided;
        StickCollided -= cueStickController.StickCollided;
        StickCollided -= debugUI.UpdateHitVelocity;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            capsuleCollider.enabled = true;
            boxCollider.enabled = true;
            GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    void OnCollisionEnter(Collision collision) {
        var cueBall = collision.gameObject.GetComponent<CueBall>();

        if (cueBall) {
            Debug.Log("hit");
            Debug.Log(collision.relativeVelocity.magnitude);
            Debug.Log(collision.relativeVelocity);

            capsuleCollider.enabled = false;
            boxCollider.enabled = false;
            GetComponent<Rigidbody>().isKinematic = true;

            StickCollided?.Invoke(collision.relativeVelocity.magnitude);
        }
    }

    public void ResetPosition() {
        transform.localPosition = originalPosition;
        transform.localEulerAngles = originalRotation;
    }
}
