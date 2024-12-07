using UnityEngine;

public class Break : GameState {
    public Break(GameController game) : base(game) {}

    public override void Enter() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        game.TurnUI.Refresh(game.CurrentPlayer, game.CurrentGroup);

        game.BreakUI.Visible = true;

        game.BreakUI.Pass.clicked += Pass;
        game.BreakUI.Redo.clicked += Redo;
        game.BreakUI.OpponentRedo.clicked += OpponentRedo;
    }

    public override void Exit() {
        game.BreakUI.Visible = false;

        game.BreakUI.Pass.clicked -= Pass;
        game.BreakUI.Redo.clicked -= Redo;
        game.BreakUI.OpponentRedo.clicked -= OpponentRedo;
    }

    public override void Update() {}

    public void Pass() {
        game.IsBreak = false;

        if (game.CueBall.gameObject.activeSelf)
            game.State = new Shot(game);
        else
            game.State = new Free(game);
    }

    public void Redo() {
        game.ClearTable();
        game.SetTable();
        game.State = new Shot(game);
    }

    public void OpponentRedo() {
        game.ClearTable();
        game.SetTable();
        game.ChangeTurn();
        game.State = new Shot(game);
    }
}
