using UnityEngine;

public class End : GameState {
    GameController.Players winner;

    public End(GameController game, GameController.Players winner) : base(game) {
        this.winner = winner;
    }

    public override void Enter() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        game.TurnUI.Visible = false;

        game.EndUI.Visible = true;
        game.EndUI.Refresh(winner, game.Player1Balls, game.Player2Balls);
    }

    public override void Exit() {
        game.EndUI.Visible = false;
    }

    public override void Update() {}
}
