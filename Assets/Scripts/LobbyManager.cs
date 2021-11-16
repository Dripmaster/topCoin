using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

    public class LobbyManager : MonoBehaviourPunCallbacks
    {
        public GameObject TopPanel;
        public GameObject StartGameButton;

        public GameObject PlayerListEntryPrefab;

        private Dictionary<int, GameObject> playerListEntries;

        public static LobbyManager instance;
        #region Photon Callbacks


        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }
        public override void OnPlayerEnteredRoom(Player other)
        {
            GameObject entry = Instantiate(PlayerListEntryPrefab);
            entry.transform.SetParent(TopPanel.transform);
            entry.transform.localScale = Vector3.one;
            entry.GetComponent<PlayerEntry>().Initialize(other.ActorNumber, other.NickName);

            playerListEntries.Add(other.ActorNumber, entry);

            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);
            playerListEntries.Remove(otherPlayer.ActorNumber);

            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }

        public void JoinedRoom()
        {
            // joining (or entering) a room invalidates any cached lobby room list (even if LeaveLobby was not called due to just joining a room)
            Debug.Log("OnJoined");
            if (playerListEntries == null)
            {
                playerListEntries = new Dictionary<int, GameObject>();
            }

            foreach (Player p in PhotonNetwork.PlayerList)
            {
                GameObject entry = Instantiate(PlayerListEntryPrefab);
                entry.transform.SetParent(TopPanel.transform);
                entry.transform.localScale = Vector3.one;
                entry.GetComponent<PlayerEntry>().Initialize(p.ActorNumber, p.NickName);

                object isPlayerReady;

                if (p.CustomProperties.TryGetValue(TopCoin.PLAYER_READY, out isPlayerReady))
                {
                    entry.GetComponent<PlayerEntry>().SetPlayerReady((bool)isPlayerReady);
                }
                playerListEntries.Add(p.ActorNumber, entry);
            }

            FindObjectOfType<LobbyManager>().LocalPlayerPropertiesUpdated();

            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                {TopCoin.PLAYER_LOADED_LEVEL, false}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);


        }
        public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            if (playerListEntries == null)
            {
                playerListEntries = new Dictionary<int, GameObject>();
            }

            GameObject entry;
            if (playerListEntries.TryGetValue(targetPlayer.ActorNumber, out entry))
            {
                object isPlayerReady;
                if (changedProps.TryGetValue(TopCoin.PLAYER_READY, out isPlayerReady))
                {
                    entry.GetComponent<PlayerEntry>().SetPlayerReady((bool)isPlayerReady);
                }
            }

            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }
        #endregion
        private void Awake()
        {
            JoinedRoom();
            instance = this;
        }
        private void Update()
        {
        }

        #region Public Methods
        public void LocalPlayerPropertiesUpdated()
        {
            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }

        public void OnStartGameButtonClicked()
        {
            try
            {

                PhotonNetwork.CurrentRoom.IsOpen = false;
                PhotonNetwork.CurrentRoom.IsVisible = false;
            }
            catch (System.Exception)
            {

            }

            //PhotonNetwork.LoadLevel("DemoAsteroids-GameScene"); 
            //GameManager.instance.StartStage();
            PhotonView photonView = PhotonView.Get(GameManager.instance);
            photonView.RPC("StartStage", RpcTarget.All);
            photonView.RPC("turnOn", RpcTarget.MasterClient);
    }
        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }
        #endregion

        #region Private Methods

        void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            }
            Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
            PhotonNetwork.LoadLevel("1_InGame");
        }
        private bool CheckPlayersReady()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return false;
            }

            foreach (Player p in PhotonNetwork.PlayerList)
            {
                object isPlayerReady;
                if (p.CustomProperties.TryGetValue(TopCoin.PLAYER_READY, out isPlayerReady))
                {
                    if (!(bool)isPlayerReady)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        #endregion




    }
