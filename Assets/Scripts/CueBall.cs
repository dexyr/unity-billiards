using UnityEngine;
using UnityEngine.Events;

public class BallCollision : MonoBehaviour {
    public delegate void BallCollisionHandler(Ball ball);
    public event BallCollisionHandler BallCollided;

    public delegate void StickCollisionHandler(CueStick stick);
    public event StickCollisionHandler StickCollided;

    GameController gameController;

    void Awake() {
        // ?Ç∆Ç¢Ç§Nullì`î¿ÇÕÉ_ÉÅÇæÇªÇ§
        var controllerObject = GameObject.FindGameObjectWithTag("GameController");
        gameController = controllerObject.GetComponent<GameController>();

        BallCollided += gameController.CueBallCollided;
        StickCollided += gameController.StickCollided;
    }


    void OnCollisionEnter(Collision collision) {
        var ball = collision.gameObject.GetComponent<Ball>();
        var stick = collision.gameObject.GetComponentInParent<CueStick>();

        Debug.Log($"{ball}, {stick}");

        if (ball)
            BallCollided?.Invoke(ball);

        if (stick) {
            GetComponent<Rigidbody>().AddForce(collision.impulse * 20, ForceMode.Impulse);
            stick.GetComponentInChildren<CapsuleCollider>().enabled = false;
            StickCollided?.Invoke(stick);
        }
    }
}
