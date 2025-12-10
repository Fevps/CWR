using HarmonyLib;

using Photon.Pun;
using System.Linq;
using UnityEngine;

using static CWR.Entry;

namespace CWR.Features
{
    public class Networking
    {
        public static bool includeSelf = false;

        // Session

        public static void AddMoney(float amount)
        {
            if (roomStats == null) return;

            roomStats.AddMoney((int)(amount * 100));
        }

        public static void RemoveMoney(float amount)
        {
            if (roomStats == null) return;

            roomStats.RemoveMoney((int)(amount * 100));
        }

        public static void AddMeta(float amount) =>
            MetaProgressionHandler.AddMetaCoins((int)amount * 100);

        public static void RemoveMeta(float amount) =>
            MetaProgressionHandler.RemoveMetaCoins((int)amount * 100);

        public static void AddNeededViews(float amount)
        {
            if (roomStats == null) return;

            Traverse.Create(roomStats).Field("m_quotaToReachInternal").SetValue(roomStats.QuotaToReach + (int)(amount * 1000));
        }

        public static void AddCurrentViews(float amount)
        {
            if (roomStats == null) return;

            roomStats.AddQuota((int)(amount * 100));
        }

        public static void PreviousDay()
        {
            if (roomStats == null) return;

            int newDay = roomStats.CurrentDay == 0 ? 0 : roomStats.CurrentDay - 1;
            roomStats.SetCurrentDay(newDay);
        }

        public static void NextDay()
        {
            if (roomStats == null) return;

            roomStats.NextDay();
        }

        public static void UnlockAllUpgrades()
        {
            IslandUnlocks unlocks = GameObject.FindObjectOfType<IslandUnlocks>();
            if (unlocks == null) return;

            PhotonView view = unlocks.GetComponent<PhotonView>();
            if (view == null) return;

            for (int i = 0; i < 9; i++)
                view.RPC("RPCA_Unlock", RpcTarget.All, new object[] { i });
        }

        // Network

        public static void SpawnPlayer()
        {
            if (localPlayer == null) return;

            CheatProperties.Instantiate("Player", localPlayer.data.groundPos + new Vector3(1, .1f, 1), Quaternion.identity);
        }

        public static void KickAll()
        {
            if (players == null) return;

            foreach (global::Player player in players)
            {
                if (!includeSelf && player.IsLocal)
                    continue;

                PlayerHandler.instance.RemovePlayer(player);
            }
        }

        public static void KillAll()
        {
            if (players == null) return;

            foreach (global::Player player in players)
                if (!player.data.dead)
                {
                    if (!includeSelf && player.IsLocal)
                        continue;

                    player.refs.view.RPC("RPCA_PlayerDie", RpcTarget.All);
                }
        }

        public static void BombPlayers()
        {
            if (players == null || items == null) return;

            foreach (global::Player player in players)
                if (!player.data.dead)
                {
                    if (!includeSelf && player.IsLocal)
                        continue;

                    CheatProperties.SpawnItem(CheatProperties.GetItemByName("Bomb"), "Bomb", player.data.groundPos, 5);
                }
        }

        public static void ReviveAll()
        {
            if (players == null) return;

            foreach (global::Player player in players)
            {
                if (!includeSelf && player.IsLocal)
                    continue;

                player.CallRevive();
            }
        }

        public static void TrapAll(int amount = 3, bool goop = false)
        {
            if (players == null) return;

            foreach (global::Player player in players)
            {
                if (!includeSelf && player.IsLocal)
                    continue;

                for (int i = 0; i < amount; i++)
                    CheatProperties.Instantiate(goop ? "ExplodedGoop" : "Web", player.data.groundPos, Quaternion.identity);
            }
        }

        public static void EquipRandomHatAll()
        {
            if (HatDatabase.instance.hats.Length == 0 || players == null) return;

            foreach (global::Player player in players)
            {
                if (!includeSelf && player.IsLocal)
                    continue;

                int random = Random.Range(0, HatDatabase.instance.hats.Length);
                MetaProgressionHandler.EquipHat(random);
            }
        }

        public static void EveryoneJump()
        {
            if (players == null) return;

            foreach (global::Player player in players)
            {
                if (!includeSelf && player.IsLocal)
                    continue;

                player.refs.view.RPC("RPCA_Jump", RpcTarget.All);
            }
        }

