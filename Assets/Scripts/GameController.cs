using UnityEngine;
using UnityEngine.Events;

public class GameController : MonoBehaviour {
    public delegate void LogUpdateHandler(string message);
    public event LogUpdateHandler LogUpdated;
    
    public void CueBallCollided(Ball ball) {
        Ball.Group group = Ball.GetGroup(ball.number);
        LogUpdated?.Invoke($"ボール{ball.number}(group {group})がキューボールに当てった");
    }

    public void PocketEntered(Ball ball) {
        Ball.Group group = Ball.GetGroup(ball.number);
        LogUpdated?.Invoke($"ボール{ball.number}(group {group})がクッションに当てった");
    }
}
