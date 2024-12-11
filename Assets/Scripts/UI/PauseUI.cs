using UnityEngine;
using UnityEngine.UIElements;

public class PauseUI : MonoBehaviour {
    UIDocument uiDocument;
    public Button Continue, Menu, Quit;

    public bool Visible {
        get => uiDocument.rootVisualElement.visible;
        set {
            uiDocument.rootVisualElement.visible = value;
        }
    }

    public void Awake() {
        uiDocument = GetComponent<UIDocument>();

        Continue = (Button) uiDocument.rootVisualElement.Query("continue");
        Menu = (Button) uiDocument.rootVisualElement.Query("menu");
        Quit = (Button) uiDocument.rootVisualElement.Query("quit");
    }
}
