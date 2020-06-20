using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class GameRoom 
{
    [JsonProperty("gtype")]
    public string gtype { get; set; }
    [JsonProperty("answers")]
    public Answer[] answers { get; set; }
    [JsonProperty("jokers")]
    public Joker[] jokers { get; set; }
    [JsonProperty("players")]
    public Player[] players { get; set; }
    [JsonProperty("questions")]
    public Question[] questions { get; set; }
    [JsonProperty("isEnd")]
    public bool isEnd { get; set; }
    [JsonProperty("isBot")]
    public bool isBot { get; set; }
    [JsonProperty("created")]
    public DateTime created; //date created

    public GameRoom(string gtype, Answer[] answers, Joker[] jokers, Player[] players, Question[] questions, bool isEnd, bool isBot, DateTime created)
    {
        this.gtype = gtype;
        this.answers = answers;
        this.jokers = jokers;
        this.players = players;
        this.questions = questions;
        this.isEnd = isEnd;
        this.isBot = isBot;
        this.created = created;
    }


    /*
     const gameSchema = new Schema({
    gameType: {
        type: String,
        required: true,
        trim: true
    },

    player:[
        {
        playerId: mongoose.Types.ObjectId,
        userName: {type: String},
        profilePhoto: {type: String},
        playerScore: {type: Number, default: 0},
        dogruSayisi: {type: Number, default: 0}
        }
    ],
    isGameEnded: {
        type: Boolean,
        default: false
    },
    tour: {
        type: Number,
        default: 1
    },
    isBotGame: {
        type: Number,
        default: 0
    },
    lastQuestion:{
        type:mongoose.Types.ObjectId,
    },
    dateCreated: {
        type: Date,
        default: new Date(),
    },
});
     */
}