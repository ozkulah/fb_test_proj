using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User 
{
    public string userName;
    public int userScore;
    public string userId;
    public string userEmail;

    public User()
    {
        userName = PlayerScores.playerName;
        userScore = PlayerScores.playerScore;
    }

}
