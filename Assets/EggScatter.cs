using UnityEngine;

// Randomizes egg placement everytime game is restarted
public class EggScatter : MonoBehaviour {
    void Start() {
        GameObject[] eggs = GameObject.FindGameObjectsWithTag("Egg");   // Find every egg

        foreach (GameObject egg in eggs) {
            // Try random spots until we find an empty one (up to 50 tries)
            for (int i = 0; i < 50; i++) {
                Vector2 spot = new Vector2(Random.Range(-7.4f, 7.4f), Random.Range(-3.5f, 3.5f));

                // Checks if there is an egg and places it if empty
                if (Physics2D.OverlapCircle(spot, 0.6f) == null) {
                    egg.transform.position = spot;
                    Physics2D.SyncTransforms();   // Let the next egg know this spot is taken
                    break;                        // This egg is placed, move to the next one
                }
            }
        }
    }
}