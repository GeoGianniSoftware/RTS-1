using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public List<Player> players = new List<Player>();
    public SelectableObject homeBuilding;
    Player humanPlayer;
    public ResourcePanel resourcePanel;

    // Start is called before the first frame update
    void Start()
    {

        players.Add(new Player("Nature Passive"));
        players.Add(new Player("Natre Hostile"));
        Player p = new Player(true);
        players.Add(p);
        humanPlayer = p;


    }

    // Update is called once per frame
    void Update()
    {
        if(humanPlayer != null) {
            resourcePanel = FindObjectOfType<ResourcePanel>();

            if(resourcePanel.woodText != null)
                resourcePanel.woodText.text = "Wood:\n" + humanPlayer.woodCount;
            if (resourcePanel.stoneText != null)
                resourcePanel.stoneText.text = "Stone:\n" + humanPlayer.stoneCount;

        }
    }

    public int GetPlayerIDByPlayer(Player playerToGetID) {
        if (players.Contains(playerToGetID)) {
            return players.IndexOf(playerToGetID);
        }
        return -1;
    }

    public Player GetPlayerByID(int playerID) {
        if (players[playerID]!= null) {
            return players[playerID];
        }
        return null;
    }

    public void GivePlayerResource(int player, Item resourceItem, int amount) {
        string s = "Giving Player " + player + " " + amount + " ";
        if(resourceItem == ItemManager.GetItemByID(0)) {
            players[player].woodCount += amount;
            s += "wood.";
        }
        if (resourceItem == ItemManager.GetItemByID(1)) {
            players[player].stoneCount += amount;
            s += "stone.";
        }
        print(s);
    }
}
public class Player
{
    public string playerName;
    public bool isHuman;
    public int stoneCount;
    public int woodCount;

    public Player(string name) {
        playerName = name;
        isHuman = false;
        stoneCount = 500;
        woodCount = 500;
    }
    public Player(bool ishuman) {
        playerName = "You";
        isHuman = ishuman;
        stoneCount = 500;
        woodCount = 500;
    }
}
