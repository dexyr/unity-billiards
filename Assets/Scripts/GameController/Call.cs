using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public struct CallInfo {
    public Ball Ball;
    public Pocket Pocket;
}

public class Call : GameState {
    GameObject callGhost, pocketGhost;

    Ball.Group validGroup;

    bool isBallSet, isPocketSet;
    CallInfo call;

    public Call(GameController game) : base(game) {
        callGhost = GameObject.Instantiate(game.CallGhostPrefab);
        callGhost.SetActive(false);

        pocketGhost = GameObject.Instantiate(game.PocketGhostPrefab);
        pocketGhost.SetActive(false);

        validGroup = game.CurrentGroup;
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
        GameObject.Destroy(pocketGhost);

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
            if (MovePocket())
                pocketGhost.SetActive(true);
            else
                pocketGhost.SetActive(false);
        }
    }

    void Select() {
        if (!isBallSet && call.Ball) {
            isBallSet = true;

            game.CallUI.Reset.visible = true;
        }
        if (isBallSet && call.Pocket) {
            isPocketSet = true;

            game.CallUI.Call.visible = true;
        }
    }

    bool MoveBall() {
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = game.Overhead.ScreenPointToRay(mousePosition);

        Ball ball = null;

        if (Physics.Raycast(ray, out RaycastHit hit, 10, game.TableLayer)) {
            List<Collider> colliders = new List<Collider>(Physics.OverlapSphere(hit.point, 0.02f, ~(game.TableLayer)));

            if (colliders.Count == 0) {
                call.Ball = null;
                return false;
            }

            var balls = new List<Ball>();

            foreach (var c in colliders) {
                Ball b = c.GetComponent<Ball>();
                if (!b)
                    continue;

                if (Ball.GetGroup(b.number) == validGroup)
                    balls.Add(b);
            }

            // ���ƂŎ��s����
            // colliders.OrderBy(c => Vector3.Distance(hit.point, c.transform.position));

            if (balls.Count > 0)
                ball = balls[0];
        }

        if (!ball) {
            call.Ball = null;
            return false;
        }

        call.Ball = ball;
        callGhost.transform.position = ball.transform.position;
        return true;
    }

    bool MovePocket() {
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = game.Overhead.ScreenPointToRay(mousePosition);

        Pocket pocket = null;

        if (Physics.Raycast(ray, out RaycastHit hit, 10, game.TableLayer)) {
            Collider[] colliders = Physics.OverlapSphere(hit.point, 0.05f, ~(game.TableLayer));

            if (colliders.Length > 0)
                pocket = colliders[0].GetComponent<Pocket>();
        }

        if (!pocket) {
            call.Pocket = null;
            return false;
        }

        call.Pocket = pocket;
        pocketGhost.transform.position = pocket.transform.position;
        return true;
    }

    void CallShot() {
        game.State = new Shot(game, call);
    }

    void Reset() {  
        isBallSet = false;
        isPocketSet = false;

        call.Ball = null;
        call.Pocket = null;

        callGhost.SetActive(false);
        pocketGhost.SetActive(false);

        game.CallUI.Reset.visible = false;
        game.CallUI.Call.visible = false;
    }

    void Safety() {
        game.State = new Shot(game);
    }
}
