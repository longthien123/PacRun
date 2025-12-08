using UnityEngine;

public class NormalCharacter : CharacterBase
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   void Start()
    {
        moveSpeed = 3f;
    }

    public override void Attack()
    {
        Debug.Log("Normal cannot attack");
    }
}
