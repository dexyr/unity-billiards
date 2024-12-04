using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Call : GameState {
    GameObject callGhost;

    Ball.Group validGroup;

    bool isBallSet, isPocketSet;
    Ball selectedBall;
    Pocket selectedPocket;

    public Call(GameController game) : base(game) {
        callGhost = GameObject.Instantiate(game.CallGhostPrefab);
        callGhost.SetActive(false);

        validGroup = Ball.Group.SOLID; // game.GetCurrentGroup();
        isBallSet = false;
    }

    public override void Enter() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        game.CallUI.Visible = true;
        game.CallUI.Reset.visible = false;
        game.CallUI.Call.visible = false;

        game.CallUI.Reset.clicked += Reset;
        game.CallUI.Call.clicked += CallShot;
        game.CallUI.Safety.clicked += Safety;
    }

    public override void Exit() {
        GameObject.Destroy(callGhost);

        game.CallUI.Visible = false;
        game.CallUI.Reset.visible = false;
        game.CallUI.Call.visible = false;

        game.CallUI.Reset.clicked -= Reset;
        game.CallUI.Call.clicked -= CallShot;
        game.CallUI.Safety.clicked -= Safety;
    }

    public override void Update() {
        if (Input.GetMouseButtonDown(0))
            Select();
        if (!isBallSet) {
            if (MoveBall())
                callGhost.SetActive(true);
            else
                callGhost.SetActive(false);
        }
        if (isBallSet && !isPocketSet) {
        }
    }

    void Select() {
        if (!isBallSet && selectedBall) {
            isBallSet = true;

            game.CallUI.Reset.visible = true;
            game.CallUI.Call.visible = true;
        }
        if (isBallSet && selectedPocket)
            isPocketSet = true;
    }

    bool MoveBall() {
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = game.Overhead.ScreenPointToRay(mousePosition);

        Ball ball = null;

        if (Physics.Raycast(ray, out RaycastHit hit, 10, game.TableLayer)) {
            List<Collider> colliders = new List<Collider>(Physics.OverlapSphere(hit.point, 0.02f, ~(game.TableLayer | game.CueBallGhostLayer)));

            if (colliders.Count == 0)
                return false;

            var balls = new List<Ball>();

            foreach (var c in colliders) {
                Ball b = c.GetComponent<Ball>();
                if (!b)
                    continue;

                if (Ball.GetGroup(b.number) == validGroup)
                    balls.Add(b);
            }

            // ‚ ‚Æ‚ÅŽÀs‚·‚é
            // colliders.OrderBy(c => Vector3.Distance(hit.point, c.transform.position));

            if (balls.Count > 0)
                ball = balls[0];
        }

        if (!ball)
            return false;

        selectedBall = ball;
        callGhost.transform.position = ball.transform.position;
        return true;
    }

    bool MovePocket() {
        return true;
    }

    void CallShot() {
        game.State = new Shot(game, selectedBall);
    }

    void Reset() {  
        isBallSet = false;

        game.CallUI.Reset.visible = false;
        game.CallUI.Call.visible = false;
    }

    void Safety() {
        game.State = new Shot(game, null);
    }
}
