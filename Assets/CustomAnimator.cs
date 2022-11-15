using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomAnimator : MonoBehaviour
{
    public int animationInt;
    void Start()
    {
        GetComponent<Animator>().SetInteger("Animation_int", animationInt);   
    }
}
