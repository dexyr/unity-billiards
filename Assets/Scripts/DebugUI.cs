using UnityEngine;
using UnityEngine.UIElements;

public class DebugUI : MonoBehaviour {
    [SerializeField] CueStick cueStick;

    GameController gameController;
    Label debug, state;
    Slider velocity, hitVelocity;

    void Awake() {
        var controllerObject = GameObject.FindGameObjectWithTag("GameController");
        gameController = controllerObject.GetComponent<GameController>();

        gameController.LogUpdated += UpdateLogText;
        gameController.StateChanged += UpdateStateText;

        var uiDocument = GetComponent<UIDocument>();
        debug = (Label) uiDocument.rootVisualElement.Query("debug");
        state = (Label) uiDocument.rootVisualElement.Query("state");
        velocity = (Slider) uiDocument.rootVisualElement.Query("velocity");
        hitVelocity = (Slider) uiDocument.rootVisualElement.Query("hit-velocity");
    }

    public void OnDestroy() {
        if (gameController) {
            gameController.LogUpdated -= UpdateLogText;
            gameController.StateChanged -= UpdateStateText;
        }
    }

    public void Update() {
        velocity.value = cueStick.GetComponent<Rigidbody>().velocity.magnitude;
    }

    public void UpdateHitVelocity(float velocity) {
        hitVelocity.value = velocity;
    }

    public void UpdateLogText(string message) {
        debug.text += "\n" + message;
    }

    public void UpdateStateText(GameController.State gameState) {
        state.text = $"(åªç›èÛë‘)\n{gameState}";
    }
}
