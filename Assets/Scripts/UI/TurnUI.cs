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
            this.player.text = "�v���C���[1";
        else
            this.player.text = "�v���C���[2";

        switch (group) {
        case Ball.Group.NONE:
            this.group.text = "�y�O���[�v�z����";
            break;

        case Ball.Group.SOLID:
            this.group.text = "�y�O���[�v�z�\���b�h";
            break;

        case Ball.Group.STRIPE:
            this.group.text = "�y�O���[�v�z�X�g���C�v";
            break;
        }
    }
}
