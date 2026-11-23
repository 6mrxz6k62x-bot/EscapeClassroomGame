using UnityEngine;

// Level 4: hardest questions.
// Every task is timed (30 seconds) and you can make only 2 mistakes in total, the third kills you.
public class TaskPanelLevel4 : TaskPanel
{
    protected override void Setup()
    {
        mistakeLimit = 2;
        mistakeTasks = 99;
        timeLimit = 30;
        timedFromTask = 1;
    }

    protected override void BuildQuestions()
    {
        Add(frontEnd, "What does console.log(0.1 + 0.2 === 0.3) print in JavaScript?", new[] { "false", "true", "undefined", "NaN" });
        Add(frontEnd, "What is a closure in JavaScript?", new[] { "A function that remembers variables from its outer scope", "A function that ends a loop", "A way to close the browser", "An object with no methods" });
        Add(frontEnd, "Ignoring !important, which CSS rule wins the cascade?", new[] { "An inline style", "A class selector", "An element selector", "The universal selector" });
        Add(frontEnd, "What does the virtual DOM in React mainly help with?", new[] { "Updating only the parts of the page that changed", "Storing data on the server", "Compiling CSS", "Encrypting requests" });
        Add(frontEnd, "What is this inside a plain function called on its own in strict mode?", new[] { "undefined", "window", "The function itself", "null" });
        Add(frontEnd, "What does the JavaScript event loop do?", new[] { "Runs queued callbacks when the call stack is empty", "Repeats CSS animations", "Reloads the page", "Compiles code" });

        Add(backEnd, "What problem does database sharding solve?", new[] { "Splitting data across servers to scale", "Encrypting data", "Removing duplicates", "Compressing indexes" });
        Add(backEnd, "Which HTTP header usually carries a JWT?", new[] { "Authorization", "Content-Type", "Accept", "Host" });
        Add(backEnd, "What is the lookup time in a balanced binary search tree?", new[] { "O(log n)", "O(1)", "O(n)", "O(n log n)" });
        Add(backEnd, "Which SQL clause filters the groups made by GROUP BY?", new[] { "HAVING", "WHERE", "FILTER", "LIMIT" });
        Add(backEnd, "What does the CAP theorem say about distributed systems?", new[] { "You can't guarantee consistency, availability and partition tolerance together", "Caches are always faster", "Databases need three replicas", "APIs must be stateless" });
        Add(backEnd, "What is a race condition?", new[] { "A bug where the result depends on the timing of concurrent operations", "A very fast API", "A database lock", "A failed login" });
    }

    protected override Question MakeMath()
    {
        int type = UnityEngine.Random.Range(0, 4);
        string text;
        int answer;

        if (type == 0)
        {
            int a = UnityEngine.Random.Range(101, 200);
            int b = UnityEngine.Random.Range(12, 30);
            text = "What is " + a + " x " + b + "?";
            answer = a * b;
        }
        else if (type == 1)
        {
            int n = UnityEngine.Random.Range(20, 51);
            text = "What is the sum of all whole numbers from 1 to " + n + "?";
            answer = n * (n + 1) / 2;
        }
        else if (type == 2)
        {
            int x = UnityEngine.Random.Range(10, 40);
            int y = UnityEngine.Random.Range(2, 10);
            text = "x + y = " + (x + y) + " and x - y = " + (x - y) + ".  What is x times y?";
            answer = x * y;
        }
        else
        {
            int[] prices = { 200, 400, 600, 800 };
            int p = prices[UnityEngine.Random.Range(0, prices.Length)];
            text = "A price of " + p + " is reduced by 25%, then by 20%. What is the final price?";
            answer = p * 60 / 100;
        }

        return BuildMath(text, answer, new[] { -100, -20, -10, -1, 1, 10, 20, 100 });
    }
}
