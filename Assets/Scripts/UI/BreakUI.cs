using UnityEngine;
using UnityEngine.UIElements;

public class BreakUI : MonoBehaviour {
    UIDocument uiDocument;
    public Button Pass, Redo, OpponentRedo;

    public bool Visible {
        get => uiDocument.rootVisualElement.visible;
        set {
            uiDocument.rootVisualElement.visible = value;
        }
    }

    public void Awake() {
        uiDocument = GetComponent<UIDocument>();

        Pass = (Button) uiDocument.rootVisualElement.Query("pass");
        Redo = (Button) uiDocument.rootVisualElement.Query("redo");
        OpponentRedo = (Button) uiDocument.rootVisualElement.Query("opponent-redo");
    }
}
