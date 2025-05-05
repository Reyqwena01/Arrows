using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    private BulletController _playerBullet = null;

    [SerializeField] private int _bronzeMedalMaxBounces = 15;
    [SerializeField] private int _bronzeMedalMultiplier = 2;

    [SerializeField] private int _silverMedalMaxBounces = 10;
    [SerializeField] private int _silverMedalMultiplier = 3;

    [SerializeField] private int _goldMedalMaxBounces = 5;
    [SerializeField] private int _goldMedalMultiplier = 4;

    [SerializeField] private List<int> _scoresOnKill = new List<int>();

    private static ScoreManager _instance = null;

    private int _bounces = 0;
    private int _score = 0;

    public static ScoreManager Instance { get => _instance; set => _instance = value; }
    public int Bounces { 
        get => _bounces;
        set
        {
            _bounces = value;
        }
    }
    public int Score { get => _score; set => _score = value; }
    public int BronzeMedalMaxBounces { get => _bronzeMedalMaxBounces; set => _bronzeMedalMaxBounces = value; }
    public int SilverMedalMaxBounces { get => _silverMedalMaxBounces; set => _silverMedalMaxBounces = value; }
    public int GoldMedalMaxBounces { get => _goldMedalMaxBounces; set => _goldMedalMaxBounces = value; }

    public int BronzeMedalMultiplier { get => _bronzeMedalMultiplier; set => _bronzeMedalMultiplier = value; }
    public int SilverMedalMultiplier { get => _silverMedalMultiplier; set => _silverMedalMultiplier = value; }
    public int GoldMedalMultiplier { get => _goldMedalMultiplier; set => _goldMedalMultiplier = value; }

    public BulletController Bullet { get => _playerBullet; set => _playerBullet = value; }
    public List<int> Scores { get => _scoresOnKill; set => _scoresOnKill = value; }


    // Start is called before the first frame update
    void Awake()
    {
      
    }

    public void Init()
    {
        _instance = FindObjectOfType<ScoreManager>();
        DontDestroyOnLoad(gameObject);
    }

    public int GetScoreOnKill(float speed, string bodypart)
    {
        int finalScore = 10;

        AudioManager.Instance.PlayTimeSound("Blood");

        Debug.Log(bodypart + "shot");

        switch (bodypart)
        {
            case "Head":
                finalScore = 50;
                AudioManager.Instance.PlayTimeSound("Headshot");
                HUDManager.Instance.ScoreAnimator.SetTrigger("Headshot");
                break;
            case "Torso":
                finalScore = 20;
                AudioManager.Instance.PlayTimeSound("Bodyshot");
                break;
            case "Arm":
                finalScore = 40;
                AudioManager.Instance.PlayTimeSound("Limbshot");
                break;
            case "Leg":
                finalScore = 30;
                AudioManager.Instance.PlayTimeSound("Limbshot");
                break;
        }

        Scores.Add((int)Mathf.Round(finalScore * speed));

        return (int)Mathf.Round(finalScore*speed);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
