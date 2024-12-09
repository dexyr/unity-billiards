using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Players { NONE, PLAYER1, PLAYER2 } // Listのほうがフレキシブルだが
public enum CameraStates { OVERHEAD, CUE, BALL }

public class GameController : MonoBehaviour {
    public delegate void LogUpdateHandler(string message);
    public event LogUpdateHandler LogUpdated;

    public delegate void StateChangeHandler(GameState state);
    public event StateChangeHandler StateChanged;

    public delegate void TurnChangeHandler(Players turn);
    public event TurnChangeHandler TurnChanged;

    [SerializeField] public MenuUI MenuUI;
    [SerializeField] public TurnUI TurnUI;
    [SerializeField] public HintUI HintUI;
    [SerializeField] public BreakUI BreakUI;
    [SerializeField] public CallUI CallUI;
    [SerializeField] public ShotResultUI ShotResultUI;
    [SerializeField] public GroupUI GroupUI;
    [SerializeField] public EndUI EndUI;
    [SerializeField] public FreeUI FreeUI;

    public TargetCamera TargetCamera;

    public CueStickController StickController;
    public CueStick Stick;
    public CueBall CueBall;
    public Camera Overhead;
    public Camera ShotCamera;

    GameObject ballSet;
    public List<Ball> Balls = new List<Ball>();
    public List<Pocket> Pockets = new List<Pocket>();
    public List<Cushion> Cushions = new List<Cushion>();

    [SerializeField] GameObject ballSetPrefab, cueBallPrefab;

    [SerializeField] public GameObject CueBallGhostPrefab, CallGhostPrefab, PocketGhostPrefab;
    [SerializeField] public LayerMask TableLayer, CueBallGhostLayer, PocketLayer;

    public List<Ball> Pocketed { get; private set; } = new List<Ball>();
    public List<Ball> Solids = new List<Ball>();
    public List<Ball> Stripes = new List<Ball>();
    public List<Ball> Player1Balls = new List<Ball>();
    public List<Ball> Player2Balls = new List<Ball>();

    private GameState state;
    public GameState State {
        get => state;
        set {
            if (state != null)
                state.Exit();

            value.Enter();
            state = value;
            StateChanged?.Invoke(State);
        }
    }

    public Players Winner = Players.NONE;
    public Players CurrentPlayer = Players.NONE;
    public Players OtherPlayer {
        get => CurrentPlayer == Players.PLAYER1 ? Players.PLAYER2 : Players.PLAYER1;
    }

    public Ball.Group CurrentGroup {
        get => SolidsPlayer == Players.NONE ? Ball.Group.NONE
            : SolidsPlayer == CurrentPlayer ? Ball.Group.SOLID
            : Ball.Group.STRIPE;
    }
    public Ball.Group OtherGroup {
        get => CurrentGroup == Ball.Group.NONE ? Ball.Group.NONE
            : CurrentGroup == Ball.Group.SOLID ? Ball.Group.STRIPE
            : Ball.Group.SOLID;
    }

    public bool IsBreak;

    public Players SolidsPlayer = Players.NONE;
    public bool IsOpen {
        get => SolidsPlayer == Players.NONE;
    }

    public bool IsEightShot {
        get {
            return SolidsPlayer == CurrentPlayer ? Solids.Count == 0 : Stripes.Count == 0;
        }
    }

    public void Start() {
        ShotResultUI.Visible = false;
        CallUI.Visible = false;
        HintUI.Visible = false;
        BreakUI.Visible = false;
        TurnUI.Visible = false;
        TurnUI.Call.visible = false;
        GroupUI.Visible = false;
        FreeUI.Visible = false;
        EndUI.Visible = false;
    }

    public void Update() {
        if (state != null)
            state.Update();

        if (Input.GetKeyDown(KeyCode.R)) {
            ClearTable();
            SetTable();
            CurrentPlayer = Players.PLAYER1;
            IsBreak = true;

            State = new Menu(this);
        }
    }

    public void StartGame() {
        SceneManager.LoadScene("Game");
    }

    public void StartMenu() {
        SceneManager.LoadScene("Menu");
    }

    public void EndGame(Players winner) {
        SceneManager.LoadScene("End");
        Winner = winner;
    }

    // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)] これ使えない
    public void AfterSceneLoad() {
        switch(SceneManager.GetActiveScene().name) {
        case "Game":
            OnGameLoad();
            break;

        case "Menu":
            OnMenuLoad();
            break;

        case "End":
            OnEndLoad();
            break;
        }
    }

    public void OnMenuLoad() {
        State = new Menu(this);
    }

    public void OnEndLoad() {
        State = new End(this);
    }

    public void OnGameLoad() {
        Pockets.Clear();
        foreach (Pocket p in FindObjectsByType<Pocket>(FindObjectsSortMode.None))
            Pockets.Add(p);

        Cushions.Clear();
        foreach (Cushion c in FindObjectsByType<Cushion>(FindObjectsSortMode.None))
            Cushions.Add(c);

        StickController = FindObjectOfType<CueStickController>();
        Stick = FindObjectOfType<CueStick>();

        StickController.gameObject.SetActive(false); // parent

        Overhead = GameObject.FindGameObjectWithTag("Overhead Camera").GetComponent<Camera>();

        ShotCamera = GameObject.FindGameObjectWithTag("Shot Camera").GetComponent<Camera>();
        ShotCamera.gameObject.SetActive(false);

        TargetCamera = FindObjectOfType<TargetCamera>();
        TargetCamera.gameObject.SetActive(false);

        ClearTable();
        SetTable();
        CurrentPlayer = Players.PLAYER1;
        Winner = Players.NONE;
        IsBreak = true;

        State = new Shot(this);
    }

    public void SetTable() {
        var rackSpawn = GameObject.FindGameObjectWithTag("Rack Spawn").GetComponent<Transform>();
        ballSet = Instantiate(ballSetPrefab, rackSpawn.position, rackSpawn.rotation);

        foreach (Ball b in ballSet.GetComponentsInChildren<Ball>()) {
            Balls.Add(b);

            if (Ball.GetGroup(b.number) == Ball.Group.SOLID)
                Solids.Add(b);

            if (Ball.GetGroup(b.number) == Ball.Group.STRIPE)
                Stripes.Add(b);
        }

        var cueBallSpawn = GameObject.FindGameObjectWithTag("Cue Spawn").GetComponent<Transform>();
        CueBall = Instantiate(cueBallPrefab, cueBallSpawn.position, cueBallSpawn.rotation).GetComponent<CueBall>();

        StickController.Target = CueBall.gameObject;

        IsBreak = true;
    }

    public void ClearTable() {
        if (ballSet)
            Destroy(ballSet);

        if (CueBall)
            Destroy(CueBall.gameObject);

        Balls.Clear();
        Pocketed.Clear();
        Solids.Clear();
        Stripes.Clear();
        Player1Balls.Clear();
        Player2Balls.Clear();

        StickController.Target = null;
    }

    public void ChangeTurn() {
        switch (CurrentPlayer) {
        case Players.NONE:
            CurrentPlayer = Players.PLAYER1;
            break;

        case Players.PLAYER1:
            CurrentPlayer = Players.PLAYER2;
            break;

        case Players.PLAYER2:
            CurrentPlayer = Players.PLAYER1;
            break;
        }

        TurnChanged?.Invoke(CurrentPlayer);
    }

    public void Log(string message) {
        LogUpdated?.Invoke(message);
    }
}
