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

    [SerializeField] private RawImage _crosshair = null;
    [SerializeField] private Canvas _gameOverScreen = null;

    [SerializeField] private Canvas _flightScreen = null;
    [SerializeField] private TMP_Text _velocity = null;
    [SerializeField] private Color _velocityGradientStart = Color.white;
    [SerializeField] private Color _velocityGradientEnd = Color.red;

    [SerializeField] private Canvas _killScreen = null;
    [SerializeField] private TMP_Text _killScore = null;

    [SerializeField] private Canvas _aimScreen = null;
    [SerializeField] private TMP_Text _aimTimer = null;

    public static HUDManager Instance { get => _instance; set => _instance = value; }
    
    // Start is called before the first frame update
    void Start()
    {
        _instance = FindObjectOfType<HUDManager>();
        Object.DontDestroyOnLoad(gameObject);

        ToggleScreen(Screen.None);
    }

    public void SetCrosshairVisibility(bool value)
    {
        _crosshair.enabled = value;
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

        switch (screen)
        {
            case Screen.GameOver:
                SetCrosshairVisibility(false);
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
        }
    }

    public void DisplayKillScreen(int score)
    {
        ToggleScreen(Screen.Kill);
        _killScore.text = score.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHUD();
    }
}
