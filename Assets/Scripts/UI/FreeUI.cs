using UnityEngine;
using UnityEngine.UIElements;

public class FreeUI : MonoBehaviour {
    UIDocument uiDocument;
    public Button Confirm, Cancel;

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
    }
}
