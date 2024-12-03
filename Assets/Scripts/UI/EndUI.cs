using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EndUI : MonoBehaviour {
    UIDocument uiDocument;
    Label winner, player1, player2;
    public Button Replay, Menu;

    public bool Visible {
        get => uiDocument.rootVisualElement.visible;
        set {
            uiDocument.rootVisualElement.visible = value;
        }
    }

    public void Awake() {
        uiDocument = GetComponent<UIDocument>();

        Replay = (Button) uiDocument.rootVisualElement.Query("replay");
        Menu = (Button) uiDocument.rootVisualElement.Query("menu");
        winner = (Label) uiDocument.rootVisualElement.Query("winner");
        player1 = (Label) uiDocument.rootVisualElement.Query("player1");
        player2 = (Label) uiDocument.rootVisualElement.Query("player2");
    }

    public void Refresh(GameController.Players winner, List<Ball> player1, List<Ball> player2) {
        if (winner == GameController.Players.PLAYER1)
            this.winner.text = "プレイヤー1の勝ち！";
        else
            this.winner.text = "プレイヤー2の勝ち！";

        List<string> player1Strings = new List<string>();
        List<string> player2Strings = new List<string>();

        foreach (Ball b in player1)
            player1Strings.Add(b.number.ToString());

        foreach (Ball b in player2)
            player2Strings.Add(b.number.ToString());

        if (player1Strings.Count > 0)
            this.player1.text = string.Join("\n", player1Strings);
        else
            this.player1.text = "無し";

        if (player2Strings.Count > 0)
            this.player2.text = string.Join("\n", player2Strings);
        else
            this.player2.text = "無し";
    }
}
