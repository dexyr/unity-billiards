using UnityEngine;

public class Ball : MonoBehaviour {
    public enum Group { NONE, SOLID, STRIPE, EIGHT, CUE };
    static Color purple = new Color(0.5f, 0f, 0.5f);
    static Color orange = new Color(1f, 0.4f, 0f);
    static Color maroon = new Color(0.7f, 0f, 0f);

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

    [field: SerializeField] public int number { get; private set; }

    void Start() {
        // SetColor();
    }

    void SetColor() {
        var material = GetComponent<Renderer>().material;

        if (number < 1 || number > 15) {
            material.color = Color.magenta;
            return;
        }

        material.color = colors[number - 1];
    }

    static public Group GetGroup(int number) {
        if (number > 0 && number < 8)
            return Group.SOLID;
        if (number > 8 && number < 16)
            return Group.STRIPE;
        if (number == 8)
            return Group.EIGHT;
        if (number == -1)
            return Group.CUE;

        return Group.NONE;
    }
}
