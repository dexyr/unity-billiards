using UnityEngine;
using System.Collections.Generic;

public class Simulation : GameState {
    float timer = 0; // É{Å[ÉãÇ™ìñÇΩÇ¡ÇΩèuä‘(movingÇ…ì¸Ç¡ÇƒÇ¢Ç»Ç¢Ç∆Ç±ÇÎ)Ç…èÛë‘à⁄ìÆÇµÇ»Ç¢ÇΩÇﬂÇ…;
    public List<Moving> Moving = new List<Moving>();
    List<Ball> pocketedNow = new List<Ball>();
    List<Ball> cushionBalls = new List<Ball>();
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

        foreach (var c in game.Cushions)
            c.CushionHit += CushionHit;

        foreach (var m in GameObject.FindObjectsOfType<Moving>())
            m.MotionUpdated += BallMotion;

        game.CueBall.BallCollided += CueBallCollided;
    }

    public override void Exit() {
        foreach (Pocket p in game.Pockets)
            p.PocketEntered -= PocketEntered;

        foreach (var c in game.Cushions)
            c.CushionHit -= CushionHit;

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

        if (game.IsBreak)
            game.State = new BreakShotResult(game, pocketedNow, cushionBalls);
        else if (game.IsEightShot)
            game.State = new EightShotResult(game, pocketedNow, call);
        else
            game.State = new ShotResult(game, pocketedNow, cushionBalls, firstTouched, call);
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

    void CushionHit(Ball ball) {
        if (ball.number == -1 || cushionBalls.Contains(ball))
            return;

        cushionBalls.Add(ball);
    }

    void PocketEntered(Ball ball) {
        if (!ball.gameObject.activeSelf)
            return;

        Ball.Group group = Ball.GetGroup(ball.number);

        pocketedNow.Add(ball);

        ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ball.gameObject.SetActive(false);
        Moving.Remove(ball.GetComponent<Moving>());

        if (ball.number == -1)
            return;

        game.Pocketed.Add(ball);

        if (game.CurrentPlayer == Players.PLAYER1)
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
    }
}
