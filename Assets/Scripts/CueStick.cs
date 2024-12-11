using System.Collections.Generic;

using UnityEngine;

public class CueStick : MonoBehaviour {
    public delegate void StickCollisionHandler(Collision collision, CueBall cueBall);
    public event StickCollisionHandler StickCollided;

    [SerializeField] Material stickMaterial, ghostMaterial;
    List<GameObject> collisions = new List<GameObject>();

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
        var ball = collision.gameObject.GetComponent<Ball>();

        if (collision.gameObject.CompareTag("Table"))
            DisableCollision(collision.gameObject);

        if (!cueBall)
            return;

        if (rigidbody.isKinematic)
            return;

        Freeze();
        StickCollided?.Invoke(collision, cueBall);
    }

    void OnCollisionExit(Collision collision) {
        if (!collision.gameObject.CompareTag("Table"))
            return;

        EnableCollision(collision.gameObject);
        ResetPosition();
    }

    void OnTriggerEnter(Collider collider) {
        var cueBall = collider.gameObject.GetComponent<CueBall>();

        if (cueBall)
            return;

        DisableCollision(collider.gameObject);
    }

    void OnTriggerExit(Collider collider) {
        var cueBall = collider.gameObject.GetComponent<CueBall>();

        if (cueBall)
            return;

        EnableCollision(collider.gameObject);
        ResetPosition();
    }


    void DisableCollision(GameObject gameObject) {
        if (collisions.Contains(gameObject))
            collisions.Add(gameObject);

        GetComponent<Renderer>().material = ghostMaterial;
        capsuleCollider.isTrigger = true;
        boxCollider.isTrigger = true;
        rigidbody.isKinematic = true;
    }

    void EnableCollision(GameObject gameObject) {
        collisions.Remove(gameObject);

        if (collisions.Count != 0)
            return;

        GetComponent<Renderer>().material = stickMaterial;
        capsuleCollider.isTrigger = false;
        boxCollider.isTrigger = false;
        rigidbody.isKinematic = false;
    }

    void Unfreeze() {
        GetComponent<Renderer>().material = stickMaterial;
        capsuleCollider.isTrigger = false;
        boxCollider.isTrigger = false;

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
