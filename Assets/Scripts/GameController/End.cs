using UnityEngine;

public class End : GameState {
    Players winner;

    public End(GameController game, Players winner) : base(game) {
        this.winner = winner;
    }

    public override void Enter() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        game.TurnUI.Visible = false;

        game.EndUI.Visible = true;
        game.EndUI.Refresh(winner, game.Player1Balls, game.Player2Balls);

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
        game.ClearTable();
        game.SetTable();
        game.CurrentPlayer = Players.PLAYER1;
        game.IsBreak = true;

        game.State = new Shot(game);
    }

    public void Return() {
        game.State = new Menu(game);
    }
}
