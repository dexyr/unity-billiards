using UnityEngine;

public class CueBall : MonoBehaviour {
    public delegate void BallCollisionHandler(Ball ball);
    public event BallCollisionHandler BallCollided;

    GameController gameController;

    void Awake() {
        // ?�Ƃ���Null�`���̓_��������
        var controllerObject = GameObject.FindGameObjectWithTag("GameController");
        gameController = controllerObject.GetComponent<GameController>();

        BallCollided += gameController.CueBallCollided;
    }

    void OnDestroy() {
        BallCollided -= gameController.CueBallCollided;
    }


    void OnCollisionEnter(Collision collision) {
        var ball = collision.gameObject.GetComponent<Ball>();

        if (ball)
            BallCollided?.Invoke(ball);
    }
}
