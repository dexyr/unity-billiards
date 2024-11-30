using UnityEngine;

public class Moving : MonoBehaviour {
    public delegate void MotionHandler(Moving self, bool isMoving);
    public MotionHandler MotionUpdated;

    void FixedUpdate() {
        // if (!isMoving)
        //     return;

        float velocity = Mathf.Abs(GetComponent<Rigidbody>().velocity.magnitude);

        if (velocity > 0.01f)
            MotionUpdated?.Invoke(this, true);
        else
            MotionUpdated?.Invoke(this, false);
    }
}
