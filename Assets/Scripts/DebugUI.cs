using UnityEngine;
using UnityEngine.UIElements;

public class DebugUI : MonoBehaviour {
    GameController gameController;
    Label debug;

    void Awake() {
        var controllerObject = GameObject.FindGameObjectWithTag("GameController");
        gameController = controllerObject.GetComponent<GameController>();

        gameController.LogUpdated += UpdateLogText;

        var uiDocument = GetComponent<UIDocument>();
        debug = (Label) uiDocument.rootVisualElement.Query("debug");
    }

    public void OnDestroy() {
        if (gameController)
            gameController.LogUpdated -= UpdateLogText;
    }

    public void UpdateLogText(string message) {
        debug.text += "\n" + message;
    }
}
