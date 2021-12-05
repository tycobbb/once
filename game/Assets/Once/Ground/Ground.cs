using UnityEngine;

/// a ground plane that follows the player
public class Ground: MonoBehaviour {
    // -- config --
    [Header("config")]
    [Tooltip("the follow target")]
    [SerializeField] Transform m_Target;

    // -- lifecycle --
    void FixedUpdate() {
        Follow();
    }

    // -- commands --
    // follow the target's position
    void Follow() {
        var tg = transform;
        var p1 = m_Target.position;
        p1.y = 0.0f;
        tg.position = p1;
    }
}
