using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;

public class RewindTime : MonoBehaviour
{
    #region Parameters 
    [SerializeField] private bool _isRewinding = false;
    [SerializeField] List<PointInTime> _pointInTime = new List<PointInTime>();
    [SerializeField] private Rigidbody _ballRb;
    [SerializeField] private float _speed = 2.0f;
    [SerializeField] private BulletController _bulletController;
    [SerializeField] private RewindTime _rewindTime = null;
    [SerializeField] private MeshRenderer[] _meshesToDisable = null;

    private int _indexPosition = 0;
    private int _rangeNumber = 5;
    private bool _isPlayingReverse = false;
    private bool _isLenghtFinish = false; 

    #endregion

    #region Structure
    [System.Serializable]
    public struct PointInTime
    {
        public Vector3 _position;
        public Quaternion _rotation;

        public PointInTime(Vector3 position, Quaternion rotation)
        {
            _position = position;
            _rotation = rotation;
        }
    }
    #endregion

    #region Propreties

    public List<PointInTime> PointInTime1 { get => _pointInTime; set => _pointInTime = value; }
    public bool IsPlayingReverse { get => _isPlayingReverse; set => _isPlayingReverse = value; }
    public bool IsLenghtFinish { get => _isLenghtFinish; set => _isLenghtFinish = value; }

    #endregion

    #region Methods
    private void Update()
    {

    }

    private void FixedUpdate()
    {
        if (_isRewinding)
        {
            Replay();
            CheckReplayPosition();
        }

        else
        {
            Record(); 
        }
    }

    private void Replay()
    {
        if (PointInTime1.Count > 0)
        {
            if (!IsPlayingReverse)
            {
                StartCoroutine(FirtReplayTowardFirtPosition());
            }
             
        }
    }

    private void Record()
    {
        if (PointInTime1.Count > Mathf.Round(8f / Time.fixedDeltaTime))
        {
            PointInTime1.RemoveAt(0); 
        }

        else if (_bulletController.Moving)
        {
            PointInTime1.Insert(0, new PointInTime(transform.position, transform.rotation));
        }

    }

    private void CheckReplayPosition()
    {
        
        int lastIndex = PointInTime1.Count - 1; 
        
        if (_bulletController != null)
        {
            if (_bulletController.Moving)
            {
                Vector3 positionOne = PointInTime1[lastIndex]._position;

                if (Vector3.Distance(transform.position, positionOne) < 0.1f && PointInTime1.Count > _rangeNumber)
                {
                    StopAllCoroutines();
                    StopRewind();
                    IsLenghtFinish = true;
                    //IsPlayingReverse = !IsPlayingReverse;
                }

            }
        }       

    }


    IEnumerator FirtReplayTowardFirtPosition()
    {
        PointInTime pointInTime = PointInTime1[_indexPosition];

        while (Vector3.Distance(transform.position, pointInTime._position) > 0.1f)
        {
            transform.position = pointInTime._position;
            transform.rotation = pointInTime._rotation;
            yield return new WaitForFixedUpdate();
            Debug.Log("FirstRewind");
        }

        _indexPosition++;
        _indexPosition = Mathf.Clamp(_indexPosition, 0, PointInTime1.Count - 1);
    }

    public void StartRewind()
    {
        _isRewinding = true;
        _ballRb.isKinematic = true;

        for (int i = 0; i < _meshesToDisable.Length; i++)
        {
            _meshesToDisable[i].enabled = false;
        }
    }

    private void StopRewind()
    {
        _isRewinding = false;
        _ballRb.isKinematic = false; 
        _bulletController.Moving = false;
    }
    #endregion
    

}
