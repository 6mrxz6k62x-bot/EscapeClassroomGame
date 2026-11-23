using UnityEngine;
using TMPro;

public class TaskpanelLvl2 : TaskPanel
{
    
     protected override void Setup()
    {
        mistakeLimit = 2;
        mistakeTasks = 2;
        timeLimit = 60;
        timedFromTask = 3;
    }

    protected override void BuildQuestions()
    {
        Add(frontEnd, "Which CSS selector targets the element with the id header?", new[] { "#header", ".header", "header", "*header" });
        Add(frontEnd, "Which JavaScript array method adds an item to the end?", new[] { "push", "pop", "shift", "concat" });
        Add(frontEnd, "What does the CSS property z-index control?", new[] { "Stacking order of elements", "Zoom level", "Font size", "Element width" });
        Add(frontEnd, "Which HTML attribute gives an image alternative text?", new[] { "alt", "title", "src", "name" });
        Add(frontEnd, "Which position value keeps an element fixed relative to the window?", new[] { "fixed", "static", "relative", "inherit" });
        Add(frontEnd, "What does === check in JavaScript?", new[] { "Value and type are equal", "Only the value", "Only the type", "Whether both are objects" });

        Add(backEnd, "Which HTTP method is normally used to delete a resource?", new[] { "DELETE", "REMOVE", "PUT", "PATCH" });
        Add(backEnd, "Which SQL keyword sorts the result of a query?", new[] { "ORDER BY", "SORT", "GROUP BY", "ARRANGE" });
        Add(backEnd, "What does JSON stand for?", new[] { "JavaScript Object Notation", "Java Standard Object Naming", "JavaScript Online Network", "Joined Server Object Node" });
        Add(backEnd, "Which status code means a resource was created successfully?", new[] { "201", "200", "204", "301" });
        Add(backEnd, "What does a primary key do in a table?", new[] { "Uniquely identifies each row", "Encrypts the table", "Sorts the table", "Links to another database" });
        Add(backEnd, "Which of these is a relational database?", new[] { "PostgreSQL", "MongoDB", "Redis", "Elasticsearch" });
    }

    protected override Question MakeMath()
    {
        int type = UnityEngine.Random.Range(0, 3);
        string text;
        int answer;

        if (type == 0)
        {
            int[] percents = { 10, 15, 20, 25, 30, 40 };
            int p = percents[UnityEngine.Random.Range(0, percents.Length)];
            int baseNumber = UnityEngine.Random.Range(2, 21) * 20;
            text = "What is " + p + "% of " + baseNumber + "?";
            answer = p * baseNumber / 100;
        }
        else if (type == 1)
        {
            int n = UnityEngine.Random.Range(11, 26);
            text = "What is " + n + " squared?";
            answer = n * n;
        }
        else
        {
            int x = UnityEngine.Random.Range(3, 15);
            int a = UnityEngine.Random.Range(3, 9);
            int b = UnityEngine.Random.Range(2, 20);
            text = "Solve for x:  " + a + "x - " + b + " = " + (a * x - b);
            answer = x;
        }

        return BuildMath(text, answer, new[] { -10, -5, -2, -1, 1, 2, 5, 10 });
    }
}
