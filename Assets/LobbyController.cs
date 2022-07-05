using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using TMPro;
using System.Linq;

public class LobbyController : MonoBehaviour
{
    public static LobbyController Instance;

    public TMP_Text LobbyNameText;

    public GameObject PlayerListViewContent;
    public GameObject PlayerListItemPrefab;
    public GameObject LocalPlayerObject;

    public ulong CurrentLobbyID;
    public bool PlayerItemCreated = false;
    private List<PlayerListItem> PlayerListItems = new List<PlayerListItem>();
    public PlayerObjectController LocalPlayerController;

    private CustomNetworkManager manager;

    private CustomNetworkManager Manager
    {
        get
        {
            if (manager != null)
                return manager;
            return manager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void UpdateLobbyName()
    {
        CurrentLobbyID = Manager.GetComponent<SteamLobby>().CurrentLobbyID;
        LobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(CurrentLobbyID), Constants.SteamLobbyDataNames.LobbyName);
    }

    public void UpdatePlayerList()
    {
        if (!PlayerItemCreated)
        {
            CreateHostPlayerItem();
        }

        if (PlayerListItems.Count < Manager.GamePlayers.Count)
        {
            CreateClientPlayerItem();
        }

        if (PlayerListItems.Count > Manager.GamePlayers.Count)
        {
            RemovePlayerItem();
        }

        if (PlayerListItems.Count == Manager.GamePlayers.Count)
        {
            UpdatePlayerItem();
        }
    }

    public void FindLocalPlayer()
    {
        LocalPlayerObject = GameObject.Find(Constants.Lobby.LocalGamePlayer);
        LocalPlayerController = LocalPlayerObject.GetComponent<PlayerObjectController>();
    }

    public void CreateHostPlayerItem()
    {
        foreach (var player in Manager.GamePlayers)
        {
            CreatePlayerItem(player);
        }

        PlayerItemCreated = true;
    }

    public void CreateClientPlayerItem()
    {
        foreach (var player in Manager.GamePlayers)
        {
            if (!PlayerListItems.Any(item => item.ConnectionId == player.ConnectionID))
            {
                CreatePlayerItem(player);
            }
        }
    }

    public void UpdatePlayerItem()
    {
        foreach (var player in Manager.GamePlayers)
        {
            foreach (var playerListItemScript in PlayerListItems)
            {
                if (playerListItemScript.ConnectionId == player.ConnectionID)
                {
                    playerListItemScript.PlayerName = player.PlayerName;
                    playerListItemScript.SetPlayerValues();
                }
            }
        }
    }

    public void RemovePlayerItem()
    {
        List<PlayerListItem> playerListItemsToRemove = new List<PlayerListItem>();

        foreach (var playerListItem in PlayerListItems)
        {
            if (!Manager.GamePlayers.Any(item => item.ConnectionID == playerListItem.ConnectionId))
            {
                playerListItemsToRemove.Add(playerListItem);
            }
        }

        if (playerListItemsToRemove.Count > 0)
        {
            foreach (var playerListItemToRemove in playerListItemsToRemove)
            {
                GameObject objectToRemove = playerListItemToRemove.gameObject;
                PlayerListItems.Remove(playerListItemToRemove);
                Destroy(objectToRemove);
                objectToRemove = null;
            }
        }
    }

    private void CreatePlayerItem(PlayerObjectController player)
    {
        GameObject newPlayerItem = Instantiate(PlayerListItemPrefab);
        PlayerListItem newPlayerItemScript = newPlayerItem.GetComponent<PlayerListItem>();

        newPlayerItemScript.PlayerName = player.PlayerName;
        newPlayerItemScript.ConnectionId = player.ConnectionID;
        newPlayerItemScript.PlayerSteamID = player.PlayerSteamID;
        newPlayerItemScript.SetPlayerValues();

        newPlayerItem.transform.SetParent(PlayerListViewContent.transform);
        newPlayerItem.transform.localScale = Vector3.one;

        PlayerListItems.Add(newPlayerItemScript);
    }
}
