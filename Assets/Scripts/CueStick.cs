using UnityEngine;

public class CueStick : MonoBehaviour {
    CueStickController cueStickController;
    [SerializeField] DebugUI debugUI;

    public delegate void StickCollisionHandler(float hitVelocity);
    public event StickCollisionHandler StickCollided;

    CapsuleCollider capsuleCollider;
    BoxCollider boxCollider;
    new Rigidbody rigidbody;
    Vector3 originalPosition;
    Vector3 originalRotation;

    // Žè“®‚É‘¬“x‚ðŒvŽZ‚·‚é

    void Awake() {
        var controllerObject = GameObject.FindGameObjectWithTag("GameController");

        cueStickController = GetComponentInParent<CueStickController>();

        StickCollided += cueStickController.StickCollided;
        StickCollided += debugUI.UpdateHitVelocity;

        capsuleCollider = GetComponent<CapsuleCollider>();
        boxCollider = GetComponent<BoxCollider>();
        rigidbody = GetComponent<Rigidbody>();

        originalPosition = transform.localPosition;
        originalRotation = transform.localEulerAngles;
    }

    void OnDestroy() {
        StickCollided -= cueStickController.StickCollided;
        StickCollided -= debugUI.UpdateHitVelocity;
    }

    void OnCollisionEnter(Collision collision) {
        var cueBall = collision.gameObject.GetComponent<CueBall>();

        if (cueBall) {
            Debug.Log("hit");
            Debug.Log(collision.relativeVelocity.magnitude);
            Debug.Log(collision.relativeVelocity);

            capsuleCollider.enabled = false;
            boxCollider.enabled = false;
            rigidbody.isKinematic = true;

            StickCollided?.Invoke(collision.relativeVelocity.magnitude);
        }
    }

    public void Enable() {
        capsuleCollider.enabled = true;
        boxCollider.enabled = true;
        rigidbody.isKinematic = false;
    }

    public void ResetPosition() {
        transform.localPosition = originalPosition;
        transform.localEulerAngles = originalRotation;
    }
}
