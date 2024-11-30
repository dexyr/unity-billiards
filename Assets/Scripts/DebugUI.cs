using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DebugUI : MonoBehaviour {
    [SerializeField] CueStick cueStick;

    GameController gameController;
    Label debug, state, turn, balls, solids;
    Slider velocity, hitVelocity;

    void Awake() {
        var controllerObject = GameObject.FindGameObjectWithTag("GameController");
        gameController = controllerObject.GetComponent<GameController>();

        gameController.LogUpdated += UpdateLogText;
        gameController.StateChanged += UpdateStateText;
        gameController.TurnChanged += UpdateTurnText;
        cueStick.StickCollided += UpdateHitVelocity;

        var uiDocument = GetComponent<UIDocument>();
        debug = (Label) uiDocument.rootVisualElement.Query("debug");
        state = (Label) uiDocument.rootVisualElement.Query("state");
        turn = (Label) uiDocument.rootVisualElement.Query("turn");
        balls = (Label) uiDocument.rootVisualElement.Query("balls");
        solids = (Label) uiDocument.rootVisualElement.Query("solids");
        velocity = (Slider) uiDocument.rootVisualElement.Query("velocity");
        hitVelocity = (Slider) uiDocument.rootVisualElement.Query("hit-velocity");
    }

    public void OnDestroy() {
        gameController.LogUpdated -= UpdateLogText;
        gameController.StateChanged -= UpdateStateText;
        gameController.TurnChanged -= UpdateTurnText;
        cueStick.StickCollided -= UpdateHitVelocity;
    }

    public void Start() {
        state.text = $"(���ݏ��)\n{gameController.state}";
        turn.text = gameController.currentPlayer.ToString();
        solids.text = $"�\���b�h:{gameController.solids}";
    }

    public void Update() {
        velocity.value = cueStick.GetComponent<Rigidbody>().velocity.magnitude;

        string moving_string = "�����Ă���{�[��";
        List<Moving> moving = gameController.moving;

        foreach (Moving m in moving) {
            var ball = m.GetComponent<Ball>();
            var cueBall = m.GetComponent<CueBall>();

            if (ball)
                moving_string += $"\n�{�[��{ball.number}";
            if (cueBall)
                moving_string += $"\n�L���[�{�[��";
        }

        balls.text = moving_string;

        solids.text = $"�\���b�h:{gameController.solids}";
    }

    public void UpdateHitVelocity(float velocity) {
        hitVelocity.value = velocity;
    }

    public void UpdateLogText(string message) {
        debug.text += "\n" + message;
    }

    public void UpdateStateText(GameController.States gameState) {
        state.text = $"(���ݏ��)\n{gameState}";
    }

    public void UpdateTurnText(GameController.Players gameTurn) {
        turn.text = gameTurn.ToString();
    }
}
