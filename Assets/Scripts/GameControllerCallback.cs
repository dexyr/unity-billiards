using UnityEngine;

public class GameControllerCallback : MonoBehaviour {

    // UIDocument��Start����A�N�Z�X�ł���
    public void Start() {
        FindObjectOfType<GameController>().AfterSceneLoad();
    }
}