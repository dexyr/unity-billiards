using UnityEngine;

public class Ball : MonoBehaviour {
    public enum Group { NONE, SOLID, STRIPE, EIGHT };
    static Color purple = new Color(0.2f, 0f, 0.8f);
    static Color orange = new Color(1f, 0.4f, 0f);
    static Color maroon = new Color(0.8f, 0f, 0f);

    static Color[] colors = {
        Color.yellow,
        Color.blue,
        Color.red,
        purple,
        orange,
        Color.green,
        maroon,
        Color.black,
        Color.yellow,
        Color.blue,
        Color.red,
        purple,
        orange,
        Color.green,
        maroon
    };

    // ‚±‚±‚Éfield‚ª•K—v
    [field: SerializeField] public int number { get; private set; }

    void Awake() {
        var material = GetComponent<Renderer>().material;

        if (number < 1 || number > 15) {
            material.color = Color.magenta;
            return;
        }

        material.color = colors[number + 1];
    }

    static public Group GetGroup(int number) {
        if (number > 0 && number < 8)
            return Group.SOLID;
        if (number > 9 && number < 16)
            return Group.STRIPE;
        if (number == 8)
            return Group.EIGHT;

        return Group.NONE;
    }
}
