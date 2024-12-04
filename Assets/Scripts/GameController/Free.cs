using UnityEngine;

public class Free : GameState {
    GameObject cueBallGhost;
    float radius;

    Color valid;
    Color invalid = new Color(1, 0, 0, 0.5f);

    bool isValid, isSet, isOverhead;

    int index;

    public Free(GameController game) : base(game) {
        isValid = false;
        isSet = false;
        isOverhead = true;
        index = 0;
    }

    public override void Enter() {
        cueBallGhost = GameObject.Instantiate(game.CueBallGhostPrefab);

        valid = cueBallGhost.GetComponent<Renderer>().material.color;

        float localRadius = cueBallGhost.GetComponent<SphereCollider>().radius; // これはlocal
        radius = localRadius * cueBallGhost.transform.localScale.x; // どれでも同じ

        game.CueBall.gameObject.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        game.TurnUI.Refresh(game.CurrentPlayer, game.GetCurrentGroup());

        game.FreeUI.Visible = true;
        game.FreeUI.Confirm.visible = false;
        game.FreeUI.Cancel.visible = false;
        game.FreeUI.SetHint(new string[] { "(C) - ボールカメラ" });
        game.TargetCamera.Target = game.Balls[0].gameObject;

        game.FreeUI.Confirm.clicked += Confirm;
        game.FreeUI.Cancel.clicked += Cancel;
    }

    public override void Exit() {
        GameObject.Destroy(cueBallGhost);

        game.TargetCamera.gameObject.SetActive(false);

        game.FreeUI.Visible = false;
        game.FreeUI.Confirm.visible = false;
        game.FreeUI.Cancel.visible = false;

        game.FreeUI.Confirm.clicked -= Confirm;
        game.FreeUI.Cancel.clicked -= Cancel;
    }

    public override void Update() {
        if (isOverhead && Input.GetMouseButtonDown(0))
            Select();
        if (isOverhead && !isSet)
            Move();

        if (!isOverhead && Input.GetMouseButtonDown(0))
            ChangeBall(1);
        if (!isOverhead && Input.GetMouseButtonDown(1))
            ChangeBall(-1);

        if (Input.GetKeyDown(KeyCode.C))
            ChangeCamera();

        // Rigidbodyを使わず、Triggerも使わず
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
    }

    void ChangeBall(int value) {
        index += value;

        if (index < 0)
            index = game.Balls.Count - 1;
        if (index >= game.Balls.Count)
            index = 0;

        game.TargetCamera.Target = game.Balls[index].gameObject;
    }

    void ChangeCamera() {
        if (game.TargetCamera.gameObject.activeSelf) {
            game.TargetCamera.gameObject.SetActive(false);
            cueBallGhost.gameObject.SetActive(true);
            isOverhead = true;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            game.FreeUI.SetHint(new string[] {"(C) - ボールカメラ"});

            if (isSet) {
                game.FreeUI.Confirm.visible = true;
                game.FreeUI.Cancel.visible = true;
            }
            else {
                game.FreeUI.Confirm.visible = false;
                game.FreeUI.Cancel.visible = false;
            }
        }
        else {
            game.TargetCamera.gameObject.SetActive(true);
            isOverhead = false;

            if (!isSet)
                cueBallGhost.gameObject.SetActive(false);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            game.FreeUI.Confirm.visible = false;
            game.FreeUI.Cancel.visible = false;
            game.FreeUI.SetHint(new string[] { "(C) - オーバーヘッドカメラ", "(クリック) - ボール移動" });
        }
    }

    public void Confirm() {
        game.CueBall.transform.position = cueBallGhost.transform.position;
        game.CueBall.gameObject.SetActive(true);

        var rigidbody = game.CueBall.GetComponent<Rigidbody>();
        rigidbody.velocity = Vector3.zero;
        rigidbody.Sleep();

        game.State = new Call(game);
    }

    public void Cancel() {
        game.FreeUI.Confirm.visible = false;
        game.FreeUI.Cancel.visible = false;

        isSet = false;
    }

    void Select() {
        if (!isValid)
            return;

        game.FreeUI.Confirm.visible = true;
        game.FreeUI.Cancel.visible = true;

        isSet = true;
    }

    void Move() {
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = game.Overhead.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 10, game.TableLayer)) {
            cueBallGhost.transform.position = hit.point;
            cueBallGhost.transform.Translate(new Vector3(0, radius, 0), Space.World);
        }
    }
}
