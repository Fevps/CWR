using EPOOutline;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine;

using static CWR.Entry;
using static UnityEngine.Rendering.DebugUI;

namespace CWR.Features
{
    public class Visuals
    {
        public static bool
            nameTags = false,
            tracers = false,
            boxes = false,
            healthBars = false,

            includePlayers = true,
            includeMonsters = true,
            includeItems = true,
            includeBells = true,

            includeSelf = false;

        public static Color32
            espColor = Color.white,
            friendlyEspColor = Color.green,
            enemyEspColor = Color.red,

            tracerColor = Color.white;

        public static int
            nameTagSize = 15;

        public static void OutlinedItems ( bool isOn )
        {
            if ( outlinables == null ) return;

            foreach ( Outlinable outlinable in outlinables )
                if ( outlinable.gameObject.GetComponent<Pickup> () != null )
                    outlinable.enabled = isOn;
        }

        public static void ESP ()
        {
            if ( nameTags )
            {
                if ( includePlayers )
                {
                    if ( players == null ) return;

                    global::Player[] _players = includeSelf
                    ? players
                    : players.Where(p => !p.IsLocal).ToArray();

                    foreach ( global::Player player in _players )
                    {
                        Vector3 worldPosition = mainCamera.WorldToScreenPoint(player.HeadPosition());

                        if ( worldPosition.z > 0 )
                        {
                            Vector3 screenPosition = new Vector3(worldPosition.x, Screen.height - worldPosition.y - 5, worldPosition.z);

                            UIProperties.DrawColorString ( screenPosition, player.name, friendlyEspColor, nameTagSize );
                        }
                    }
                }

                if ( includeMonsters )
                {
                    if ( bots == null ) return;

                    foreach ( Bot bot in bots )
                    {
                        Vector3 worldPosition = mainCamera.WorldToScreenPoint(bot.transform.position);

                        if ( worldPosition.z > 0 )
                        {
                            Vector3 screenPosition = new Vector3(worldPosition.x, Screen.height - worldPosition.y - 5, worldPosition.z);

                            UIProperties.DrawColorString ( screenPosition, bot.name, enemyEspColor, nameTagSize );
                        }
                    }
                }

                if ( includeItems )
                {
                    if ( itemInstances == null ) return;

                    foreach ( ItemInstance item in itemInstances )
                    {
                        Vector3 worldPosition = mainCamera.WorldToScreenPoint(item.transform.position);

                        if ( worldPosition.z > 0 )
                        {
                            Vector3 screenPosition = new Vector3(worldPosition.x, Screen.height - worldPosition.y - 5, worldPosition.z);

                            UIProperties.DrawColorString ( screenPosition, item.name, espColor, nameTagSize );
                        }
                    }
                }

                if ( includeBells )
                {
                    if ( bells == null ) return;

                    foreach ( UseDivingBellButton bell in bells )
                    {
                        Vector3 worldPosition = mainCamera.WorldToScreenPoint(bell.transform.position);

                        if ( worldPosition.z > 0 )
                        {
                            Vector3 screenPosition = new Vector3(worldPosition.x, Screen.height - worldPosition.y, worldPosition.z);

                            UIProperties.DrawColorString ( screenPosition, bell.name, espColor, nameTagSize );
                        }
                    }
                }
            }

            if ( healthBars )
            {
                if ( includePlayers )
                {
                    if ( players == null ) return;

                    global::Player[] _players = includeSelf
                    ? players
                    : players.Where(p => !p.IsLocal).ToArray();

                    foreach ( global::Player player in _players )
                    {
                        Vector3 worldPosition = mainCamera.WorldToScreenPoint(player.HeadPosition());

                        if ( worldPosition.z > 0 )
                        {
                            Vector3 screenPosition = new Vector3(worldPosition.x, Screen.height - worldPosition.y - 5, worldPosition.z);

                            UIProperties.DrawColorString ( screenPosition + new Vector3 ( 0, 50, 0 ), player.data.health.ToString (), friendlyEspColor, nameTagSize );
                        }
                    }
                }
            }

            if ( tracers )
            {
                Vector2 origin = new Vector2(Screen.width / 2, 0);

                if ( includePlayers )
                {
                    if ( players == null ) return;

                    global::Player[] _players = includeSelf
                    ? players
                    : players.Where(p => !p.IsLocal).ToArray();

                    foreach ( global::Player player in _players )
                    {
                        Vector3 worldPosition = mainCamera.WorldToScreenPoint(player.HeadPosition());

                        if ( worldPosition.z > 0 )
                        {
                            Vector2 screenPosition = new Vector2(worldPosition.x, Screen.height - worldPosition.y - 5);

                            UIProperties.DrawLine ( origin, screenPosition, friendlyEspColor, 1.5f );
                        }
                    }
                }

                if ( includeMonsters )
                {
                    if ( bots == null ) return;

                    foreach ( Bot bot in bots )
                    {
                        Vector3 worldPosition = mainCamera.WorldToScreenPoint(bot.transform.position);

                        if ( worldPosition.z > 0 )
                        {
                            Vector2 screenPosition = new Vector2(worldPosition.x, Screen.height - worldPosition.y - 5);

                            UIProperties.DrawLine ( origin, screenPosition, enemyEspColor, 1.5f );
                        }
                    }
                }

                if ( includeItems )
                {
                    if ( itemInstances == null ) return;

                    foreach ( ItemInstance item in itemInstances )
                    {
                        if ( localPlayer.data.currentItem == item )
                            continue;

                        Vector3 worldPosition = mainCamera.WorldToScreenPoint(item.transform.position);

                        if ( worldPosition.z > 0 )
                        {
                            Vector2 screenPosition = new Vector2(worldPosition.x, Screen.height - worldPosition.y - 5);

                            UIProperties.DrawLine ( origin, screenPosition, espColor, 1.5f );
                        }
                    }
                }

                if ( includeBells )
                {
                    if ( bells == null ) return;

                    foreach ( UseDivingBellButton bell in bells )
                    {
                        Vector3 worldPosition = mainCamera.WorldToScreenPoint(bell.transform.position);

                        if ( worldPosition.z > 0 )
                        {
                            Vector2 screenPosition = new Vector2(worldPosition.x, Screen.height - worldPosition.y);

                            UIProperties.DrawLine ( origin, screenPosition, espColor, 1.5f );
                        }
                    }
                }
            }

            if ( boxes )
            {
                float minSize = 5, maxSize = 40;
                float Scale ( float distance )
                {
                    float scale = 4000 / distance;
                    return Mathf.Clamp ( scale, minSize, maxSize );
                }

                if ( includePlayers )
                {
                    if ( players == null ) return;

                    global::Player[] _players = includeSelf
                    ? players
                    : players.Where(p => !p.IsLocal).ToArray();

                    foreach ( global::Player player in _players )
                    {
                        Vector3 worldPosition = mainCamera.WorldToScreenPoint(player.transform.position + Vector3.up * 1.8f);

                        if ( worldPosition.z > 0 )
                        {
                            float size = Scale(Vector3.Distance(mainCamera.transform.position, player.transform.position));

                            Vector2 center = new Vector2(worldPosition.x, Screen.height - worldPosition.y);

                            UIProperties.DrawBox ( center, size, size, friendlyEspColor );
                        }
                    }
                }

                if ( includeMonsters )
                {
                    if ( bots == null ) return;

                    foreach ( Bot bot in bots )
                    {
                        Vector3 worldPosition = mainCamera.WorldToScreenPoint(bot.transform.position + Vector3.up * 1.8f);

                        if ( worldPosition.z > 0 )
                        {
                            float size = Scale(Vector3.Distance(mainCamera.transform.position, bot.transform.position));

                            Vector2 center = new Vector2(worldPosition.x, Screen.height - worldPosition.y);

                            UIProperties.DrawBox ( center, size, size, enemyEspColor );
                        }
                    }
                }

                if ( includeItems )
                {
                    if ( itemInstances == null ) return;

                    foreach ( ItemInstance item in itemInstances )
                    {
                        if ( localPlayer.data.currentItem == item )
                            continue;

                        Vector3 worldPosition = mainCamera.WorldToScreenPoint(item.transform.position);

                        if ( worldPosition.z > 0 )
                        {
                            float size = Scale(Vector3.Distance(mainCamera.transform.position, item.transform.position)) * .75f;
                            size = Mathf.Clamp ( size, minSize, maxSize );

                            Vector2 center = new Vector2(worldPosition.x, Screen.height - worldPosition.y);

                            UIProperties.DrawBox ( center, size, size, espColor );
                        }
                    }
                }

                if ( includeBells )
                {
                    if ( bells == null ) return;

                    foreach ( UseDivingBellButton bell in bells )
                    {
                        Vector3 worldPosition = mainCamera.WorldToScreenPoint(bell.transform.position);

                        if ( worldPosition.z > 0 )
                        {
                            float size = Scale(Vector3.Distance(mainCamera.transform.position, bell.transform.position));

                            Vector2 center = new Vector2(worldPosition.x, Screen.height - worldPosition.y);

                            UIProperties.DrawBox ( center, size, size, espColor );
                        }
                    }
                }
            }
        }