        public static void EveryoneHighJump()
        {
            if (playerControllers == null) return;

            foreach (PlayerController controller in playerControllers)
            {
                if (controller == localPlayer.refs.controller)
                    continue;

                controller.jumpImpulse = 100;
            }
        }

        public static void SuperSpeedAll(bool enable)
        {
            if (playerControllers == null) return;

            foreach (PlayerController controller in playerControllers)
            {
                if (!includeSelf && controller == localPlayer.refs.controller)
                    continue;

                controller.movementForce = enable ? 25 : 10;
            }
        }

        public static void FlingPlayers() // Failed grab players
        {
            if (players == null || localPlayer == null) return;

            int[] types = new int[5];

            for (int i = 0; i < 5; i++) // First 5 parts
                types[i] = i;

            foreach (global::Player player in players)
            {
                if (!includeSelf && player.IsLocal)
                    continue;

                Vector3[] forces = new Vector3[types.Length];

                for (int i = 0; i < types.Length; i++)
                    forces[i] = (localPlayer.transform.position - player.transform.position).normalized * 10;

                player.refs.view.RPC("RPCA_AddForceToBodyParts", RpcTarget.All, new object[]
                {
                    types,
                    forces
                });
            }
        }

        public static void ShadowRealmPlayers()
        {
            if (players == null) return;

            foreach (global::Player player in players)
            {
                if (!includeSelf && player.IsLocal)
                    continue;

                ShadowRealmHandler.instance.TeleportPlayerToRandomRealm(player);
            }
        }

        public static void RechargeCameras()
        {
            if (cameras == null) return;

            foreach (VideoCamera camera in cameras)
            {
                VideoInfoEntry entry = Entries.GetFilmEntry(camera);
                if (entry == null) return;
                
                entry.timeLeft = 100;
            }
        }

        public static void GodModeAll()
        {
            if (players == null) return;

            foreach (global::Player player in players)
            {
                if (!includeSelf && player.IsLocal)
                    continue;

                if (player.data.dead)
                    player.refs.view.RPC("RPCA_PlayerRevive", RpcTarget.All);

                if (player.data.health < 100)
                    player.CallHeal(100 - player.data.health);
            }
        }

        public static void InfiniteOxygenAll()
        {
            if (players == null) return;

            foreach (global::Player player in players)
            {
                if (!includeSelf && player.IsLocal)
                    continue;

                player.data.usingOxygen = false;
            }
        }

        public static void InfiniteStaminaAll()
        {
            if (players == null) return;

            foreach (global::Player player in players)
            {
                if (!includeSelf && player.IsLocal)
                    continue;

                if (player.data.currentStamina < 10)
                    player.data.currentStamina = 10;
            }
        }

        public static void HealAll()
        {
            if (players == null) return;

            foreach (global::Player player in players)
            {
                if (!includeSelf && player.IsLocal)
                    continue;

                if (player.data.health <= 75)
                    player.CallHeal(25);
            }
        }

        public static void HurtAll()
        {
            if (players == null) return;

            foreach (global::Player player in players)
            {
                if (!includeSelf && player.IsLocal)
                    continue;

                player.CallTakeDamage(25);
            }
        }

        public static void GrabItems(Vector3 pos)
        {
            if (itemInstances == null || localPlayer == null) return;

            foreach (ItemInstance item in itemInstances)
                item.transform.position = pos;
        }

        public static void ToggleState(bool surface)
        {
            DivingBell bell = GameObject.FindObjectOfType<DivingBell>();

            if (bell == null) return;

            string target = CheatProperties.SurfaceScene() && !surface ? "RPC_GoToUnderground" : "RPC_GoToSurface";
            bell.photonView.RPC(target, RpcTarget.All);
        }

        public static void ToggleBellDoor()
        {
            DivingBell bell = GameObject.FindObjectOfType<DivingBell>();

            if (bell == null) return;

            bell.AttemptSetOpen(!bell.opened);
        }

        public static void ToggleBellLock()
        {
            DivingBell bell = GameObject.FindObjectOfType<DivingBell>();
            if (bell == null) return;

            bell.locked = !bell.locked;
        }
    }
}