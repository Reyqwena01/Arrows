using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Play : MonoBehaviour
{
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
        if (_hit) return;
        HUDManager.Instance.ContinueTutorial();
        _hit = true;
        SceneManager.LoadScene("Level1");
    }
}
