using UnityEngine;


public class MazeLevel : MonoBehaviour
{
    [Range(1, 4)] public int level = 1;
    public Sprite background;      
    [Header("Optional: your own player object (with your own animation + movement)")]
    public GameObject myPlayer;

    [Header("Player sprites: drag your PNGs in. Leave a direction empty to reuse another.")]
    public Sprite idleDown, idleUp, idleLeft, idleRight;   
    public Sprite[] walkDown, walkUp, walkLeft, walkRight; 
    public float animFps = 8f;
    public bool mirrorRightForLeft = true;  
    public float moveSpeed = 3.5f;
    public float taskSeconds = 1.5f;  

    const int MY = 4;              
    static readonly int[] Ws = {22,25,31,37};
    static readonly int[] Hs = {17,20,23,26};
    static readonly string[][] Grids = {
        new[] {
            "1111111111111111111111",
            "1111111111111111111111",
            "1000000000000000000001",
            "1000000000000000000001",
            "1111111111111111111111",
            "1000001000000000000000",
            "1000001000000000000000",
            "1001001001001111111001",
            "1001001001000000000001",
            "1001001001000000000001",
            "1001001001001111001111",
            "1000000001000001000001",
            "1000000001000001000001",
            "1001111001111001111001",
            "1000000000000001000001",
            "1000000000000001000001",
            "1001111111111111111111"
        },
        new[] {
            "1111111111111111111111111",
            "1111111111111111111111111",
            "1000000000000000000000001",
            "1000000000000000000000001",
            "1111111111111111111111111",
            "1000000000000001000000000",
            "1000000000000001000000000",
            "1001111111001111001001001",
            "1000000001000000001001001",
            "1000000001000000001001001",
            "1001111001111001111001111",
            "1000001000000000001000001",
            "1000001000000000001000001",
            "1111111111111111001111001",
            "1000000000001000000000001",
            "1000000000001000000000001",
            "1001111111001001111111001",
            "1001000000000001000000001",
            "1001000000000001000000001",
            "1001111111111111111111111"
        },
        new[] {
            "1111111111111111111111111111111",
            "1111111111111111111111111111111",
            "1000000000000000000000000000001",
            "1000000000000000000000000000001",
            "1111111111111111111111111111111",
            "1000001000000000000000000000000",
            "1000001000000000000000000000000",
            "1001001111111111111111001111001",
            "1001000001000000000001000001001",
            "1001000001000000000001000001001",
            "1001001001001111111001111001111",
            "1001000001001000001000001000001",
            "1001000001001000001000001000001",
            "1001001111001001001111001111001",
            "1001000000001000000001001000001",
            "1001000000001000000001001000001",
            "1001111111111001001001001001111",
            "1000000001000001001001001000001",
            "1000000001000001001001001000001",
            "1111111001001111001111001111001",
            "1000000001000001000000000000001",
            "1000000001000001000000000000001",
            "1001111111111111111111111111111"
        },
        new[] {
            "1111111111111111111111111111111111111",
            "1111111111111111111111111111111111111",
            "1000000000000000000000000000000000001",
            "1000000000000000000000000000000000001",
            "1111111111111111111111111111111111111",
            "1000001000000000001000001000000000000",
            "1000001000000000001000001000000000000",
            "1001001001111111001001001001111001111",
            "1001001000000001000001000000001000001",
            "1001001000000001000001000000001000001",
            "1001001111001111111111111111111111001",
            "1001000001001000000000000000001000001",
            "1001000001001000000000000000001000001",
            "1001111001001001111111111111001001001",
            "1001000001001000001000001000000001001",
            "1001000001001000001000001000000001001",
            "1001001111001111001001001111111111001",
            "1001000000000001000001000001001000001",
            "1001000000000001000001000001001000001",
            "1001111111111001111111001001001001111",
            "1000001000001001000001001001001000001",
            "1000001000001001000001001001001000001",
            "1111111001001111001001111001001111001",
            "1000000001000000001000000001000000001",
            "1000000001000000001000000001000000001",
            "1001111111111111111111111111111111111"
        },
    };
    
    static readonly int[][][] Stations = {
        new[] { new[] {7, 11}, new[] {10, 5} },
        new[] { new[] {7, 14}, new[] {16, 14}, new[] {19, 11} },
        new[] { new[] {1, 8}, new[] {10, 14}, new[] {22, 11}, new[] {28, 14} },
        new[] { new[] {10, 23}, new[] {22, 23}, new[] {19, 14}, new[] {19, 11}, new[] {31, 11} },
    };

    BoxCollider2D exitBlocker;
    int next = 1, total, W, H;

    void Start() { Build(); }

 
    [ContextMenu("Build maze now")]
    public void BuildNow()
    {
        Build();
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(gameObject);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
#endif
    }

    [ContextMenu("Clear maze")]
    public void Clear()
    {
        var old = transform.Find("MazeGenerated");
        if (old != null) DestroyImmediate(old.gameObject);
    }

