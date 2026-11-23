using UnityEngine;


public class TaskPanelLevel3 : TaskPanel
{
    protected override void Setup()
    {
        mistakeLimit = 3;
        mistakeTasks = 99;
        timeLimit = 45;
        timedFromTask = 1;
    }

    protected override void BuildQuestions()
    {
        Add(frontEnd, "What does the CSS rule box-sizing: border-box do?", new[] { "Padding and border are included in the width", "Removes the border", "Makes the border thicker", "Adds margin outside the box" });
        Add(frontEnd, "What does typeof null return in JavaScript?", new[] { "object", "null", "undefined", "number" });
        Add(frontEnd, "Which method turns a JSON string into a JavaScript object?", new[] { "JSON.parse", "JSON.stringify", "JSON.object", "JSON.decode" });
        Add(frontEnd, "What does event.preventDefault() do?", new[] { "Stops the browser's default action", "Stops the event reaching parents", "Removes the event listener", "Deletes the element" });
        Add(frontEnd, "Which CSS display value creates a layout of rows and columns?", new[] { "grid", "block", "inline", "contents" });
        Add(frontEnd, "What does async / await help you handle in JavaScript?", new[] { "Asynchronous code like network requests", "Loops", "CSS animations", "Variable types" });

        Add(backEnd, "What does ACID stand for in databases?", new[] { "Atomicity, Consistency, Isolation, Durability", "Access, Control, Integrity, Data", "Atomicity, Concurrency, Indexing, Distribution", "Authorization, Consistency, Integrity, Durability" });
        Add(backEnd, "Which SQL join returns only rows that match in both tables?", new[] { "INNER JOIN", "LEFT JOIN", "FULL JOIN", "CROSS JOIN" });
        Add(backEnd, "Which status code means the client is not authenticated?", new[] { "401", "403", "404", "418" });
        Add(backEnd, "What is a database index used for?", new[] { "Speeding up searches on a column", "Backing up the table", "Encrypting rows", "Joining tables" });
        Add(backEnd, "Which HTTP method is idempotent and replaces a whole resource?", new[] { "PUT", "POST", "PATCH", "CONNECT" });
        Add(backEnd, "What does CORS control?", new[] { "Which origins a browser may request from", "Database caching", "Password hashing", "Server load balancing" });
    }

    protected override Question MakeMath()
    {
        int type = UnityEngine.Random.Range(0, 3);
        string text;
        int answer;

        if (type == 0)
        {
            int n = UnityEngine.Random.Range(33, 64);
            text = "What is the binary number " + System.Convert.ToString(n, 2) + " in decimal?";
            answer = n;
        }
        else if (type == 1)
        {
            int n = UnityEngine.Random.Range(100, 500);
            int m = UnityEngine.Random.Range(7, 20);
            if (n % m == 0) n++;
            text = "What is " + n + " mod " + m + "?";
            answer = n % m;
        }
        else
        {
            int x = UnityEngine.Random.Range(2, 16);
            int a = UnityEngine.Random.Range(2, 10);
            int b = UnityEngine.Random.Range(1, 10);
            text = "Solve for x:  " + a + "(x + " + b + ") = " + (a * (x + b));
            answer = x;
        }

        return BuildMath(text, answer, new[] { -10, -4, -2, -1, 1, 2, 4, 10 });
    }
}
