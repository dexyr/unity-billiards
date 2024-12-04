using System.Collections.Generic;
using UnityEngine;

public class ShotResult : GameState {
    List<Ball> pocketedNow;
    GameState next;

    public ShotResult(GameController game, List<Ball> pocketedNow) : base(game) {
        this.pocketedNow = pocketedNow;
    }

    public override void Enter() {
        CalculateResults();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

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
        Ball.Group goodGroup = game.GetCurrentGroup();
        Ball.Group badGroup = goodGroup == Ball.Group.SOLID ? Ball.Group.STRIPE : Ball.Group.SOLID;

        game.ShotResultUI.Results.Add($"ボールが{pocketedNow.Count}個ポケットしました。");

        if (Is8Scratch()) {
            game.ShotResultUI.Results.Add($"エイトボールスクラッチしました。");

            GameController.Players winner;
            if (game.CurrentPlayer == GameController.Players.PLAYER1)
                winner = GameController.Players.PLAYER2;
            else
                winner = GameController.Players.PLAYER1;

            next = new End(game, winner);
            return;
        }

        if (IsCueScratch()) {
            game.ShotResultUI.Results.Add("キュースクラッチしました。");
            game.ShotResultUI.Results.Add("ターンが交代します。");

            game.ChangeTurn();
            next = new Free(game);
            return;
        }

        if (IsGroupScratch(badGroup)) {
            game.ShotResultUI.Results.Add("グループ相違クラッチしました。");
            game.ShotResultUI.Results.Add("ターンが交代します。");

            game.ChangeTurn();
            next = new Free(game);
            return;
        }

        if (!game.IsBreak && game.SolidsPlayer == GameController.Players.NONE && pocketedNow.Count > 0) {
            SetGroup();

            goodGroup = game.SolidsPlayer == game.CurrentPlayer ? Ball.Group.SOLID : Ball.Group.STRIPE;
        }

        if (game.IsBreak && pocketedNow.Count > 0) {
            game.ShotResultUI.Results.Add("ブレイク成功でグループ選択します。");

            next = new Group(game, pocketedNow);
            return;
        }

        if (pocketedNow.Find(b => Ball.GetGroup(b.number) == goodGroup) == null) {
            game.ShotResultUI.Results.Add("グループボールポケットできませんでした。");
            game.ShotResultUI.Results.Add("ターンが交代します。");
            game.ChangeTurn();
        }

        next = new Call(game);
    }

    public override void Update() {}

    public void Confirm() {
        game.State = next;
    }

    bool Is8Scratch() {
        return pocketedNow.Find(b => b.number == 8) != null;
    }

    bool IsCueScratch() {
        return pocketedNow.Find(b => b.number == -1) != null;
    }

    bool IsGroupScratch(Ball.Group badGroup) {
        if (game.SolidsPlayer == GameController.Players.NONE)
            return false;

        return pocketedNow.Find(b => Ball.GetGroup(b.number) == badGroup) != null;
    }

    void SetGroup() {
        game.ShotResultUI.Results.Add("グループが決まりました。");

        GameController.Players otherPlayer = game.GetOtherPlayer();

        if (Ball.GetGroup(pocketedNow[0].number) == Ball.Group.SOLID) {
            game.SolidsPlayer = game.CurrentPlayer;
            game.ShotResultUI.Results.Add("グループはソリッドです。");
        }
        else {
            game.ShotResultUI.Results.Add("グループはストライプです。");
            game.SolidsPlayer = otherPlayer;
        }
    }
}
