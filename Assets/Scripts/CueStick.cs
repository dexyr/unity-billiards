using UnityEngine;

public class CueStick : MonoBehaviour {
    public delegate void StickCollisionHandler(Collision collision, CueBall cueBall);
    public event StickCollisionHandler StickCollided;

    CapsuleCollider capsuleCollider;
    BoxCollider boxCollider;
    new Rigidbody rigidbody;
    public Vector3 OriginalPosition;
    Vector3 originalRotation;

    void Awake() {
        capsuleCollider = GetComponent<CapsuleCollider>();
        boxCollider = GetComponent<BoxCollider>();
        rigidbody = GetComponent<Rigidbody>();

        OriginalPosition = transform.localPosition;
        originalRotation = transform.localEulerAngles;
    }

    void OnEnable() {
        Unfreeze();
    }

    void OnDisable() {
        ResetPosition();
    }

    void OnCollisionEnter(Collision collision) {
        var cueBall = collision.gameObject.GetComponent<CueBall>();

        if (!cueBall)
            return;

        if (rigidbody.isKinematic)
            return;

        Freeze();
        StickCollided?.Invoke(collision, cueBall);
    }

    void Unfreeze() {
        capsuleCollider.enabled = true;
        boxCollider.enabled = true;
        rigidbody.isKinematic = false;
        rigidbody.velocity = Vector3.zero;
        rigidbody.Sleep();
    }

    void Freeze() {
        capsuleCollider.enabled = false;
        boxCollider.enabled = false;
        rigidbody.isKinematic = true;
    }

    public void ResetPosition() {
        transform.localPosition = OriginalPosition;
        transform.localEulerAngles = originalRotation;
    }
}
