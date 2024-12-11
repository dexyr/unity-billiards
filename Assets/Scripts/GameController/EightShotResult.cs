using System.Collections.Generic;
using UnityEngine;

public class EightShotResult : GameState {
    List<Ball> pocketedNow;
    List<Ball> cushionBalls;
    Ball firstTouched;
    bool ballOut;
    bool changeTurn = false;

    CallInfo? call;

    GameState next;

    public EightShotResult(GameController game, List<Ball> pocketedNow, List<Ball> cushionBalls, Ball firstTouched, bool ballOut, CallInfo? call) : base(game) {
        this.pocketedNow = pocketedNow;
        this.cushionBalls = cushionBalls;
        this.ballOut = ballOut;
        this.firstTouched = firstTouched;
        this.ballOut = ballOut;
        this.call = call;
    }

    public override void Enter() {
        CalculateResults();

        game.TurnUI.Visible = true;
        game.TurnUI.OptionHint.visible = true;
        game.TurnUI.Refresh(game.CurrentPlayer, game.CurrentGroup);

        game.ShotResultUI.Refresh();
        game.ShotResultUI.Visible = true;

        game.ShotResultUI.Confirm.clicked += Confirm;
    }

    public override void Exit() {
        game.ShotResultUI.Results.Clear();
        game.ShotResultUI.Visible = false;

        game.ShotResultUI.Confirm.clicked -= Confirm;
    }

    public void CalculateResults() {
        if (ballOut) {
            game.ShotResultUI.Results.Add("��O�X�N���b�`���܂����B");
            game.ShotResultUI.Results.Add("�����ł��B");

            game.Winner = game.OtherPlayer;
            next = new End(game);
            return;
        }

        if (IsCueScratch()) {
            game.ShotResultUI.Results.Add("�苅�X�N���b�`���܂����B");
            game.ShotResultUI.Results.Add("�����ł��B");

            game.Winner = game.OtherPlayer;
            next = new End(game);
            return;
        }

        if (IsNoHit()) {
            game.ShotResultUI.Results.Add("�m�[�q�b�g�t�@�E�����܂����B");
            game.ShotResultUI.Results.Add("�����ł��B");

            game.Winner = game.OtherPlayer;
            next = new End(game);
            return;
        }

        if (IsNoCushion()) {
            game.ShotResultUI.Results.Add("�m�[�N�b�V�����t�@�E�����܂����B");
            game.ShotResultUI.Results.Add("�����ł��B");

            game.Winner = game.OtherPlayer;
            next = new End(game);
            return;
        }
        if (IsEightPocketed() ) {
            game.ShotResultUI.Results.Add("�苅�X�N���b�`���܂����B");
            game.ShotResultUI.Results.Add("�����ł��B");

            game.Winner = game.OtherPlayer;
            next = new End(game);
            return;
        }

        if (IsEightPocketed()) {
            game.ShotResultUI.Results.Add("�G�C�g�{�[�����|�P�b�g���܂����B");
            game.ShotResultUI.Results.Add("�����ł��B");

            game.Winner = game.CurrentPlayer;
            next = new End(game);
            return;
        }

        game.ShotResultUI.Results.Add("�G�C�g�{�[�����|�P�b�g�ł��܂���ł����B");
        game.ShotResultUI.Results.Add("�^�[������サ�܂��B");

        changeTurn = true;
        next = new Call(game);
    }

    public override void Update() {
        if (Input.GetKeyDown(KeyCode.O))
            game.Pause(new Settings(game));
    }

    bool IsNoHit() {
        if (call == null)
            return false;

        return firstTouched == null || Ball.GetGroup(firstTouched.number) != Ball.Group.EIGHT;
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

    bool IsEightPocketed() {
        return pocketedNow.Find(b => b.number == 8) != null;
    }

    public void Confirm() {
        game.State = next;

        if (changeTurn)
            game.ChangeTurn();

        if (next is End)
            game.EndGame();
    }
}
