using UnityEngine;

public class CueStick : MonoBehaviour {
    GameController gameController;
    CueStickController cueStickController;

    public delegate void StickCollisionHandler();
    public event StickCollisionHandler StickCollided;

    CapsuleCollider capsuleCollider;
    Vector3 originalPosition;
    Vector3 originalRotation;

    // Žè“®‚É‘¬“x‚ðŒvŽZ‚·‚é

    void Awake() {
        var controllerObject = GameObject.FindGameObjectWithTag("GameController");
        gameController = controllerObject.GetComponent<GameController>();

        cueStickController = GetComponentInParent<CueStickController>();

        StickCollided += gameController.StickCollided;
        StickCollided += cueStickController.StickCollided;

        capsuleCollider = GetComponent<CapsuleCollider>();

        originalPosition = transform.localPosition;
        originalRotation = transform.localEulerAngles;
    }

    void OnDestroy() {
        StickCollided -= gameController.StickCollided;
        StickCollided -= cueStickController.StickCollided;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            capsuleCollider.enabled = true;
            GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    void OnCollisionEnter(Collision collision) {
        var cueBall = collision.gameObject.GetComponent<CueBall>();

        if (cueBall) {
            Debug.Log(collision.relativeVelocity);

            capsuleCollider.enabled = false;
            GetComponent<Rigidbody>().isKinematic = true;

            StickCollided?.Invoke();
        }
    }

    public void ResetPosition() {
        transform.localPosition = originalPosition;
        transform.localEulerAngles = originalRotation;
    }
}
