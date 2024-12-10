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
        game.StickController.Sensitivity = game.CurrentPlayerSensitivity;
        Debug.Log($"setting {game.CurrentPlayer} sens to {game.CurrentPlayerSensitivity}");
        Debug.Log($"{game.Player1Sensitivity}, {game.Player2Sensitivity}");

        game.ShotCamera.gameObject.SetActive(true);

        game.HintUI.Visible = true;
        game.HintUI.SetHint(new string[] { "(C) - オーバーヘッドカメラ" });

        game.TurnUI.Visible = true;
        game.TurnUI.Refresh(game.CurrentPlayer, game.CurrentGroup);
        game.TurnUI.OptionHint.visible = true;
        
        if (game.SolidsPlayer != Players.NONE) {
            game.TurnUI.SetCall(call);
            game.TurnUI.Call.visible = true;
        }

        game.Stick.StickCollided += StickCollided;
        game.Unpaused += Unpaused;
    }

    public override void Exit() {
        game.StickController.gameObject.SetActive(false);
        game.ShotCamera.gameObject.SetActive(false);

        game.HintUI.Visible = false;
        game.TurnUI.Call.visible = false;

        game.Stick.StickCollided -= StickCollided;
        game.Unpaused -= Unpaused;
    }

    public override void Update() {
        if (Input.GetKeyDown(KeyCode.C)) {
            isOverhead = !isOverhead;

            game.StickController.gameObject.SetActive(!isOverhead);
            game.ShotCamera.gameObject.SetActive(!isOverhead);

            if (isOverhead)
                game.HintUI.SetHint(new string[] { "(C) - ショットカメラ" });
            else
                game.HintUI.SetHint(new string[] { "(C) - オーバーヘッドカメラ" });
        }

        if (Input.GetKeyDown(KeyCode.O))
            game.IsPaused = true;
    }

    void Unpaused() {
        game.StickController.Sensitivity = game.CurrentPlayerSensitivity;
        Debug.Log($"setting {game.CurrentPlayer} sens to {game.CurrentPlayerSensitivity}");
    }

    void StickCollided(Collision collision, CueBall cueBall) {
        game.State = new Simulation(game, call);
    }
}
