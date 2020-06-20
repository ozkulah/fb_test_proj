using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MockGameRoom 
{
    public static GameRoom createMockGameRoom()
    {
        string queId = "5ec094c1e4ced51a47b088a2";
        string que = "Mardin ve çevresinde yaşayan medeniyet hangisidir?";
        string qAns1 = "Hititler";
        string qAns2 = "Etiler";
        string qAns3 = "Artuklular";
        string qAns4 = "İyonlar";
        string qTrueAns = "Artuklular";
        string qcat = "tarih";
        GameRoom gameRoom = new GameRoom("solo", new Answer[12], new Joker[6], new Player[2], new Question[12], false, false, DateTime.Now);
        for (int i = 0; i < 12; i=i+2)
        {
            Question a1 = new Question(queId, que, qAns1, qAns2, qAns3, qAns4, qTrueAns, qcat);
            Question a2 = new Question("5ec094c1e4ced51a47b088a1", "Kayısı ile ünlü ilimiz?", "Malatya", "İzmir", "Elazığ", "Kayseri", "Malatya", "coğrafya");
            Answer an1 = new Answer();
            
            gameRoom.questions[i] = a1;
            gameRoom.questions[i+1] = a2;
            gameRoom.answers[i] = null;
            gameRoom.answers[i+1] = null;
        }
        Player player = new Player(Configuration.CurrentUser.userId, Configuration.CurrentUser.userName, "");
        gameRoom.players[0] = player;
        return gameRoom; 
    }
}
