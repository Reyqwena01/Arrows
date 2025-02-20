using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class RewindTime : MonoBehaviour
{
    #region Parameters 
    [SerializeField] private bool _isRewinding = false;
    [SerializeField] List<PointInTime> _pointInTime = new List<PointInTime>();
    [SerializeField] private Rigidbody _ballRb;
    [SerializeField] private float _speed = 2.0f; 

    private int _indexPosition = 0;
    #endregion


    #region Structure
    [System.Serializable]
    private struct PointInTime
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

    #region Methods
    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (_isRewinding)
        {
            Replay(); 
        }

        else
        {
            Record(); 
        }

        //FakeReplayPosition();
    }

    private void Replay()
    {
        if (_pointInTime.Count > 0)
        {

            PointInTime pointInTime = _pointInTime[_indexPosition];

            while(Vector3.Distance(transform.position, pointInTime._position) > 0.1f)
            {
                transform.position = pointInTime._position;
                transform.rotation = pointInTime._rotation;    
                _indexPosition++;
                _indexPosition = Mathf.Clamp(_indexPosition, 0, _pointInTime.Count - 1);
            }

            //_pointInTime.RemoveAt(0);
            //transform.position = Vector3.Lerp(transform.position, pointInTime._position, Time.fixedDeltaTime);
            //transform.rotation = pointInTime._rotation;

        }

        else
        {
            StopRewind(); 
        }
    }

    private void Record()
    {
        if (_pointInTime.Count > Mathf.Round(5f / Time.fixedDeltaTime))
        {
            _pointInTime.RemoveAt(0); 
        }

        _pointInTime.Insert(0, new PointInTime(transform.position, transform.rotation));

    }

    private void FakeReplayPosition()
    {

        //_ballRb.velocity = nextPos; 

        
        //int lastIndex = _pointInTime.Count - 1;
       
        
        //if (Vector3.Distance(_ballRb.velocity,_pointInTime[lastIndex]._position) < 0.1f)
        //{
        //    Debug.Log("You've reached your position");
        //}
        
        //for (int i = 0; i < _pointInTime.Count; i++)
        //{
        //    Vector3 nextPos = _pointInTime[i]._position;
        //    Quaternion nextRo = _pointInTime[i]._rotation;

        //    _ballRb.velocity = Vector3.Lerp(_ballRb.velocity, nextPos, 1);
        //}

    }

    public void StartRewind()
    {
        _isRewinding = true;
        _ballRb.isKinematic = true;

    }

    private void StopRewind()
    {
        _isRewinding = false;
        _ballRb.isKinematic = false; 
    }
    #endregion

}
