using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    private static GameManager _instance = null;

    #region "Methode"
    void Start()
    {
        _instance = FindAnyObjectByType<GameManager>();
        DontDestroyOnLoad(_instance);
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    #endregion

    #region "Properties"

    public static GameManager Instance
        { get { return _instance; } }

    #endregion

}
