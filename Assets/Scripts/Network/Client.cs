using BoM.Core;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BoM.Network {
	// Data
	public class ClientData : ScriptableObject {
		public string AccountId;
		[SerializeField] protected Teams.Manager teamManager;
	}

	// Logic
	[CreateAssetMenu(fileName = "Client", menuName = "BoM/Client", order = 50)]
	public class Client : ClientData {
		public void Start() {
			SceneManager.sceneLoaded += SceneLoaded;
			SceneManager.LoadScene("Arena", LoadSceneMode.Additive);
			NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.UTF8.GetBytes(AccountId);
			NetworkManager.Singleton.StartClient();
			Listen();
		}

		public void Listen() {
			NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(CustomMessage.ServerPosition, ServerPosition);
		}

		public void SceneLoaded(Scene scene, LoadSceneMode mode) {
			SceneManager.SetActiveScene(scene);
			//teamManager.FindSpawns();
		}

		public void ServerPosition(ulong senderClientId, FastBufferReader reader) {
			reader.ReadValueSafe(out ulong clientId);
			reader.ReadValueSafe(out Vector3 position);
			reader.ReadValueSafe(out Vector3 direction);

			var player = PlayerManager.GetByClientId(clientId);

			if(player == null) {
				Debug.LogWarning($"Received position for non-existing player {clientId}");
				return;
			}

			player.RemotePosition = position;
			player.RemoteDirection = direction;
		}
	}
}
