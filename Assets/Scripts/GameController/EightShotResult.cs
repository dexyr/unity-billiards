using System.Collections.Generic;
using UnityEngine;

public class EightShotResult : GameState {
    List<Ball> pocketedNow;

    public EightShotResult(GameController game, List<Ball> pocketedNow, CallInfo? call) : base(game) {
        this.pocketedNow = pocketedNow;
    }

    public override void Enter() {
        game.TurnUI.Visible = true;
        game.TurnUI.OptionHint.visible = true;
    }

    public override void Exit() {}

    public override void Update() {
        if (IsEightPocketed() && IsCueScratch()) {
            game.EndGame(game.OtherPlayer);
            return;
        }
        
        if (IsEightPocketed()) {
            game.EndGame(game.CurrentPlayer);
            return;
        }

        game.ChangeTurn();
        game.State = new Call(game);

        if (Input.GetKeyDown(KeyCode.O))
            game.IsPaused = true;
    }

    bool IsCueScratch() {
        return pocketedNow.Find(b => b.number == -1) != null;
    }

    bool IsEightPocketed() {
        return pocketedNow.Find(b => b.number == 8) != null;
    }
}
