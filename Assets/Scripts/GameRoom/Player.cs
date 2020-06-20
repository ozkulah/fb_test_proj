using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class Player 
{
    /*
     player:[
        {
        playerId: mongoose.Types.ObjectId,
        userName: {type: String},
        profilePhoto: {type: String},
        playerScore: {type: Number, default: 0},
        dogruSayisi: {type: Number, default: 0}
        }
    ],
     */
    [JsonProperty("pId")]
    public string pId { get; set; }
    [JsonProperty("uName")]
    public string uName { get; set; }//user name
    [JsonProperty("pp")]
    public string pp { get; set; }//profile photo
    [JsonProperty("pSc")]
    public int pSc { get; set; }//player score
    [JsonProperty("count")]
    public int count { get; set; }//right count

    public Player(string pId, string uName, string pp)
    {
        this.pId = pId;
        this.uName = uName;
        this.pp = pp;
        this.pSc = 0;
        this.count = 0;
    }
}
