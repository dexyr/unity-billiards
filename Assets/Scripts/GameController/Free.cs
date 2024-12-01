using System.Collections.Generic;
using UnityEngine;

public class Free : GameState {
    GameObject cueBallGhost;
    float radius;

    Color valid;
    Color invalid = new Color(1, 0, 0, 0.5f);

    bool isValid, isSet;

    public Free(GameController game) : base(game) {
        isValid = false;
        isSet = false;
    }

    public override void Enter() {
        cueBallGhost = GameObject.Instantiate(game.CueBallGhostPrefab);

        valid = cueBallGhost.GetComponent<Renderer>().material.color;

        float localRadius = cueBallGhost.GetComponent<SphereCollider>().radius; // Ç±ÇÍÇÕlocal
        radius = localRadius * cueBallGhost.transform.localScale.x; // Ç«ÇÍÇ≈Ç‡ìØÇ∂

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        game.FreeUI.Visible = true;
        game.FreeUI.Confirm.visible = false;
        game.FreeUI.Cancel.visible = false;

        game.FreeUI.Confirm.clicked += Confirm;
        game.FreeUI.Cancel.clicked += Cancel;
    }

    public override void Exit() {
        GameObject.Destroy(cueBallGhost);

        game.FreeUI.Visible = false;
        game.FreeUI.Confirm.visible = false;
        game.FreeUI.Cancel.visible = false;

        game.FreeUI.Confirm.clicked -= Confirm;
        game.FreeUI.Cancel.clicked -= Cancel;
    }

    public override void Update() {
        // RigidbodyÇégÇÌÇ∏ÅATriggerÇ‡égÇÌÇ∏
        Collider[] colliders = Physics.OverlapSphere(cueBallGhost.transform.position, radius, ~(game.TableLayer | game.CueBallGhostLayer));

        var material = cueBallGhost.GetComponent<Renderer>().material;

        if (colliders.Length == 0) {
            isValid = true;
            material.color = valid;
        }
        else {
            isValid = false;
            material.color = invalid;
        }

        if (Input.GetMouseButtonDown(0))
            select();
        if (!isSet)
            move();
    }

    public void Confirm() {
        game.CueBall.transform.position = cueBallGhost.transform.position;
        game.CueBall.gameObject.SetActive(true);

        var rigidbody = game.CueBall.GetComponent<Rigidbody>();
        rigidbody.velocity = Vector3.zero;
        rigidbody.Sleep();

        game.State = new Shot(game);
    }

    public void Cancel() {
        game.FreeUI.Confirm.visible = false;
        game.FreeUI.Cancel.visible = false;

        isSet = false;
    }

    void select() {
        if (!isValid)
            return;

        game.FreeUI.Confirm.visible = true;
        game.FreeUI.Cancel.visible = true;

        isSet = true;
    }

    void move() {
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = game.Overhead.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 10, game.TableLayer)) {
            cueBallGhost.transform.position = hit.point;
            cueBallGhost.transform.Translate(new Vector3(0, radius, 0), Space.World);
        }
    }
}
