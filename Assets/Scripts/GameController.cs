using UnityEngine;
using UnityEngine.Events;

public class GameController : MonoBehaviour {
    public enum State { MENU, SHOT, SIMULATION, FREE, END };

    public delegate void LogUpdateHandler(string message);
    public event LogUpdateHandler LogUpdated;

    public delegate void StateChangeHandler(State state);
    public event StateChangeHandler StateChanged;

    State state;
    // propertyのほうがいいかな
    public void changeState(State newState) {
        state = newState;
        StateChanged?.Invoke(state);
    }

    public void Start() {
        changeState(State.SIMULATION);
    }

    public void CueBallCollided(Ball ball) {
        Ball.Group group = Ball.GetGroup(ball.number);
        LogUpdated?.Invoke($"ボール{ball.number}(group {group})がキューボールに当てった");
    }

    public void PocketEntered(Ball ball) {
        Ball.Group group = Ball.GetGroup(ball.number);
        LogUpdated?.Invoke($"ボール{ball.number}(group {group})がクッションに当てった");

        changeState(State.END);
    }
}
