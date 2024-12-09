using UnityEngine;

public class End : GameState {

    public End(GameController game) : base(game) {}

    public override void Enter() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        game.TurnUI.Visible = false;

        game.EndUI.Visible = true;
        game.EndUI.Refresh(game.Winner, game.Player1Balls, game.Player2Balls);

        game.EndUI.Replay.clicked += Replay;
        game.EndUI.Menu.clicked += Return;
    }

    public override void Exit() {
        game.EndUI.Visible = false;

        game.EndUI.Replay.clicked -= Replay;
        game.EndUI.Menu.clicked -= Return;
    }

    public override void Update() {}

    public void Replay() {
        game.StartGame();
    }

    public void Return() {
        game.StartMenu();
    }
}
