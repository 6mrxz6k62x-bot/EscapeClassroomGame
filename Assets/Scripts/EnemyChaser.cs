using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaser : MonoBehaviour
{
    public MazeLevel level;
    public float patrolSpeed = 1.8f;
    public float chaseSpeed = 3.0f;      
    public float sightRange = 9f;
    public float viewAngle = 120f;      
    public float hearRadius = 0f;        
    public float loseSightTime = 0.5f;   
    public float killDistance = 0.7f;
    public float startDelay = 5f;
    public bool flipToFaceDirection = false; 
    

    Transform player;
    SpriteRenderer sr;
    List<Vector2> path = new List<Vector2>();
    int pathIndex;
    bool ready;
    bool chasing;
    float delay;
    float repathTimer;
    float lostTimer;
    Vector2 facing = Vector2.down;
    Vector2 lastSeen;

    IEnumerator Start()
    {
        sr = GetComponent<SpriteRenderer>();

     
        Rigidbody2D body = GetComponent<Rigidbody2D>();
        if (body != null)
        {
            body.gravityScale = 0;
            body.linearVelocity = Vector2.zero;
            body.bodyType = RigidbodyType2D.Kinematic;
        }

        yield return null;  

        if (level == null)
            level = FindFirstObjectByType<MazeLevel>();

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p == null || level == null)
        {
            Debug.LogError("EnemyChaser needs a MazeLevel and an object tagged Player.");
            yield break;
        }

        player = p.transform;
        transform.position = FarthestFrom(player.position);
        delay = startDelay;
        ready = true;
    }

    
    void Update()
    {
        if (!ready || TaskPanel.IsDead)
            return;

        if (delay > 0)
        {
            delay -= Time.deltaTime;
            return;
        }

        float dist = Vector2.Distance(transform.position, player.position);
        if (dist < killDistance)
        {
            if (TaskPanel.Instance)
                TaskPanel.Instance.KillTask("The monster caught you");
            return;
        }
        
        
        bool sees = dist <= sightRange && InFront(player.position) && HasLineOfSight(transform.position, player.position);
        bool hears = hearRadius > 0 && dist <= hearRadius;

        if (sees || hears)
        {
            chasing = true;
            lostTimer = 0;
            lastSeen = player.position;
        }
        else if (chasing)
        {
            lostTimer += Time.deltaTime;
            if (lostTimer > loseSightTime)
                chasing = false;
        }

        repathTimer -= Time.deltaTime;
        if (repathTimer <= 0)
        {
            repathTimer = 0.25f;
            if (chasing)
                SetPath(sees || hears ? (Vector2)player.position : lastSeen);
            else if (pathIndex >= path.Count)
                SetPath(RandomFloorTile());
        }

        Follow(chasing ? chaseSpeed : patrolSpeed);
    }

    void Follow(float speed)
    {
        if (pathIndex >= path.Count)
            return;

        Vector2 target = path[pathIndex];
        Vector2 pos = transform.position;

        if (flipToFaceDirection && sr != null && Mathf.Abs(target.x - pos.x) > 0.05f)
            sr.flipX = target.x < pos.x;

        Vector2 dir = target - pos;
        if (dir.sqrMagnitude > 0.0001f)
            facing = dir.normalized;

        transform.position = Vector2.MoveTowards(pos, target, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target) < 0.05f)
            pathIndex++;
    }



    int Col(Vector2 w) { return Mathf.FloorToInt(w.x); }
    int Row(Vector2 w) { return level.MapHeight - 1 - Mathf.FloorToInt(w.y); }
    Vector2 Center(int col, int row) { return new Vector2(col + 0.5f, level.MapHeight - row - 0.5f); }

    void Bfs(int startCol, int startRow, int[] parent, int[] dist)
    {
        int W = level.MapWidth;
        for (int i = 0; i < parent.Length; i++)
        {
            parent[i] = -1;
            dist[i] = -1;
        }

        Queue<int> queue = new Queue<int>();
        int start = startRow * W + startCol;
        dist[start] = 0;
        queue.Enqueue(start);

        int[] dx = { 1, -1, 0, 0 };
        int[] dy = { 0, 0, 1, -1 };

        while (queue.Count > 0)
        {
            int cur = queue.Dequeue();
            int c = cur % W;
            int r = cur / W;

            for (int d = 0; d < 4; d++)
            {
                int nc = c + dx[d];
                int nr = r + dy[d];
                if (level.IsBlocked(nc, nr))
                    continue;

                int n = nr * W + nc;
                if (dist[n] != -1)
                    continue;

                dist[n] = dist[cur] + 1;
                parent[n] = cur;
                queue.Enqueue(n);
            }
        }
    }

    void SetPath(Vector2 goal)
    {
        int W = level.MapWidth;
        int H = level.MapHeight;
        int[] parent = new int[W * H];
        int[] dist = new int[W * H];

        Vector2 here = transform.position;
        int sc = Col(here), sr2 = Row(here);
        int gc = Col(goal), gr = Row(goal);

        if (level.IsBlocked(sc, sr2) || level.IsBlocked(gc, gr))
            return;

        Bfs(sc, sr2, parent, dist);

        int goalIndex = gr * W + gc;
        if (dist[goalIndex] == -1)
            return;

        List<Vector2> newPath = new List<Vector2>();
        for (int i = goalIndex; i != -1; i = parent[i])
            newPath.Add(Center(i % W, i / W));
        newPath.Reverse();

     
        if (newPath.Count > 1 && Vector2.Distance(here, newPath[1]) < Vector2.Distance(newPath[0], newPath[1]))
            newPath.RemoveAt(0);

        path = newPath;
        pathIndex = 0;
    }

    Vector2 FarthestFrom(Vector2 from)
    {
        int W = level.MapWidth;
        int H = level.MapHeight;
        int[] parent = new int[W * H];
        int[] dist = new int[W * H];
        Bfs(Col(from), Row(from), parent, dist);

        int best = 0;
        for (int i = 0; i < dist.Length; i++)
            if (dist[i] > dist[best])
                best = i;

        return Center(best % W, best / W);
    }

    Vector2 RandomFloorTile()
    {
        for (int i = 0; i < 200; i++)
        {
            int c = Random.Range(1, level.MapWidth - 1);
            int r = Random.Range(1, level.MapHeight - 1);
            if (!level.IsBlocked(c, r))
                return Center(c, r);
        }
        return transform.position;
    }

    bool InFront(Vector2 target)
    {
        Vector2 toTarget = (target - (Vector2)transform.position).normalized;
        return Vector2.Angle(facing, toTarget) <= viewAngle / 2f;
    }

    bool HasLineOfSight(Vector2 a, Vector2 b)
    {
        float distance = Vector2.Distance(a, b);
        int steps = Mathf.CeilToInt(distance / 0.2f);
        for (int i = 1; i < steps; i++)
        {
            Vector2 p = Vector2.Lerp(a, b, i / (float)steps);
            if (level.IsBlocked(Col(p), Row(p)))
                return false;
        }
        return true;
    }
}
