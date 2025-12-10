using CWR.Features;

using System;

using UnityEngine;

using static CWR.Entry;

namespace CWR
{
    public class Cheats
    {
        public static CheatButton[] cheats = new CheatButton[]
        {
            // Player
            new CheatButton(Tab.Player, "God Mode", Features.Player.GodMode),
            new CheatButton(Tab.Player, "Infinite Oxygen", Features.Player.InfiniteOxygen),
            new CheatButton(Tab.Player, "Infinite Stamina", Features.Player.InfiniteStamina),

            new CheatButton(Tab.Player, "Suicide", Features.Player.Suicide, toggle: false),
            new CheatButton(Tab.Player, "Revive", Features.Player.Revive, toggle: false),

            new CheatButton(Tab.Player, "Glide", () => Features.Player.Glide(true), () => Features.Player.Glide(false)),

            new CheatButton(Tab.Player, "Super Throw", Features.Player.SuperThrow),
            new CheatButton(Tab.Player, "Super Speed", () => Features.Player.SuperSpeed(true), () => Features.Player.SuperSpeed(false)),

            new CheatButton(Tab.Player, "Low Gravity", () => Features.Player.SetGravity(5), () => Features.Player.SetGravity(20)),
            new CheatButton(Tab.Player, "High Gravity", () => Features.Player.SetGravity(40), () => Features.Player.SetGravity(20)),

            // Monsters
            new CheatButton(Tab.Monsters, "Make Mobs Jump", Monsters.Jump, toggle: false),
            new CheatButton(Tab.Monsters, "Fling Mobs", Monsters.Jump),

            new CheatButton(Tab.Monsters, "Kill All Mobs", Monsters.KillAll, toggle: false),

            new CheatButton(Tab.Monsters, "Player Transform", Monsters.PlayerTransform, toggle: false),

            new CheatButton(Tab.Monsters, "Grab Mobs", () => Monsters.GrabMonsters()),

            new CheatButton(Tab.Monsters, "Freeze Mobs", Monsters.FreezeMonsters),

            new CheatButton(Tab.Monsters, "Spawn New Mob", () => Monsters.SpawnMonster(Monsters.getRandom), toggle : false),
            new CheatButton(Tab.Monsters, "Spawn Mob", () => Monsters.SpawnMonster(Monsters.getRandom, localPlayer.data.groundPos), toggle: false),

            // Visuals
            new CheatButton(Tab.Visuals, "Include Self", () => Visuals.includeSelf = true, () => Visuals.includeSelf = false),

            new CheatButton(Tab.Visuals, "Outlined Items", () => Visuals.OutlinedItems(true), () => Visuals.OutlinedItems(false)),
            new CheatButton(Tab.Visuals, "Nametags", () => Visuals.nameTags = true, () => Visuals.nameTags = false),
            new CheatButton(Tab.Visuals, "Tracers", () => Visuals.tracers = true, () => Visuals.tracers = false),
            new CheatButton(Tab.Visuals, "Boxes", () => Visuals.boxes = true, () => Visuals.boxes = false),
            new CheatButton(Tab.Visuals, "Health Bars", () => Visuals.healthBars = true, () => Visuals.healthBars = false),
            new CheatButton(Tab.Visuals, "Chams", () => Visuals.Chams(true), () => Visuals.Chams(false)),

            new CheatButton(Tab.Visuals, "Low Poly", () => Visuals.LowPoly(true), () => Visuals.LowPoly(false)),
            new CheatButton(Tab.Visuals, "Third Person", () => Visuals.ThirdPerson(true), () => Visuals.ThirdPerson(false)),
            new CheatButton(Tab.Visuals, "Fov", () => mainCamera.fieldOfView = fovSlider, value: fovSlider, toggle: false),

            // Network
            new CheatButton(Tab.Network, "Include Self", () => Networking.includeSelf = true, () => Networking.includeSelf = false),

            new CheatButton(Tab.Network, "Recharge Cameras", Networking.RechargeCameras, toggle: false),

            new CheatButton(Tab.Network, "Grab Items", () => Networking.GrabItems(localPlayer.HeadPosition() + new Vector3(0, 1, 0)), toggle: false),
            new CheatButton(Tab.Network, "Destroy Items", () => Networking.GrabItems(new Vector3(99, 99, 99)), toggle: false),

            new CheatButton(Tab.Network, "Equip Random Hat All", Networking.EquipRandomHatAll, toggle: false),
            new CheatButton(Tab.Network, "Spam Hat All", Networking.EquipRandomHatAll),

            new CheatButton(Tab.Network, "God Mode All", Networking.GodModeAll),
            new CheatButton(Tab.Network, "Infinite Oxygen All", Networking.InfiniteOxygenAll),
            new CheatButton(Tab.Network, "Infinite Stamina All", Networking.InfiniteStaminaAll),

            new CheatButton(Tab.Network, "Spawn New Player", Networking.SpawnPlayer, toggle: false),

            new CheatButton(Tab.Network, "Kick Players", Networking.KickAll, toggle: false),

            new CheatButton(Tab.Network, "Heal All", Networking.HealAll, toggle: false),
            new CheatButton(Tab.Network, "Hurt All", Networking.HurtAll, toggle: false),

            new CheatButton(Tab.Network, "Kill Players", Networking.KillAll, toggle: false),
            new CheatButton(Tab.Network, "Revive Players", Networking.ReviveAll, toggle: false),

            new CheatButton(Tab.Network, "Make Players Jump", Networking.EveryoneJump, toggle: false),
            new CheatButton(Tab.Network, "Super Jump All", Networking.EveryoneHighJump),

            new CheatButton(Tab.Network, "Super Speed All", () => Networking.SuperSpeedAll(true), () => Networking.SuperSpeedAll(false)),

            new CheatButton(Tab.Network, "Floating Players", Networking.EveryoneJump),
            new CheatButton(Tab.Network, "Fling Players", Networking.FlingPlayers),

            new CheatButton(Tab.Network, "Bomb Players", Networking.BombPlayers, toggle: false),
            new CheatButton(Tab.Network, "Web Players", () => Networking.TrapAll(), toggle: false),
            new CheatButton(Tab.Network, "Goop Players", () => Networking.TrapAll(goop: true), toggle: false),

            new CheatButton(Tab.Network, "Shadow Realm All", Networking.ShadowRealmPlayers, toggle: false),

            new CheatButton(Tab.Network, "Force Bell Surface", () => Networking.ToggleState(true), toggle: false),
            new CheatButton(Tab.Network, "Force Bell Under", () => Networking.ToggleState(false), toggle: false),
            new CheatButton(Tab.Network, "Toggle Bell Door", Networking.ToggleBellDoor, toggle: false),
            new CheatButton(Tab.Network, "Toggle Bell Lock", Networking.ToggleBellLock, toggle: false),

            // Session
            new CheatButton(Tab.Session, "+10k Money", () => Networking.AddMoney(10000), toggle: false),
            new CheatButton(Tab.Session, "-10k Money", () => Networking.RemoveMoney(10000), toggle: false),

            new CheatButton(Tab.Session, "+500 Meta", () => Networking.AddMeta(500), toggle: false),
            new CheatButton(Tab.Session, "-500 Meta", () => Networking.RemoveMeta(500), toggle: false),

            new CheatButton(Tab.Session, "+10k Views", () => Networking.AddCurrentViews(10000), toggle: false),
            new CheatButton(Tab.Session, "+10k Needed Views", () => Networking.AddNeededViews(10000), toggle: false),

            new CheatButton(Tab.Session, "Next Day", Networking.NextDay, toggle: false),
            new CheatButton(Tab.Session, "Previous Day", Networking.PreviousDay, toggle: false),

            new CheatButton(Tab.Session, "Unlock All Upgrades", Networking.UnlockAllUpgrades, toggle: false),

            new CheatButton(Tab.Session, "Grab Camera", Features.Player.GrabCamera, toggle: false),

            new CheatButton(Tab.Session, "Teleport To Bell", Features.Player.TeleportToBell, toggle: false),

            new CheatButton(Tab.Session, "Equip Random Hat", Features.Player.EquipRandomHat, toggle: false),
            new CheatButton(Tab.Session, "Unlock All Hats", Features.Player.UnlockAllHats, toggle: false),
            new CheatButton(Tab.Session, "Lock All Hats", Features.Player.LockAllHats, toggle: false),

            // Settings
            new CheatButton(Tab.Settings, "Watermark", () => showWatermark = true, () => showWatermark = false, enabled: true),

            new CheatButton(Tab.Settings, "Arraylist", () => showArraylist = true, () => showArraylist = false, enabled: true),
            new CheatButton(Tab.Settings, "Spacial Arraylist", () => spacialArraylist = true, () => spacialArraylist = false),

            new CheatButton(Tab.Settings, "Notifications", () => notifications = true, () => notifications = false, enabled: true),
        };
    }

    public class CheatButton
    {
        public Tab category;

        public string name;

        public bool
            enabled,
            disabled,
            toggle;

        public float
            value,
            appliedValue;

        public Action
            action,
            disable;

        public CheatButton(Tab category, string name, Action action, Action disable = null, float value = 0, bool toggle = true, bool enabled = false)
        {
            this.category = category;
            this.name = name;
            this.action = action;
            this.disable = disable;
            this.toggle = toggle;
            this.value = value;
            this.appliedValue = value;
            this.enabled = enabled;
        }
    }
}