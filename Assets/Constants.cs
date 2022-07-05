using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public static class ScenesName
    {
        public const string Game = "Game";
        
        public const string MainMenu = "MainMenu";
        public const string GameMain = "GameMain";
    }

    public static class SteamLobbyDataNames
    {
        public const string HostAddress = "HostAddress";
        public const string LobbyName = "LobbyName";
    }

    public static class Lobby
    {
        public const string LocalGamePlayer = "LocalGamePlayer";
    }

    public static class KeyBindings
    {
        public static KeyCode PlayerList = KeyCode.Tab;
    }
}
