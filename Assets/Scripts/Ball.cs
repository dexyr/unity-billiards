using UnityEngine;

public class Ball : MonoBehaviour {
    public delegate void MotionHandler(Ball self, bool isMoving);
    public MotionHandler MotionUpdated;

    public enum Group { NONE, SOLID, STRIPE, EIGHT };
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

    new Rigidbody rigidbody;
    bool isMoving;

    void Awake() {
        rigidbody = GetComponent<Rigidbody>();
    }

    void Start() {
        var material = GetComponent<Renderer>().material;

        if (number < 1 || number > 15) {
            material.color = Color.magenta;
            return;
        }

        material.color = colors[number - 1];
    }

    void FixedUpdate() {
        // if (!isMoving)
        //     return;

        float velocity = Mathf.Abs(rigidbody.velocity.magnitude);

        if (velocity > 0.01f)
            MotionUpdated?.Invoke(this, true);
        else
            MotionUpdated?.Invoke(this, false);
    }

    /*
    void OnCollisionEnter(Collision collision) {
        var ball = collision.gameObject.GetComponent<Ball>();
        var cueBall = collision.gameObject.GetComponent<CueBall>();

        isMoving = true;
        MotionUpdated?.Invoke(this, true);
    }
    */

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
