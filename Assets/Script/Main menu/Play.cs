using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Play : MonoBehaviour
{
    [SerializeField] private string _firstLevel = string.Empty;
    private bool _hit = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        HUDManager.Instance.ContinueTutorial();
        HUDManager.Instance.ToggleScreen(Screen.Aim);
        SceneManager.LoadScene(_firstLevel);
        GameManager.Instance.ListEnemy.Clear();
    }
}
