using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class Question 
{
    [JsonProperty("queId")]
    public string queId { get; set; }
    [JsonProperty("que")]
    public string que { get; set; } //question content
    [JsonProperty("qAns1")]
    public string qAns1 { get; set; }
    [JsonProperty("qAns2")]
    public string qAns2 { get; set; }
    [JsonProperty("qAns3")]
    public string qAns3 { get; set; }
    [JsonProperty("qAns4")]
    public string qAns4 { get; set; }
    [JsonProperty("qTrueAns")]
    public string qTrueAns { get; set; }
    [JsonProperty("qcat")]
    public string qcat { get; set; }

    public Question(string queId, string que, string qAns1, string qAns2, string qAns3, string qAns4, string qTrueAns, string qcat)
    {
        this.queId = queId;
        this.que = que;
        this.qAns1 = qAns1;
        this.qAns2 = qAns2;
        this.qAns3 = qAns3;
        this.qAns4 = qAns4;
        this.qTrueAns = qTrueAns;
        this.qcat = qcat;
    }
}
