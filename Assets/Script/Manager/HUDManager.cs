using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    private static HUDManager _instance = null;

    private BulletController _playerBullet = null;

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

    [Header("Score")]
    [SerializeField] private Canvas _scoreScreen = null;
    [SerializeField] private Vector3 _startScaleFactor = Vector3.one;
    [SerializeField] private Vector3 _endScaleFactor = Vector3.one;
    [SerializeField] private GameObject _prefabScoreText = null; 

    [Header("Fade")]
    [SerializeField] private Animator _imageAnimator = null;

    [Header("VFX Score")]
    [SerializeField] private Image _scoreEndVFX;
    [SerializeField] private Image _scoreHeadShotVFX;

    [Header("Tutorial")]
    [SerializeField] private Canvas _bounceTutorial = null;
    [SerializeField] private Canvas _targetTutorial = null;
    private int _bounces = 0;

    private Screen _currentScreen;

    private int _score = 0;
    private int _multiplier = 1;
    private int _finalScore = 0;
    private float _alpha = 0f;

    public static HUDManager Instance { get => _instance; set => _instance = value; }
    public BulletController Bullet { get => _playerBullet; set => _playerBullet = value; }
    public Screen CurrentScreen { get => _currentScreen;}

    // Start is called before the first frame update
    void Start()
    {
       
    }

    public void ContinueTutorial()
    {
        if (_bounces > 5) return;

        _bounces++;
        Debug.Log(_bounces);

        switch (_bounces)
        {
            case 1:
                _targetTutorial.enabled = false;
                _bounceTutorial.enabled = true;
                break;
            case 3:
                _bounceTutorial.enabled = false;
                _targetTutorial.enabled = true;
                break;
            case 5:
                _bounceTutorial.enabled = false;
                _targetTutorial.enabled = false;
                break;
        }
    }

    public void Init()
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
        _scoreScreen.enabled = false; 
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
            case Screen.Score:
                _scoreScreen.enabled = true;
                break;
        }
    }


    #region finalscorescreen

    #region FeedBack

    private void ScaleScoreUp(float scaleFactor)
    {
        _scoreText.gameObject.transform.localScale = Vector3.Lerp(_startScaleFactor, _endScaleFactor, scaleFactor);
    }

    private void ScaleScoreDown()
    {
        _scoreText.gameObject.transform.localScale = _startScaleFactor;
        _scoreEndVFX.enabled = true;
        //SFX satisfaisant/ feedback  ( je ne sais pas si il doit être placée là mais ça serais le plus logique )
        //AudioManager.Instance.PlaySound("ScoreScaleDown");
    }

    #endregion
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

    //Fonction pour set le score à la fin
    private void UpdateScoreAtEnd()
    {
        //Mettre ce qu'il y a dans le update pour afficher le score et l'update
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
    #endregion finalscorescreen

    public void DisplayKillScreen(int score, string bodypart)
    {
        ToggleScreen(Screen.Kill);
        _killScore.text = score.ToString();
        //afficher l'image (VFX) pour un headshot
        if(bodypart == "Headshot")
        {
            //_scoreHeadShotVFX possède une animation au lancement de lui même
            _scoreHeadShotVFX.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHUD();
        UpdateScoreAtEnd();
    }

    public void FadeInOut()
    {
        _imageAnimator.SetTrigger("Fade");
    }

    public void ShowEnemyScoreAtLocation(Vector3 location, string text)
    {
        
        GameObject scoreObject = Instantiate(_prefabScoreText, location, Quaternion.Euler(75, 0, 0));
        TextMeshProUGUI scoreTxt = scoreObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        scoreTxt.text = text;

        Destroy(scoreObject, 1f); 
    }
}
