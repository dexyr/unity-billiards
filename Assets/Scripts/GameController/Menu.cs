using UnityEngine;

public class Menu : GameState {
    public Menu(GameController game) : base(game) {}

    public override void Enter() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        game.MenuUI.Visible = true;
        game.MenuUI.Start.clicked += StartGame;
    }
    public override void Exit() {
        game.MenuUI.Visible = false;
        game.MenuUI.Start.clicked -= StartGame;

        game.TurnUI.Visible = true;
    }
    public override void Update() { }

    public void StartGame() {
        game.ClearTable();
        game.SetTable();

        game.State = new Shot(game);
    }
}
