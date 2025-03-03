using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;

    [SerializeField] BulletController _bullet = null;

    public static GameManager Instance { get => _instance; }
    public BulletController Bullet { get => _bullet;}

    // Start is called before the first frame update
    void Start()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
