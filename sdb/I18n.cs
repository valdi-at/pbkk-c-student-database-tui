namespace StudentDatabaseTUI.sdb;

using System.Text.Json;

class I18n
{
    private readonly string langDir;
    private string currentLanguage = "";
    private Dictionary<string, Dictionary<string, object>> allLanguages = new();
    private Dictionary<string, object> currentTranslations = new();
    private Dictionary<string, string> languageNames = new();

    public I18n(string langDirectory = "lang", string defaultLanguage = "en_us")
    {
        langDir = langDirectory;
        LoadAllLanguages();
        SetLanguage(defaultLanguage);
    }

    private void LoadAllLanguages()
    {
        if (!Directory.Exists(langDir))
        {
            Console.WriteLine($"Language directory not found: {langDir}");
            return;
        }

        var jsonFiles = Directory.GetFiles(langDir, "*.json");

        foreach (var filePath in jsonFiles)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string json = File.ReadAllText(filePath);
            using JsonDocument doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("name", out var nameElement))
            {
                languageNames[fileName] = nameElement.GetString() ?? fileName;
            }

            if (doc.RootElement.TryGetProperty("translation", out var translationElement))
            {
                var languageTranslations = new Dictionary<string, object>();
                foreach (var property in translationElement.EnumerateObject())
                {
                    languageTranslations[property.Name] = property.Value.Clone();
                }
                allLanguages[fileName] = languageTranslations;
            }
        }
    }

    public string Get(string key)
    {
        string[] parts = key.Split('.');
        object current = currentTranslations;

        foreach (var part in parts)
        {
            if (current is JsonElement element)
            {
                if (element.TryGetProperty(part, out var next))
                {
                    current = next;
                }
                else
                {
                    return key;
                }
            }
            else if (current is Dictionary<string, object> dict)
            {
                if (dict.TryGetValue(part, out var next))
                {
                    current = next;
                }
                else
                {
                    return key;
                }
            }
            else
            {
                return key;
            }
        }

        if (current is JsonElement elem)
        {
            return elem.GetString() ?? key;
        }

        return key;
    }

    public void SetLanguage(string language)
    {
        if (allLanguages.ContainsKey(language))
        {
            currentLanguage = language;
            currentTranslations = allLanguages[language];
        }
        else
        {
            Console.WriteLine($"Language not found: {language}");
        }
    }

    public string GetLanguage()
    {
        return currentLanguage;
    }

    public string GetLanguageName(string languageId)
    {
        return languageNames.TryGetValue(languageId, out var name) ? name : languageId;
    }

    public List<string> GetAvailableLanguages()
    {
        return allLanguages.Keys.ToList();
    }
}
