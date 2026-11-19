using POS.Domain.Models;

namespace POS.Domain.Services;

/// <summary>
/// GoF Strategy (behavioural) and the Domain Service for the final score.
/// The rule does not belong to a single entity (it needs both the student's
/// evaluations and the whole rubric), so it is a Domain Service. The rule is
/// swappable, and the final score is always derived from the current evaluations
/// and rubric, never stored as a separate number.
/// </summary>
public interface IFinalScoreStrategy
{
    string Name { get; }

    // Computes the final score. Throws if an evaluation references a criterion
    // that is not in the rubric (consistency invariant).
    FinalScore Calculate(IReadOnlyCollection<Evaluation> evaluations, Rubric rubric);
}