using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;

    private BulletController _bullet = null;
    [SerializeField] private List<EnemyController> _listEnemy = new List<EnemyController>(); 

    public static GameManager Instance { get => _instance; }
    public BulletController Bullet { get => _bullet;}
    public List<EnemyController> ListEnemy { get => _listEnemy; set => _listEnemy = value; }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject); 
    }

    public void SetBulletController(BulletController value)
    {
        _bullet = value;
        HUDManager.Instance.Bullet = value;
        ScoreManager.Instance.Bullet = value;
    }
}
