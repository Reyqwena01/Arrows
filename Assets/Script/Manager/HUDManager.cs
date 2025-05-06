using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.SceneManagement;
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
    [SerializeField] private Animator _scoreAnimator = null;

    [Header("Aim")]
    [SerializeField] private Canvas _aimScreen = null;
    [SerializeField] private TMP_Text _aimTimer = null;
    [SerializeField] private Color _aimTimerGoodColor = Color.green;
    [SerializeField] private Color _aimTimerBadColor = Color.yellow;
    [SerializeField] private Color _aimTimerVeryBadColor = Color.red;
    [SerializeField] private Animator _timerAnimator = null;

    [Header("Final Score")]
    [SerializeField] private Canvas _finalScoreScreen = null;
    [SerializeField] private TMP_Text _scoreText = null;
    [SerializeField] private TMP_Text _bronzeMedalText = null;
    [SerializeField] private TMP_Text _silverMedalText = null;
    [SerializeField] private TMP_Text _goldMedalText = null;
    [SerializeField] private Color _completedMedal = Color.green;
    [SerializeField] private GameObject _nextLevelButton = null;

    [Header("Score")]
    [SerializeField] private Canvas _scoreScreen = null;
    [SerializeField] private Vector3 _startScaleFactor = Vector3.one;
    [SerializeField] private Vector3 _endScaleFactor = Vector3.one;
    [SerializeField] private GameObject _prefabScoreText = null;
    [SerializeField] private GameObject _prefabDamageScore = null;
    [SerializeField] private GameObject _prefabBoingVFX = null;
    [SerializeField] private TMP_Text _scoreCumulatedText = null;
    [SerializeField] private Animator _scoreMergedAnimator = null;
    [SerializeField] private GameObject[] _VFXRewind = null;

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
    private bool _isScoreLerping = false;
    private int _scoreCumulated = 0; 

    public static HUDManager Instance { get => _instance; set => _instance = value; }
    public BulletController Bullet { get => _playerBullet; set => _playerBullet = value; }
    public Screen CurrentScreen { get => _currentScreen; }
    public bool IsScoreLerping { get => _isScoreLerping; set => _isScoreLerping = value; }
    public TMP_Text AimTimer { get => _aimTimer; set => _aimTimer = value; }
    public Animator ScoreAnimator { get => _scoreAnimator; set => _scoreAnimator = value; }

    // Start is called before the first frame update
    void Start()
    {
        IsScoreLerping = false;
        _scoreCumulatedText.text = 0.ToString();
    }

    public void ContinueTutorial()
    {
        //if (_bounces > 5) return;
        //
        //_bounces++;
        //Debug.Log(_bounces);
        //
        //switch (_bounces)
        //{
        //    case 1:
        //        _targetTutorial.enabled = false;
        //        _bounceTutorial.enabled = true;
        //        break;
        //    case 3:
        //        _bounceTutorial.enabled = false;
        //        _targetTutorial.enabled = true;
        //        break;
        //    case 5:
        //        _bounceTutorial.enabled = false;
        //        _targetTutorial.enabled = false;
        //        break;
        //}
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

        if (_playerBullet.TimeoutCounter > 4)
        {
            AimTimer.color = _aimTimerGoodColor;
        }
        else if (_playerBullet.TimeoutCounter > 2)
        {
            AimTimer.color = _aimTimerBadColor;
        }
        else
        {
            AimTimer.color = _aimTimerVeryBadColor;
        }

        AimTimer.text = Mathf.Round(_playerBullet.TimeoutCounter).ToString();

        _timerAnimator.SetTrigger("Pop");
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
                _aimTimer.enabled = true;
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
        _score = ScoreManager.Instance.Score;
        _scoreText.text = ScoreManager.Instance.Score.ToString();
        _bronzeMedalText.text = ScoreManager.Instance.BronzeMedalMaxBounces.ToString() + " or less";
        _silverMedalText.text = ScoreManager.Instance.SilverMedalMaxBounces.ToString() + " or less";
        _goldMedalText.text = ScoreManager.Instance.GoldMedalMaxBounces.ToString() + " or less";

        _bronzeMedalText.color = Color.white;
        _silverMedalText.color = Color.white;
        _goldMedalText.color = Color.white;

        Cursor.visible = true;

        if (SceneManager.sceneCountInBuildSettings-1 == SceneManager.GetActiveScene().buildIndex)
        {
            _nextLevelButton.SetActive(false);
        }
        else
        {
            _nextLevelButton.SetActive(true);
        }

        Invoke("ShowBronzeMedal", 1f);
    }

    private void ShowBronzeMedal()
    {
        if (ScoreManager.Instance.Bounces <= ScoreManager.Instance.BronzeMedalMaxBounces)
        {
            _bronzeMedalText.color = _completedMedal;
            _multiplier += ScoreManager.Instance.BronzeMedalMultiplier;
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
            _silverMedalText.color = _completedMedal;
            _multiplier += ScoreManager.Instance.SilverMedalMultiplier;
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
            _goldMedalText.color = _completedMedal;
            _multiplier += ScoreManager.Instance.GoldMedalMultiplier;
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
        if (_finalScore > 0 && _alpha < 1)
        {
            _scoreText.text = Mathf.Round(Mathf.Lerp(_score, _finalScore, _alpha)).ToString();
            _alpha += 0.001f;

        }
    }
    #endregion finalscorescreen

    public void DisplayKillScreen(int score, string bodypart)
    {
        ToggleScreen(Screen.Kill);
        _killScore.text = score.ToString();
        //afficher l'image (VFX) pour un headshot
        if (bodypart == "Headshot")
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

    #region ScoreRewind

    public void ShowEnemyScoreAtLocation(Vector3 location, string text)
    {

        Vector3 offset = new Vector3(0, 0, 20);
        Vector3 offsetDamage = new Vector3(0, 0, 30);
        Vector3 offsetRandom = new Vector3(Random.Range(2, 5), 0, Random.Range(5, 9));

        //Score effect 
        GameObject scoreObject = Instantiate(_prefabScoreText, location - offset, Quaternion.Euler(75, 0, 0));
        TextMeshProUGUI scoreTxt = scoreObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        scoreTxt.text = "+ " + text;

        //Damage effect 
        GameObject damageObject = Instantiate(_prefabDamageScore, location, Quaternion.Euler(90, 0, 0));

        //Slash and Boom effect 
        int random = Random.Range(0, _VFXRewind.Length); 
        GameObject vfxObject = Instantiate(_VFXRewind[random], location - offsetRandom, Quaternion.Euler(90, 0, 0));

        Destroy(scoreObject, 1f);
    }

    public void ShowBoingEffectAtLocation(Vector3 location)
    {
        //Vector3 offsetRandom = new Vector3(Random.Range(2, 5), 0, 12);
        Vector3 offsetRandom = new Vector3(0, 0, 2);

        GameObject boingVFXObject = Instantiate(_prefabBoingVFX, location - offsetRandom, Quaternion.Euler(90, 0, 0));
    }

    private void UpdateScoreCumulated()
    {
        if (IsScoreLerping)
        {
            float currentLerpTime = 0f;
            float lerpValue = 1f; 
            currentLerpTime += Time.deltaTime;

            if (currentLerpTime > lerpValue) { currentLerpTime = lerpValue; }
            
            float perc = currentLerpTime/lerpValue;

            _scoreCumulatedText.text = _scoreCumulated.ToString();
        }     
    }

    public void CallLerpCoroutine()
    {
        StartCoroutine(LerpScore(0.75f));
        _scoreCumulated += ScoreManager.Instance.Scores[0];
        _scoreCumulatedText.text = _scoreCumulated.ToString();
    }

    private IEnumerator LerpScore(float delay)
    {
        
        IsScoreLerping = true;
        _scoreMergedAnimator.SetBool("CanAnimBool", true);
        
        yield return new WaitForSeconds(delay);
        
        _scoreMergedAnimator.SetBool("CanAnimBool", false);
        IsScoreLerping = false; 

        //if (IsScoreLerping)
        //{
        //    _scoreCumulatedText.text = Mathf.Round(Mathf.Lerp(_score, _finalScore, 0.001f)).ToString();
        //}
    }

    #endregion
}
