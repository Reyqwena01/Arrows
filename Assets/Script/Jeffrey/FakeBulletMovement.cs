using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RewindTime;

public class FakeBulletMovement : MonoBehaviour
{
    [SerializeField] private RewindTime _rewindTime = null;
    [SerializeField] private BulletController _bulletController = null;
    [SerializeField] private SphereCollider _sphereCollider = null;

    private int _lastIndex = 0;
    private bool _isReplaying = false;
    private bool _canReplay = false; 


    private void FixedUpdate()
    {
        MakeReplay();
    }

    private void MakeReplay()
    {
        if (_rewindTime != null)
        {

            if (_rewindTime.IsPlayingReverse && !_isReplaying && _rewindTime.IsLenghtFinish)
            {
                _sphereCollider.enabled = false; // C'est pas la meilleur soluce, si on veut faire rewind les ennemis également    
                _lastIndex = _rewindTime.PointInTime1.Count - 1;
                _isReplaying = true;
                _bulletController.BulletTrail.SetActive(true);
                
                if (!_canReplay)
                {
                    StartCoroutine(Replay()); 
                }

            }
        }
    }

    IEnumerator Replay()
    {

        while (_lastIndex >= 0)
        {
            PointInTime pointInTime = _rewindTime.PointInTime1[_lastIndex];

            while (Vector3.Distance(transform.position, _rewindTime.PointInTime1[_lastIndex]._position) > 0.1f)
            {

                transform.position = pointInTime._position;
                transform.rotation = pointInTime._rotation;
                
                Debug.Log("SecondRewind");

                yield return new WaitForFixedUpdate();
            }

            _lastIndex--;
            _rewindTime.PointInTime1.Remove(pointInTime);

            if (_lastIndex < 0)
            {
                _isReplaying = false;
                _canReplay = !_canReplay;
                Invoke("EndLevel", 1.25f);
            }
        }


    }

    public void EndLevel()
    {
        Time.timeScale = 0f;
        HUDManager.Instance.ToggleScreen(Screen.FinalScore);
    }


}
