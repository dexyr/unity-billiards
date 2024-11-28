using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    public enum State { MENU, SHOT, SIMULATION, FREE, END };

    public delegate void LogUpdateHandler(string message);
    public event LogUpdateHandler LogUpdated;

    public delegate void StateChangeHandler(State state);
    public event StateChangeHandler StateChanged;

    CueStick stick;

    [SerializeField] GameObject ballSetPrefab;
    List<Ball> balls = new List<Ball>();
    List<Ball> moving = new List<Ball>();

    State state;
    // propertyのほうがいいかな

    float timer = 0;

    public void ChangeState(State newState) {
        state = newState;
        StateChanged?.Invoke(state);
    }

    public void Awake() {
        var spawnTransform = GameObject.FindGameObjectWithTag("Ball Spawn").GetComponent<Transform>();
        
        GameObject ballSet = Instantiate(ballSetPrefab, spawnTransform.position, spawnTransform.rotation);
        var ballSetBalls = ballSet.GetComponentsInChildren<Ball>();
        foreach (Ball b in ballSetBalls) {
            balls.Add(b);
            b.MotionUpdated += BallMotion;
        }

        stick = FindObjectOfType<CueStick>();
        stick.StickCollided += StickCollided;
    }

    public void OnDestroy() {
        foreach (Ball b in balls)
            b.MotionUpdated -= BallMotion;

        stick.StickCollided -= StickCollided;
    }

    public void Update() {
        timer += Time.deltaTime;

        if (state != State.SIMULATION)
            return;

        if (moving.Count > 0)
            return;

        if (timer < 0.5)
            return;

        ChangeState(State.SHOT);
        stick.Enable();
    }

    public void StartGame() {
        ChangeState(State.SHOT);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void StickCollided(float velocity) {
        timer = 0;
        ChangeState(State.SIMULATION);
    }

    public void BallMotion(Ball ball, bool isMoving) {
        if (isMoving) {
            timer = 0;
            Debug.Log($"{ball.number}が動いている");
            moving.Add(ball);
        }
        else {
            Debug.Log($"{ball.number}が止まった");
            moving.Remove(ball);
        }
    }

    public void CueBallCollided(Ball ball) {
        Ball.Group group = Ball.GetGroup(ball.number);
        LogUpdated?.Invoke($"ボール{ball.number}(group {group})がキューボールに当てった");
    }

    public void PocketEntered(Ball ball) {
        Ball.Group group = Ball.GetGroup(ball.number);
        LogUpdated?.Invoke($"ボール{ball.number}(group {group})がクッションに当てった");

        // ChangeState(State.END);
    }
}
