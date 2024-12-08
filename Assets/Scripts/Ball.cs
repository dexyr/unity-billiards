using UnityEngine;

public class Ball : MonoBehaviour {
    public enum Group { NONE, SOLID, STRIPE, EIGHT, CUE };
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

    [field: SerializeField] public int number { get; private set; }
    [SerializeField] float staticFriction;
    [SerializeField] float dynamicFriction;
    [SerializeField] public float Bounce;
    [SerializeField] float minBounce;

    PhysicMaterial physicMaterial;

    new Rigidbody rigidbody;

    void Awake() {
        physicMaterial = new PhysicMaterial();
        physicMaterial.dynamicFriction = dynamicFriction;
        physicMaterial.staticFriction = staticFriction;
        physicMaterial.bounciness = minBounce;

        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.material = physicMaterial;

        rigidbody = GetComponent<Rigidbody>();
        rigidbody.maxAngularVelocity = float.MaxValue;
    }
    void FixedUpdate() {
        Bounce = minBounce + rigidbody.velocity.magnitude * Time.fixedDeltaTime * 5;
        Bounce = Mathf.Clamp(Bounce, minBounce, 1);

        physicMaterial.bounciness = Bounce;
    }
}
