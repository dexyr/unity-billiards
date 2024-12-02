using UnityEngine;
using UnityEngine.UIElements;

public class FreeUI : MonoBehaviour {
    UIDocument uiDocument;
    public Button Confirm, Cancel;
    Label hint1, hint2;

    public bool Visible {
        get => uiDocument.rootVisualElement.visible;
        set {
            uiDocument.rootVisualElement.visible = value;
        }
    }

    public void Awake() {
        uiDocument = GetComponent<UIDocument>();

        Confirm = (Button) uiDocument.rootVisualElement.Query("confirm");
        Cancel = (Button) uiDocument.rootVisualElement.Query("cancel");

        hint1 = (Label) uiDocument.rootVisualElement.Query("hint1");
        hint2 = (Label) uiDocument.rootVisualElement.Query("hint2");
    }

    public void SetHint(string[] hints) {
        if (hints.Length == 0)
            return;

        hint1.text = hints[0];

        if (hints.Length > 1)
            hint2.text = hints[1];
        else
            hint2.text = "";
    }
}
