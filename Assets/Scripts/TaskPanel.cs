using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class TaskPanel : MonoBehaviour
{
    public static TaskPanel Instance;
    public static bool IsOpen;
    public static bool IsDead;

    public GameObject window;
    public TMP_Text titleText;
    public TMP_Text questionText;
    public TMP_Text feedbackText;
    public TMP_Text infoText;          
    public Button[] optionButtons;

    public Image categoryIcon;
    public Sprite iconFrontEnd;
    public Sprite iconBackEnd;
    public Sprite iconMath;
    public Sprite correctSprite;

   
    protected int mistakeLimit = 0;     
    protected int mistakeTasks = 0;    
    protected float timeLimit = 0;     
    protected int timedFromTask = 1;   

    protected class Question
    {
        public string text;
        public string[] options;
        public int correct;
    }

    protected List<Question> frontEnd = new List<Question>();
    protected List<Question> backEnd = new List<Question>();

    Sprite[] normalSprites;
    Question current;
    Action onSolved;
    int correctButton;
    bool busy;
    bool limited;
    bool timed;
    float timeLeft;
    int mistakes;
    string deathReason = "";
    GUIStyle bigStyle;
    GUIStyle midStyle;

    void Awake()
    {
        Instance = this;
        IsOpen = false;
        IsDead = false;
        Time.timeScale = 1;
        window.SetActive(false);

        normalSprites = new Sprite[optionButtons.Length];
        for (int i = 0; i < optionButtons.Length; i++)
            normalSprites[i] = optionButtons[i].image.sprite;

        for (int i = 0; i < optionButtons.Length; i++)
        {
            int index = i;
            optionButtons[i].onClick.AddListener(() => Pick(index));
        }

        Setup();
        BuildQuestions();
    }

    void Update()
    {
        Keyboard kb = Keyboard.current;

        if (IsDead)
        {
            if (kb != null && kb.rKey.wasPressedThisFrame)
            {
                Time.timeScale = 1;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            return;
        }

        if (!IsOpen || busy)
            return;

        if (timed)
        {
            timeLeft -= Time.unscaledDeltaTime;
            if (timeLeft <= 0)
            {
                Die("Time ran out");
                return;
            }
        }

        UpdateInfo();

      
        if (!timed && kb != null && kb.escapeKey.wasPressedThisFrame)
            Close();
    }

    public void Open(int kind, int order, Action solved)
    {
        onSolved = solved;
        limited = mistakeLimit > 0 && order <= mistakeTasks;
        timed = timeLimit > 0 && order >= timedFromTask;
        timeLeft = timeLimit;

        int category = kind % 3;

        if (category == 0)
        {
            titleText.text = "Front-end challenge";
            if (categoryIcon != null) categoryIcon.sprite = iconFrontEnd;
            current = frontEnd[UnityEngine.Random.Range(0, frontEnd.Count)];
        }
        else if (category == 1)
        {
            titleText.text = "Back-end challenge";
            if (categoryIcon != null) categoryIcon.sprite = iconBackEnd;
            current = backEnd[UnityEngine.Random.Range(0, backEnd.Count)];
        }
        else
        {
            titleText.text = "Math challenge";
            if (categoryIcon != null) categoryIcon.sprite = iconMath;
            current = MakeMath();
        }

        questionText.text = current.text;
        feedbackText.text = "";

      
        List<int> order2 = new List<int>();
        for (int i = 0; i < current.options.Length; i++)
            order2.Add(i);
        for (int i = order2.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            int temp = order2[i];
            order2[i] = order2[j];
            order2[j] = temp;
        }

        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (i < order2.Count)
            {
                optionButtons[i].gameObject.SetActive(true);
                optionButtons[i].interactable = true;
                optionButtons[i].image.sprite = normalSprites[i];
                optionButtons[i].GetComponentInChildren<TMP_Text>().text = current.options[order2[i]];
                if (order2[i] == current.correct)
                    correctButton = i;
            }
            else
            {
                optionButtons[i].gameObject.SetActive(false);
            }
        }

        window.SetActive(true);
        IsOpen = true;
        busy = false;
        Time.timeScale = 0;
        UpdateInfo();
    }

    void Pick(int index)
    {
        if (busy || IsDead)
            return;

        if (index == correctButton)
        {
            busy = true;
            feedbackText.text = "Correct!";
            if (correctSprite != null) optionButtons[index].image.sprite = correctSprite;
            StartCoroutine(CloseAndFinish());
            return;
        }

        optionButtons[index].interactable = false;

        if (limited)
        {
            mistakes++;
            if (mistakes > mistakeLimit)
            {
                Die("Too many mistakes");
                return;
            }
            feedbackText.text = "Wrong! Mistakes left: " + (mistakeLimit - mistakes);
        }
        else
        {
            feedbackText.text = "Wrong, try again";
        }

        UpdateInfo();
    }

    public void KillTask(string reason)
    {
        if (IsDead)
            return;

        Die(reason);
    }
    
    void UpdateInfo()
    {
        if (infoText == null)
            return;

        string text = "";
        if (timed)
            text += "Time: " + Mathf.CeilToInt(Mathf.Max(0, timeLeft));
        if (limited)
        {
            if (text != "") text += "     ";
            text += "Mistakes left: " + Mathf.Max(0, mistakeLimit - mistakes);
        }
        infoText.text = text;
    }

    IEnumerator CloseAndFinish()
    {
        yield return new WaitForSecondsRealtime(0.7f);
        Close();
        if (onSolved != null)
            onSolved();
    }

    void Close()
    {
        window.SetActive(false);
        IsOpen = false;
        busy = false;
        Time.timeScale = 1;
    }

    void Die(string reason)
    {
        StopAllCoroutines();
        deathReason = reason;
        IsDead = true;
        IsOpen = false;
        window.SetActive(false);
        Time.timeScale = 0;
    }

    void OnGUI()
    {
        if (!IsDead)
            return;

        if (bigStyle == null)
        {
            bigStyle = new GUIStyle(GUI.skin.label) { fontSize = 64, alignment = TextAnchor.MiddleCenter };
            bigStyle.normal.textColor = new Color(0.8f, 0.15f, 0.15f);
            midStyle = new GUIStyle(GUI.skin.label) { fontSize = 24, alignment = TextAnchor.MiddleCenter };
            midStyle.normal.textColor = Color.white;
        }

        GUI.color = new Color(0, 0, 0, 0.85f);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = Color.white;

        GUI.Label(new Rect(0, Screen.height / 2 - 100, Screen.width, 90), "YOU DIED", bigStyle);
        GUI.Label(new Rect(0, Screen.height / 2 - 10, Screen.width, 40), deathReason, midStyle);
        GUI.Label(new Rect(0, Screen.height / 2 + 40, Screen.width, 40), "Press R to try again", midStyle);
    }

 

    protected virtual void Setup()
    {
    }

    protected void Add(List<Question> list, string text, string[] options)
    {
       
        Question q = new Question();
        q.text = text;
        q.options = options;
        q.correct = 0;
        list.Add(q);
    }

    protected virtual void BuildQuestions()
    {
        Add(frontEnd, "Which CSS property changes the color of text?", new[] { "color", "font-color", "text-color", "foreground" });
        Add(frontEnd, "Which HTML element creates a hyperlink?", new[] { "a", "link", "href", "url" });
        Add(frontEnd, "Which flexbox property can stack items vertically when set to column?", new[] { "flex-direction", "flex-wrap", "align-items", "justify-content" });
        Add(frontEnd, "Which JavaScript keyword declares a variable that cannot be reassigned?", new[] { "const", "let", "var", "static" });
        Add(frontEnd, "Which HTML element holds the visible content of a page?", new[] { "body", "head", "title", "meta" });
        Add(frontEnd, "Which CSS unit is relative to the width of the browser window?", new[] { "vw", "px", "pt", "em" });

        Add(backEnd, "Which HTTP status code means a page was not found?", new[] { "404", "200", "500", "301" });
        Add(backEnd, "Which HTTP method is normally used to create a new resource?", new[] { "POST", "GET", "DELETE", "HEAD" });
        Add(backEnd, "Which SQL keyword filters the rows of a query?", new[] { "WHERE", "ORDER BY", "GROUP", "JOIN" });
        Add(backEnd, "What does API stand for?", new[] { "Application Programming Interface", "Applied Program Integration", "Automatic Page Index", "Application Process Input" });
        Add(backEnd, "Which status code means an internal server error?", new[] { "500", "403", "204", "302" });
        Add(backEnd, "Which SQL statement adds a new row to a table?", new[] { "INSERT", "SELECT", "UPDATE", "ALTER" });
    }

 
    protected Question BuildMath(string text, int answer, int[] offsets)
    {
        List<int> wrong = new List<int>();
        while (wrong.Count < 3)
        {
            int w = answer + offsets[UnityEngine.Random.Range(0, offsets.Length)];
            if (w >= 0 && w != answer && !wrong.Contains(w))
                wrong.Add(w);
        }

        Question q = new Question();
        q.text = text;
        q.options = new[] { answer.ToString(), wrong[0].ToString(), wrong[1].ToString(), wrong[2].ToString() };
        q.correct = 0;
        return q;
    }

    protected virtual Question MakeMath()
    {
        int type = UnityEngine.Random.Range(0, 3);
        string text;
        int answer;

        if (type == 0)
        {
            int a = UnityEngine.Random.Range(12, 60);
            int b = UnityEngine.Random.Range(12, 60);
            text = "What is " + a + " + " + b + "?";
            answer = a + b;
        }
        else if (type == 1)
        {
            int a = UnityEngine.Random.Range(3, 13);
            int b = UnityEngine.Random.Range(3, 13);
            text = "What is " + a + " x " + b + "?";
            answer = a * b;
        }
        else
        {
            int x = UnityEngine.Random.Range(2, 12);
            int a = UnityEngine.Random.Range(2, 6);
            int b = UnityEngine.Random.Range(1, 15);
            text = "Solve for x:  " + a + "x + " + b + " = " + (a * x + b);
            answer = x;
        }

        return BuildMath(text, answer, new[] { -10, -3, -2, -1, 1, 2, 3, 10 });
    }
}