        public static void Chams ( bool enable )
        {
            if ( players == null ) return;

            if ( includePlayers )
            {
                global::Player[] _players = includeSelf
                ? players
                : players.Where(p => !p.IsLocal).ToArray();

                foreach ( global::Player player in _players )
                    if ( player != null && player.transform != null )
                    {
                        Shader targetShader = Shader.Find(enable ? "GUI/Text Shader" : "NiceShader");
                        Color targetColor = enable ? Color.green : new Color(.1887f, .1887f, .1887f, .9137f);

                        GameObject playerModel = player.transform.GetChild(1).gameObject;
                        GameObject bodyRenderer = playerModel.gameObject.transform.GetChild(3).gameObject;

                        SkinnedMeshRenderer renderer = bodyRenderer.GetComponent<SkinnedMeshRenderer>();

                        renderer.material.shader = targetShader;
                        renderer.material.color = targetColor;
                    }
            }
        }

        public static void LowPoly ( bool enable )
        {
            GameObject poly = GameObject.Find("Sky_Day");
            if ( poly == null ) return;

            poly.layer = enable ? poly.layer + 1000 : poly.layer - 1000;
        }

        public static void ThirdPerson ( bool enable )
        {
            if ( localPlayer == null ) return;

            HeadFollower headFollower = localPlayer.GetComponentInChildren<HeadFollower>();
            if ( headFollower == null ) return;

            var field = typeof(HeadFollower).GetField("offset",
                            System.Reflection.BindingFlags.NonPublic |
                            System.Reflection.BindingFlags.Instance);

            field?.SetValue ( headFollower, new Vector3 ( 0, enable ? 3 : 1.53f, enable ? -12 : 0 ) );
        }
    }
}