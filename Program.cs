using StudentDatabaseTUI.sdb;
var studentManager = new StudentManager("students.json");

Dictionary<string, (string description, Action action)> BuildMenuItems()
{
    return new Dictionary<string, (string description, Action action)>
    {
        { "1", ("View All Students", ViewAllStudents) },
        { "2", ("Search by Name", SearchByName) },
        { "3", ("Search by ID", SearchById) },
        { "4", ("Add Student", AddStudent) },
        { "5", ("Update Student", UpdateStudent) },
        { "6", ("Delete Student", DeleteStudent) },
        { "q", ("Exit", () => {}) }
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
            Console.WriteLine("Goodbye!");
            break;
        }
        menuItems[choice].action();
        menuItems = BuildMenuItems();
    }
    else
    {
        Console.WriteLine("Invalid option. Try again.");
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
    Console.Write("Choose an option: ");

    return Console.ReadLine() ?? "";
}

void ViewAllStudents()
{
    var students = studentManager.GetAllStudents();
    if (students.Count == 0)
    {
        Console.WriteLine("No Students found.");
        return;
    }

    Console.WriteLine("\n--- All Students ---");
    foreach (var student in students)
    {
        Console.WriteLine($"ID: {student.Id}, Name: {student.Name}");
    }
}

void SearchByName()
{
    Console.Write("Enter Student Name: ");
    string name = Console.ReadLine() ?? "";

    var found = studentManager.FindByName(name);
    if (found != null)
    {
        Console.WriteLine($"Found - ID: {found.Id}, Name: {found.Name}");
    }
    else
    {
        Console.WriteLine("Student not found.");
    }
}

void SearchById()
{
    Console.Write("Enter student ID: ");
    string id = Console.ReadLine() ?? "";
    if (!string.IsNullOrWhiteSpace(id))
    {
        var found = studentManager.FindById(id);
        if (found != null)
        {
            Console.WriteLine($"Found - ID: {found.Id}, Name: {found.Name}");
        }
        else
        {
            Console.WriteLine("Student not found.");
        }
    }
    else
    {
        Console.WriteLine("Invalid ID.");
    }
}

void AddStudent()
{
    Console.Write("Enter student name: ");
    string name = Console.ReadLine() ?? "";

    Console.Write("Enter student ID: ");
    string id = Console.ReadLine() ?? "";
    if (!string.IsNullOrWhiteSpace(id))
    {
        studentManager.AddStudent(name, id);
        Console.WriteLine("Student added successfully.");
    }
    else
    {
        Console.WriteLine("Invalid ID.");
    }
}

void UpdateStudent()
{
    Console.Write("Enter student ID to update: ");
    string id = Console.ReadLine() ?? "";
    if (!string.IsNullOrWhiteSpace(id))
    {
        var student = studentManager.FindById(id);
        if (student != null)
        {
            Console.Write("Enter new name: ");
            string newName = Console.ReadLine() ?? "";
            studentManager.UpdateStudent(id, newName);
            Console.WriteLine("Student updated successfully.");
        }
        else
        {
            Console.WriteLine("Student not found.");
        }
    }
    else
    {
        Console.WriteLine("Invalid ID.");
    }
}

void DeleteStudent()
{
    Console.Write("Enter ID to delete: ");
    string id = Console.ReadLine() ?? "";
    if (!string.IsNullOrWhiteSpace(id))
    {
        if (studentManager.FindById(id) != null)
        {
            studentManager.DeleteStudent(id);
            Console.WriteLine("Student deleted successfully.");
        }
        else
        {
            Console.WriteLine("Student not found.");
        }
    }
    else
    {
        Console.WriteLine("Invalid ID.");
    }
}