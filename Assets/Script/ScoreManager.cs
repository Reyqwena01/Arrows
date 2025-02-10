using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private BulletController _playerBullet = null;

    private static ScoreManager _instance = null;

    private int _bounces = 0;
    private int _score = 0;

    public static ScoreManager Instance { get => _instance; set => _instance = value; }
    public int Bounces { get => _bounces; set => _bounces = value; }
    public int Score { get => _score; set => _score = value; }

    // Start is called before the first frame update
    void Start()
    {
        _instance = FindObjectOfType<ScoreManager>();
        Object.DontDestroyOnLoad(gameObject);
    }

    public int GetScoreOnKill(float speed, string bodypart)
    {
        int finalScore = 10;

        Debug.Log(bodypart + "shot");

        switch (bodypart)
        {
            case "Head":
                finalScore = 50;
                break;
            case "Torso":
                finalScore = 20;
                break;
            case "Arm":
                finalScore = 40;
                break;
            case "Leg":
                finalScore = 30;
                break;
        }

        Score += finalScore;
        return (int)Mathf.Round(finalScore*speed);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
