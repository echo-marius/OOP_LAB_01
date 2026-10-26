using lab_one;

Logger logger = new Logger("logger__v1");

logger.Format = "[%t] [%l] %n: %m %r";
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