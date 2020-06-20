using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class Answer 
{
    //[JsonProperty("pId")]
    //public string pId { get; set; }
    [JsonProperty("qId")]
    public string qId { get; set; }
    [JsonProperty("ans")]
    public string ans { get; set; }
    [JsonProperty("isTrue")]
    public bool isTrue { get; set; }

    public Answer()
    {
        //this.pId = "";
        this.qId = "";
        this.ans = "";
        this.isTrue = false;
    }

    public Answer(string pId, string qId, string ans, bool isTrue)
    {
        //this.pId = pId;
        this.qId = qId;
        this.ans = ans;
        this.isTrue = isTrue;
    }
}
