using UnityEngine;

public class ChestItemAnimation : MonoBehaviour
{
    private Animator animator;
    [SerializeField] GameObject item;
    public bool isOpened = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();   
    }

    // Update is called once per frame
    public void OpenChest()
    {
        Instantiate(item, transform.position, Quaternion.identity);
        isOpened = true;
        if (animator != null)
        {
            animator.SetBool("open", true);
        }
    }
}
