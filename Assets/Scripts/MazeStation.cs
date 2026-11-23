using UnityEngine;
using UnityEngine.InputSystem;

public class MazeStation : MonoBehaviour
{
    public MazeLevel owner;
    public int order;
    public int kind;

    static readonly string[] categories = { "Front-end", "Back-end", "Math" };

    bool inside;
    bool done;
    GUIStyle titleStyle;
    GUIStyle smallStyle;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            inside = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            inside = false;
    }

    void Update()
    {
        if (!inside || done || TaskPanel.IsOpen || order != owner.NextOrder)
            return;

        Keyboard kb = Keyboard.current;
        if (kb == null || !kb.eKey.wasPressedThisFrame)
            return;

        if (TaskPanel.Instance == null)
        {
            Debug.LogError("No TaskPanel in the scene. Add the TaskPanel script to your task Canvas.");
            return;
        }

        TaskPanel.Instance.Open(kind, order, OnSolved);
    }

    void OnSolved()
    {
        done = owner.TryComplete(order);
    }

    void OnGUI()
    {
        if (!inside || done || TaskPanel.IsOpen)
            return;

        if (titleStyle == null)
        {
            titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 20, alignment = TextAnchor.MiddleCenter };
            smallStyle = new GUIStyle(GUI.skin.label) { fontSize = 14, alignment = TextAnchor.MiddleCenter };
        }

        Rect box = new Rect(Screen.width / 2 - 200, Screen.height - 130, 400, 90);
        GUI.Box(box, "");

        if (order != owner.NextOrder)
        {
            GUI.Label(new Rect(box.x, box.y + 12, box.width, 30), "This one is locked", titleStyle);
            GUI.Label(new Rect(box.x, box.y + 48, box.width, 24), "Finish station " + owner.NextOrder + " first", smallStyle);
            return;
        }

        GUI.Label(new Rect(box.x, box.y + 12, box.width, 30), "Station " + order + ": " + categories[kind % 3], titleStyle);
        GUI.Label(new Rect(box.x, box.y + 48, box.width, 24), "Press E to start the task", smallStyle);
    }
}
