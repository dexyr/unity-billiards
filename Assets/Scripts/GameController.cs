using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    public enum Players { NONE, PLAYER1, PLAYER2 } // Listのほうがフレキシブルだが
    public enum States { MENU, SHOT, SIMULATION, FREE, END };

    public delegate void LogUpdateHandler(string message);
    public event LogUpdateHandler LogUpdated;

    public delegate void StateChangeHandler(States state);
    public event StateChangeHandler StateChanged;

    public delegate void TurnChangeHandler(Players turn);
    public event TurnChangeHandler TurnChanged;

    CueStick stick;
    CueBall cueBall;
    List<Pocket> pockets = new List<Pocket>();

    [SerializeField] GameObject ballSetPrefab;
    List<Ball> balls = new List<Ball>();
    public List<Moving> moving = new List<Moving>();
    List<Ball> pocketed = new List<Ball>();
    float timer = 0; // ボールが当たった瞬間(movingに入っていないところ)に状態移動しないために

    public Players solids { get; private set; } = Players.NONE;
    bool isBreak = true;
    Players winner = Players.NONE;

    public States state { get; private set; } = States.MENU;
    // propertyのほうがいいかな
    void ChangeState(States newState) {
        state = newState;
        StateChanged?.Invoke(state);
    }

    public Players currentPlayer { get; private set; } = Players.NONE;
    void ChangeTurn() {
        if (currentPlayer == Players.PLAYER1)
            currentPlayer = Players.PLAYER2;
        else
            currentPlayer = Players.PLAYER1;

        TurnChanged?.Invoke(currentPlayer);
    }

    public void Awake() {
        var spawnTransform = GameObject.FindGameObjectWithTag("Ball Spawn").GetComponent<Transform>();
        GameObject ballSet = Instantiate(ballSetPrefab, spawnTransform.position, spawnTransform.rotation);

        var ballSetBalls = ballSet.GetComponentsInChildren<Ball>();
        foreach (Ball b in ballSetBalls)
            balls.Add(b);

        var pocket_objects = FindObjectsByType<Pocket>(FindObjectsSortMode.None);
        foreach (Pocket p in pocket_objects) {
            pockets.Add(p);
            p.PocketEntered += PocketEntered;
        }

        var moving_objects = FindObjectsByType<Moving>(FindObjectsSortMode.None);
        foreach (Moving m in moving_objects) {
            moving.Add(m);
            m.MotionUpdated += BallMotion;
        }

        stick = FindObjectOfType<CueStick>();
        stick.StickCollided += StickCollided;

        cueBall = FindObjectOfType<CueBall>();
        cueBall.BallCollided += CueBallCollided;
    }

    public void OnDestroy() {
        foreach (Pocket p in pockets)
            p.PocketEntered += PocketEntered;

        foreach (Moving m in moving)
            m.MotionUpdated -= BallMotion;

        stick.StickCollided -= StickCollided;
        cueBall.BallCollided -= CueBallCollided;
    }

    public void Update() {
        timer += Time.deltaTime;

        switch (state) {
        case States.SIMULATION:
            Simulate();
            break;
        }
    }

    void Simulate() {
        if (timer < 0.1f)
            return;

        if (moving.Count > 0)
            return;

        // 転換 ===============================================================

        Ball.Group goodGroup = solids == currentPlayer ? Ball.Group.SOLID : Ball.Group.STRIPE;
        Ball.Group badGroup = solids == currentPlayer ? Ball.Group.STRIPE: Ball.Group.SOLID;

        if (is8Scratch()) {
            if (currentPlayer == Players.PLAYER1)
                winner = Players.PLAYER2;
            else
                winner = Players.PLAYER1;

            ChangeState(States.END);
            return;
        }

        if (isGroupScratch(badGroup)) {
            ChangeTurn();
            ChangeState(States.FREE);
            return;
        }

        if (!isBreak && solids == Players.NONE)
            setGroup();

        if (isBreak && pocketed.Count > 0) {
            ChooseGroup();
        }

        if (pocketed.Find(b => Ball.GetGroup(b.number) == goodGroup) == null)
            ChangeTurn();

        ChangeState(States.SHOT);
        stick.Enable();
    }

    void setGroup() {
        if (pocketed.Count == 0)
            return;

        var otherPlayer = currentPlayer == Players.PLAYER1 ? Players.PLAYER2 : Players.PLAYER1;

        if (Ball.GetGroup(pocketed[0].number) == Ball.Group.SOLID)
            solids = currentPlayer;
        else {
            solids = otherPlayer;
        }
    }

    void ChooseGroup() {
        // choose group ui
    }

    bool is8Scratch() {
        if (pocketed.Find(b => b.number == 8) == null)
            return false;

        return true;
    }

    bool isGroupScratch(Ball.Group badGroup) {
        if (solids == Players.NONE)
            return false;

        if (pocketed.Find(b => Ball.GetGroup(b.number) == badGroup) == null)
            return false;

        return true;
    }

    public void StartGame() {
        currentPlayer = Players.PLAYER1;
        isBreak = true;
        ChangeState(States.SHOT);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void StickCollided(float velocity) {
        timer = 0;
        pocketed.Clear();
        ChangeState(States.SIMULATION);
    }

    void BallMotion(Moving moving_object, bool isMoving) {
        if (isMoving) {
            if (moving.Contains(moving_object))
                return;

            moving.Add(moving_object);
        }
        else
            moving.Remove(moving_object);
    }

    void CueBallCollided(Ball ball) {
        Ball.Group group = Ball.GetGroup(ball.number);
        LogUpdated?.Invoke($"ボール{ball.number}(group {group})がキューボールに当てった");
    }

    void PocketEntered(Ball ball) {
        pocketed.Add(ball);

        Ball.Group group = Ball.GetGroup(ball.number);
        LogUpdated?.Invoke($"ボール{ball.number}(group {group})がクッションに当てった");
    }
}
