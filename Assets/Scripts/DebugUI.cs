using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DebugUI : MonoBehaviour {
    [SerializeField] CueStick cueStick;

    GameController gameController;
    Label debug, state, turn, balls, solids, pocketed, breakShot;
    Slider velocity, hitVelocity;

    void Awake() {
        var controllerObject = GameObject.FindGameObjectWithTag("GameController");
        gameController = controllerObject.GetComponent<GameController>();

        gameController.LogUpdated += UpdateLogText;
        gameController.StateChanged += UpdateStateText;
        // gameController.TurnChanged += UpdateTurnText;
        cueStick.StickCollided += UpdateHitVelocity;

        var uiDocument = GetComponent<UIDocument>();
        debug = (Label) uiDocument.rootVisualElement.Query("debug");
        state = (Label) uiDocument.rootVisualElement.Query("state");
        breakShot = (Label) uiDocument.rootVisualElement.Query("break");
        turn = (Label) uiDocument.rootVisualElement.Query("turn");
        balls = (Label) uiDocument.rootVisualElement.Query("balls");
        solids = (Label) uiDocument.rootVisualElement.Query("solids");
        pocketed = (Label) uiDocument.rootVisualElement.Query("pocketed");
        velocity = (Slider) uiDocument.rootVisualElement.Query("velocity");
        hitVelocity = (Slider) uiDocument.rootVisualElement.Query("hit-velocity");
    }

    public void OnDestroy() {
        gameController.LogUpdated -= UpdateLogText;
        gameController.StateChanged -= UpdateStateText;
        // gameController.TurnChanged -= UpdateTurnText;
        cueStick.StickCollided -= UpdateHitVelocity;
    }

    public void Start() {
        state.text = $"(現在状態)\n{gameController.state}";
        turn.text = gameController.currentPlayer.ToString();
        solids.text = $"ソリッド:{gameController.solids}";
    }

    public void Update() {
        velocity.value = cueStick.GetComponent<Rigidbody>().velocity.magnitude;

        turn.text = gameController.currentPlayer.ToString();
        breakShot.text = $"ブレイク:{gameController.isBreak}";

        string moving_string = "動いているボール";

        foreach (Moving m in gameController.moving) {
            var ball = m.GetComponent<Ball>();

            if (ball.number == -1)
                moving_string += $"\nキューボール";
            else
                moving_string += $"\nボール{ball.number}";
        }
        balls.text = moving_string;

        string pocketed_string = "クッションボール";

        foreach (Ball b in gameController.pocketed) {
            if (b.number == -1)
                pocketed_string += $"\nキューボール";
            else
                pocketed_string += $"\nボール{b.number}";
        }
        pocketed.text = pocketed_string;

        solids.text = $"ソリッド:{gameController.solids}";
    }

    public void UpdateHitVelocity(float velocity) {
        hitVelocity.value = velocity;
    }

    public void UpdateLogText(string message) {
        debug.text += "\n" + message;
    }

    public void UpdateStateText(GameController.States gameState) {
        state.text = $"(現在状態)\n{gameState}";
    }

    /*
    public void UpdateTurnText(GameController.Players gameTurn) {
        turn.text = gameTurn.ToString();
    }
    */
}
