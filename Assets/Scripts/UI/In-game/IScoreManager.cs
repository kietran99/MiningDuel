using UnityEngine;
public interface IScoreManager
{
    int GetCurrentScore();
    void DecreaseScore(int score);
    void IncreaseScore(int score);
}