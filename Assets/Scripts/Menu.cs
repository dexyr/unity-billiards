using UnityEngine;
using UnityEngine.UIElements;

public class Menu : MonoBehaviour {
    GameController gameController;

    [SerializeField] UIDocument uiDocument;
    Button start;

    void Awake() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        start = (Button) uiDocument.rootVisualElement.Query("start");
        start.clicked += GameStart;
    }

    void OnDestroy() {
        start.clicked -= GameStart;
    }

    void GameStart() {
        uiDocument.rootVisualElement.style.display = DisplayStyle.None;
        gameController.StartGame();
    }
}
