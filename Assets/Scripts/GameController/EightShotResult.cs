using System.Collections.Generic;
using UnityEngine;

public class EightShotResult : GameState {
    List<Ball> pocketedNow;

    public EightShotResult(GameController game, List<Ball> pocketedNow, CallInfo? call) : base(game) {
        this.pocketedNow = pocketedNow;
    }

    public override void Enter() {}

    public override void Exit() {}

    public override void Update() {
        if (IsEightPocketed() && IsCueScratch()) {
            game.State = new End(game, game.OtherPlayer);
            return;
        }
        
        if (IsEightPocketed()) {
            game.State = new End(game, game.CurrentPlayer);
            return;
        }

        game.ChangeTurn();
        game.State = new Call(game);
    }

    bool IsCueScratch() {
        return pocketedNow.Find(b => b.number == -1) != null;
    }

    bool IsEightPocketed() {
        return pocketedNow.Find(b => b.number == 8) != null;
    }
}
