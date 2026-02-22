using UnityEngine;

public interface IPuzzle
{
    string PuzzleId { get; }

    public enum PuzzleState
    {
        Unsolved,
        Solved,
        InProgress
    }

    PuzzleState State { get; }

    void ResetPuzzle();
}
