using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    private static HUDManager _instance = null;

    [SerializeField] private BulletController _playerBullet = null;

    [Header("Crosshair")]
    [SerializeField] private RawImage _crosshair = null;
    [SerializeField] private RawImage _powerUpWind = null;
    [SerializeField] private RawImage _powerUpPierce = null;

    [Header("Game Over")]
    [SerializeField] private Canvas _gameOverScreen = null;

    [Header("Flight")]
    [SerializeField] private Canvas _flightScreen = null;
    [SerializeField] private TMP_Text _velocity = null;
    [SerializeField] private Color _velocityGradientStart = Color.white;
    [SerializeField] private Color _velocityGradientEnd = Color.red;

    [Header("Kill")]
    [SerializeField] private Canvas _killScreen = null;
    [SerializeField] private TMP_Text _killScore = null;

    [Header("Aim")]
    [SerializeField] private Canvas _aimScreen = null;
    [SerializeField] private TMP_Text _aimTimer = null;

    [Header("Final Score")]
    [SerializeField] private Canvas _finalScoreScreen = null;
    [SerializeField] private TMP_Text _scoreText = null;
    [SerializeField] private TMP_Text _bronzeMedalText = null;
    [SerializeField] private TMP_Text _silverMedalText = null;
    [SerializeField] private TMP_Text _goldMedalText = null;

    [Header("Fade")]
    [SerializeField] private Image _imageToFade = null; 

    private Screen _currentScreen;

    private int _score = 0;
    private int _multiplier = 1;
    private int _finalScore = 0;
    private float _alpha = 0f;

    public static HUDManager Instance { get => _instance; set => _instance = value; }
    public Screen CurrentScreen { get => _currentScreen;}

    // Start is called before the first frame update
    void Start()
    {
        _instance = FindObjectOfType<HUDManager>();
        Object.DontDestroyOnLoad(gameObject);

        SetPowerUpVisibility(Power.Wind, false);
        SetPowerUpVisibility(Power.Pierce, false);

        ToggleScreen(Screen.None);
        SetCrosshairVisibility(true);

        //Color c = _imageToFade.color;
        //c.a = 1.0f;

    }

    public void SetCrosshairVisibility(bool value)
    {
        _crosshair.enabled = value;
    }

    public void SetPowerUpVisibility(Power power, bool value)
    {
        switch (power)
        {
            case Power.Wind:
                _powerUpWind.enabled = value; 
                break;
            case Power.Pierce:
                _powerUpPierce.enabled = value;
                break;
        }
    }

    public void UpdateHUD()
    {
        _velocity.text = _playerBullet.Velocity.ToString();
        _velocity.color = Color.Lerp(_velocityGradientStart, _velocityGradientEnd, _playerBullet.CurrentSpeedPerc);

        _aimTimer.text = Mathf.Round(_playerBullet.TimeoutCounter).ToString();
    }

    public void ToggleScreen(Screen screen)
    {
        _gameOverScreen.enabled = false;
        _flightScreen.enabled = false;
        _killScreen.enabled = false;
        _aimScreen.enabled = false;
        _finalScoreScreen.enabled = false;
        SetCrosshairVisibility(false);

        _currentScreen = screen;

        switch (screen)
        {
            case Screen.GameOver:
                _gameOverScreen.enabled = true;
                break;
            case Screen.Flight:
                _flightScreen.enabled = true;
                break;
            case Screen.Kill:
                _killScreen.enabled = true;
                break;
            case Screen.Aim:
                SetCrosshairVisibility(true);
                _aimScreen.enabled = true;
                break;
            case Screen.FinalScore:
                _finalScoreScreen.enabled = true;
                ShowCurrentScore();
                break;
        }
    }


    #region finalscorescreen
    private void ShowCurrentScore()
    {
        _alpha = 0f;
        _score = ScoreManager.Instance.Score;
        Invoke("ShowBronzeMedal", 1f);
    }

    private void ShowBronzeMedal()
    {
        if (ScoreManager.Instance.Bounces <= ScoreManager.Instance.BronzeMedalMaxBounces)
        {
            _bronzeMedalText.color = Color.white;
            _multiplier *= ScoreManager.Instance.BronzeMedalMultiplier;
            Invoke("ShowSilverMedal", 0.8f);
        }

        else
        {
            ShowFinalScore();
        }
    }

    private void ShowSilverMedal()
    {
        if (ScoreManager.Instance.Bounces <= ScoreManager.Instance.SilverMedalMaxBounces)
        {
            _silverMedalText.color = Color.white;
            _multiplier *= ScoreManager.Instance.SilverMedalMultiplier;
            Invoke("ShowGoldMedal", 0.8f);
        }

        else
        {
            ShowFinalScore();
        }
    }

    private void ShowGoldMedal()
    {
        if (ScoreManager.Instance.Bounces <= ScoreManager.Instance.GoldMedalMaxBounces)
        {
            _goldMedalText.color = Color.white;
            _multiplier *= ScoreManager.Instance.GoldMedalMultiplier;
            Invoke("ShowFinalScore", 0.8f);
        }

        else
        {
            ShowFinalScore();
        }
    }

    private void ShowFinalScore()
    {
        _alpha = 0f;
        _finalScore = _score * _multiplier;
    }
    #endregion finalscorescreen

    public void DisplayKillScreen(int score)
    {
        ToggleScreen(Screen.Kill);
        _killScore.text = score.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHUD();

        if (_finalScore > 0 && _alpha < 1)
        {
            _scoreText.text = Mathf.Round(Mathf.Lerp(_score, _finalScore, _alpha)).ToString();
            _alpha += 0.001f;
        }

        else if (_score > 0 && _alpha < 1)
        {
            _scoreText.text = Mathf.Round(Mathf.Lerp(0, _score, _alpha)).ToString();
            _alpha += 0.001f;
        }
    }

    public IEnumerator FadeInOut()
    {
        Color c = _imageToFade.color;
        c.a = 1.0f; 
        c = Color.red;
        yield return null; 

        //for (float alpha = 0f; alpha <= 1.0f; alpha += 0.1f)
        //{
        //    c.a = alpha;
        //    Debug.Log(c.a);
        //    yield return null;
        //}

    }
}
