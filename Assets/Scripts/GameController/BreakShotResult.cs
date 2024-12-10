using System.Collections.Generic;
using UnityEngine;

public class BreakShotResult : GameState {
    List<Ball> pocketedNow;
    List<Ball> cushionBalls;

    GameState next;

    public BreakShotResult(GameController game, List<Ball> pocketedNow, List<Ball> cushionBalls) : base(game) {
        this.pocketedNow = pocketedNow;
        this.cushionBalls = cushionBalls;
    }

    public override void Enter() {
        CalculateResults();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        game.TurnUI.Visible = true;
        game.TurnUI.OptionHint.visible = true;

        game.ShotResultUI.Refresh();
        game.ShotResultUI.Visible = true;

        game.ShotResultUI.Confirm.clicked += Confirm;
    }

    public override void Exit() {
        game.IsBreak = false;

        game.ShotResultUI.Visible = false;
        game.ShotResultUI.Results.Clear();

        game.ShotResultUI.Confirm.clicked -= Confirm;
    }

    void CalculateResults() {
        game.ShotResultUI.Results.Add($"ボールが{pocketedNow.Count}個ポケットしました。");

        if (Is8Scratch()) {
            game.ShotResultUI.Results.Add($"エイトボールスクラッチしました。");

            Players winner;
            if (game.CurrentPlayer == Players.PLAYER1)
                winner = Players.PLAYER2;
            else
                winner = Players.PLAYER1;

            game.EndGame(winner);
            return;
        }

        if (IsCueScratch()) {
            game.ShotResultUI.Results.Add("キュースクラッチしました。");
            game.ShotResultUI.Results.Add("ブレイク失敗しました。");
            game.ShotResultUI.Results.Add("ターンが交代します。");

            game.ChangeTurn();
            next = new Break(game);
            return;
        }

        if (pocketedNow.Count == 0 && cushionBalls.Count < 4) {
            game.ShotResultUI.Results.Add("ボールが1個もポケットできませんでした。");
            game.ShotResultUI.Results.Add("ボールが4個クッションまで届けませんでした。");
            game.ShotResultUI.Results.Add("ブレイク失敗しました。");
            game.ShotResultUI.Results.Add("ターンが交代します。");

            game.ChangeTurn();
            next = new Break(game);
            return;
        }

        if (pocketedNow.Count == 0) {
            game.ShotResultUI.Results.Add("ブレイク成功でした。");
            game.ShotResultUI.Results.Add("ターンが交代します。");

            game.ChangeTurn();
            next = new Shot(game);
            return;
        }

        game.ShotResultUI.Results.Add("ブレイク成功でグループ選択します。");

        game.IsBreak = false;
        next = new Group(game, pocketedNow);
    }

    public override void Update() {
        if (Input.GetKeyDown(KeyCode.O))
            game.IsPaused = true;
    }

    public void Confirm() {
        game.State = next;
    }

    bool Is8Scratch() {
        return pocketedNow.Find(b => b.number == 8) != null;
    }

    bool IsCueScratch() {
        return pocketedNow.Find(b => b.number == -1) != null;
    }
}
