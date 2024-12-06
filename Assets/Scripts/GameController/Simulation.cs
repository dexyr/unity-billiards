using UnityEngine;
using System.Collections.Generic;

public class Simulation : GameState {
    float timer = 0; // ボールが当たった瞬間(movingに入っていないところ)に状態移動しないために;
    public List<Moving> Moving = new List<Moving>();
    List<Ball> pocketedNow = new List<Ball>();
    Ball firstTouched;
    CallInfo? call;

    public Simulation(GameController game, CallInfo? call=null) : base(game) {
        this.call = call;
    }

    public override void Enter() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        foreach (Pocket p in game.Pockets)
            p.PocketEntered += PocketEntered;

        foreach (var m in GameObject.FindObjectsOfType<Moving>())
            m.MotionUpdated += BallMotion;

        game.CueBall.BallCollided += CueBallCollided;
    }

    public override void Exit() {
        foreach (Pocket p in game.Pockets)
            p.PocketEntered -= PocketEntered;

        foreach (var m in GameObject.FindObjectsOfType<Moving>())
            m.MotionUpdated -= BallMotion;

        game.CueBall.BallCollided -= CueBallCollided;
    }

    public override void Update() {
        timer += Time.deltaTime;

        if (timer < 0.1f)
            return;

        if (Moving.Count > 0)
            return;

        if (game.IsEightShot())
            game.State = new EightShotResult(game, pocketedNow, call);
        else
            game.State = new ShotResult(game, pocketedNow, firstTouched, call);

    }

    void BallMotion(Moving movingObject, bool isMoving) {
        if (isMoving) {
            if (Moving.Contains(movingObject))
                return;

            Moving.Add(movingObject);
        }
        else
            Moving.Remove(movingObject);
    }

    void PocketEntered(Ball ball) {
        Ball.Group group = Ball.GetGroup(ball.number);
        game.Log($"ボール{ball.number}(group {group})がクッションに当てった");

        pocketedNow.Add(ball);

        ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ball.gameObject.SetActive(false);
        Moving.Remove(ball.GetComponent<Moving>());

        if (ball.number == -1)
            return;

        game.Pocketed.Add(ball);

        if (game.CurrentPlayer == GameController.Players.PLAYER1)
            game.Player1Balls.Add(ball);
        else
            game.Player2Balls.Add(ball);

        game.Balls.Remove(ball);

        game.Solids.Remove(ball);
        game.Stripes.Remove(ball);
    }

    void CueBallCollided(Ball ball) {
        if (!firstTouched)
            firstTouched = ball;

        Ball.Group group = Ball.GetGroup(ball.number);
        game.Log($"ボール{ball.number}(group {group})がキューボールに当てった");
    }
}
