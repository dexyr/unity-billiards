using UnityEngine;
using UnityEngine.Events;

public class GameController : MonoBehaviour {
    public delegate void LogUpdateHandler(string message);
    public event LogUpdateHandler LogUpdated;
    
    public void CueBallCollided(Ball ball) {
        Ball.Group group = Ball.GetGroup(ball.number);
        LogUpdated?.Invoke($"�{�[��{ball.number}(group {group})���L���[�{�[���ɓ��Ă���");
    }

    public void PocketEntered(Ball ball) {
        Ball.Group group = Ball.GetGroup(ball.number);
        LogUpdated?.Invoke($"�{�[��{ball.number}(group {group})���N�b�V�����ɓ��Ă���");
    }
}
