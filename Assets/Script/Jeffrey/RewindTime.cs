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
    private bool _canpPlayPosition = false; 
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

        FakeReplayPosition();
    }

    private void Replay()
    {
        if (_pointInTime.Count > 0)
        {

            PointInTime pointInTime = _pointInTime[_indexPosition];
            //StartCoroutine(MoveTowardDirection(5)); 
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
        if (_pointInTime.Count > Mathf.Round(5f / Time.fixedDeltaTime))
        {
            _pointInTime.RemoveAt(0); 
        }

        _pointInTime.Insert(0, new PointInTime(transform.position, transform.rotation));

    }

    private void FakeReplayPosition()
    {
        int lastIndex = _pointInTime.Count - 1; 
        
        if (Vector3.Distance(transform.position, _pointInTime[lastIndex]._position) > 0.1f)
        {
            //PointInTime pointInTime = _pointInTime[_indexPosition]; 

            //transform.position = pointInTime._position;
            //transform.rotation = pointInTime._rotation;

            //Debug.Log("Reached First Position"); 

        }
        

    }

    IEnumerator MoveTowardDirection(float duration)
    {
        PointInTime pointInTime = _pointInTime[_indexPosition];

        while (Vector3.Distance(transform.position, pointInTime._position) > 0.1f)
        {
            transform.position = pointInTime._position;
            transform.rotation = pointInTime._rotation;
            _indexPosition = Mathf.Clamp(_indexPosition, 0, _pointInTime.Count - 1);
            yield return new WaitForFixedUpdate();
        }

        //transform.position = pointInTime._position;
        //transform.rotation = pointInTime._rotation;

        _indexPosition++;
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
