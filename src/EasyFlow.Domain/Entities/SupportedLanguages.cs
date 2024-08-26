namespace EasyFlow.Domain.Entities;

public sealed record SupportedLanguage(string Name, string Code)
{
    public static readonly SupportedLanguage English = new("English", "en-US");
    public static readonly SupportedLanguage Portuguese = new("Portuguese", "pt-BR");

    public static SupportedLanguage FromCode(string code) => code switch
    {
        "en-US" => English,
        "pt-BR" => Portuguese,
        _ => throw new NotSupportedException($"Language code '{code}' is not supported.")
    };

    public static SupportedLanguage FromName(string name) => name switch
    {
        "English" => English,
        "Portuguese" => Portuguese,
        _ => throw new NotSupportedException($"Language name '{name}' is not supported.")
    };
}