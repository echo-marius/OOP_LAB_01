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