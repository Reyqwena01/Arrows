using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorKillHeadShotController : MonoBehaviour
{
    // Start is called before the first frame update
    public void PlaySound()
    {
        AudioManager.Instance.PlaySound("HeadShotScore");
    }


}
