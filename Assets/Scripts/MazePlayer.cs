using UnityEngine;

public class MazePlayer : MonoBehaviour
{
    float speed = 3.5f, t;
    MazeLevel cfg; Rigidbody2D rb; SpriteRenderer sr; Vector2 dir; int facing;   // 0 down, 1 up, 2 left, 3 right

    public void Setup(MazeLevel c) { cfg = c; speed = c.moveSpeed; }
    void Awake() { rb = GetComponent<Rigidbody2D>(); sr = GetComponent<SpriteRenderer>(); }

    static bool Has(Sprite[] a) { return a != null && a.Length > 0; }

    void Update()
    {
        dir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        bool moving = dir.sqrMagnitude > 0.01f;
        if (moving)
        {
            facing = Mathf.Abs(dir.x) >= Mathf.Abs(dir.y) ? (dir.x < 0 ? 2 : 3) : (dir.y > 0 ? 1 : 0);
            t += Time.deltaTime * cfg.animFps;
        }
        else t = 0;

        Sprite[] walk = cfg.walkDown; Sprite idle = cfg.idleDown; bool flip = false;
        if (facing == 1) { walk = cfg.walkUp; idle = cfg.idleUp; }
        else if (facing == 3) { walk = cfg.walkRight; idle = cfg.idleRight; }
        else if (facing == 2)
        {
            walk = cfg.walkLeft; idle = cfg.idleLeft;
            if (!Has(walk) && cfg.mirrorRightForLeft) { walk = cfg.walkRight; idle = idle != null ? idle : cfg.idleRight; flip = true; }
        }
        if (!Has(walk)) walk = cfg.walkDown;           // fallbacks so nothing ever shows blank
        if (idle == null) idle = cfg.idleDown;

        sr.flipX = flip;
        if (moving && Has(walk)) sr.sprite = walk[(int)t % walk.Length];
        else if (idle != null) sr.sprite = idle;
    }

    void FixedUpdate() { rb.linearVelocity = dir.normalized * speed; }
}
