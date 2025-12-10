using MelonLoader;
using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine;

using static CWR.Entry;

namespace CWR.Features
{
    public class Player // try catch so doesn't load before instantiation
    {
        public static void GrabCamera() // fix this
        {
            if (itemInstances == null || localPlayer == null) return;

            foreach (ItemInstance item in itemInstances)
            {
                Item camera = item.item;
                if (camera == null) return;

                if (camera.name == "Camera1(Clone)")
                    Item.EquipItem(camera);
            }
        }

        public static void Glide(bool enable)
        {
            if (localPlayer == null) return;

            localPlayer.refs.ragdoll.enabled = !enable;
        }

        public static void GodMode()
        {
            if (localPlayer == null) return;
            
            if (localPlayer.data.dead)
                localPlayer.refs.view.RPC("RPCA_PlayerRevive", RpcTarget.All);

            if (localPlayer.data.health < 100)
                localPlayer.CallHeal(100 - localPlayer.data.health);
        }

        public static void InfiniteOxygen()
        {
            if (localPlayer == null) return;

            localPlayer.data.usingOxygen = false;

            /*
            if (localPlayer.data.usingOxygen &&
                localPlayer.data.remainingOxygen < 500)
                localPlayer.data.remainingOxygen = 500;
            */
        }

        public static void InfiniteStamina()
        {
            if (localPlayer == null) return;

            if (localPlayer.data.currentStamina < 10)
                localPlayer.data.currentStamina = 10;
        }

        public static void Suicide()
        {
            if (localPlayer == null || localPlayer.data.dead) return;

            localPlayer.refs.view.RPC("RPCA_PlayerDie", RpcTarget.All, Array.Empty<object>());
        }

        public static void Revive()
        {
            if (localPlayer == null || !localPlayer.data.dead) return;

            localPlayer.refs.view.RPC("RPCA_PlayerRevive", RpcTarget.All, Array.Empty<object>());
        }

        public static void UnlockAllHats() =>
            MetaProgressionHandler.UnlockAllHats();

        public static void LockAllHats() =>
            MetaProgressionHandler.ClearAllUnlockedHatsHats();

        public static void EquipRandomHat()
        {
            if (HatDatabase.instance.hats.Length == 0 || localPlayer == null) return;

            int random = UnityEngine.Random.Range(0, HatDatabase.instance.hats.Length);
            MetaProgressionHandler.EquipHat(random);
        }

        public static void TeleportToBell()
        {
            DivingBell bell = GameObject.FindObjectOfType<DivingBell>();

            if (bell == null || localPlayer == null) return;

            localPlayer.data.groundPos = bell.transform.position;
        }

        public static void SuperThrow()
        {
            
        }

        public static void SuperSpeed(bool enable)
        {
            if (localPlayer == null) return;

            localPlayer.refs.controller.movementForce = enable ? 25 : 10;
        }

        public static void SetGravity(float gravity)
        {
            if (localPlayer == null) return;

            localPlayer.refs.controller.gravity = gravity;
        }

        private void Something()
        {
            if (localPlayer == null) return;

            PlayerController playerController = localPlayer.refs.controller;
            PlayerRagdoll playerRagdoll = localPlayer.refs.ragdoll;
            //PlayerAnimRefHandler playerAnimRefHandler = new PlayerAnimRefHandler();
            //PlayerAnimationHandler playerAnimationHandler = new PlayerAnimationHandler();
            PlayerItems playerItems = localPlayer.refs.items;
            //PlayerSyncer playerSyncer = new PlayerSyncer();
            PlayerInventory playerInventory = new PlayerInventory(); //
            PlayerContentProvider playerContentProvider = new PlayerContentProvider();
            PlayerVisor playerVisor = localPlayer.refs.visor;
            PlayerDataSounds playerDataSounds = new PlayerDataSounds();
            HandReference handReference = new HandReference();
            PlayerEmotes playerEmotes = new PlayerEmotes();
        }
    }
}