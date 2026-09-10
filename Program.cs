using StudentDatabaseTUI.sdb;

var studentManager = new StudentManager("students.json");
var i18n = new I18n("lang", "en_US");

Dictionary<string, (string description, Action action)> BuildMenuItems()
{
    return new Dictionary<string, (string description, Action action)>
    {
        { "1", (i18n.Get("menu.viewAllStudents"), ViewAllStudents) },
        { "2", (i18n.Get("menu.searchByName"), SearchByName) },
        { "3", (i18n.Get("menu.searchById"), SearchById) },
        { "4", (i18n.Get("menu.addStudent"), AddStudent) },
        { "5", (i18n.Get("menu.updateStudent"), UpdateStudent) },
        { "6", (i18n.Get("menu.deleteStudent"), DeleteStudent) },
        { "l", ("Choose Language", ChooseLanguage) },
        { "q", (i18n.Get("menu.exit"), () => {}) }
    };
}

var menuItems = BuildMenuItems();

while (true)
{
    string choice = PrintMenu(menuItems);

    if (menuItems.ContainsKey(choice))
    {
        if (choice == "q")
        {
            Console.WriteLine(i18n.Get("messages.goodbye"));
            break;
        }
        menuItems[choice].action();
        menuItems = BuildMenuItems();
    }
    else
    {
        Console.WriteLine(i18n.Get("messages.invalidOption"));
    }
}

string PrintMenu(Dictionary<string, (string description, Action action)> items)
{
    var longestLine = items.Max(item => $"{item.Key}. {item.Value.description}".Length);
    var decorationLine = new string('=', longestLine);

    Console.WriteLine($"\n{decorationLine}");
    foreach (var item in items)
    {
        Console.WriteLine($"{item.Key}. {item.Value.description}");
    }
    Console.WriteLine(decorationLine);
    Console.Write(i18n.Get("prompts.chooseOption"));

    return Console.ReadLine() ?? "";
}

void ViewAllStudents()
{
    var students = studentManager.GetAllStudents();
    if (students.Count == 0)
    {
        Console.WriteLine(i18n.Get("messages.noStudentsFound"));
        return;
    }

    Console.WriteLine($"\n--- {i18n.Get("messages.allStudents")} ---");
    foreach (var student in students)
    {
        Console.WriteLine($"ID: {student.Id}, Name: {student.Name}");
    }
}

void SearchByName()
{
    Console.Write(i18n.Get("prompts.enterStudentName"));
    string name = Console.ReadLine() ?? "";

    var found = studentManager.FindByName(name);
    if (found != null)
    {
        Console.WriteLine($"{i18n.Get("messages.found")} - ID: {found.Id}, Name: {found.Name}");
    }
    else
    {
        Console.WriteLine(i18n.Get("messages.studentNotFound"));
    }
}

void SearchById()
{
    Console.Write(i18n.Get("prompts.enterStudentId"));
    string id = Console.ReadLine() ?? "";
    if (!string.IsNullOrWhiteSpace(id))
    {
        var found = studentManager.FindById(id);
        if (found != null)
        {
            Console.WriteLine($"{i18n.Get("messages.found")} - ID: {found.Id}, Name: {found.Name}");
        }
        else
        {
            Console.WriteLine(i18n.Get("messages.studentNotFound"));
        }
    }
    else
    {
        Console.WriteLine(i18n.Get("messages.invalidId"));
    }
}

void AddStudent()
{
    Console.Write(i18n.Get("prompts.enterStudentName"));
    string name = Console.ReadLine() ?? "";

    Console.Write(i18n.Get("prompts.enterStudentId"));
    string id = Console.ReadLine() ?? "";
    if (!string.IsNullOrWhiteSpace(id))
    {
        studentManager.AddStudent(name, id);
        Console.WriteLine(i18n.Get("messages.studentAddedSuccessfully"));
    }
    else
    {
        Console.WriteLine(i18n.Get("messages.invalidId"));
    }
}

void UpdateStudent()
{
    Console.Write(i18n.Get("prompts.enterIdToUpdate"));
    string id = Console.ReadLine() ?? "";
    if (!string.IsNullOrWhiteSpace(id))
    {
        var student = studentManager.FindById(id);
        if (student != null)
        {
            Console.Write(i18n.Get("prompts.enterNewName"));
            string newName = Console.ReadLine() ?? "";
            studentManager.UpdateStudent(id, newName);
            Console.WriteLine(i18n.Get("messages.studentUpdatedSuccessfully"));
        }
        else
        {
            Console.WriteLine(i18n.Get("messages.studentNotFound"));
        }
    }
    else
    {
        Console.WriteLine(i18n.Get("messages.invalidId"));
    }
}

void DeleteStudent()
{
    Console.Write(i18n.Get("prompts.enterIdToDelete"));
    string id = Console.ReadLine() ?? "";
    if (!string.IsNullOrWhiteSpace(id))
    {
        if (studentManager.FindById(id) != null)
        {
            studentManager.DeleteStudent(id);
            Console.WriteLine(i18n.Get("messages.studentDeletedSuccessfully"));
        }
        else
        {
            Console.WriteLine(i18n.Get("messages.studentNotFound"));
        }
    }
    else
    {
        Console.WriteLine(i18n.Get("messages.invalidId"));
    }
}

void ChooseLanguage()
{
    var languages = i18n.GetAvailableLanguages();
    var languageMenuItems = new Dictionary<string, (string description, Action action)>();

    for (int i = 0; i < languages.Count; i++)
    {
        string langId = languages[i];
        string langName = i18n.GetLanguageName(langId);
        languageMenuItems[(i + 1).ToString()] = (langName, () =>
        {
            i18n.SetLanguage(langId);
            Console.WriteLine(i18n.Get("messages.languageChanged") + i18n.GetLanguageName(langId));
        }
        );
    }

    string choice = PrintMenu(languageMenuItems);

    if (languageMenuItems.ContainsKey(choice))
    {
        languageMenuItems[choice].action();
    }
    else
    {
        Console.WriteLine(i18n.Get("messages.invalidLanguage"));
    }
}

