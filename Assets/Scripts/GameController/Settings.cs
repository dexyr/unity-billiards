using UnityEngine;
using UnityEngine.UIElements;

public class Settings : GameState {
    public Settings (GameController game) : base(game) {}

    public override void Enter() {
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;

        game.TurnUI.Visible = false;
        game.SettingsUI.Visible = true;

        switch (game.CurrentPlayer) {
        case Players.PLAYER1:
            game.SettingsUI.Player1.style.display = DisplayStyle.Flex;
            break;

        case Players.PLAYER2:
            game.SettingsUI.Player2.style.display = DisplayStyle.Flex;
            break;

        default:
            game.SettingsUI.Player1.style.display = DisplayStyle.Flex;
            game.SettingsUI.Player2.style.display = DisplayStyle.Flex;
            break;
        }

        game.SettingsUI.Player1.value = game.Player1Sensitivity;
        game.SettingsUI.Player2.value = game.Player2Sensitivity;

        game.SettingsUI.Save.clicked += Save;
        game.SettingsUI.Reset.clicked += Reset;
    }

    public override void Exit() {
        game.SettingsUI.Visible = false;

        game.SettingsUI.Player1.style.display = DisplayStyle.None;
        game.SettingsUI.Player2.style.display = DisplayStyle.None;

        game.SettingsUI.Save.clicked -= Save;
        game.SettingsUI.Reset.clicked -= Reset;
    }

    public override void Update() {}

    public void Save() {
        game.Player1Sensitivity = game.SettingsUI.Player1.value;
        game.Player2Sensitivity = game.SettingsUI.Player2.value;

        game.IsPaused = false;
    }

    public void Reset() {
        game.SettingsUI.Player1.value = game.Player1Sensitivity;
        game.SettingsUI.Player2.value = game.Player2Sensitivity;

        // ïsévãc
        game.SettingsUI.Player1.MarkDirtyRepaint();
        game.SettingsUI.Player2.MarkDirtyRepaint();
    }
}
