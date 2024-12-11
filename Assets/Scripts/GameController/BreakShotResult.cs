using System.Collections.Generic;
using UnityEngine;

public class BreakShotResult : GameState {
    List<Ball> pocketedNow;
    List<Ball> cushionBalls;
    bool ballOut;
    bool changeTurn = false;

    GameState next;

    public BreakShotResult(GameController game, List<Ball> pocketedNow, List<Ball> cushionBalls, bool ballOut) : base(game) {
        this.pocketedNow = pocketedNow;
        this.cushionBalls = cushionBalls;
        this.ballOut = ballOut;
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
        game.IsBreak = false;

        game.ShotResultUI.Visible = false;
        game.ShotResultUI.Results.Clear();

        game.ShotResultUI.Confirm.clicked -= Confirm;
    }

    void CalculateResults() {
        int pocketed = pocketedNow.Count;
        if (pocketedNow.Contains(game.CueBall.GetComponent<Ball>()) && pocketed > 0)
            pocketed--;

        game.ShotResultUI.Results.Add($"�{�[����{pocketed}�|�P�b�g���܂����B");

        if (Is8Scratch()) {
            game.ShotResultUI.Results.Add($"�G�C�g�{�[���X�N���b�`���܂����B");

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
            game.ShotResultUI.Results.Add("��O�X�N���b�`���܂����B");
            game.ShotResultUI.Results.Add("�u���C�N���s���܂����B");
            game.ShotResultUI.Results.Add("�^�[������サ�܂��B");

            changeTurn = true;
            next = new Break(game);
            return;
        }

        if (IsCueScratch()) {
            game.ShotResultUI.Results.Add("�苅�X�N���b�`���܂����B");
            game.ShotResultUI.Results.Add("�u���C�N���s���܂����B");
            game.ShotResultUI.Results.Add("�^�[������サ�܂��B");

            changeTurn = true;
            next = new Break(game);
            return;
        }

        if (pocketedNow.Count == 0 && cushionBalls.Count < 4) {
            game.ShotResultUI.Results.Add("�{�[����1���|�P�b�g�ł��܂���ł����B");
            game.ShotResultUI.Results.Add("�{�[����4�N�b�V�����܂œ͂��܂���ł����B");
            game.ShotResultUI.Results.Add("�u���C�N���s���܂����B");
            game.ShotResultUI.Results.Add("�^�[������サ�܂��B");

            changeTurn = true;
            next = new Break(game);
            return;
        }

        if (pocketedNow.Count == 0) {
            game.ShotResultUI.Results.Add("�u���C�N�����ł����B");
            game.ShotResultUI.Results.Add("�^�[������サ�܂��B");

            changeTurn = true;
            next = new Shot(game);
            return;
        }

        game.ShotResultUI.Results.Add("�u���C�N�����ŃO���[�v�I�����܂��B");

        game.IsBreak = false;
        next = new Group(game, pocketedNow);
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

    bool IsCueScratch() {
        return pocketedNow.Find(b => b.number == -1) != null;
    }
}
