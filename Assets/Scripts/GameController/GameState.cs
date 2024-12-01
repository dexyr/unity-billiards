public abstract class GameState {
    protected GameController game;

    public GameState(GameController game) {
        this.game = game;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}
