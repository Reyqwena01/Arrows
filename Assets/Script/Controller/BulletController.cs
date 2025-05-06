using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.SceneManagement;

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
    private Vector3 _orbitalVector;

    [Header("Menu")]
    [SerializeField] private bool _isInMenu = false;
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
    [SerializeField] private TrailRenderer _trailRenderer = null; 

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
    private bool _boucing = false;

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

    private bool _isShaking = false; 
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
    public bool IsShaking { get => _isShaking; set => _isShaking = value; }

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
        _camera.transform.rotation = Quaternion.Euler(Vector3.zero);

        _camera.transform.localPosition = Vector3.zero;
        _camera.transform.localRotation = Quaternion.Euler(Vector3.zero);

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        _boucing = false;

        if (TimeoutCounter > 4)
        {
            AudioManager.Instance.PlaySound("Whip");
            AudioManager.Instance.PlaySound("QuickShot");

            StartCoroutine(ShakeCamera(0.25f));

            _speedEffectStrength = 1.4f;
        }
        else if (TimeoutCounter > 2)
        {
            AudioManager.Instance.PlaySound("QuickShot");
            _speedEffectStrength = 1.2f;
        }
        else
        {
            _speedEffectStrength = 1f;
        }

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
        HUDManager.Instance.ContinueTutorial();

        AudioManager.Instance.PlaySound("Bounce");
        AudioManager.Instance.StopPlayingWind();

        _rotating = true;

        Instantiate(_bounceImpactPrefab, transform.position, Quaternion.identity, transform);

        ScoreManager.Instance.Bounces++;
        HUDManager.Instance.ToggleScreen(Screen.Bounce);

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

        HUDManager.Instance.BounceTextAnimator.SetTrigger("BounceExit");
    }
    #endregion Bounce

    #region Kill
    public void Kill(Transform enemyToTrack)
    {
        Time.timeScale = 0.45f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        AudioManager.Instance.PauseWind();
        AudioManager.Instance.PlaySound("Slowdown");

        _rb.velocity = _direction * 2f;
        Moving = false;
        _enemyToTrack = enemyToTrack;
        _bulletCollider.enabled = false;
        MoveCamera(new Vector3(_camera.transform.localPosition.x, _camera.transform.localPosition.y, _camera.transform.localPosition.z - _killCameraDistance));
        
        _orbitalVector = new Vector3(Random.Range(-1, 2), Random.Range(-1f, 1f), 0);

        TurnAround();

        Instantiate(_bloodImpactPrefab, transform.position, Quaternion.identity, transform);

        StartCoroutine(ShakeCamera(0.25f));
    }

    private void TurnAround()
    {
        if (!_dropped)
        {
            Time.timeScale = 0.3f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            _cameraRotationSpeed = 100f;
            Invoke("StopTurnAround", 1.65f);
        }
        else if (_dropped)
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
            _bulletCollider.enabled = true;
        }
    }

    private void StopTurnAround()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        _enemyToTrack = null;
        _cameraRotationSpeed = 0f;
        MoveCamera(_cameraStartPos);
        Invoke("Shoot", 0.3f);
        AudioManager.Instance.UnpauseWind();
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

        AudioManager.Instance.StopPlayingWind();
        AudioManager.Instance.PlaySound("Drop");
        Invoke("ReloadLevel", 1.5f);
    }

    private void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void Start()
    {
       GameManager.Instance.SetBulletController(this);
       _camera.enabled = true;
       _virtualCamera.Follow = null;
       Cursor.visible = false;

        HUDManager.Instance.ToggleScreen(Screen.Aim);
        HUDManager.Instance.AimTimer.enabled = false;
        AudioManager.Instance.StopPlayingWind();

        ScoreManager.Instance.Score = 0;
        ScoreManager.Instance.Bounces = 0;

        HUDManager.Instance.IsScoreLerping = false;
        HUDManager.Instance.ScoreCumulatedText.text = 0.ToString();

        Cursor.visible = false;

        Time.timeScale = 0.3f;
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Bouncy") && !_dropped && !_boucing)
        {
            Bounce();
            _boucing = true;
        }

        else if (!_dropped && collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Head") || collision.gameObject.CompareTag("Torso") || collision.gameObject.CompareTag("Arm") || collision.gameObject.CompareTag("Leg"))
        {

            int score = ScoreManager.Instance.GetScoreOnKill(_maxCurrentSpeed, collision.gameObject.tag);

            HUDManager.Instance.DisplayKillScreen(score, collision.gameObject.tag);
            ScoreManager.Instance.Score += score;

            Kill(collision.gameObject.transform);
        }

        else if (collision.gameObject.CompareTag("Stiff") && !_dropped)
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

        if (other.CompareTag("Bouncy"))
        {
            Debug.Log("Touch Wall While Rewinding");
            Vector3 position = transform.TransformDirection(transform.forward); 
            HUDManager.Instance.ShowBoingEffectAtLocation(transform.localPosition); 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!_dropped && other.CompareTag("Enemy"))
        {
            IsShaking = false; 
        }
    }

    #region Rewind

    public void Rewind()
    {
        _rb.velocity = Vector3.zero;

        //_camera.enabled = false;
        BulletTrail.SetActive(false); // ??? pourquoi le set a false 
        Transform cam = gameObject.transform.GetChild(1);
        cam.parent = null;
        HUDManager.Instance.ToggleScreen(Screen.Score);

        CinemachineVirtualCamera cinemachine = cam.GetComponent<CinemachineVirtualCamera>();
        cinemachine.m_Lens.FieldOfView = 60;

        _camBehavior.CanMove = true;
        _rewindTime.StartRewind();

        Time.timeScale = 1.75f;

        if (_trailRenderer != null) { _trailRenderer.enabled = true; }


        if (GameManager.Instance.ListEnemy.Count > 0)
        {
            for (int i = 0; i < GameManager.Instance.ListEnemy.Count; i++)
            {
                GameManager.Instance.ListEnemy[i].GetComponent<EnemyController>().SetRagdollOff();
            }
        }
    }

    public IEnumerator ShakeCamera(float delay)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasic = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        if(IsShaking)
        {
            cinemachineBasic.m_AmplitudeGain = 2.5f;
            cinemachineBasic.m_FrequencyGain = 1.8f;
        }

        yield return new WaitForSeconds(delay);

        IsShaking = false;
        cinemachineBasic.m_AmplitudeGain = 0f;
        cinemachineBasic.m_FrequencyGain = 0f;
    }

    #endregion

    // Update is called once per frame
    void Update()
    {
        
        if (TimeoutCounter > 0)
        {
            _timeoutCounter -= Time.deltaTime/Time.timeScale;

            if (TimeoutCounter <= 0)
            {
                Shoot();
                AudioManager.Instance.PlaySound("Shoot");
                AudioManager.Instance.StartPlayingWind();
            }
        }

        if (_controllable)
        {
            RaycastHit hit;
            Physics.Raycast(transform.position, transform.forward, out hit, 1000f, _raycastLayer);

            if (hit.collider != null && !_aimAssistActive && (hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("Head") || hit.collider.CompareTag("Torso") || hit.collider.CompareTag("Arm") || hit.collider.CompareTag("Leg") || hit.collider.CompareTag("Sign")))
            {
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

            if (Input.GetKeyDown(KeyCode.Mouse0) && (_isInMenu == false || _aimAssistActive))
            {
                Shoot();
                AudioManager.Instance.PlaySound("Shoot");
                AudioManager.Instance.StartPlayingWind();
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                HUDManager.Instance.ToggleScreen(Screen.FinalScore);
            }
        }
        else if (Moving && !(HUDManager.Instance.CurrentScreen == Screen.FinalScore))
        {
            _rb.AddForce((transform.forward * _speed*0.0005f * _speedEffectStrength * _rb.velocity.magnitude * 8.25f) * Time.deltaTime, ForceMode.Acceleration);
            _virtualCamera.m_Lens.FieldOfView += (_virtualCamera.m_Lens.FieldOfView*0.00045f*_speedEffectStrength* _rb.velocity.magnitude * 8.25f)* Time.deltaTime;
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
            _virtualCamera.m_Lens.FieldOfView -= _virtualCamera.m_Lens.FieldOfView * 0.00500f * _speedEffectStrength * _rb.velocity.magnitude * 0.02f;
            _virtualCamera.m_Lens.FieldOfView = Mathf.Clamp(_virtualCamera.m_Lens.FieldOfView, _minFovEffect, _maxFovEffect);

            RaycastHit hit;
            Physics.Raycast(_enemyToTrack.transform.position, (_camera.transform.position- _enemyToTrack.transform.position).normalized, out hit, _killCameraDistance, _raycastLayer);

            if (hit.collider != null && hit.collider.CompareTag("Bouncy"))
            {
                _camera.transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            }
            else
            {
                _camera.transform.position = _enemyToTrack.transform.position + (_camera.transform.position - _enemyToTrack.transform.position).normalized * _killCameraDistance;
            }
        }

    }

    private void LateUpdate()
    {
        if (_cameraMovementAlpha < 1f)
        {
            _camera.transform.localPosition = Vector3.Lerp(_cameraStartPos, _cameraTargetPos, _cameraMovementAlpha);

            if (_cameraTargetRot != Vector3.zero)
            {
                _rotationY += 2000f * Time.deltaTime;
            }

            _cameraMovementAlpha = Mathf.Clamp01(_cameraMovementAlpha + 10f * Time.deltaTime);
        }

        if (_enemyToTrack != null && _cameraRotationSpeed > 0f)
        {
            // Fait tourner la caméra autour de l'ennemi selon le vecteur orbital
            _camera.transform.RotateAround(
                _enemyToTrack.position,
                _orbitalVector.normalized,
                _cameraRotationSpeed * Time.deltaTime
            );

            // Regarde toujours l'ennemi
            _camera.transform.LookAt(_enemyToTrack);
        }
    }


}
