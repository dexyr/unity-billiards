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
    Camera shotCamera;
    List<Pocket> pockets = new List<Pocket>();

    [SerializeField] GameObject ballSetPrefab;
    List<Ball> balls = new List<Ball>();
    public List<Moving> moving { get; private set; } = new List<Moving>();
    public List<Ball> pocketed { get; private set; } = new List<Ball>();
    float timer = 0; // ボールが当たった瞬間(movingに入っていないところ)に状態移動しないために

    public Players solids { get; private set; } = Players.NONE;
    public bool isBreak { get; private set; } = true;
    Players winner = Players.NONE;

    public States state { get; private set; } = States.MENU;
    // propertyのほうがいいかな
    void ChangeState(States newState) {
        state = newState;
        StateChanged?.Invoke(state);

        switch (state) {
        case States.SHOT:
            stick.gameObject.SetActive(true);
            shotCamera.gameObject.SetActive(true);
            break;
        }
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

        shotCamera = GameObject.FindGameObjectWithTag("Shot Camera").GetComponent<Camera>();
    }

    public void Start() {
        stick.gameObject.SetActive(false);
        shotCamera.gameObject.SetActive(false);
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

        if (Is8Scratch()) {
            Debug.Log("8 ball scratch");
            if (currentPlayer == Players.PLAYER1)
                winner = Players.PLAYER2;
            else
                winner = Players.PLAYER1;

            isBreak = false;
            ChangeState(States.END);
            return;
        }

        if (IsCueScratch()) {
            Debug.Log("cue scratch");

            isBreak = false;
            ChangeTurn();
            // ChangeState(States.FREE);
            ChangeState(States.SHOT); // デバッグ用
            return;
        }

        if (IsGroupScratch(badGroup)) {
            Debug.Log("group scratch");

            isBreak = false;
            ChangeTurn();
            // ChangeState(States.FREE);
            ChangeState(States.SHOT); // デバッグ用
            return;
        }

        if (!isBreak && solids == Players.NONE && pocketed.Count > 0) {
            Debug.Log("setting group");
            setGroup();

            goodGroup = solids == currentPlayer ? Ball.Group.SOLID : Ball.Group.STRIPE;
        }

        if (isBreak && pocketed.Count > 0) {
            Debug.Log("グループ選ぶ(ブレイクショットの成功のみ)");
            ChooseGroup();

            isBreak = false;
            ChangeState(States.SHOT);
            return;
        }

        isBreak = false;

        if (pocketed.Find(b => Ball.GetGroup(b.number) == goodGroup) == null)
            ChangeTurn();

        ChangeState(States.SHOT);
    }

    void setGroup() {
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

    bool Is8Scratch() {
        return pocketed.Find(b => b.number == 8) != null;
    }

    bool IsCueScratch() {
        return pocketed.Find(b => b.number == -1) != null;
    }

    bool IsGroupScratch(Ball.Group badGroup) {
        if (solids == Players.NONE)
            return false;

        return pocketed.Find(b => Ball.GetGroup(b.number) == badGroup) != null;
    }

    public void StartGame() {
        currentPlayer = Players.PLAYER1;
        isBreak = true;
        stick.gameObject.SetActive(true);
        shotCamera.gameObject.SetActive(true);
        ChangeState(States.SHOT);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void StickCollided(float velocity) {
        timer = 0;
        pocketed.Clear();
        stick.gameObject.SetActive(false);
        shotCamera.gameObject.SetActive(false);

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

        if (ball.number != -1) {
            balls.Remove(ball);
            moving.Remove(ball.GetComponent<Moving>());
            ball.gameObject.SetActive(false);
        }

        Ball.Group group = Ball.GetGroup(ball.number);
        LogUpdated?.Invoke($"ボール{ball.number}(group {group})がクッションに当てった");
    }
}
