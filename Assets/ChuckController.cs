using UnityEngine;

// Uses WASD to move Chuck
public class ChuckController : MonoBehaviour {
    public float speed = 5f;   // Chuck's speed

    void Update() {
        float x = 0;   // Left/right movement
        float y = 0;   // Up/down movement

        if (Input.GetKey(KeyCode.W)) y = 1;    // Up
        if (Input.GetKey(KeyCode.S)) y = -1;   // Down
        if (Input.GetKey(KeyCode.A)) x = -1;   // Left
        if (Input.GetKey(KeyCode.D)) x = 1;    // Right

        // Move Chuck using physics so he can't walk through obstacles
        GetComponent<Rigidbody2D>().linearVelocity = new Vector2(x, y) * speed;
    }
}