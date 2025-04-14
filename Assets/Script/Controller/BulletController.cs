using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    #region parameters
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _maxSpeed = 200f;
    [SerializeField] private float _impactOffset = 1f;
    [SerializeField] private float _bulletRotationSpeedOnImpact = 1f;
    [Space(10)]
    [SerializeField] private float _sensitivity = 15f;
    [SerializeField] private float _aimAssistStrength = 0.5f;
    [Space(10)]
    [SerializeField] private float _speedEffectStrength = 1f;
    [SerializeField] private float _minFovEffect = 40f;
    [SerializeField] private float _maxFovEffect = 160f;
    [Space(10)]
    [SerializeField] private float _timeoutTimer = 10f;
    [Space(10)]
    [SerializeField] private LayerMask _raycastLayer = 0;
    [Header("Kill")]
    [SerializeField] private float _killCameraDistance = 30f;
    #endregion

    #region references
    [Space(25)]
    [SerializeField] private Rigidbody _rb = null;
    [SerializeField] private Collider _bulletCollider = null;
    [SerializeField] private GameObject _bulletMesh = null;
    [Space(10)]
    [SerializeField] private Camera _camera = null;
    [SerializeField] private Camera _dropCamera = null;
    [SerializeField] private CinemachineVirtualCamera _virtualCamera = null;
    [SerializeField] private RewindTime _rewindTime = null;
    [SerializeField] private CameraBehavior _camBehavior = null;
    [SerializeField] private EnemyController _enemyController = null; 

    [SerializeField] private GameObject _bulletTrail = null;

    [SerializeField] private GameObject _bounceImpactPrefab = null;
    [SerializeField] private GameObject _bloodImpactPrefab = null;

    #endregion

    #region booleans
    private bool _controllable = true;
    private bool _dropped = false;
    private bool _moving = false;
    private bool _aimAssistActive = false;
    private bool _rotating = false;

    private bool _windPowerUp = false;
    private bool _piercePowerUp = false;
    #endregion

    #region technical (ne pas touchew)

    private float _timeoutCounter = 0f;

    private Vector3 _direction = Vector3.zero;
    private float _maxCurrentSpeed = 0f;

    private float _rotationX = 0f;
    private float _rotationY = 0f;
    private Vector3 _tempVector = Vector3.zero;

    private float _cameraRotationSpeed = 0f;
    private Transform _enemyToTrack = null;

    private Vector3 _baseForward = Vector3.zero;

    private float _cameraMovementAlpha = 1f;

    private Vector3 _cameraStartPos = Vector3.zero;
    private Vector3 _cameraTargetPos = Vector3.zero;

    private Vector3 _cameraStartRot = Vector3.zero;
    private Vector3 _cameraTargetRot = Vector3.zero;
    #endregion

    #region properties
    public float Velocity
    {
        get => Mathf.Round(_rb.velocity.magnitude);
    }

    public float CurrentSpeedPerc
    {
        get => Velocity/_maxSpeed;
    }
    public float TimeoutCounter { get => _timeoutCounter; }
    
    public bool WindPowerUp { 
        get => _windPowerUp;
        set
        {
            _windPowerUp = value;
            HUDManager.Instance.SetPowerUpVisibility(Power.Wind, value);
        }
    }
    public bool PiercePowerUp { 
        get => _piercePowerUp;
        set
        {
            _piercePowerUp = value;
            HUDManager.Instance.SetPowerUpVisibility(Power.Pierce, value);
        }
    }

    public bool Moving { get => _moving; set => _moving = value; }
    public GameObject BulletTrail { get => _bulletTrail; set => _bulletTrail = value; }

    #endregion properties

    // Start is called before the first frame update

    public void MoveCamera(Vector3 targetPosition)
    {
        _cameraMovementAlpha = 0f;
        _cameraStartPos = _camera.transform.localPosition;
        _cameraTargetPos = targetPosition;

        _cameraStartRot = Vector3.zero;
        _cameraTargetRot = Vector3.zero;
    }

    public void MoveCamera(Vector3 targetPosition, Vector3 rotation)
    {
        _cameraMovementAlpha = 0f;
        _cameraStartPos = _camera.transform.localPosition;
        _cameraTargetPos = targetPosition;

        _cameraStartRot = _camera.transform.localRotation.eulerAngles;
        _cameraTargetRot = rotation;
    }

    public void Shoot()
    {
        _camera.transform.localPosition = Vector3.zero;
        _camera.transform.localRotation = Quaternion.Euler(Vector3.zero);

        Time.timeScale = 1f;

        _timeoutCounter = 0f;

        HUDManager.Instance.ToggleScreen(Screen.Flight);

        if (_direction == Vector3.zero)
        {
            _direction = transform.forward;
        }
        
        _rb.AddForce(_direction * _speed, ForceMode.Acceleration);

        _controllable = false;
        Moving = true;

        HUDManager.Instance.SetCrosshairVisibility(false);

        Invoke("ResetCollision", 0.15f);
    }

    private void GetPowerUp(Power power)
    {
        switch (power)
        {
            case Power.Wind:
                WindPowerUp = true;
                Debug.Log("Wind PowerUp Acquired !");
                break;
            case Power.Pierce:
                PiercePowerUp = true;
                Debug.Log("Pierce PowerUp Acquired !");
                break;
        }
    }

    private void ResetCollision()
    {
        _bulletCollider.enabled = true;
    }

    #region Bounce
    public void Bounce()
    {
        _rotating = true;

        Instantiate(_bounceImpactPrefab, transform.position, Quaternion.identity, transform);

        HUDManager.Instance.ToggleScreen(Screen.None);

        ScoreManager.Instance.Bounces++;

        Time.timeScale = 0.1f;

        transform.localPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z + (-transform.forward.z*_impactOffset));
        _direction = Vector3.zero;

        _virtualCamera.m_Lens.FieldOfView = 40;

        _rb.constraints = RigidbodyConstraints.FreezeAll;
        
        MoveCamera(new Vector3(_camera.transform.localPosition.x, _camera.transform.localPosition.y, _camera.transform.localPosition.z - 5));
        Moving = false;

        Invoke("ZoomIn", 0.75f * Time.timeScale);
    }

    private void ZoomIn()
    {
        _rb.constraints = RigidbodyConstraints.None;
        MoveCamera(_cameraStartPos, transform.forward);
        Invoke("TakeControl", 1f * Time.timeScale);
    }

    private void TakeControl()
    {
        _bulletMesh.transform.forward = transform.forward;

        _controllable = true;

        _timeoutCounter = _timeoutTimer;
        _maxCurrentSpeed = 0f;

        HUDManager.Instance.ToggleScreen(Screen.Aim);

        _camera.transform.localPosition = Vector3.zero;
        _camera.transform.localRotation = Quaternion.Euler(Vector3.zero);

        _rotating = false;
    }
    #endregion Bounce

    #region Kill
    public void Kill(Transform enemyToTrack)
    {
        Time.timeScale = 0.45f;

        _rb.velocity = _direction * 4f;
        Moving = false;
        _enemyToTrack = enemyToTrack;
        _bulletCollider.enabled = false;
        MoveCamera(new Vector3(_camera.transform.localPosition.x, _camera.transform.localPosition.y, _camera.transform.localPosition.z - _killCameraDistance));
        Invoke("TurnAround", 0.4f);

        Instantiate(_bloodImpactPrefab, transform.position, Quaternion.identity, transform);
    }

    private void TurnAround()
    {
        if (!_dropped)
        {
            Time.timeScale = 0.3f;
            _cameraRotationSpeed = 100f;
            Invoke("StopTurnAround", 1.65f);
        }
        else if (_dropped)
        {
            Time.timeScale = 1f;
            _bulletCollider.enabled = true;
        }
    }

    private void StopTurnAround()
    {
        Time.timeScale = 1f;
        _enemyToTrack = null;
        _cameraRotationSpeed = 0f;
        MoveCamera(_cameraStartPos);
        Invoke("Shoot", 0.4f);
        _camera.transform.localEulerAngles = Vector3.zero;

        _camera.transform.localPosition = Vector3.zero;
        _camera.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }
    #endregion Kill

    public void Drop()
    {
        Time.timeScale = 1f;

        _dropped = true;
        _controllable = false;
        _rb.useGravity = true;

        _camera.enabled = false;
        _dropCamera.enabled = true;
        Moving = false; 

        HUDManager.Instance.ToggleScreen(Screen.GameOver);
    }

    void Start()
    {
       GameManager.Instance.SetBulletController(this);
        _camera.enabled = true;
        _virtualCamera.Follow = null;
        Cursor.visible = false;
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Bouncy") && !_dropped)
        {
            Bounce();
        }
        else if (!_dropped && collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Head") || collision.gameObject.CompareTag("Torso") || collision.gameObject.CompareTag("Arm") || collision.gameObject.CompareTag("Leg"))
        {
            //EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
            //enemy.Die();

            int score = ScoreManager.Instance.GetScoreOnKill(_maxCurrentSpeed, collision.gameObject.tag);

            HUDManager.Instance.DisplayKillScreen(score);
            ScoreManager.Instance.Score += score;

            Kill(collision.gameObject.transform);
        }

        else if (!_dropped)
        {
            Drop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_dropped && other.gameObject.CompareTag("Powerup"))
        {
            Powerup powerup = other.gameObject.GetComponent<Powerup>();
            GetPowerUp(powerup.Type);
            Destroy(other.gameObject);
        }
    }

    public void Rewind()
    {
        //_camera.enabled = false;
        BulletTrail.SetActive(false); // ??? pourquoi le set a false 
        Transform cam = gameObject.transform.GetChild(4);
        cam.parent = null;

        _enemyController.SetRagdollOff();

        GameObject enemyChild = _enemyController.transform.GetChild(1).gameObject;
        enemyChild.transform.position = _enemyController.FirstPosition;

        _camBehavior.CanMove = true;
        HUDManager.Instance.StartCoroutine(HUDManager.Instance.FadeInOut());
        _rewindTime.StartRewind();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (TimeoutCounter > 0)
        {
            _timeoutCounter -= Time.deltaTime/Time.timeScale;

            if (TimeoutCounter <= 0)
            {
                Shoot();
            }
        }
        
        if (_cameraMovementAlpha < 1f)
        {
            _camera.transform.localPosition = Vector3.Lerp(_cameraStartPos, _cameraTargetPos, _cameraMovementAlpha);

            if (_cameraTargetRot != Vector3.zero)
            {
                _rotationY += 2000f*Time.deltaTime;
            }

            _cameraMovementAlpha += 10f*Time.deltaTime;

        }

        if (_enemyToTrack != null)
        {
            _camera.transform.LookAt(_enemyToTrack);
            
            if (_cameraRotationSpeed > 0f)
            {
                _camera.transform.Translate(Vector3.right * _cameraRotationSpeed * Time.deltaTime);
            }
        }

        if (_controllable)
        {
            RaycastHit hit;
            Physics.Raycast(transform.position, transform.forward, out hit, 1000f, _raycastLayer);

            if (hit.collider != null && !_aimAssistActive && (hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("Head") || hit.collider.CompareTag("Torso") || hit.collider.CompareTag("Arm") || hit.collider.CompareTag("Leg")))
            {
                Debug.Log("Aim assist active");
                _aimAssistActive = true;
                _sensitivity /= _aimAssistStrength;
            }
            else if (_aimAssistActive && (hit.collider == null || hit.collider.CompareTag("Bouncy")))
            {
                _aimAssistActive = false;
                _sensitivity *= _aimAssistStrength;
            }

            _rotationY += Input.GetAxis("Mouse X") * _sensitivity;
            _rotationX += Input.GetAxis("Mouse Y") * -1 * _sensitivity;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Shoot();
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                HUDManager.Instance.ToggleScreen(Screen.FinalScore);
            }
        }
        else if (Moving && !(HUDManager.Instance.CurrentScreen == Screen.FinalScore))
        {
            _rb.AddForce(transform.forward * _speed*0.0005f *_speedEffectStrength * _rb.velocity.magnitude * 0.02f, ForceMode.Acceleration);
            _virtualCamera.m_Lens.FieldOfView += _virtualCamera.m_Lens.FieldOfView*0.00045f*_speedEffectStrength* _rb.velocity.magnitude * 0.02f;
            _virtualCamera.m_Lens.FieldOfView = Mathf.Clamp(_virtualCamera.m_Lens.FieldOfView, _minFovEffect, _maxFovEffect);

            if (Velocity > _maxCurrentSpeed)
            {
                _maxCurrentSpeed = Velocity;
            }

            if (Input.GetKeyDown(KeyCode.Space) && WindPowerUp)
            {
                Bounce();
                WindPowerUp = false;
            }
        }

        transform.localEulerAngles = new Vector3(_rotationX, _rotationY, 0);

        if (_rotating)
        {
            _bulletMesh.transform.Rotate(_bulletRotationSpeedOnImpact*Time.deltaTime, _bulletRotationSpeedOnImpact * Time.deltaTime, _bulletRotationSpeedOnImpact * Time.deltaTime);
        }

        if (_enemyToTrack != null)
        {
            RaycastHit hit;
            Physics.Raycast(_enemyToTrack.transform.position, (_camera.transform.position- _enemyToTrack.transform.position).normalized, out hit, _killCameraDistance, _raycastLayer);

            if (hit.collider != null && hit.collider.CompareTag("Bouncy"))
            {
                _camera.transform.position = hit.point;
            }
            else
            {
                _camera.transform.position = _enemyToTrack.transform.position + (_camera.transform.position - _enemyToTrack.transform.position).normalized * _killCameraDistance;
            }
        }

    }

}
