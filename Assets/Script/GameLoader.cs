using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoader : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private ScoreManager _scoreManager;
    [SerializeField] private HUDManager _hudManager;

    // Start is called before the first frame update
    void Start()
    {
        _gameManager.Init();
        _audioManager.Init();
        _scoreManager.Init();
        _hudManager.Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
