using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int score = 0;
    private readonly int coinValue = 500;

    private void Start()
    {
        Lander.Instance.OnCoinCollected += Lander_OnCoinCollected;
        Lander.Instance.OnLanded += Lander_OnLanded;
    }

    private void Lander_OnCoinCollected(object sender, System.EventArgs e)
    {
        AddScore(coinValue);
    }

    private void Lander_OnLanded(object sender, Lander.OnLandedEventArgs e)
    {
        AddScore(e.onLandedScore);
    }

    private int AddScore(int value)
    {
        score += value;
        Debug.Log(score);
        return score;
    }
}
