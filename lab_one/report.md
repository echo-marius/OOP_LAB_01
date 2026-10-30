# Laboratory Handout 1: Logger and Parentheses Validator

**Student:** Saganean Marius  
**Group:** FAF 252  
**Course:** Programming Languages (C#)

---

## Project Structure

The project contains 3 files, each with its corresponding functionality:

### **Logger**
Provides formatted console logging. It filters log entries against a global severity threshold (`Level`), generates random status strings (`%r`), and formats output according to custom template tags (`%t`, `%m`, `%n`, `%l`, `%r`).

#### `Logger.cs`
```csharp
using System.Text;

namespace lab_one;

public enum LogLevel
{
    Trace,
    Info,
    Warning,
    Error
}

public class Logger
{
    private static readonly Random _rng = new Random();
    public string Name { get; set; }
    public static string Format { get; set; }
    public static LogLevel Level { get; set; } = LogLevel.Trace;

    public Logger(string name)
    {
        Name = name;
    }

    private string GetRandomString(LogLevel level)
    {
        int index = _rng.Next(0, 3);
        switch (level)
        {
            case LogLevel.Trace:
                return index switch
                {
                    0 => "Entering execution block",
                    1 => "Trace point reached",
                    _ => "Checking local state"
                };
            case LogLevel.Info:
                return index switch
                {
                    0 => "Operation completed",
                    1 => "Everything looks good",
                    _ => "Here is your result"
                };
            case LogLevel.Warning:
                return index switch
                {
                    0 => "Resource utilization high",
                    1 => "Deprecated API method invoked",
                    _ => "Response time threshold exceeded"
                };
            case LogLevel.Error:
                return index switch
                {
                    0 => "Failed to establish connection",
                    1 => "Unhandled exception caught",
                    _ => "Database query failed"
                };
            default:
                return "Status update recorded";
        }
    }

    public void Log(LogLevel level, string message)
    {
        if (level < Level || String.IsNullOrWhiteSpace(message) ||  String.IsNullOrWhiteSpace(Format))
        {
            return;
        }

        StringBuilder builder = new StringBuilder();
        builder.Append(Format);

        builder.Replace("%t", TimeOnly.FromDateTime(DateTime.Now).ToString("HH:mm:ss"));
        builder.Replace("%m", message);
        builder.Replace("%n", this.Name);
        builder.Replace("%l", level.ToString());
        builder.Replace("%r", GetRandomString(Level));

        Console.WriteLine(builder.ToString());
    }
}
```

---

### **ParenthesesValidator**
Validates whether parentheses are valid using a simple counter algorithm (`num`), returning `true` for valid strings and `false` for invalid strings.

#### `ParenthesesValidator.cs`
```csharp
using System.Runtime.CompilerServices;

namespace lab_one;

public class ParenthesesValidator
{
    public readonly Logger logger;

    public ParenthesesValidator(Logger logger)
    {
        this.logger = logger;
    }

    public bool IsValid(string input)
    {
        int num = 0;

        foreach (char c in input)
        {
            if (c == '(')
            {
                num++;
                this.logger.Log(LogLevel.Trace, "new parentheses found");
            }
            else
            {
                num--;
                if (num < 0)
                {
                    this.logger.Log(LogLevel.Warning, "match error encountered");
                    this.logger.Log(LogLevel.Error, "the string is not valid");
                    return false;
                }
                this.logger.Log(LogLevel.Info, "match found");
            }
        }

        if (num == 0)
        {
            this.logger.Log(LogLevel.Info, "the string is valid");
            return true;
        }
        else
        {
            this.logger.Log(LogLevel.Warning, "match error encountered");
            this.logger.Log(LogLevel.Error, "the string is not valid");
            return false;
        }
    }
}
```

---

### **How They Collaborate**

- **Dependency Injection:** `ParenthesesValidator` accepts a `Logger` instance via its constructor and stores it in an instance field.
- **Instrumentation:** As `ParenthesesValidator` iterates through characters, it calls `logger.Log(...)` to record execution trace telemetry (`Trace`, `Info`, `Warning`, `Error`).

#### `Program.cs`
```csharp
using lab_one;

Logger logger = new Logger("logger__v1");
Logger.Format = "[%t] [%l] %n: %m %r";
Logger.Level = LogLevel.Trace;

ParenthesesValidator parenthesesValidator = new ParenthesesValidator(logger);

Console.WriteLine("test 1 -> input = \"()()()()\"");
parenthesesValidator.IsValid("()()()()");

Console.WriteLine("test 2 -> input = \"())\"");
parenthesesValidator.IsValid("())");

Console.WriteLine("test 3 -> input = \"(()\"");
parenthesesValidator.IsValid("(()");

Console.WriteLine("test 4 -> input = \"((\"");
parenthesesValidator.IsValid("((");
```

---

## 1. Object-Oriented Principles

### **What is a class in your implementation?**
A class is a blueprint that defines data fields, properties, and behaviors. In this code, **`Logger`** and **`ParenthesesValidator`** are classes:
- **`Logger`** encapsulates logging properties (`Name`, `Format`), a static threshold (`Level`), and the `Log()` method.
- **`ParenthesesValidator`** encapsulates validation logic (`IsValid()`) and a reference to its dependent `Logger`.

### **Example of an object created from a class**
An object is an active instance of a class instantiated in memory using the `new` keyword:
```csharp
Logger logger = new Logger("logger__v1");
ParenthesesValidator parenthesesValidator = new ParenthesesValidator(logger);
```

### **What state does each object store?**
#### **Logger Instance State**
- **`Name`** (`string`): The name assigned to the logger instance (e.g., `"logger__v1"`).

#### **ParenthesesValidator Instance State**
- **`logger`** (`Logger`): A reference to the specific `Logger` instance injected during instantiation.

### **Which members of Logger are static, and why?**
The following members are declared with the `static` keyword:
- **`_rng` (`private static readonly Random`):** Marked static to share a single pseudorandom number generator across all logger instances. Re-instantiating `Random` repeatedly per instance can lead to duplicate seed values and predictable output sequences.
- **`Level` (`public static LogLevel`):** Marked static to act as a global severity threshold across the entire application runtime.
- **`Format` (`public static string Format`):** Marked static to define a unified template format for all loggers across the application.

### **Which member belongs to an individual logger instance?**
The instance-specific members belong to individual objects allocated on the heap:
- **`Name` (`public string`):** Identifies a specific logger instance (e.g., `"logger__v1"`).
- **`Log(LogLevel, string)` & `GetRandomString(LogLevel)`:** Instance methods that process log events using instance-specific context.

### **What happens when the logging level is changed? Does it affect every logger instance? Explain why.**
Changing `Logger.Level` instantly affects **every** `Logger` instance in the application. Because `Level` is attached to the class type (`Logger`) rather than individual object instances, all instances read from the same memory location when filtering log output in `Log()`.

---

## 2. Methods and Responsibilities

### **What is the responsibility of the Log method?**
The `Log` method handles log message filtering, string formatting, dynamic token replacement, and console output. Its responsibilities are:
- **Filtering:** Ignores messages whose severity is below the global `Logger.Level` or if the message is null/whitespace.
- **Template Processing:** Replaces string placeholders (`%t` for time, `%m` for message, `%n` for instance name, `%l` for severity level, `%r` for random status) using a `StringBuilder`.

### **Why does ParenthesesValidator use a logger instead of implementing logging itself?**
This design adheres to the **Single Responsibility Principle (SRP)** and **Separation of Concerns**:
- **`ParenthesesValidator`** is solely responsible for determining bracket sequence correctness.
- **`Logger`** is responsible for formatting and writing log output.

### **Identify one example of a method call between objects or classes in your program.**
Inside `ParenthesesValidator.IsValid()`, the validator object calls the `Log` method on its injected logger reference:
```csharp
this.logger.Log(LogLevel.Trace, "new parentheses found");
```

---

## 5. Testing and Correctness

### **Give one valid and one invalid input for the parentheses validator.**
- **Valid Input:** `"()()()()"`, where every opening parenthesis is properly closed and balanced.
- **Invalid Input:** `")("` or `"(())( "`, where parentheses are either closed before being opened or left unclosed at the end of the string.

### **Explain how the program determines whether the input is valid.**
The `ParenthesesValidator.IsValid` method uses an integer counter variable (`num`). As it iterates through each character:
1. It increments `num` by `1` when encountering `'('`.
2. It decrements `num` by `1` when encountering `')'`.
3. If `num` drops below `0` at any point, a closing parenthesis appeared without a matching opening parenthesis, causing the method to log an error and return `false` immediately.
4. After processing all characters, the string is valid if and only if `num == 0`. If `num > 0`, unclosed opening parentheses remain, and it returns `false`.

### **Describe one test that helped you find or prevent an error.**
Testing the input `")("` helped prevent a subtle error where a simple count check (only verifying `num == 0` at the end) would mistakenly mark `")("` as valid because it contains one `'('` and one `')'`. Checking `if (num < 0)` during iteration ensures that closing parentheses appearing before opening ones trigger an immediate early return of `false`.

---

## 6. Reflection

### **What was the most important OOP concept you applied in this laboratory?**
The most important OOP concept applied was **Encapsulation and Separation of Concerns**. `Logger` encapsulates all log formatting, timestamping, and threshold filtering logic, whereas `ParenthesesValidator` focuses solely on string validation logic. By passing a `Logger` instance into `ParenthesesValidator` via dependency injection, each class maintains a single responsibility.

### **If you had more time, what would you improve in your design, and why?**
1. **Interface Abstraction:** Introduce an `ILogger` interface that `Logger` implements. This would allow `ParenthesesValidator` to depend on an abstraction rather than a concrete class, making unit testing and swapping logger implementations easier.
2. **Support for Multiple Bracket Types:** Extend `ParenthesesValidator` using a `Stack<char>` instead of a single integer counter to support matching combinations of `()`, `[]`, and `{}`.
