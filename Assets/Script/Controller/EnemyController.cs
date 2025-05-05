using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Animator _animator = null;
    [SerializeField] private bool _isTarget = false;
    [SerializeField] private bool _isDead = false;
    [SerializeField] private Rigidbody _hips = null;
    
    private Rigidbody[] _rigidbodys = null;
    private Vector3 _firstPosition; 
    private List<string> _animationBoolList = new List<string>();

    public bool IsDead { get => _isDead; set => _isDead = value; }
    public Vector3 FirstPosition { get => _firstPosition; set => _firstPosition = value; }


    #region Colision
    private void OnCollisionEnter(Collision collision)
    {
        BulletController bullet = collision.gameObject.GetComponent<BulletController>();

        if (bullet != null)
        {
            if (!IsDead)
            {
                Die((bullet.transform.position - transform.position).normalized, 50f);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        FakeBulletMovement fakeBulletMovement = other.GetComponent<FakeBulletMovement>();

        if (fakeBulletMovement != null)
        {
            if (other.CompareTag("Bullet") && fakeBulletMovement.IsReplaying == true)
            {
                BulletController bullet = other.gameObject.GetComponent<BulletController>();

                //GameObject enemyObject = other.gameObject;
                //EnemyController enemyController = enemyObject.GetComponent<EnemyController>();
                //enemyController.SetRagdollOn();
                SetRagdollOn();

                switch (Random.Range(1, 3))
                {
                    case 0:
                        AudioManager.Instance.PlayTimeSound("RewindKill1"); break;
                    case 1:
                        AudioManager.Instance.PlayTimeSound("RewindKill2"); break;
                    case 2:
                        AudioManager.Instance.PlayTimeSound("RewindKill3"); break;
                }

                //bullet.IsShaking = true;

                HUDManager.Instance.ShowEnemyScoreAtLocation(other.transform.position, ScoreManager.Instance.Scores[0].ToString());
                HUDManager.Instance.CallLerpCoroutine();

                if (ScoreManager.Instance.Scores.Count > 1)
                {
                    ScoreManager.Instance.Scores.RemoveAt(0);
                }

                //StartCoroutine(ShakeCamera(0.25f));
                //bullet.CallCoroutineShakeCamera();

                Debug.Log("Hit");
            }
        }
    }
    #endregion


    #region Methode

    
    public void Die(Vector3 direction, float force)
    {
        IsDead = true;
        SetRagdollOn();
        switch (Random.Range(1, 5))
        {
            case 0:
                AudioManager.Instance.PlayTimeSound("Death1"); break;
            case 1:
                AudioManager.Instance.PlayTimeSound("Death2"); break;
            case 2:
                AudioManager.Instance.PlayTimeSound("Death3"); break;
            case 3:
                AudioManager.Instance.PlayTimeSound("Death4"); break;
            case 4:
                AudioManager.Instance.PlayTimeSound("Death5"); break;
        }
        

        _hips.AddForce(direction*force, ForceMode.Impulse);

        if (_isTarget)
        {
            //FIN DE NIVEAU
            Debug.Log("Level Finished");

            HUDManager.Instance.FadeInOut();
            Invoke("StartRewind", 2f); //Attend 2 sec puis lance le rewind DEPUIS le début du lerp de la caméra
        }
    }

    public void StartRewind()
    {
        GameManager.Instance.Bullet.Rewind();
    }

    // Start is called before the first frame update

    private void Awake()
    {
        AddAnimatorNameBoolToList();
    }


    void Start()
    {
        FirstPosition = transform.position;
        GameManager.Instance.ListEnemy.Insert(0, this);
        SetRagdollOff();
        _animator.SetBool("Music", true);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetRagdollOn()
    {
        _animator.enabled = false;
    }

    public void SetRagdollOff()
    {
        _animator.enabled = true;
        SetEnemiesAnimation();
    }

    private void AddAnimatorNameBoolToList()
    {
        _animationBoolList.Add("Sad");
        _animationBoolList.Add("Drunk");
        _animationBoolList.Add("Idle");
        _animationBoolList.Add("Bored");
        _animationBoolList.Add("Music");
        _animationBoolList.Add("LookAround");
    }

    private void SetEnemiesAnimation()
    {
        int i = Random.Range(0,_animationBoolList.ToArray().Length);
        _animator.SetBool(_animationBoolList[i], true);
        
    }
    #endregion
}
