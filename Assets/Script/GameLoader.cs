using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoader : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private ScoreManager _scoreManager;
    [SerializeField] private HUDManager _hudManager;
    [SerializeField, SceneInBuild] private int _sceneInBuild;
    

    // Start is called before the first frame update
    void Start()
    {
        _gameManager.Init();
        _audioManager.Init();
        _scoreManager.Init();
        _hudManager.Init();
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(_sceneInBuild);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
