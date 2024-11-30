using UnityEngine;

public class Pocket : MonoBehaviour {
    public delegate void PocketEnterHandler(Ball ball);
    public event PocketEnterHandler PocketEntered;

    void OnCollisionEnter(Collision collision) {
        var ball = collision.gameObject.GetComponent<Ball>();

        if (!ball)
            return;

        PocketEntered?.Invoke(ball);
    }
}
