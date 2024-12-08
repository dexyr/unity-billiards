using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GroupUI : MonoBehaviour {
    public delegate void GroupChoiceHandler(Ball.Group group);
    public event GroupChoiceHandler GroupChosen;

    UIDocument uiDocument;
    Label solids, stripes;
    Button solidButton, stripeButton;

    public bool Visible {
        get => uiDocument.rootVisualElement.visible;
        set {
            uiDocument.rootVisualElement.visible = value;
        }
    }

    void Awake() {
        uiDocument = GetComponent<UIDocument>();

        solids = (Label) uiDocument.rootVisualElement.Query("solids");
        stripes = (Label) uiDocument.rootVisualElement.Query("stripes");
        solidButton = (Button) uiDocument.rootVisualElement.Query("solid-button");
        stripeButton = (Button) uiDocument.rootVisualElement.Query("stripe-button");
        solidButton.clicked += ChooseSolids;
        stripeButton.clicked += ChooseStripes;
    }

    void OnDestroy() {
        solidButton.clicked -= ChooseSolids;
        stripeButton.clicked -= ChooseStripes;
    }

    public void Refresh(List<Ball> solidBalls, List<Ball> stripeBalls) {
        List<string> solidsStrings = new List<string>();
        List<string> stripesStrings = new List<string>();

        foreach (Ball b in solidBalls)
            solidsStrings.Add(b.number.ToString());

        foreach (Ball b in stripeBalls)
            stripesStrings.Add(b.number.ToString());

        if (solidsStrings.Count > 0)
            solids.text = string.Join("\n", solidsStrings);
        else
            solids.text = "–³‚µ";

        if (stripesStrings.Count > 0)
            stripes.text = string.Join("\n", stripesStrings);
        else
            stripes.text = "–³‚µ";
    }

    public void ChooseSolids() {
        GroupChosen?.Invoke(Ball.Group.SOLID);
    }

    public void ChooseStripes() {
        GroupChosen?.Invoke(Ball.Group.STRIPE);
    }
}
