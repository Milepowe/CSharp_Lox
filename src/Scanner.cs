using static TokenType;

public class Scanner
{
    private readonly string source;
    private readonly List<Token> tokens = new List<Token>();

    private int start = 0;
    private int current = 0;
    private int line = 1;

    public Scanner(string source)
    {
        this.source = source;
    }

    public List<Token> ScanTokens()
    {
        while (!IsAtEnd())
        {
            start = current;
            ScanToken();
        }

        tokens.Add(new Token(TokenType.EOF, "", null, line));
        return tokens;
    }

    private void ScanToken()
    {
        char c = Advance();
        switch (c)
        {
            case '(':
                AddToken(LEFT_PAREN);
                break;
            case ')':
                AddToken(RIGHT_PAREN);
                break;
            case '{':
                AddToken(LEFT_BRACE);
                break;
            case '}':
                AddToken(RIGHT_BRACE);
                break;
            case ',':
                AddToken(COMMA);
                break;
            case '.':
                AddToken(DOT);
                break;
            case '-':
                AddToken(MINUS);
                break;
            case '+':
                AddToken(PLUS);
                break;
            case ';':
                AddToken(SEMICOLON);
                break;
            case '*':
                if (Match('/'))
                {
                    break;
                }
                else
                {
                    AddToken(STAR);
                    break;
                }

            case '!':
                AddToken(Match('=') ? BANG_EQUAL : BANG);
                break;
            case '=':
                AddToken(Match('=') ? EQUAL_EQUAL : EQUAL);
                break;
            case '<':
                AddToken(Match('=') ? LESS_EQUAL : LESS);
                break;
            case '>':
                AddToken(Match('=') ? GREATER_EQUAL : GREATER);
                break;
            case '/':
                if (Match('/'))
                { // A comment goes until the end of the line
                    while (Peek() != '\n' && !IsAtEnd())
                        Advance();
                }
                else if (Match('*'))
                {
                    MultiComment();
                }
                else
                {
                    AddToken(TokenType.SLASH);
                }
                break;
            // ignore whitespace
            case ' ':
            case '\r':
            case '\t':
                break;

            case '\n':
                line++;
                break;

            case '"':
                GetString();
                break;

            default:
                if (IsDigit(c))
                {
                    Number();
                }
                else if (IsAlpha(c))
                {
                    Identifier();
                }
                break;
        }
    }

    private Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>
    {
        { "and", TokenType.AND },
        { "class", TokenType.CLASS },
        { "else", TokenType.ELSE },
        { "false", TokenType.FALSE },
        { "for", TokenType.FOR },
        { "fun", TokenType.FUN },
        { "if", TokenType.IF },
        { "nil", TokenType.NIL },
        { "or", TokenType.OR },
        { "print", TokenType.PRINT },
        { "return", TokenType.RETURN },
        { "super", TokenType.SUPER },
        { "this", TokenType.THIS },
        { "true", TokenType.TRUE },
        { "var", TokenType.VAR },
        { "while", TokenType.WHILE },
    };

    private void Identifier()
    {
        while (IsAlphaNumeric(Peek()))
            Advance();

        string text = source[start..current];
        if (!keywords.TryGetValue(text, out TokenType type))
        {
            type = TokenType.IDENTIFIER;
        }

        AddToken(type);
    }

    private void MultiComment()
    {
        while (Peek() != '*' && PeekNext() != '/' && !IsAtEnd())
        {
            if (Peek() == '\n')
                line++;
            Advance();
        }

        if (IsAtEnd())
        {
            Lox.Error(line, "Unterminated comment.");
        }
    }

    private void GetString()
    {
        while (Peek() != '"' && !IsAtEnd())
        {
            if (Peek() == '\n')
                line++;

            Advance();
        }

        if (IsAtEnd())
        {
            Lox.Error(line, "Unterminated string.");
            return;
        }

        // The closing
        Advance();

        string value = source[(start + 1)..(current - 1)];
        AddToken(STRING, value);
    }

    private bool Match(char expected)
    {
        if (IsAtEnd())
            return false;
        if (source[current] != expected)
            return false;

        current++;
        return true;
    }

    private char Peek()
    {
        if (IsAtEnd())
            return '\0';
        return source[current];
    }

    private char PeekNext()
    {
        if (current + 1 >= source.Length)
            return '\0';
        return source[current + 1];
    }

    private bool IsAlpha(char c)
    {
        return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_';
    }

    private bool IsAlphaNumeric(char c)
    {
        return IsAlpha(c) || IsDigit(c);
    }

    private void Number()
    {
        while (IsDigit(Peek()))
            Advance();

        // Look for a fractional part.
        if (Peek() == '.' && IsDigit(PeekNext()))
        {
            // Consume the .
            Advance();

            while (IsDigit(Peek()))
                Advance();
        }

        AddToken(NUMBER, Double.Parse(source[start..current]));
    }

    private bool IsDigit(char c)
    {
        return c >= '0' && c <= '9';
    }

    private bool IsAtEnd()
    {
        return current >= source.Length;
    }

    private char Advance()
    {
        current++;
        return source[current - 1];
    }

    private void AddToken(TokenType type)
    {
        AddToken(type, null);
    }

    private void AddToken(TokenType type, object literal)
    {
        string text = source[start..current];
        tokens.Add(new Token(type, text, literal, line));
    }
}
