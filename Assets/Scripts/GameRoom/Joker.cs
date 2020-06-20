using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class Joker
{
    /*
         jokers:[{
        playerId: mongoose.Types.ObjectId,
        questionId: mongoose.Types.ObjectId,
        jtype:Number,
    }],
     */
    [JsonProperty("pId")]
    public string pId { get; set; }
    [JsonProperty("qId")]
    public string qId { get; set; }
    [JsonProperty("jtype")]
    public int jtype { get; set; }

    public Joker()
    {
        this.pId = "";
        this.qId = "";
        this.jtype = 0;
    }

    public Joker(string pId, string qId, int jtype)
    {
        this.pId = pId;
        this.qId = qId;
        this.jtype = jtype;
    }
}
