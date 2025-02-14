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
        if (Input.GetKeyDown(KeyCode.E))
        {
            //StartRewind();
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            StopRewind();   
        }
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
    }

    private void Replay()
    {
        if (_pointInTime.Count > 0)
        {
            PointInTime pointInTime = _pointInTime[0];
            transform.position = pointInTime._position; 
            transform.rotation = pointInTime._rotation;
            _pointInTime.RemoveAt(0);
           
        }

        else
        {
            StopRewind(); 
        }
    }

    private void Record()
    {
        _pointInTime.Insert(0, new PointInTime(transform.position, transform.rotation));
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
