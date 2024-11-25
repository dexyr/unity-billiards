using UnityEngine;

public class CueBall : MonoBehaviour {
    public delegate void BallCollisionHandler(Ball ball);
    public event BallCollisionHandler BallCollided;

    GameController gameController;

    void Awake() {
        // ?Ç∆Ç¢Ç§Nullì`î¿ÇÕÉ_ÉÅÇæÇªÇ§
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
