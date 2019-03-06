using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // To manipulate the slider which denotes our speed.

public class UIManager : MonoBehaviour
{

    public static UIManager instance; // Singleton design pattern

    public Slider speedometer; // the forward back tilting, tracks the z axis
    public Slider tiltometer; //the left right tilting, tracks the x axis
    public Text test;
	public Text timer; // Displays time remaining to the player.
	public Text score;
	public Text multiplier;
	public Text railsRemaining;
	public RectTransform gameOverPanel; // Panel with UI elements that display when the game is over.
	public RectTransform startPanel; // Panel with UI elements that display when the game starts.

	private Image gameOverPanelBG; // Background of the game over panel. 

	private int FONT_SIZE = 24; // font size for the multiplier.

	private AudioSource audi; // For playing the "checkpoint reached" jingle.

	private RailManager rm; // reference to rail manager

    // Set up the singleton design pattern
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        // Set up the speedometer
        speedometer.minValue = GameManager.MIN_SPEED;
        speedometer.maxValue = GameManager.MAX_SPEED;

        //Setup the tiltometer
        tiltometer.minValue = -GameManager.TILT_THRESHHOLD;
        tiltometer.maxValue = GameManager.TILT_THRESHHOLD;

		// Hide game over panel and get the reference to the image on it
		gameOverPanel.gameObject.SetActive(false);
		gameOverPanelBG = gameOverPanel.GetComponent<Image>();
		gameOverPanelBG.color = new Color(gameOverPanelBG.color.r, gameOverPanelBG.color.g, gameOverPanelBG.color.b, 0);

		rm = GameObject.FindObjectOfType<RailManager>();
		audi = GetComponent<AudioSource>();
    }

    private void Start()    {
		UpdateScore();
		Invoke("HideStartPanel", 3f);
    }

    private void Update()
    {
#if UNITY_EDITOR
        test.text = "In editor\nX: " + GameManager.instance.Xinput + "\nY: " + GameManager.instance.Yinput + "\nZ: " + GameManager.instance.Zinput;
#else //this would be on a phone
        test.text = "In phone \nX: " + GameManager.instance.Xinput + "\nY: " + GameManager.instance.Yinput + "\nZ: " + GameManager.instance.Zinput;
        UpdateUI();
#endif
		UpdateScore();
		UpdateRM();
		Jukebox.instance.ChangeVolume(0, speedometer.normalizedValue);
    }

	// Reveals the UI panel that displays when the game is over.
	private IEnumerator GameOver() {
		gameOverPanel.gameObject.SetActive(true);
		while (gameOverPanelBG.color.a < 0.99f) {
			gameOverPanelBG.color = new Color(gameOverPanelBG.color.r, gameOverPanelBG.color.g, gameOverPanelBG.color.b, gameOverPanelBG.color.a + 0.01f);
			yield return new WaitForEndOfFrame();
		}
	}

	// Update the score and multiplier UI elements. Also dynamically resize the score multiplier for gamefeel.
	private void UpdateScore() {
		score.text = "1P " + ScoreManager.instance.GetScore().ToString().PadLeft(7, '0');
		multiplier.text = "x" + ScoreManager.instance.GetMultiplier().ToString();
		multiplier.fontSize = FONT_SIZE + (int) Mathf.Round(ScoreManager.instance.GetMultiplier());
	}

	private void UpdateRM() {
		if (GameManager.instance.GetGameIsOver()) {
			railsRemaining.text = "GAME\nOVER";
		} else {
			railsRemaining.text = "NEXT\nCHECK\nPOINT\nIN\n" + rm.GetRailsRemaining() + " rails";
		}
	}

    // Update the UI slider.
    public void UpdateUI()
    {
        speedometer.value = GameManager.instance.speed;
        tiltometer.value = -GameManager.instance.Xinput;
    }
		
	private void HideStartPanel() {
		startPanel.gameObject.SetActive(false);
	}

	// Plays the "checkpoint reached" jingle.
	public void PlayCheckpoint() {
		audi.Play();
	}

	// Calls the coroutine which displays the Game Over panel.
	public void ShowGameOverPanel() {
		StartCoroutine(GameOver());
	}
}
