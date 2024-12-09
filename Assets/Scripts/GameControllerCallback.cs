using UnityEngine;

public class GameControllerCallback : MonoBehaviour {

    // UIDocumentはStartからアクセスできる
    public void Start() {
        FindObjectOfType<GameController>().AfterSceneLoad();
    }
}