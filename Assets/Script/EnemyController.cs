using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Animator _animator = null;
    [SerializeField] private int _animationSelection = 0;
    
    [SerializeField] private Rigidbody _rb = null;
    [SerializeField] private bool _isTarget = false;
    [SerializeField] private bool _isDead = false;
    [SerializeField] private GameObject[] _bones = null;

    public bool IsDead { get => _isDead; set => _isDead = value; }

    private void OnCollisionEnter(Collision collision)
    {
        BulletController bullet = collision.gameObject.GetComponent<BulletController>();

        if (bullet != null)
        {
            if (!IsDead)
            {
                Die();
            }
            else
            {
                bullet.Drop();
            }
        }
    }

    public void Die()
    {
        IsDead = true;
        Debug.Log("Dead");

        if (_isTarget)
        {
            Debug.Log("Level Finished");
            Invoke("EndLevel", 3f);
        }
    }

    public void EndLevel()
    {
        Time.timeScale = 0f;
        HUDManager.Instance.ToggleScreen(Screen.FinalScore);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