    void Build()
    {
        Clear();
        var root = new GameObject("MazeGenerated");
        root.transform.SetParent(transform); root.transform.position = Vector3.zero;
        next = 1;
        if (background == null) { Debug.LogError("MazeLevel: Background is empty. Drag levelN.png into it."); return; }
        if (!Mathf.Approximately(background.pixelsPerUnit, 32f))
            Debug.LogWarning("MazeLevel: " + background.name + " has Pixels Per Unit " + background.pixelsPerUnit + ". It must be 32 or the walls won't line up.");
        int i = level - 1; W = Ws[i]; H = Hs[i]; var grid = Grids[i]; mapGrid = grid; total = Stations[i].Length;

     
        var bg = new GameObject("Background"); bg.transform.SetParent(root.transform); var sr = bg.AddComponent<SpriteRenderer>();
        sr.sprite = background; sr.sortingOrder = -10;
        bg.transform.position = new Vector3(W / 2f, H / 2f, 0) - sr.sprite.bounds.center;

     
        var walls = new GameObject("Walls").transform; walls.SetParent(root.transform);
        walls.gameObject.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        walls.gameObject.AddComponent<CompositeCollider2D>().geometryType = CompositeCollider2D.GeometryType.Polygons;
        for (int r = 0; r < H; r++)
        {
            int c = 0;
            while (c < W)
            {
                if (!Blocked(grid, r, c)) { c++; continue; }
                int start = c;
                while (c < W && Blocked(grid, r, c)) c++;
                var go = new GameObject("Wall"); go.transform.SetParent(walls);
                var wc = go.AddComponent<BoxCollider2D>();
                wc.size = new Vector2(c - start, 1);
                wc.compositeOperation = Collider2D.CompositeOperation.Merge;
                go.transform.position = new Vector3(start + (c - start) / 2f, H - r - 0.5f, 0);
            }
        }

        var door = new GameObject("ExitDoor"); door.transform.SetParent(root.transform);
        door.transform.position = new Vector3(W - 0.5f, H - (MY + 1) - 1f, 0);
        exitBlocker = door.AddComponent<BoxCollider2D>(); exitBlocker.size = new Vector2(1, 2);

       
        for (int s = 0; s < total; s++)
        {
            var go = new GameObject("Station_" + (s + 1)); go.transform.SetParent(root.transform);
            go.transform.position = new Vector3(Stations[i][s][0] + 1f, H - Stations[i][s][1] - 1f, 0);
            var bc = go.AddComponent<BoxCollider2D>(); bc.isTrigger = true; bc.size = new Vector2(2, 2);
            var t = go.AddComponent<MazeStation>(); t.owner = this; t.order = s + 1; t.kind = s % 5;
        }

      
        GameObject p = myPlayer;
        if (p == null)
        {
            p = new GameObject("Player"); p.transform.SetParent(root.transform);
            p.AddComponent<SpriteRenderer>().sprite = idleDown;
            p.GetComponent<SpriteRenderer>().sortingOrder = 10;
            p.AddComponent<MazePlayer>().Setup(this);
        }
        p.tag = "Player";
        var rb = p.GetComponent<Rigidbody2D>(); if (rb == null) rb = p.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0; rb.freezeRotation = true;
        if (p.GetComponent<Collider2D>() == null) p.AddComponent<BoxCollider2D>().size = new Vector2(0.7f, 0.7f);
        p.transform.position = new Vector3(2f, 1.5f, 0);  

      
        var cam = Camera.main;
        if (cam == null) cam = FindFirstObjectByType<Camera>();
        if (cam == null) { cam = new GameObject("Main Camera").AddComponent<Camera>(); cam.tag = "MainCamera"; }
        cam.orthographic = true; cam.orthographicSize = H / 2f;
        cam.clearFlags = CameraClearFlags.SolidColor; cam.backgroundColor = new Color(0.03f, 0.04f, 0.04f);
        cam.cullingMask = ~0; cam.nearClipPlane = 0.1f; cam.farClipPlane = 100f;
        cam.transform.position = new Vector3(W / 2f, H / 2f, -10);
        cam.transform.rotation = Quaternion.identity;
        Debug.Log("MazeLevel built: level " + level + ", " + W + "x" + H + " tiles, sprite " + background.name + " " + background.rect.width + "x" + background.rect.height + "px, camera '" + cam.name + "' at " + cam.transform.position);
    }

    static bool Blocked(string[] g, int r, int c) { return r == g.Length - 1 || g[r][c] == '1'; }

    string[] mapGrid;
    public int MapWidth { get { return W; } }
    public int MapHeight { get { return H; } }
    public int NextOrder { get { return next; } }


    public bool IsBlocked(int col, int row)
    {
        if (mapGrid == null || col < 0 || row < 0 || col >= W || row >= H) return true;
        if (col == W - 1) return true;
        return Blocked(mapGrid, row, col);
    }

    void OnGUI()
    {
        if (!Application.isPlaying) return;
        var style = new GUIStyle(GUI.skin.label) { fontSize = 18 };
        string text = "Stations " + (next - 1) + "/" + total;
        if (next > total) text += "  -  the exit is open";
        GUI.Label(new Rect(14, 10, 400, 30), text, style);
    }

    public bool TryComplete(int order)
    {
        if (order != next) return false;
        next++; Debug.Log("Station " + order + " done");
        if (next > total) { exitBlocker.enabled = false; Debug.Log("Exit unlocked!"); }
        return true;
    }
}
