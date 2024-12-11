using System.Collections.Generic;
using UnityEngine;

public class Group : GameState {
    List<Ball> pocketedNow;

    public Group(GameController game, List<Ball> pocketedNow) : base(game) {
        this.pocketedNow = pocketedNow;
    }

    public override void Enter() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        List<Ball> solids = new List<Ball>();
        List<Ball> stripes = new List<Ball>();

        foreach (Ball b in pocketedNow) {
            if (Ball.GetGroup(b.number) == Ball.Group.SOLID)
                solids.Add(b);

            if (Ball.GetGroup(b.number) == Ball.Group.STRIPE)
                stripes.Add(b);
        }

        game.GroupUI.Refresh(solids, stripes);
        game.GroupUI.Visible = true;

        game.TurnUI.Visible = true;
        game.TurnUI.OptionHint.visible = true;

        game.GroupUI.GroupChosen += ChooseGroup;
    }

    public override void Exit() {
        game.GroupUI.Visible = false;

        game.GroupUI.GroupChosen -= ChooseGroup;
    }

    public override void Update() {
        if (Input.GetKeyDown(KeyCode.O))
            game.Pause(new Settings(game));
    }

    public void ChooseGroup(Ball.Group group) {
        Players otherPlayer = game.OtherPlayer;

        if (group == Ball.Group.SOLID) {
            game.SolidsPlayer = game.CurrentPlayer;
            game.TurnUI.Refresh(game.CurrentPlayer, Ball.Group.SOLID);
        }
        else {
            game.SolidsPlayer = otherPlayer;
            game.TurnUI.Refresh(game.CurrentPlayer, Ball.Group.STRIPE);
        }
        
        game.State = new Call(game);
    }

}
