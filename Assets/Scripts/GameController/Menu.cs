using UnityEngine;

public class Menu : GameState {
    public Menu(GameController game) : base(game) {}

    public override void Enter() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        game.TargetCamera.Target = game.Balls[0].gameObject;

        game.MenuUI.Visible = true;
        game.MenuUI.Start.clicked += StartGame;
    }
    public override void Exit() {
        game.MenuUI.Visible = false;
        game.MenuUI.Start.clicked -= StartGame;
    }
    public override void Update() { }

    public void StartGame() {
        game.ChangeTurn();
        game.IsBreak = true;
        game.TurnUI.Visible = true;
        game.State = new Shot(game);
    }
}
