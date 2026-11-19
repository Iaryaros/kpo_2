namespace POS.Domain.Models;

public sealed class Student
{
    public Guid Id { get; }
    public FullName Name { get; }
    public string Group { get; }

    public Student(Guid id, FullName name, string group)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Student id cannot be empty", nameof(id));
        ArgumentNullException.ThrowIfNull(name);
        if (string.IsNullOrWhiteSpace(group))
            throw new ArgumentException("Group cannot be empty", nameof(group));

        Id = id;
        Name = name;
        Group = group.Trim();
    }

    public static Student Create(FullName name, string group) => new(Guid.NewGuid(), name, group);

    public override string ToString() => $"{Name} ({Group}) [{Id}]";
}