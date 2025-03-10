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
    private Rigidbody[] _rigidbodys = null;

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
            else if (IsDead)
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
            //FIN DE NIVEAU
            Debug.Log("Level Finished");


            Invoke("StartRewind", 2f); //Attend 2 sec puis lance le rewind DEPUIS le début du lerp de la caméra
        }
    }

    public void StartRewind()
    {
        GameManager.Instance.Bullet.Rewind();
    }

    // Start is called before the first frame update
    void Start()
    {
        GetAllBonesRigidBody();
        SetRagdollOff();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            SetRagdollOn();
        }

    }

    private void GetAllBonesRigidBody()
    {
        _rigidbodys = GetComponentsInChildren<Rigidbody>();
    }

    public void SetRagdollOn()
    {
        for (int i = 0, rL = _rigidbodys.Length; i < rL; i++)
        {
            _rigidbodys[i].isKinematic = false;
        }
        _animator.enabled = false;
    }

    public void SetRagdollOff()
    {
        int randomValue = Random.Range(0, 3); ;
        for (int i = 0, rL = _rigidbodys.Length; i < rL; i++)
        {
            _rigidbodys[i].isKinematic = true;
        }
        _animator.SetInteger("SelectAnimation", randomValue);
    }

}
