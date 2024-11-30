using UnityEngine;

public class CueStick : MonoBehaviour {
    public delegate void StickCollisionHandler(float hitVelocity);
    public event StickCollisionHandler StickCollided;

    CapsuleCollider capsuleCollider;
    BoxCollider boxCollider;
    new Rigidbody rigidbody;
    Vector3 originalPosition;
    Vector3 originalRotation;

    // Žè“®‚É‘¬“x‚ðŒvŽZ‚·‚é

    void Awake() {
        capsuleCollider = GetComponent<CapsuleCollider>();
        boxCollider = GetComponent<BoxCollider>();
        rigidbody = GetComponent<Rigidbody>();

        originalPosition = transform.localPosition;
        originalRotation = transform.localEulerAngles;
    }

    void OnEnable() {
        Unfreeze();
    }

    void OnDisable() {
        resetPosition();
    }

    void OnCollisionEnter(Collision collision) {
        var cueBall = collision.gameObject.GetComponent<CueBall>();

        if (!cueBall)
            return;

        Freeze();
        StickCollided?.Invoke(collision.relativeVelocity.magnitude);
    }

    void Unfreeze() {
        capsuleCollider.enabled = true;
        boxCollider.enabled = true;
        rigidbody.isKinematic = false;
    }

    void Freeze() {
        capsuleCollider.enabled = false;
        boxCollider.enabled = false;
        rigidbody.isKinematic = true;
    }

    void resetPosition() {
        transform.localPosition = originalPosition;
        transform.localEulerAngles = originalRotation;
    }
}
