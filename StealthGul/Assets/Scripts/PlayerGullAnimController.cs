using UnityEngine;
using System.Collections;

public class PlayerGullAnimController : MonoBehaviour 
{
    public bool animating = false;
    public delegate void AnimationDone();
    public AnimationDone AnimationDoneEvent;

    private Animator m_animController;

    private void Awake()
    {
        m_animController = gameObject.GetComponent<Animator>();
    }

    public void PeckAnim()
    {
        if(!animating)
            StartCoroutine(PlayAnim("Peck", 0));
    }

    private IEnumerator PlayAnim(string animBool, int stateLayerIndex = 0)
    {
        AnimatorStateInfo currentStateInfo = m_animController.GetCurrentAnimatorStateInfo(stateLayerIndex);

        m_animController.SetBool(animBool, true);
        animating = true;

        while (currentStateInfo.IsName("IdleState"))
        {
            yield return null;
            currentStateInfo = m_animController.GetCurrentAnimatorStateInfo(stateLayerIndex);
        }

        m_animController.SetBool(animBool, false);        

        float waitTime = currentStateInfo.length;  
        yield return new WaitForSeconds(waitTime);

        // fire the animation done event
        if (AnimationDoneEvent != null)
        {
            AnimationDoneEvent();
        }

        animating = false;
    }
}
