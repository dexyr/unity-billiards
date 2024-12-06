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

        game.TurnUI.Refresh(game.CurrentPlayer, game.GetCurrentGroup());
        
        if (game.SolidsPlayer != GameController.Players.NONE) {
            game.TurnUI.SetCall(call);
            game.TurnUI.Call.visible = true;
        }

        game.ShotUI.Visible = true;

        game.Stick.StickCollided += StickCollided;
    }

    public override void Exit() {
        game.StickController.gameObject.SetActive(false);
        game.ShotCamera.gameObject.SetActive(false);

        game.TurnUI.Call.visible = false;
        game.ShotUI.Visible = false;

        game.Stick.StickCollided -= StickCollided;
    }

    public override void Update() {
        if (Input.GetKeyDown(KeyCode.C)) {
            isOverhead = !isOverhead;

            game.StickController.gameObject.SetActive(!isOverhead);
            game.ShotCamera.gameObject.SetActive(!isOverhead);

            if (isOverhead)
                game.ShotUI.SetHint("(C) - ショットカメラ");
            else
                game.ShotUI.SetHint("(C) - オーバーヘッドカメラ");
        }
    }

    void StickCollided(float velocity) {
        game.State = new Simulation(game, call);
    }
}
