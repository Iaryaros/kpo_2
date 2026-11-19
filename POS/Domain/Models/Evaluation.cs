namespace POS.Domain.Models;

public sealed class Evaluation
{
    public Guid Id { get; }
    public Guid StudentId { get; }
    public Guid CriterionId { get; }
    public Score Score { get; }
    public DateOnly Date { get; }
    public string? Comment { get; private set; }

    public Evaluation(Guid id, Guid studentId, Guid criterionId, Score score, DateOnly date, string? comment = null)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Evaluation id cannot be empty", nameof(id));
        if (studentId == Guid.Empty)
            throw new ArgumentException("Student id cannot be empty", nameof(studentId));
        if (criterionId == Guid.Empty)
            throw new ArgumentException("Criterion id cannot be empty", nameof(criterionId));
        ArgumentNullException.ThrowIfNull(score);

        Id = id;
        StudentId = studentId;
        CriterionId = criterionId;
        Score = score;
        Date = date;
        Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();
    }

    // The only mutation; does not break invariants.
    public void UpdateComment(string? comment)
        => Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();

    public override string ToString() => $"[{Date}] {Score}" + (Comment is null ? "" : $" - {Comment}");
}