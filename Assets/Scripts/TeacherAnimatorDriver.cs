using UnityEngine;


public class TeacherAnimatorDriver : MonoBehaviour
{
    Animator animator;
    Vector3 lastPosition;
    Vector2 facing = Vector2.down;

    void Start()
    {
        animator = GetComponent<Animator>();
        lastPosition = transform.position;
    }

    void LateUpdate()
    {
        if (Time.deltaTime <= 0)
            return;   

        Vector2 delta = transform.position - lastPosition;
        lastPosition = transform.position;
        bool moving = delta.sqrMagnitude > 0.000001f;

        if (moving)
        {
            
            if (Mathf.Abs(delta.x) >= Mathf.Abs(delta.y))
                facing = new Vector2(Mathf.Sign(delta.x), 0);
            else
                facing = new Vector2(0, Mathf.Sign(delta.y));
        }

        animator.SetBool("Moving", moving);
        animator.SetFloat("MoveX", facing.x);
        animator.SetFloat("MoveY", facing.y);
    }
}
