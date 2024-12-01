using UnityEngine;
using UnityEngine.UIElements;

public class MenuUI : MonoBehaviour {
    [SerializeField] UIDocument uiDocument;
    public Button Start { get; private set; }

    public bool Visible {
        get => uiDocument.rootVisualElement.visible;
        set {
            uiDocument.rootVisualElement.visible = value;
        }
    }

    void Awake() {
        Start = (Button) uiDocument.rootVisualElement.Query("start");
    }
}
