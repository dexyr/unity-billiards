using UnityEngine;
using UnityEngine.Events;

public class GameController : MonoBehaviour {
    public enum State { MENU, SHOT, SIMULATION, FREE, END };

    public delegate void LogUpdateHandler(string message);
    public event LogUpdateHandler LogUpdated;

    public delegate void StateChangeHandler(State state);
    public event StateChangeHandler StateChanged;

    State state;
    // property�̂ق�����������
    public void changeState(State newState) {
        state = newState;
        StateChanged?.Invoke(state);
    }

    public void Start() {
        changeState(State.SIMULATION);
    }

    public void CueBallCollided(Ball ball) {
        Ball.Group group = Ball.GetGroup(ball.number);
        LogUpdated?.Invoke($"�{�[��{ball.number}(group {group})���L���[�{�[���ɓ��Ă���");
    }

    public void PocketEntered(Ball ball) {
        Ball.Group group = Ball.GetGroup(ball.number);
        LogUpdated?.Invoke($"�{�[��{ball.number}(group {group})���N�b�V�����ɓ��Ă���");

        changeState(State.END);
    }
}
