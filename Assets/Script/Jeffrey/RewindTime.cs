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

    private int _indexPosition = 0;
    private bool _canpPlayPosition = false;
    private int _rangeNumber = 5;
    private bool _isPlayingRevrse = false; 
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
            if (_isPlayingRevrse)
            {
                StartCoroutine(MoveTowardDirection());
            }

            else
            {
                StartCoroutine(FakeMovement()); 
            }
             
        }

        else
        {
            StopRewind(); 
        }
    }

    private void Record()
    {
        if (_pointInTime.Count > Mathf.Round(8f / Time.fixedDeltaTime))
        {
            _pointInTime.RemoveAt(0); 
        }

        else if (_bulletController.Moving)
        {
            _pointInTime.Insert(0, new PointInTime(transform.position, transform.rotation));
        }

    }

    private void FakeReplayPosition()
    {
        
        int lastIndex = _pointInTime.Count - 1; 
        
        if (_bulletController != null)
        {
            if (_bulletController.Moving)
            {
                PointInTime firstPosition = new PointInTime(new Vector3(11.7f, 1.99f, -2.54f), Quaternion.identity);
                int index = _pointInTime.IndexOf(firstPosition);
                Vector3 PositionOne = _pointInTime[lastIndex]._position;

                if (Vector3.Distance(transform.position, PositionOne) < 0.1f && _pointInTime.Count > _rangeNumber)
                {
                    StopRewind();
                    if (!_isRewinding)
                    {
                        StartCoroutine(FakeMovement()); 
                    }
                }

            }
        }       

    }


    IEnumerator MoveTowardDirection()
    {   
        if (_isRewinding)
        {
            PointInTime pointInTime = _pointInTime[_indexPosition];

            while (Vector3.Distance(transform.position, pointInTime._position) > 0.1f)
            {
                transform.position = pointInTime._position;
                transform.rotation = pointInTime._rotation;
                yield return new WaitForFixedUpdate();
            }

            _indexPosition++;
            _indexPosition = Mathf.Clamp(_indexPosition, 0, _pointInTime.Count - 1);
        };
    }

    IEnumerator FakeMovement()
    {
        if (!_isRewinding)
        {
            int lastIndex = _pointInTime.Count - 1;
            PointInTime pointInTime = _pointInTime[lastIndex];

            while (Vector3.Distance(transform.position, pointInTime._position) > 0.1f)
            {
                transform.position = pointInTime._position;
                transform.rotation = pointInTime._rotation;
                yield return new WaitForFixedUpdate();
                Debug.Log(lastIndex); 
            }

            lastIndex--;
            //lastIndex= Mathf.Clamp(lastIndex, 0, lastIndex);
        };
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
        _bulletController.Moving = false;
    }
    #endregion

}
