using UnityEngine;

public class Cushion : MonoBehaviour {
    public delegate void CushionHitHandler(Ball ball);
    public event CushionHitHandler CushionHit;

    void OnCollisionEnter(Collision collision) {
        var ball = collision.gameObject.GetComponent<Ball>();

        if (!ball)
            return;

        CushionHit?.Invoke(ball);
    }
}
