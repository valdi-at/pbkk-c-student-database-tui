namespace StudentDatabaseTUI.sdb;

using System.Text.Json;
using StudentDatabaseTUI.sdb;

class StudentManager
{
    private readonly string filePath;
    private List<Student> students = new();

    public StudentManager(string filePath)
    {
        this.filePath = filePath;
        LoadStudents();
    }

    private void LoadStudents()
    {
        if (File.Exists(filePath))
        {
            var json = File.ReadAllText(filePath);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            students = JsonSerializer.Deserialize<List<Student>>(json, options) ?? new List<Student>();
        }
        else
        {
            students = new List<Student>();
        }
    }

    private void SaveStudents()
    {
        var json = JsonSerializer.Serialize(students, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }

    public List<Student> GetAllStudents()
    {
        return students;
    }

    public Student? FindByName(string name)
    {
        return students.FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    public Student? FindById(string id)
    {
        return students.FirstOrDefault(s => s.Id == id);
    }

    public void AddStudent(string name, string id)
    {
        students.Add(new Student { Name = name, Id = id });
        SaveStudents();
    }

    public void UpdateStudent(string id, string newName)
    {
        var student = FindById(id);
        if (student != null)
        {
            student.Name = newName;
            SaveStudents();
        }
    }

    public void DeleteStudent(string id)
    {
        var student = FindById(id);
        if (student != null)
        {
            students.Remove(student);
            SaveStudents();
        }
    }
}
