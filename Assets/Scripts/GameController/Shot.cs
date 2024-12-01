using UnityEngine;

public class Shot : GameState {
    public Shot(GameController game) : base(game) {}

    public override void Enter() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        game.Stick.gameObject.SetActive(true);
        game.ShotCamera.gameObject.SetActive(true);

        game.TurnUI.Refresh(game.CurrentPlayer, game.GetCurrentGroup());

        game.Stick.StickCollided += StickCollided;
    }

    public override void Exit() {
        game.Stick.gameObject.SetActive(false);
        game.ShotCamera.gameObject.SetActive(false);

        game.Stick.StickCollided -= StickCollided;
    }

    public override void Update() {}

    void StickCollided(float velocity) {
        game.State = new Simulation(game);
    }
}
