using UnityEngine;
using UnityEngine.UIElements;

public class ShotUI : MonoBehaviour {
    UIDocument uiDocument;
    Label hint1;

    public bool Visible {
        get => uiDocument.rootVisualElement.visible;
        set {
            uiDocument.rootVisualElement.visible = value;
        }
    }

    public void Awake() {
        uiDocument = GetComponent<UIDocument>();

        hint1 = (Label) uiDocument.rootVisualElement.Query("hint1");
    }

    public void SetHint(string hint) {
        hint1.text = hint;
    }
}
