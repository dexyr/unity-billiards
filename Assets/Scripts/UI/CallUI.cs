using UnityEngine;
using UnityEngine.UIElements;

public class CallUI : MonoBehaviour {
    UIDocument uiDocument;
    public Button Call, Reset, Safety;

    public bool Visible {
        get => uiDocument.rootVisualElement.visible;
        set {
            uiDocument.rootVisualElement.visible = value;
        }
    }

    public void Awake() {
        uiDocument = GetComponent<UIDocument>();

        Call = (Button) uiDocument.rootVisualElement.Query("call");
        Reset = (Button) uiDocument.rootVisualElement.Query("reset");
        Safety = (Button) uiDocument.rootVisualElement.Query("safety");
    }
}
