using UnityEngine;
using TMPro;   
using UnityEngine.Rendering.Universal;   // 2D light
using UnityEngine.SceneManagement;       // Reloads scenes
using UnityEngine.UI;                    // Changes the color of the sky

// Uses WASD to move Chuck
public class ChuckController : MonoBehaviour {
    public float speed = 5f;        // Chuck's speed
    public int eggsCollected = 0;   // Eggs Chuck has collected
    public int score = 0;           // Total points

    public TextMeshProUGUI eggText;     // Displays egg counter on screen
    public TextMeshProUGUI scoreText;   // Displays score on screen
    public TextMeshProUGUI messageText; // Displays message
    public TextMeshProUGUI timerText;   // Displays time
    private int totalEggs;              // How many eggs are in the garden

    public float timeLimit = 60f;       // Time left until sunset
    public Light2D globalLight;         // Sky changes color
    private float timeLeft;             // Seconds remaining

    public Image skyWindow;             // The sky above the garden
    public RectTransform sun;           // The sun that sets behind the hills
    public RectTransform moon;          // The moon that rises at night

    public GameObject endPanel;         // Displays the win/lose screen
    public TextMeshProUGUI endText;     

    public AudioClip eggSound;          // Plays when Chuck picks up an egg
    public AudioClip rainbowSound;      // Plays when Chuck picks up the rainbow egg
    public AudioClip chirpSound;        // Plays when Chuck tries the basket too early
    public AudioClip winSound;          // Plays when Chuck wins
    public AudioClip loseSound;         // Plays when the sun sets
    private AudioSource audioSource;    // Plays the sounds

    private bool nearBasket = false;    // Checks if Chuck is on the basket
    private bool gameOver = false;      

    void Start() {
        totalEggs = GameObject.FindGameObjectsWithTag("Egg").Length;   // Counts all eggs
        timeLeft = timeLimit;                                          // Begins timer
        endPanel.SetActive(false);                                     // Hides the end screen
        audioSource = GetComponent<AudioSource>();                     // Finds the audio source
        UpdateUI();
    }

    void Update() {
        if (gameOver) {
            GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            return;
        }

        // Sunset timer
        timeLeft -= Time.deltaTime;
        timerText.text = "Sunset in: " + Mathf.CeilToInt(timeLeft);

        // Changes the sky color as time runs out (1 = start of day, 0 = sunset)
        float daylight = timeLeft / timeLimit;

        Color dayColor = new Color(0.8f, 0.9f, 1f);      // Bright, slightly blue daylight
        Color sunsetColor = new Color(1f, 0.6f, 0.3f);   // Orange sunset
        Color nightColor = new Color(0.2f, 0.2f, 0.45f); // Dark blue night

        if (daylight > 0.5f) {
            // First half of the timer: blue daylight fades to orange
            globalLight.color = Color.Lerp(sunsetColor, dayColor, (daylight - 0.5f) * 2);
            globalLight.intensity = 1f;
        } else {
            // Second half of the timer: orange fades to dark night
            globalLight.color = Color.Lerp(nightColor, sunsetColor, daylight * 2);
            globalLight.intensity = Mathf.Lerp(0.3f, 1f, daylight * 2);   // Gets dimmer too
        }

        // Changes the color of the sky
        if (daylight > 0.5f) {
            skyWindow.color = Color.Lerp(new Color(1f, 0.55f, 0.25f), new Color(0.5f, 0.8f, 1f), (daylight - 0.5f) * 2);
        } else {
            skyWindow.color = Color.Lerp(new Color(0.1f, 0.1f, 0.3f), new Color(1f, 0.55f, 0.25f), daylight * 2);
        }

        // Sun lowers into the hills as time runs out
        sun.anchoredPosition = new Vector2(sun.anchoredPosition.x, Mathf.Lerp(-130f, 40f, daylight));

        // Moon rises during the second half of the timer
        if (daylight < 0.5f) {
            moon.anchoredPosition = new Vector2(moon.anchoredPosition.x, Mathf.Lerp(30f, -130f, daylight * 2));
        }

        // Lose if the sun fully sets
        if (timeLeft <= 0) {
            timerText.text = "Sunset in: 0";
            PlaySound(loseSound);                              // Lose sound
            EndGame("The sun set! Chuck ran out of time.");   // Lose screen
            return;
        }

        float x = 0;   // Left/right movement
        float y = 0;   // Up/down movement

        if (Input.GetKey(KeyCode.W)) y = 1;    // Up
        if (Input.GetKey(KeyCode.S)) y = -1;   // Down
        if (Input.GetKey(KeyCode.A)) x = -1;   // Left
        if (Input.GetKey(KeyCode.D)) x = 1;    // Right

        // Move Chuck using physics so he can't walk through obstacles
        GetComponent<Rigidbody2D>().linearVelocity = new Vector2(x, y) * speed;

        // Make Chuck face the way he's walking
        if (x < 0) GetComponent<SpriteRenderer>().flipX = true;    // Facing left
        if (x > 0) GetComponent<SpriteRenderer>().flipX = false;   // Facing right

        // Waddle side to side while walking, stand straight when stopped
        if (x != 0 || y != 0) {
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time * 15f) * 10f);
        } else {
            transform.rotation = Quaternion.identity;
        }

        // Press E at the basket to return the eggs
        if (nearBasket && Input.GetKeyDown(KeyCode.E)) {
            if (eggsCollected == totalEggs) {
                PlaySound(winSound);                              // Win sound
                EndGame("You win! Chuck found all the eggs!");  
            } else {
                PlaySound(chirpSound);                            
                messageText.text = "Find all the eggs first!";
            }
        }
    }

    // Egg collection
    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Egg")) {
            int points = other.GetComponent<Egg>().points;   // How much this egg is worth
            eggsCollected++;                                 // Add 1 to the egg count
            score += points;                                 // Add the egg's points

            // Special sound for the rainbow egg, normal sound for the others
            if (points > 1) {
                PlaySound(rainbowSound);
            } else {
                PlaySound(eggSound);
            }

            Destroy(other.gameObject);
            UpdateUI();                                      // Updates numbers
        }

        if (other.CompareTag("Basket")) {
            nearBasket = true;
            messageText.text = "Press E to return the eggs";
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Basket")) {
            nearBasket = false;
            if (!gameOver) messageText.text = "";   // Clear the message
        }
    }

    // Updates the egg counter and score on screen
    void UpdateUI() {
        eggText.text = "Eggs: " + eggsCollected + " / " + totalEggs;
        scoreText.text = "Score: " + score;
    }

    void PlaySound(AudioClip clip) {
        if (clip != null && audioSource != null) {
            audioSource.PlayOneShot(clip);
        }
    }

    // Ends the game and shows the win/lose screen with the final score
    void EndGame(string message) {
        gameOver = true;
        messageText.text = "";                              // Clear the middle message
        endText.text = message + "\nFinal Score: " + score; 
        endPanel.SetActive(true);                           
    }

    // Called by the Play Again button
    public void PlayAgain() {
        SceneManager.LoadScene("SampleScene");   // Restart the garden
    }

    // Called by the Main Menu button
    public void MainMenu() {
        SceneManager.LoadScene("TitleScreen");   // Go back to the title screen
    }
}