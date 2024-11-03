using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleTransitionAnimation : MonoBehaviour
{

    private Animator circleAnimation;

    // Start is called before the first frame update
    void Start()
    {
        circleAnimation = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        circleAnimation.SetBool("sceneTransition", FeedbackAnimationParameters.sceneTransition);
    }
}
