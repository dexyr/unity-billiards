using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    public enum State { MENU, SHOT, SIMULATION, FREE, END };

    public delegate void LogUpdateHandler(string message);
    public event LogUpdateHandler LogUpdated;

    public delegate void StateChangeHandler(State state);
    public event StateChangeHandler StateChanged;

    [SerializeField] CueStick stick;

    [SerializeField] GameObject ballSetPrefab;
    List<GameObject> balls = new List<GameObject>();

    State state;
    // property�̂ق�����������

    public void ChangeState(State newState) {
        state = newState;
        StateChanged?.Invoke(state);
    }

    public void Start() {
        var spawnTransform = GameObject.FindGameObjectWithTag("Ball Spawn").GetComponent<Transform>();
        GameObject ballSet = Instantiate(ballSetPrefab, spawnTransform.position, spawnTransform.rotation);
        var ballSetBalls = ballSet.GetComponentsInChildren<Ball>();
        
        foreach (Ball b in ballSetBalls)
            balls.Add(b.gameObject);

        ChangeState(State.SHOT);
    }

    public void Update() {
        // �f�o�b�O�p

        if (Input.GetKeyDown(KeyCode.Space)) {
            stick.enabled = true;
            stick.GetComponentInChildren<CapsuleCollider>().enabled = true;
            stick.ResetAim();
        }
    }

    void FreezeBalls() {
        foreach (var b in balls) {
            var rigidBody = b.GetComponent<Rigidbody>();
            rigidBody.isKinematic = true;
        }
    }

    public void StickCollided(CueStick stick) {
        stick.enabled = false;
    }

    public void CueBallCollided(Ball ball) {
        Ball.Group group = Ball.GetGroup(ball.number);
        LogUpdated?.Invoke($"�{�[��{ball.number}(group {group})���L���[�{�[���ɓ��Ă���");
    }

    public void PocketEntered(Ball ball) {
        Ball.Group group = Ball.GetGroup(ball.number);
        LogUpdated?.Invoke($"�{�[��{ball.number}(group {group})���N�b�V�����ɓ��Ă���");

        ChangeState(State.END);
    }
}
