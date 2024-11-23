using UnityEngine;
using UnityEngine.Events;

public class BallCollision : MonoBehaviour {
    public delegate void BallCollisionHandler(Ball ball);
    public event BallCollisionHandler BallCollided;

    GameController gameController;

    void Awake() {
        // ?Ç∆Ç¢Ç§Nullì`î¿ÇÕÉ_ÉÅÇæÇªÇ§
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
