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
    }

    public void Start() {
        state.text = $"(���ݏ��)\n{gameController.State}";
        turn.text = gameController.CurrentPlayer.ToString();
        solids.text = $"�\���b�h:{gameController.SolidsPlayer}";
    }

    public void Update() {
        velocity.value = cueStick.GetComponent<Rigidbody>().velocity.magnitude;

        turn.text = gameController.CurrentPlayer.ToString();
        breakShot.text = $"�u���C�N:{gameController.IsBreak}";

        string moving_string = "�����Ă���{�[��";

        if (gameController.State is Simulation) {
            foreach (Moving m in ((Simulation) gameController.State).Moving) {
                var ball = m.GetComponent<Ball>();

                if (ball.number == -1)
                    moving_string += $"\n�L���[�{�[��";
                else
                    moving_string += $"\n�{�[��{ball.number}";
            }
        }

        balls.text = moving_string;

        string pocketed_string = "�N�b�V�����{�[��";

        foreach (Ball b in gameController.Pocketed) {
            if (b.number == -1)
                pocketed_string += $"\n�L���[�{�[��";
            else
                pocketed_string += $"\n�{�[��{b.number}";
        }
        pocketed.text = pocketed_string;

        solids.text = $"�\���b�h:{gameController.SolidsPlayer}";
    }

    public void UpdateHitVelocity(float velocity) {
        hitVelocity.value = velocity;
    }

    public void UpdateLogText(string message) {
        debug.text += "\n" + message;
    }

    public void UpdateStateText(GameState gameState) {
        state.text = $"(���ݏ��)\n{gameState}";
    }

    /*
    public void UpdateTurnText(GameController.Players gameTurn) {
        turn.text = gameTurn.ToString();
    }
    */
}
