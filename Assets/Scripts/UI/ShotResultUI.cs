using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShotResultUI : MonoBehaviour {
    UIDocument uiDocument;
    Label result;
    public Button Confirm;
    public List<string> Results;

    public bool Visible {
        get => uiDocument.rootVisualElement.visible;
        set {
            uiDocument.rootVisualElement.visible = value;
        }
    } 

    public void Awake() {
        uiDocument = GetComponent<UIDocument>();

        result = (Label) uiDocument.rootVisualElement.Query("result");
        Confirm = (Button) uiDocument.rootVisualElement.Query("confirm");
    }

    public void Refresh() {
        result.text = string.Join("\n", Results);
    }
}
