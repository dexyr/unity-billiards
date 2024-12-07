using System.Collections.Generic;
using UnityEngine;

public class Group : GameState {
    List<Ball> pocketedNow;

    public Group(GameController game, List<Ball> pocketedNow) : base(game) {
        this.pocketedNow = pocketedNow;

        foreach (Ball b in pocketedNow)
            Debug.Log(b);
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

        game.GroupMenuUI.Refresh(solids, stripes);
        game.GroupMenuUI.Visible = true;

        game.GroupMenuUI.GroupChosen += ChooseGroup;
    }

    public override void Exit() {
        game.GroupMenuUI.Visible = false;

        game.GroupMenuUI.GroupChosen -= ChooseGroup;
    }

    public override void Update() {}

    public void ChooseGroup(Ball.Group group) {
        GameController.Players otherPlayer = game.OtherPlayer;

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
