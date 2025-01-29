using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;
    [SerializeField] private Rigidbody _rb = null;

    [SerializeField] private Camera _camera = null;
    private Camera _enemyCamera = null;
    [SerializeField] private CinemachineVirtualCamera _virtualCamera = null;

    [SerializeField] private Vector3 _direction = Vector3.zero;
    [SerializeField] private Collider _bulletCollider = null;

    private bool _controllable = true;
    private bool _moving = true;

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

    [SerializeField] private float _sensitivity = 15f;

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
        if (_direction == Vector3.zero)
        {
            _direction = transform.forward;
        }
        _rb.AddForce(_direction * _speed, ForceMode.Acceleration);
        _controllable = false;
        _moving = true;
        Invoke("ResetCollision", 0.25f);
    }

    private void ResetCollision()
    {
        _bulletCollider.enabled = true;
    }

    public void Bounce()
    {
        //transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z - 3); //Bullet clipping failsafe
        _direction = Vector3.zero;
        _virtualCamera.m_Lens.FieldOfView = 40;
        _rb.constraints = RigidbodyConstraints.FreezeAll;
        MoveCamera(new Vector3(_camera.transform.localPosition.x, _camera.transform.localPosition.y, _camera.transform.localPosition.z - 5));
        Invoke("ZoomIn", 0.75f);
        _moving = false;
    }

    private void ZoomIn()
    {
        _rb.constraints = RigidbodyConstraints.None;
        MoveCamera(_cameraStartPos, transform.forward);
        Invoke("TakeControl", 1f);
    }

    private void TakeControl()
    {
        _controllable = true;
    }

    public void Kill(Transform enemyToTrack)
    {
        Time.timeScale = 0.45f;
        _rb.velocity = _direction * 3f;
        _moving = false;
        _enemyToTrack = enemyToTrack;
        _bulletCollider.enabled = false;
        MoveCamera(new Vector3(_camera.transform.localPosition.x, _camera.transform.localPosition.y, _camera.transform.localPosition.z - 10));
        Invoke("TurnAround", 0.4f);
    }

    private void TurnAround()
    {
        Time.timeScale = 0.3f;
        _cameraRotationSpeed = 60f;
        Invoke("StopTurnAround", 1.2f);
    }

    private void StopTurnAround()
    {
        Time.timeScale = 1f;
        _enemyToTrack = null;
        _cameraRotationSpeed = 0f;
        MoveCamera(_cameraStartPos);
        Invoke("Shoot", 1f);
        _camera.transform.localEulerAngles = Vector3.zero;
    }

    void Start()
    {
        _camera.enabled = true;
        _virtualCamera.Follow = null;
        Cursor.visible = false;
    }



    // Update is called once per frame
    void Update()
    {
        if (_cameraMovementAlpha < 1f)
        {
            _camera.transform.localPosition = Vector3.Lerp(_cameraStartPos, _cameraTargetPos, _cameraMovementAlpha);

            if (_cameraTargetRot != Vector3.zero)
            {
                _rotationY += 0.4f;
            }

            _cameraMovementAlpha += 0.002f;

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
            _rotationY += Input.GetAxis("Mouse X") * _sensitivity;
            _rotationX += Input.GetAxis("Mouse Y") * -1 * _sensitivity;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Shoot();
            }
        }
        else if (_moving)
        {
            _rb.AddForce(transform.forward * _speed*0.0005f, ForceMode.Acceleration);
            _virtualCamera.m_Lens.FieldOfView += _virtualCamera.m_Lens.FieldOfView*0.00045f;
            _virtualCamera.m_Lens.FieldOfView = Mathf.Clamp(_virtualCamera.m_Lens.FieldOfView, 40, 160);
        }

        transform.localEulerAngles = new Vector3(_rotationX, _rotationY, 0);
    }
}
