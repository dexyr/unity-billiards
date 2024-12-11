using System.Collections.Generic;
using UnityEngine;

public class ShotResult : GameState {
    List<Ball> pocketedNow;
    List<Ball> cushionBalls;
    Ball firstTouched;
    bool ballOut;
    bool changeTurn = false;

    CallInfo? call;

    GameState next;

    public ShotResult(GameController game, List<Ball> pocketedNow, List<Ball> cushionBalls, Ball firstTouched, bool ballOut, CallInfo? call=null) : base(game) {
        this.pocketedNow = pocketedNow;
        this.cushionBalls = cushionBalls;
        this.firstTouched = firstTouched;
        this.ballOut = ballOut;
        this.call = call;
    }

    public override void Enter() {
        CalculateResults();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        game.TurnUI.Visible = true;
        game.TurnUI.OptionHint.visible = true;
        game.TurnUI.Refresh(game.CurrentPlayer, game.CurrentGroup);

        game.ShotResultUI.Refresh();
        game.ShotResultUI.Visible = true;

        game.ShotResultUI.Confirm.clicked += Confirm;
    }

    public override void Exit() {
        game.ShotResultUI.Visible = false;
        game.ShotResultUI.Results.Clear();

        game.ShotResultUI.Confirm.clicked -= Confirm;
    }

    void CalculateResults() {
        int pocketed = pocketedNow.Count;
        if (pocketedNow.Contains(game.CueBall.GetComponent<Ball>()) && pocketed > 0)
            pocketed--;

        game.ShotResultUI.Results.Add($"ボールが{pocketed}個ポケットしました。");

        if (Is8Scratch()) {
            game.ShotResultUI.Results.Add($"エイトボールスクラッチしました。");

            Players winner;
            if (game.CurrentPlayer == Players.PLAYER1)
                winner = Players.PLAYER2;
            else
                winner = Players.PLAYER1;

            game.Winner = winner;
            next = new End(game);
            return;
        }

        if (ballOut) {
            game.ShotResultUI.Results.Add("場外スクラッチしました。");
            game.ShotResultUI.Results.Add("ターンが交代します。");
            game.ShotResultUI.Results.Add("フリーボールです。");

            changeTurn = true;
            next = new Free(game);
            return;
        }

        if (IsCueScratch()) {
            game.ShotResultUI.Results.Add("手球スクラッチしました。");
            game.ShotResultUI.Results.Add("ターンが交代します。");
            game.ShotResultUI.Results.Add("フリーボールです。");

            changeTurn = true;
            next = new Free(game);
            return;
        }

        if (IsNoHit(game.CurrentGroup)) {
            game.ShotResultUI.Results.Add("ノーヒットファウルしました。");
            game.ShotResultUI.Results.Add("ターンが交代します。");
            game.ShotResultUI.Results.Add("フリーボールです。");

            changeTurn = true;
            next = new Free(game);
            return;
        }

        if (IsNoCushion()) {
            game.ShotResultUI.Results.Add("ノークッションファウルしました。");
            game.ShotResultUI.Results.Add("ターンが交代します。");
            game.ShotResultUI.Results.Add("フリーボールです。");

            changeTurn = true;
            next = new Free(game);
            return;
        }

        if (pocketedNow.Count == 0) {
            game.ShotResultUI.Results.Add("ボールが1個もポケットできませんでした。");
            game.ShotResultUI.Results.Add("ターンが交代します。");

            changeTurn = true;

            if (game.IsOpen)
                next = new Shot(game);
            else
                next = new Call(game);

            return;
        }

        if (IsGroupScratch(game.OtherGroup)) {
            game.ShotResultUI.Results.Add("グループの相違でポケットしました。");
            game.ShotResultUI.Results.Add("ターンが交代します。");

            changeTurn = true;

            if (game.IsOpen)
                next = new Shot(game);
            else
                next = new Call(game);

            return;
        }

        if (IsSafety()) {
            game.ShotResultUI.Results.Add("セーフティでした。");
            game.ShotResultUI.Results.Add("ターンが交代します。");

            changeTurn = true;
            next = new Call(game);
            return;
        }

        if (game.IsOpen && pocketedNow.Count > 0) {
            SetGroup();

            next = new Call(game);
            return;
        }

        if (!game.IsOpen) {
            next = new Call(game);
            return;
        }

        game.ShotResultUI.Results.Add("ターンが交代します。");

        changeTurn = true;
        next = new Shot(game);
    }

    public override void Update() {
        if (Input.GetKeyDown(KeyCode.O))
            game.Pause(new Settings(game));
    }

    public void Confirm() {
        game.State = next;

        if (changeTurn)
            game.ChangeTurn();

        if (next is End)
            game.EndGame();
    }

    bool Is8Scratch() {
        return pocketedNow.Find(b => b.number == 8) != null;
    }

    bool IsNoHit(Ball.Group goodGroup) {
        if (call == null)
            return false;

        return firstTouched == null || Ball.GetGroup(firstTouched.number) != goodGroup;
    }

    bool IsNoCushion() {
        if (call == null)
            return false;

        if (pocketedNow.Contains(call?.Ball))
            return false;

        var cueBall = game.CueBall.GetComponent<Ball>();

        return !(cushionBalls.Contains(cueBall) || cushionBalls.Contains(call?.Ball));
    }

    bool IsCueScratch() {
        return pocketedNow.Find(b => b.number == -1) != null;
    }

    bool IsGroupScratch(Ball.Group badGroup) {
        if (game.IsOpen)
            return false;

        return pocketedNow.Find(b => Ball.GetGroup(b.number) == badGroup) != null;
    }

    bool IsSafety() {
        return !game.IsOpen && call == null;
    }

    void SetGroup() {
        game.ShotResultUI.Results.Add("グループが決まりました。");

        if (Ball.GetGroup(pocketedNow[0].number) == Ball.Group.SOLID) {
            game.SolidsPlayer = game.CurrentPlayer;
            game.ShotResultUI.Results.Add("グループはソリッドです。");

            game.TurnUI.Refresh(game.CurrentPlayer, Ball.Group.SOLID);
        }
        else {
            game.ShotResultUI.Results.Add("グループはストライプです。");
            game.SolidsPlayer = game.OtherPlayer;

            game.TurnUI.Refresh(game.CurrentPlayer, Ball.Group.STRIPE);
        }

    }
}
