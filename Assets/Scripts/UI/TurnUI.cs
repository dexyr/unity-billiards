using UnityEngine;
using UnityEngine.UIElements;

public class TurnUI : MonoBehaviour {
    UIDocument uiDocument;
    Label player, group;

    public bool Visible {
        get => uiDocument.rootVisualElement.visible;
        set {
            uiDocument.rootVisualElement.visible = value;
        }
    }

    public void Awake() {
        uiDocument = GetComponent<UIDocument>();

        player = (Label) uiDocument.rootVisualElement.Query("player");
        group = (Label) uiDocument.rootVisualElement.Query("group");
    }

    public void Refresh(GameController.Players player, Ball.Group group) {
        if (player == GameController.Players.PLAYER1)
            this.player.text = "プレイヤー1";
        else
            this.player.text = "プレイヤー2";

        switch (group) {
        case Ball.Group.NONE:
            this.group.text = "【グループ】無し";
            break;

        case Ball.Group.SOLID:
            this.group.text = "【グループ】ソリッド";
            break;

        case Ball.Group.STRIPE:
            this.group.text = "【グループ】ストライプ";
            break;
        }
    }
}
