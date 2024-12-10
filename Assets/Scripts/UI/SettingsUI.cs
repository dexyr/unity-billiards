using UnityEngine;
using UnityEngine.UIElements;

public class SettingsUI : MonoBehaviour {
    UIDocument uiDocument;
    public Slider Player1, Player2;
    public Button Save, Reset;

    public bool Visible {
        get => uiDocument.rootVisualElement.visible;
        set {
            uiDocument.rootVisualElement.visible = value;
        }
    }

    public void Awake() {
        uiDocument = GetComponent<UIDocument>();

        Player1 = (Slider) uiDocument.rootVisualElement.Query("player1");
        Player2 = (Slider) uiDocument.rootVisualElement.Query("player2");
        Save = (Button) uiDocument.rootVisualElement.Query("save");
        Reset = (Button) uiDocument.rootVisualElement.Query("reset");

        Player1.style.display = DisplayStyle.None;
        Player2.style.display = DisplayStyle.None;
    }
}
