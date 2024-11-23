using UnityEngine;
using UnityEngine.UIElements;

public class DebugUI : MonoBehaviour {
    GameController gameController;
    Label debug, state;

    void Awake() {
        var controllerObject = GameObject.FindGameObjectWithTag("GameController");
        gameController = controllerObject.GetComponent<GameController>();

        gameController.LogUpdated += UpdateLogText;
        gameController.StateChanged += UpdateStateText;

        var uiDocument = GetComponent<UIDocument>();
        debug = (Label) uiDocument.rootVisualElement.Query("debug");
        state = (Label) uiDocument.rootVisualElement.Query("state");
    }

    public void OnDestroy() {
        if (gameController) {
            gameController.LogUpdated -= UpdateLogText;
            gameController.StateChanged -= UpdateStateText;
        }
    }

    public void UpdateLogText(string message) {
        debug.text += "\n" + message;
    }

    public void UpdateStateText(GameController.State gameState) {
        state.text = $"(åªç›èÛë‘)\n{gameState}";
    }
}
