using UnityEngine;
using TMPro;

public class TimeCountdown : MonoBehaviour
{
    public int startMinutes = 0;
    public int startSeconds = 30;

    public TextMeshProUGUI timeText;
    public TextMeshProUGUI timeUsedText;

    private float _timeRemaining;
    private bool _isRunning = false;

    private StatusBar statusBar;


    void Start()
    {
        statusBar = FindObjectOfType<StatusBar>();

        _timeRemaining = startMinutes * 60f + startSeconds;
        _isRunning = true;
        UpdateUI();
    }

    void Update()
    {
        if (!_isRunning) return;

        if (_timeRemaining > 0f)
        {
            _timeRemaining -= Time.deltaTime;
            if (_timeRemaining <= 0f)
            {
                _timeRemaining = 0f;
                _isRunning = false;
                statusBar.GameOver();
                // TODO: trigger any “timer finished” event here
            }
            UpdateUI();
        }
    }

    private void UpdateUI()
    {

        float t = Mathf.Max(_timeRemaining, 0f);
        int minutes = Mathf.FloorToInt(t / 60f);
        int seconds = Mathf.FloorToInt(t % 60f);
        int msec = Mathf.FloorToInt((t * 1000f) % 1000f);

        // Format: "MM:SS.mmm" (e.g. "01:05.042")
        timeText.text = $"{minutes:00}:{seconds:00}.{msec:000}";
        float initialTime = startMinutes * 60f + startSeconds;
        // how much time has actually elapsed
        float elapsed = initialTime - _timeRemaining;
        elapsed = Mathf.Max(elapsed, 0f);

        int mUsed = Mathf.FloorToInt(elapsed / 60f);
        int sUsed = Mathf.FloorToInt(elapsed % 60f);
        int msUsed = Mathf.FloorToInt((elapsed * 1000f) % 1000f);

        timeUsedText.text = $"{mUsed:00}:{sUsed:00}.{msUsed:000}";
    }

    public void PauseTimer()
    {
        _isRunning = false;
    }

    public void ResumeTimer()
    {
        if (_timeRemaining > 0f)
            _isRunning = true;
    }

    public void ResetTimer()
    {
        _timeRemaining = startMinutes * 60f + startSeconds;
        _isRunning = true;
        UpdateUI();
    }

    public bool IsFinished()
    {
        return !_isRunning && _timeRemaining <= 0f;
    }

    public void SetTime(int minutes, int seconds)
    {
        // Clamp seconds to [0..59] and push overflow into minutes if needed
        int totalSeconds = minutes * 60 + Mathf.Clamp(seconds, 0, 59);

        _timeRemaining = Mathf.Max(totalSeconds, 0f);
        _isRunning = true;

        // Update inspector defaults so ResetTimer() will use these values if needed
        startMinutes = minutes + (seconds / 60);
        startSeconds = seconds % 60;

        UpdateUI();
    }
}
