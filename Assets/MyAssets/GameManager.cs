using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Menus")]
    [SerializeField] GameObject winFirstButton;
    [SerializeField] GameObject loseFirstButton;
    [SerializeField] GameObject winPanel;
    [SerializeField] GameObject losePanel;
    [SerializeField] TMP_Text winTitle;

    [Header("Loading")]
    [SerializeField] GameObject loadingScreen;
    [SerializeField] Image bar;

    [Header("Game")]
    [SerializeField] int maxLives = 5;
    [SerializeField] int targetScore = 10;
    [SerializeField] int lives = 3;
    [SerializeField] float respawnDelay = 0.6f;
    [SerializeField] float spawnProtection = 1.5f;
    [SerializeField] Health player;
    [SerializeField] Transform respawnPoint;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text livesText;

    [Header("Timer")]
    [SerializeField] float timeLimit = 60f;
    [SerializeField] TMP_Text timerText;

    [Header("Heart")]
    [SerializeField] GameObject heartPrefab;
    [SerializeField] Transform[] heartSpawnPoints;

    const int LastLevelIndex = 5;
    const int CreditsIndex = 6;

    GameObject heart;
    int score;
    float timeLeft;
    bool finished;
    bool loading;

    public bool Finished => finished;

    void Awake() => Instance = this;

    void Start()
    {
        timeLeft = timeLimit;
        UpdateUI();
        UpdateTimerUI();
    }

    void Update()
    {
        if (finished || timeLimit <= 0f) return;

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            UpdateTimerUI();
            Lose();
            return;
        }
        UpdateTimerUI();
    }

    void UpdateTimerUI()
    {
        if (timerText == null) return;

        if (timeLimit <= 0f)
        {
            timerText.text = "";
            return;
        }

        int minutes = Mathf.FloorToInt(timeLeft / 60f);
        int seconds = Mathf.FloorToInt(timeLeft % 60f);
        timerText.text = $"Time: {minutes:00}:{seconds:00}";
        timerText.color = timeLeft <= 10f ? Color.red : Color.white;
    }

    public void AddScore(int amount)
    {
        if (finished) return;

        score += amount;
        UpdateUI();
        if (score >= targetScore) Win();
    }

    public void UpdateUI()
    {
        scoreText.text = $"Score: {score} / {targetScore}";
        livesText.text = $"Lives: {lives}";
    }

    void CheckHeart()
    {
        if (lives != 1 || heart != null) return;
        if (heartPrefab == null || heartSpawnPoints.Length == 0) return;

        Transform p = heartSpawnPoints[Random.Range(0, heartSpawnPoints.Length)];
        heart = Instantiate(heartPrefab, p.position, Quaternion.identity);
    }

    public bool AddLife(int amount)
    {
        if (finished || lives >= maxLives) return false;

        lives = Mathf.Min(lives + amount, maxLives);
        UpdateUI();
        return true;
    }

    public void PlayerDied()
    {
        if (finished) return;

        lives--;
        UpdateUI();
        if (lives <= 0) Lose();
        else StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnDelay);
        Rigidbody body = player.GetComponent<Rigidbody>();
        body.position = respawnPoint.position;
        body.linearVelocity = Vector3.zero;
        player.Restore(spawnProtection);
    }

    void Win()
    {
        if (finished) return;
        finished = true;

        bool lastLevel = SceneManager.GetActiveScene().buildIndex == LastLevelIndex;
        if (winTitle != null) winTitle.text = lastLevel ? "You finished the game!" : "You Win";

        winPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(winFirstButton);
        Time.timeScale = 0f;
    }

    void Lose()
    {
        if (finished) return;
        finished = true;

        losePanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(loseFirstButton);
        Time.timeScale = 0f;
    }

    // ---- Scene loading (formerly LevelLoader) ----

    public void Restart() => Load(SceneManager.GetActiveScene().buildIndex);
    public void MainMenu() => Load(0);

    public void NextLevel()
    {
        int i = SceneManager.GetActiveScene().buildIndex;
        Load(i >= CreditsIndex ? 0 : i + 1);
    }

    public void Load(int index)
    {
        if (loading) return;
        loading = true;
        Time.timeScale = 1f;
        StartCoroutine(LoadRoutine(index));
    }

    IEnumerator LoadRoutine(int index)
    {
        if (loadingScreen) loadingScreen.SetActive(true);

        AsyncOperation op = SceneManager.LoadSceneAsync(index);
        while (!op.isDone)
        {
            if (bar) bar.fillAmount = Mathf.Clamp01(op.progress / 0.9f);
            yield return null;
        }
    }
    public void Quit()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }
}