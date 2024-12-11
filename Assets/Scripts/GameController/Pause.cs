using UnityEngine;

public class Pause : GameState {
    public Pause(GameController game) : base(game) {}

    public override void Enter() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        game.PauseUI.Visible = true;

        game.PauseUI.Continue.clicked += Continue;
        game.PauseUI.Menu.clicked += Menu;
        game.PauseUI.Quit.clicked += Quit;
    }

    public override void Exit() {
        game.PauseUI.Visible = false;

        game.PauseUI.Continue.clicked -= Continue;
        game.PauseUI.Menu.clicked -= Menu;
        game.PauseUI.Quit.clicked -= Quit;
    }

    public override void Update() {
    }

    public void Continue() {
        game.Unpause();
    }

    public void Menu() {
        game.StartMenu();
    }

    public void Quit() {
        Application.Quit();
    }
}
