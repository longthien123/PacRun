using UnityEngine;

public class NinjaSmoke : MonoBehaviour
{
    private Animator smokeAnimator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        smokeAnimator = GetComponent<Animator>();
        Destroy(gameObject, smokeAnimator.GetCurrentAnimatorStateInfo(0).length);
    }

   
}
