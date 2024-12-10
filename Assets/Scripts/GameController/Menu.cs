using UnityEngine;

public class Menu : GameState {
    public Menu(GameController game) : base(game) {}

    public override void Enter() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        game.MenuUI.Visible = true;
        game.TurnUI.OptionHint.visible = true;

        game.MenuUI.Start.clicked += StartGame;
    }
    public override void Exit() {
        game.MenuUI.Visible = false;

        game.MenuUI.Start.clicked -= StartGame;
    }
    public override void Update() {
        if (Input.GetKeyDown(KeyCode.O))
            game.IsPaused = true;
    }

    public void StartGame() {
        game.StartGame();
    }
}
