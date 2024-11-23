using UnityEngine;
using UnityEngine.Events;

public class BallCollision : MonoBehaviour {
    public delegate void BallCollisionHandler(Ball ball);
    public event BallCollisionHandler BallCollided;

    GameController gameController;

    void Awake() {
        // ?�Ƃ���Null�`���̓_��������
        var controllerObject = GameObject.FindGameObjectWithTag("GameController");
        gameController = controllerObject.GetComponent<GameController>();
        BallCollided += gameController.CueBallCollided;
    }

    void OnCollisionEnter(Collision collision) {
        var ball = collision.gameObject.GetComponent<Ball>();

        if (!ball)
            return;

        BallCollided?.Invoke(ball);
    }
}
