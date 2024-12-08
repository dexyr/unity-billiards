using UnityEngine;

public class Shot : GameState {
    bool isOverhead;
    CallInfo? call;

    public Shot(GameController game, CallInfo? call=null) : base(game) {
        this.call = call;
        isOverhead = false;
    }

    public override void Enter() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        game.StickController.gameObject.SetActive(true);
        game.ShotCamera.gameObject.SetActive(true);

        game.HintUI.Visible = true;
        game.HintUI.SetHint(new string[] { "(C) - �I�[�o�[�w�b�h�J����" });

        game.TurnUI.Refresh(game.CurrentPlayer, game.CurrentGroup);
        
        if (game.SolidsPlayer != Players.NONE) {
            game.TurnUI.SetCall(call);
            game.TurnUI.Call.visible = true;
        }

        game.Stick.StickCollided += StickCollided;
    }

    public override void Exit() {
        game.StickController.gameObject.SetActive(false);
        game.ShotCamera.gameObject.SetActive(false);

        game.HintUI.Visible = false;
        game.TurnUI.Call.visible = false;

        game.Stick.StickCollided -= StickCollided;
    }

    public override void Update() {
        if (Input.GetKeyDown(KeyCode.C)) {
            isOverhead = !isOverhead;

            game.StickController.gameObject.SetActive(!isOverhead);
            game.ShotCamera.gameObject.SetActive(!isOverhead);

            if (isOverhead)
                game.HintUI.SetHint(new string[] { "(C) - �V���b�g�J����" });
            else
                game.HintUI.SetHint(new string[] { "(C) - �I�[�o�[�w�b�h�J����" });
        }
    }

    void StickCollided(Collision collision, CueBall cueBall) {
        game.State = new Simulation(game, call);
    }
}
