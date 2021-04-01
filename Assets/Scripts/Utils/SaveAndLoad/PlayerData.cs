[System.Serializable]
public class PlayerData 
{
    public string Name =>name;
    public int TotalWin => totalWin;
    public int[,] MatchScores => matchScore;
    string name;
    int totalWin;
    int[,] matchScore;
    void AnotherWinMatch()
    {
        totalWin+=1;
    }
    public PlayerData(string name)
    {
        this.name = name;
        totalWin = 0;
        matchScore = new int[2,10];
    }
    public void AddToMatchHistory(bool Win,int playerScore)
    {
        int status;
        if(Win) 
        {
            status = 1;
            AnotherWinMatch();
        }
        else status = -1;
        int i;
        for(i = 0; i < 10; i++)
        {
            if(matchScore[0,i] == 0)
            {
                matchScore[0,i] = status;
                matchScore[1,i] = playerScore;
                return;
            }
        }
        for(i = 0; i < 10 ;i++)
        {
            if(i == 9)
            {
                matchScore[0,i] = status;
                matchScore[1,i] = playerScore;
            }
            matchScore[0,i] = matchScore[0,i+1];
            matchScore[1,i] = matchScore[1,i+1];
        }
    }
}
