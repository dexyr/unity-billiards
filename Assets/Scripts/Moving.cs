using UnityEngine;

public class Moving : MonoBehaviour {
    public delegate void MotionHandler(Moving self, bool isMoving);
    public MotionHandler MotionUpdated;

    void FixedUpdate() {
        float velocity = Mathf.Abs(GetComponent<Rigidbody>().velocity.magnitude);

        if (velocity > 0.005f)
            MotionUpdated?.Invoke(this, true);
        else
            MotionUpdated?.Invoke(this, false);
    }
}
