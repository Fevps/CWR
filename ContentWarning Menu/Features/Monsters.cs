using Photon.Pun;

using UnityEngine;

using static CWR.Entry;

namespace CWR.Features
{
    public class Monsters
    {
        public static string[] monsterNames = new string[]
        {
            "BarnacleBall",
            "BigSlap",
            "Bombs",
            "Dog",
            "Ear",
            "EyeGuy",         
            "Flicker",        
            "Ghost",          
            "Jello",          
            "Knifo",          
            "Larva",          
            "Mouthe",         
            "Slurper",        
            "Snatcho",        
            "Spider",         
            "Zombe",          
            "Toolkit_Fan",    
            "Toolkit_Hammer", 
            "Toolkit_Iron",   
            "Toolkit_Vaccuum",
            "Toolkit_Whisk",
            "Toolkit_Wisk",
            "Weeping",
            "Zombe"
        };

        public static string getRandom = Random.Range(0, monsterNames.Length - 1).ToString();

        public static void Jump()
        {
            if (monsters == null) return;

            foreach (global::Player monster in monsters)
                monster.refs.view.RPC("RPCA_Jump", RpcTarget.All);
        }

        public static void SpawnMonster(string monsterPrefab) =>
            Monster.SpawnMonster(monsterPrefab);

        public static void SpawnMonster(string monsterPrefab, Vector3 pos) =>
            CheatProperties.Instantiate(monsterPrefab, pos, Quaternion.identity);

        public static void KillAll() =>
            BotHandler.instance.DestroyAll();

        public static void GrabMonsters(float y = 2)
        {
            if (monsters == null) return;

            foreach (global::Player monster in monsters)
                monster.transform.root.gameObject.transform.position = localPlayer.transform.position + new Vector3(0, y, 0);
        }

        private static bool m_Enabled;
        public static void FreezeMonsters()
        {
            m_Enabled = !m_Enabled;

            if (bots == null) return;

            foreach (Bot bot in bots)
                bot.moveSpeedMultiplier = m_Enabled ? 0 : 1;
        }

        public static void PlayerTransform() // Kills a player and spawns a monster in their position
        {
            if (players == null) return;

            global::Player target = players[Random.Range(0, players.Length)];
            if (target == null) return;

            CheatProperties.Instantiate("Zombe", target.data.groundPos, Quaternion.identity);

            target.Die();
        }
    }
}