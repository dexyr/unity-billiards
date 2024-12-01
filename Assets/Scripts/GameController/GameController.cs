using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    public enum Players { NONE, PLAYER1, PLAYER2 } // Listのほうがフレキシブルだが

    public delegate void LogUpdateHandler(string message);
    public event LogUpdateHandler LogUpdated;

    public delegate void StateChangeHandler(GameState state);
    public event StateChangeHandler StateChanged;

    public delegate void TurnChangeHandler(Players turn);
    public event TurnChangeHandler TurnChanged;

    [SerializeField] public MenuUI MenuUI;
    [SerializeField] public TurnUI TurnUI;
    [SerializeField] public ShotResultUI ShotResultUI;
    [SerializeField] public GroupMenuUI GroupMenuUI;
    [SerializeField] public EndUI EndUI;
    [SerializeField] public FreeUI FreeUI;

    public CueStick Stick;
    public CueBall CueBall;
    public Camera Overhead;
    public Camera ShotCamera;

    public List<Ball> Balls = new List<Ball>();
    public List<Pocket> Pockets = new List<Pocket>();

    [SerializeField] GameObject ballSetPrefab;

    [SerializeField] public GameObject CueBallGhostPrefab;
    [SerializeField] public LayerMask TableLayer, CueBallGhostLayer;

    public List<Ball> Pocketed { get; private set; } = new List<Ball>();
    public List<Ball> Solids = new List<Ball>();
    public List<Ball> Stripes = new List<Ball>();
    public List<Ball> Player1Balls = new List<Ball>();
    public List<Ball> Player2Balls = new List<Ball>();

    public bool IsBreak;
    public Players SolidsPlayer = Players.NONE;

    private GameState state;
    public GameState State {
        get => state;
        set {
            state.Exit();
            value.Enter();
            state = value;
            StateChanged?.Invoke(State);
        }
    }

    public Players CurrentPlayer { get; private set; } = Players.NONE;
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

    public void Start() {
        var spawnTransform = GameObject.FindGameObjectWithTag("Ball Spawn").GetComponent<Transform>();
        GameObject ballSet = Instantiate(ballSetPrefab, spawnTransform.position, spawnTransform.rotation);

        foreach (Ball b in ballSet.GetComponentsInChildren<Ball>()) {
            Balls.Add(b);

            if (Ball.GetGroup(b.number) == Ball.Group.SOLID)
                Solids.Add(b);

            if (Ball.GetGroup(b.number) == Ball.Group.STRIPE)
                Stripes.Add(b);
        }

        foreach (Pocket p in FindObjectsByType<Pocket>(FindObjectsSortMode.None))
            Pockets.Add(p);


        Stick = FindObjectOfType<CueStick>();
        Stick.gameObject.SetActive(false);

        Overhead = GameObject.FindGameObjectWithTag("Overhead Camera").GetComponent<Camera>();

        ShotCamera = GameObject.FindGameObjectWithTag("Shot Camera").GetComponent<Camera>();
        ShotCamera.gameObject.SetActive(false);

        CueBall = FindObjectOfType<CueBall>();

        ShotResultUI.Visible = false;
        TurnUI.Visible = false;
        GroupMenuUI.Visible = false;
        FreeUI.Visible = false;
        EndUI.Visible = false;
        state = new Menu(this);
        state.Enter();
    }

    public void Update() {
        state.Update();
    }

    public Players GetOtherPlayer() {
        return CurrentPlayer == Players.PLAYER1 ? Players.PLAYER2 : Players.PLAYER1;
    }

    public Ball.Group GetCurrentGroup() {
        if (SolidsPlayer == Players.NONE)
            return Ball.Group.NONE;

        return SolidsPlayer == CurrentPlayer ? Ball.Group.SOLID : Ball.Group.STRIPE;
    }

    public bool IsEightShot() {
        if (SolidsPlayer == CurrentPlayer)
            return Solids.Count == 0;
        else
            return Stripes.Count == 0;
    }

    public void Log(string message) {
        LogUpdated?.Invoke(message);
    }
}
