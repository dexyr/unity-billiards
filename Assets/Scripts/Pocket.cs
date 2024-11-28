using UnityEngine;

public class Pocket : MonoBehaviour {
    public delegate void PocketEnterHandler(Ball ball);
    public event PocketEnterHandler PocketEntered;

    GameController gameController;

    void Awake() {
        var controllerObject = GameObject.FindGameObjectWithTag("GameController");
        gameController = controllerObject.GetComponent<GameController>();
        PocketEntered += gameController.PocketEntered;
    }

    void OnDestroy() {
        PocketEntered -= gameController.PocketEntered;
    }

    void OnCollisionEnter(Collision collision) {
        var ball = collision.gameObject.GetComponent<Ball>();

        if (!ball)
            return;

        PocketEntered?.Invoke(ball);
    }
}
