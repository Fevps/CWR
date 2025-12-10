using CWR.Features;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEngine;
using UnityEngine.SceneManagement;

using static CWR.Entry;

namespace CWR
{
    public class CheatProperties
    {
        public static void DrawGrid<T> ( IEnumerable<T> items, int columns, int rows, Action<T> drawCell, float spacing = 5, bool padIncompleteRow = false )
        {
            int count = 0,
                maxRows = columns * rows;

            GUILayout.BeginVertical ();

            GUILayout.BeginHorizontal ();

            foreach ( var item in items )
            {
                drawCell ( item );
                count++;

                if ( count >= maxRows )
                    break;

                if ( count % columns == 0 )
                {
                    GUILayout.EndHorizontal ();
                    //GUILayout.Space(spacing);

                    if ( count < items.Count () )
                        GUILayout.BeginHorizontal ();
                }
            }

            if ( count % columns != 0 )
            {
                if ( padIncompleteRow )
                {
                    int remaining = columns - (count % columns);
                    for ( int i = 0; i < remaining; i++ )
                        GUILayout.Label ( "" );
                }

                GUILayout.EndHorizontal ();
                //GUILayout.Space(spacing);
            }

            GUILayout.EndVertical ();
        }

        public static void DrawButton ( CheatButton button )
        {
            if ( button.value <= 0 )
            {
                int width = 170,
                height = 55,
                offset = 5,

                buttonWidth = 20,
                buttonHeight = 20;

                GUILayout.BeginVertical ( GUI.skin.box,
                    GUILayout.Width ( width ),
                    GUILayout.Height ( height ),
                    GUILayout.ExpandWidth ( false ) );

                GUILayout.Label ( button.name, new GUIStyle ( GUI.skin.label )
                {
                    alignment = TextAnchor.MiddleLeft,
                    fontSize = 16,
                    fontStyle = FontStyle.Bold
                },
                GUILayout.Width ( width ) );

                UIProperties.DrawLine ( offset );

                GUILayout.BeginHorizontal ( GUILayout.Width ( width ) );

                GUILayout.Label ( button.toggle ? button.enabled ? "Disable" : "Enable" : "Toggle", new GUIStyle ( GUI.skin.label )
                {
                    alignment = TextAnchor.MiddleLeft,
                    fontSize = 14,
                    fontStyle = FontStyle.Bold,

                    normal = {
                        textColor = button.enabled
                        ? Color.yellow
                        : Color.white
                    }
                },
                GUILayout.Width ( width - ( offset * 4 ) ) );

                if ( UIProperties.DrawButton ( "", GUILayout.Width ( buttonWidth ), GUILayout.Height ( buttonHeight ) ) )
                {
                    button.enabled = !button.enabled;

                    if ( button.toggle )
                        button.disabled = false;

                    if ( notifications )
                        UserInterface.ShowMoneyNotification ( button.name, !button.toggle ? "TOGGLED" : button.enabled ? "ENABLED" : "DISABLED", MoneyCellUI.MoneyCellType.MetaCoins );
                }

                GUILayout.EndHorizontal ();

                GUILayout.EndVertical ();
            }
            else if ( button.value > 0 )
            {
                int width = 170,
                    height = 55,
                    offset = 5,

                    buttonWidth = 20,
                    buttonHeight = 20;

                GUILayout.BeginVertical ( GUI.skin.box,
                    GUILayout.Width ( width ),
                    GUILayout.Height ( height ),
                    GUILayout.ExpandWidth ( false ) );

                GUILayout.Label ( button.name, new GUIStyle ( GUI.skin.label )
                {
                    alignment = TextAnchor.MiddleLeft,
                    fontSize = 16,
                    fontStyle = FontStyle.Bold,
                },
                GUILayout.Width ( width ) );

                UIProperties.DrawLine ( offset );

                GUILayout.BeginHorizontal ( GUILayout.Width ( width ) );

                GUILayout.Label ( button.value.ToString ( "F1" ), new GUIStyle ( GUI.skin.label )
                {
                    alignment = TextAnchor.MiddleLeft,
                    fontSize = 14,
                    fontStyle = FontStyle.Bold,

                    normal = {
                        textColor = button.enabled
                        ? Color.yellow
                        : Color.white
                    }
                },
                GUILayout.Width ( 50 ) );

                button.value = GUILayout.HorizontalSlider ( button.value, .1f, 200, GUILayout.Width ( 96 ), GUILayout.Height ( 20 ) );

                if ( UIProperties.DrawButton ( "", GUILayout.Width ( buttonWidth ), GUILayout.Height ( buttonHeight ) ) )
                {
                    button.enabled = !button.enabled;

                    if ( button.toggle )
                        button.disabled = false;

                    if ( button.enabled && button.action != null )
                        button.action ();
                    else if ( !button.enabled && button.disable != null )
                        button.disable ();

                    button.appliedValue = button.value;

                    if ( notifications )
                        UserInterface.ShowMoneyNotification ( $"{button.name} | {button.appliedValue:F1}", !button.toggle ? "TOGGLED" : button.enabled ? "ENABLED" : "DISABLED", MoneyCellUI.MoneyCellType.MetaCoins );
                }

                GUILayout.EndHorizontal ();
                GUILayout.EndVertical ();
            }
        }

        public static void RunButton ( CheatButton button )
        {
            if ( button.enabled )
            {
                button.action?.Invoke ();

                if ( !button.toggle )
                    button.enabled = false;
            }
            else if ( !button.enabled && button.toggle )
                button.disable?.Invoke ();
        }

        public static CheatButton GetButton ( string name ) =>
            Cheats.cheats.FirstOrDefault ( b => b.name == name );

        public static Item GetItemByName ( string name ) =>
            items.FirstOrDefault ( i => i.name == name );

        public static bool SurfaceScene () =>
            SceneManager.GetActiveScene ().name == "SurfaceScene";

        public static GameObject Instantiate ( string prefab, Vector3 pos, Quaternion rot )
        {
            /*
            if (!string.IsNullOrEmpty(prefab) || pos.z > 0 || rot.z > 0)
                return Photon.Pun.PhotonNetwork.Instantiate(prefab, pos, rot);
            return null;
            */

            return Photon.Pun.PhotonNetwork.Instantiate ( prefab, pos, rot );
        }

        public static void SpawnItem ( Item item, string itemName, Vector3 pos, int amount = 1 )
        {
            item = ItemDatabase.Instance.lastLoadedItems.FirstOrDefault ( i => i.name == itemName );
            if ( item == null || amount <= 0 ) return;

            for ( int i = 0; i < amount; i++ )
                localPlayer.RequestCreatePickup ( item, new ItemInstanceData ( Guid.NewGuid () ), pos, Quaternion.identity );
        }
    }

    public class UIProperties : MonoBehaviour
    {
        public static void Theme ( Color menuColor, Color boxColor, Color buttonColor, Color hoverButtonColor )
        {
            if ( menuTexture == null )
                menuTexture = new Texture2D ( 1, 1 );

            menuTexture.SetPixel ( 0, 0, menuColor );
            menuTexture.Apply ();

            if ( boxTexture == null )
                boxTexture = new Texture2D ( 1, 1 );

            boxTexture.SetPixel ( 0, 0, boxColor );
            boxTexture.Apply ();

            if ( buttonTexture == null )
                buttonTexture = new Texture2D ( 1, 1 );

            buttonTexture.SetPixel ( 0, 0, buttonColor );
            buttonTexture.Apply ();

            if ( hoverButtonTexture == null )
                hoverButtonTexture = new Texture2D ( 1, 1 );

            hoverButtonTexture.SetPixel ( 0, 0, hoverButtonColor );
            hoverButtonTexture.Apply ();

            if ( lineTexture == null )
                lineTexture = new Texture2D ( 1, 1 );

            lineTexture.SetPixel ( 0, 0, Visuals.tracerColor );
            lineTexture.Apply ();
        }

        public static void CreateTab ( Tab tab, float width, string tabName = "Error" )
        {
            Color textColor = tab == interfaceTab ? Color.yellow : Color.white;

            if ( DrawTab ( tabName, textColor, -5, GUILayout.Width ( width ), GUILayout.Height ( 40 ) ) )
                if ( interfaceTab != tab )
                    interfaceTab = tab;
        }

        public static bool DrawTab ( string content, Color textColor, float offset = 5, params GUILayoutOption[ ] options )
        {
            Rect rect = GUILayoutUtility.GetRect(new GUIContent(content), GUI.skin.button, options);

            if ( rect.Contains ( Event.current.mousePosition ) && Event.current.type == EventType.MouseDown )
                return true;

            GUI.DrawTexture ( rect, buttonTexture, ScaleMode.StretchToFill, false, 0, GUI.color, Vector4.zero, buttonEdges );

            DrawText ( new Rect ( rect.x, rect.y - offset, rect.width, 25 ), content, 12, textColor, true, true );

            return false;
        }

        public static bool DrawButton ( string content, params GUILayoutOption[ ] options )
        {
            Rect rect = GUILayoutUtility.GetRect(new GUIContent(content), GUI.skin.button, options);

            if ( rect.Contains ( Event.current.mousePosition ) && Event.current.type == EventType.MouseDown )
                return true;

            GUI.DrawTexture ( rect, buttonTexture, ScaleMode.StretchToFill, false, 0, GUI.color, Vector4.zero, buttonEdges );
            DrawText ( new Rect ( rect.x, rect.y - 5, rect.width, 25 ), content, 12, Color.white, true, true );

            return false;
        }

        public static void DrawText ( Rect rect, string text, int fontSize = 12, Color textColor = default ( Color ), bool centerX = false, bool centerY = true )
        {
            GUIStyle guistyle = new GUIStyle(GUI.skin.label) { fontSize = fontSize, fontStyle = FontStyle.Bold, normal = { textColor = textColor }, alignment = TextAnchor.MiddleLeft };
            GUI.Label ( new Rect ( centerX ? ( rect.x + rect.width / 2 - guistyle.CalcSize ( new GUIContent ( text ) ).x / 2 ) : rect.x, centerY ? ( rect.y + rect.height / 2 - guistyle.CalcSize ( new GUIContent ( text ) ).y / 2 ) : rect.y, rect.width, rect.height ), new GUIContent ( text ), guistyle );
        }

        public static void DrawColorString ( Vector2 position, string label, Color color, float size, bool centered = true )
        {
            GUIContent content = new GUIContent(label);
            GUIStyle style = new GUIStyle();

            style.fontSize = Mathf.RoundToInt ( size );
            style.normal.textColor = color;

            Vector2
                sizeVec = style.CalcSize(content),
                upperLeft = centered ? position - sizeVec / 2 : position;

            GUI.Label ( new Rect ( upperLeft, sizeVec ), content, style );
        }

        public static void DrawLine ( Vector2 pointA, Vector2 pointB, Color color, float thickness )
        {
            Color originalColor = GUI.color;
            GUI.color = color;

            Vector2 delta = pointB - pointA;
            float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
            float length = delta.magnitude;

            GUIUtility.RotateAroundPivot ( angle, pointA );

            GUI.DrawTexture ( new Rect ( pointA.x, pointA.y, length, thickness ), lineTexture );

            GUIUtility.RotateAroundPivot ( -angle, pointA );

            GUI.color = originalColor;
        }

        public static void DrawBox ( Vector2 center, float width, float height, Color color, float thickness = 1.5f )
        {
            Vector2 topLeft = new Vector2(center.x - width / 2, center.y - height / 2);
            Vector2 topRight = new Vector2(center.x + width / 2, center.y - height / 2);
            Vector2 bottomRight = new Vector2(center.x + width / 2, center.y + height / 2);
            Vector2 bottomLeft = new Vector2(center.x - width / 2, center.y + height / 2);

            DrawLine ( topLeft, topRight, color, thickness );
            DrawLine ( topRight, bottomRight, color, thickness );
            DrawLine ( bottomRight, bottomLeft, color, thickness );
            DrawLine ( bottomLeft, topLeft, color, thickness );
        }

        public static Vector2 BeginScrollView ( Vector2 scrollPosition, params GUILayoutOption[ ] options )
        {
            GUIStyle scrollView = new GUIStyle(GUI.skin.verticalScrollbar);

            scrollView.normal.background = buttonTexture;
            scrollView.hover.background = buttonTexture;
            scrollView.active.background = buttonTexture;
            scrollView.onNormal.background = buttonTexture;
            scrollView.onHover.background = buttonTexture;
            scrollView.onActive.background = buttonTexture;

            scrollView.fixedWidth = 0;

            return GUILayout.BeginScrollView (
                scrollPosition,
                scrollView,
                options
            );
        }

        public static void DrawLine ( float space )
        {
            GUILayout.Space ( space );

            DrawButton ( "", GUILayout.Height ( 2 ) );

            GUILayout.Space ( space );
        }

        private static void TabRun ( ref Vector2 slider, int[ ] values, bool padIncompleteRow = false )
        {
            //GUILayout.Space(-10);

            Color color = GUI.color;
            GUI.color = new Color ( 0, 0, 0, 0 );

            slider = BeginScrollView ( slider, GUILayout.Width ( 600 ), GUILayout.Height ( 445 ) );

            GUI.color = color;

            var category = Cheats.cheats.Where(c => c.category == (Tab)values[0]);

            CheatProperties.DrawGrid (
                category,
                values[ 1 ],
                values[ 2 ],
                CheatProperties.DrawButton,
                padIncompleteRow: padIncompleteRow
            );

            GUILayout.EndScrollView ();
        }

        private static void DrawSlider ( ref Vector2 slider, Action code )
        {
            Color color = GUI.color;
            GUI.color = new Color ( 0, 0, 0, 0 );

            slider = BeginScrollView ( slider, GUILayout.Width ( 600 ), GUILayout.Height ( 445 ) );

            GUI.color = color;

            code?.Invoke ();

            GUILayout.EndScrollView ();
        }

        private static Vector2
            hSlider,
            pSlider,
            mSlider,
            vSlider,
            nSlider,
            sSlider,
            SSlider,
            HSlider;

        private static void Line ( float space )
        {
            GUILayout.Space ( space );

            DrawButton ( "", GUILayout.Height ( 2 ) );

            GUILayout.Space ( space );
        }

        private static void DrawRights ( float space = 5, float space2 = 5 )
        {
            GUILayout.Space ( -space );

            GUILayout.Label (
                "© 2025 Rift Development. All rights reserved.",
                new GUIStyle ( GUI.skin.label )
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 14,
                    fontStyle = FontStyle.BoldAndItalic
                } );

            GUILayout.Space ( -space2 );
        }

        public static void DrawTab ( Tab tab )
        {
            GUIStyle
            titleStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 18,
                fontStyle = FontStyle.Bold
            },
            generalStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 14,
                fontStyle = FontStyle.Bold
            },
            infoStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 13,
                fontStyle = FontStyle.Italic
            },
            detailStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 12
            };

            switch ( tab )
            {
                case Tab.Home:
                    DrawSlider ( ref hSlider, () =>
                    {
                        GUILayout.BeginVertical ();

                        // Title
                        GUILayout.Label ( "Welcome to Rift", new GUIStyle ( GUI.skin.label )
                        {
                            alignment = TextAnchor.MiddleCenter,
                            fontSize = 25,
                            fontStyle = FontStyle.Bold,

                            normal = {
                                textColor = Color.Lerp(Color.white, new Color32(199, 199, 199, 255), Mathf.PingPong(Time.time / 1, 1))
                            }
                        } );

                        GUILayout.Space ( -7.5f );

                        GUILayout.Label ( "operating since 2023.", new GUIStyle ( GUI.skin.label )
                        {
                            alignment = TextAnchor.MiddleCenter,
                            fontSize = 13,
                            fontStyle = FontStyle.Italic,

                            normal = {
                                textColor = Color.Lerp(Color.white, new Color32(199, 199, 199, 255), Mathf.PingPong(Time.time / 1, 1))
                            }
                        } );

                        Line ( 5 );

                        // Release Notes
                        GUILayout.Label ( "Release Notes", titleStyle );

                        GUILayout.BeginVertical ( GUI.skin.box );

                        GUILayout.Label ( "Modules", generalStyle );

                        GUILayout.Space ( -7.5f );

                        GUILayout.Label ( "Every cheat/module shown.", infoStyle );

                        GUILayout.Label ( "User Interface", generalStyle );

                        GUILayout.Space ( -7.5f );

                        GUILayout.Label ( "Redesigned and recoded entire GUI.", infoStyle );

                        GUILayout.Label ( "Project", generalStyle );

                        GUILayout.Space ( -7.5f );

                        GUILayout.Label ( "Formatted and recoded the project for User-Friendly and Memory enhancements.", infoStyle );

                        GUILayout.EndVertical ();
                        // End Release Notes

                        // Info
                        GUILayout.Label ( "Information", titleStyle );

                        GUILayout.BeginVertical ( GUI.skin.box );

                        GUILayout.Label ( "Rift", generalStyle );

                        GUILayout.Space ( -7.5f );

                        GUILayout.Label (
                            "Rift Development has been in motion since December 2023,\n" +
                            "we're happy to deliver you this custom module cheat from our developers.\n" +
                            "This cheat is still in development for wonderous users,\n" +
                            "full version will be released before 2026.",
                            infoStyle );

                        GUILayout.Space ( 5 );

                        GUILayout.Label (
                            "Rift (Content Warning) is\n" +
                            "Lightweight, Open-Source, Accessible, User-Friendly, Compact,\n" +
                            "and the #1 Content Warning (Client/Hack/Cheat/Menu),\n" +
                            $"with over {Cheats.cheats.Length + 1} undetected cheats.",
                            infoStyle );

                        GUILayout.EndVertical ();
                        // End Info

                        // Credits
                        GUILayout.Label ( "Credits", titleStyle );

                        GUILayout.BeginVertical ( GUI.skin.box );

                        GUILayout.Label ( "@groominator / Fevps", generalStyle );

                        GUILayout.Space ( -7.5f );

                        GUILayout.Label (
                            "GUI Development, Module/Cheat Development, Frontend Development.",
                            infoStyle );

                        GUILayout.Space ( 5 );

                        GUILayout.Label ( "@allocconsole / Catlicker", generalStyle );

                        GUILayout.Space ( -7.5f );

                        GUILayout.Label (
                            "Assistant, Module/Cheat Development, Backend Development.",
                            infoStyle );

                        GUILayout.Space ( 5 );

                        GUILayout.Label ( "@f7cv6ewfn9je / Monkey", generalStyle );

                        GUILayout.Space ( -7.5f );

                        GUILayout.Label (
                            "Assistant, Creativity, Ideas, Formation.",
                            infoStyle );

                        GUILayout.EndVertical ();
                        // End Credits

                        Line ( 5 );

                        DrawRights ();

                        GUILayout.EndVertical ();
                    } );
                    break;

                case Tab.Player:
                    TabRun ( ref pSlider, new int[ ] { 1, columnAmount, rows } );
                    Line ( 5 );
                    break;

                case Tab.Monsters:
                    TabRun ( ref mSlider, new int[ ] { 2, columnAmount, rows } );
                    Line ( 5 );
                    break;

                case Tab.Visuals:
                    TabRun ( ref vSlider, new int[ ] { 3, columnAmount, rows } );
                    Line ( 5 );
                    break;

                case Tab.Network:
                    TabRun ( ref nSlider, new int[ ] { 4, columnAmount, rows } );
                    Line ( 5 );
                    break;

                case Tab.Session:
                    DrawSlider ( ref sSlider, () =>
                    {
                        GUILayout.Label ( "Room Details", titleStyle );

                        GUILayout.BeginVertical ( GUI.skin.box );

                        string details =
                        PhotonNetwork.InRoom
                        ? $@"Room; {PhotonNetwork.CurrentRoom.Name}
Players; {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}
Ping; {PhotonNetwork.GetPing()}
Master; {PhotonNetwork.MasterClient.NickName}
Region; {PhotonNetwork.CloudRegion.ToUpper()} {PhotonNetwork.ServerTimestamp}
Address; {PhotonNetwork.ServerAddress}
Elapsed; {Time.deltaTime:F4},
Public; {PhotonNetwork.CurrentRoom.IsOpen}"
                        : "NOT CONNECTED";

                        GUILayout.Label ( details, detailStyle );

                        if ( DrawButton ( "Copy" ) && PhotonNetwork.InRoom )
                            GUIUtility.systemCopyBuffer = details;

                        GUILayout.EndVertical ();

                        Line ( 5 );

                        var category = Cheats.cheats.Where(c => c.category == (Tab)5);

                        CheatProperties.DrawGrid (
                            category,
                            columnAmount,
                            rows,
                            CheatProperties.DrawButton,
                            padIncompleteRow: false
                        );
                    } );

                    Line ( 5 );
                    break;

                case Tab.Settings:
                    TabRun ( ref SSlider, new int[ ] { 6, columnAmount, rows } );
                    Line ( 5 );
                    break;

                case Tab.Help:
                    DrawSlider ( ref HSlider, () =>
                    {
                        GUILayout.BeginVertical ();

                        // Title
                        GUILayout.Label ( "Help", new GUIStyle ( GUI.skin.label )
                        {
                            alignment = TextAnchor.MiddleCenter,
                            fontSize = 25,
                            fontStyle = FontStyle.Bold,

                            normal = {
                                textColor = Color.Lerp(Color.white, new Color32(199, 199, 199, 255), Mathf.PingPong(Time.time / 1, 1))
                            }
                        } );

                        Line ( 5 );

                        // Navigation
                        GUILayout.Label ( "Navigation", titleStyle );

                        GUILayout.BeginVertical ( GUI.skin.box );

                        GUILayout.Label ( "Tabs", generalStyle );

                        GUILayout.Space ( -7.5f );

                        GUILayout.Label (
                            "To the left, there's a list of categories you can choose to display said modules.",
                            infoStyle );

                        GUILayout.Label ( "Modules", generalStyle );

                        GUILayout.Space ( -7.5f );

                        GUILayout.Label (
                            "Using your scroll wheel or mouse, you can scroll through the category you're on.",
                            infoStyle );

                        GUILayout.EndVertical ();
                        // End Navigation

                        // Questions & Answers
                        GUILayout.Label ( "Questions & Answers", titleStyle );

                        GUILayout.BeginVertical ( GUI.skin.box );

                        GUILayout.Label ( "How undetected is it?", generalStyle );

                        GUILayout.Space ( -7.5f );

                        GUILayout.Label (
                            "100%, Content Warning doesn't have an Anti-Cheat.",
                            infoStyle );

                        GUILayout.Label ( "How is this different from other cheats?", generalStyle );

                        GUILayout.Space ( -7.5f );

                        GUILayout.Label (
                            "We're different in every way.\n" +
                            "From Modules to the GUI, everything is neatly and thoroughly packed into one project.\n" +
                            "We have multiple developers with precise skill and training\n" +
                            "working on this every week for the greatest user experience no one else can provide.",
                            infoStyle );

                        GUILayout.Label (
                            "We also have the most overpowered mods available on Content Warning.",
                            infoStyle );

                        GUILayout.EndVertical ();
                        // End Questions & Answers

                        // Useful Links
                        GUILayout.Label ( "Useful Links", titleStyle );

                        GUILayout.BeginVertical ( GUI.skin.box );

                        GUILayout.Label ( "Github", generalStyle );

                        GUILayout.Space ( -7.5f );

                        GUILayout.BeginHorizontal ();

                        GUILayout.Label (
                            "https://github.com/Fevps/CWR",
                            infoStyle );

                        if ( DrawButton ( "COPY", GUILayout.Width ( 45 ), GUILayout.Height ( 19 ) ) )
                            GUIUtility.systemCopyBuffer = "https://github.com/Fevps/CWR";

                        GUILayout.EndHorizontal ();

                        GUILayout.Label ( "Rift Development", generalStyle );

                        GUILayout.Space ( -7.5f );

                        GUILayout.BeginHorizontal ();

                        GUILayout.Label (
                            "https://discord.gg/W5urYA9Fz9", infoStyle );

                        if ( DrawButton ( "COPY", GUILayout.Width ( 45 ), GUILayout.Height ( 19 ) ) )
                            GUIUtility.systemCopyBuffer = "https://discord.gg/W5urYA9Fz9";

                        GUILayout.EndHorizontal ();

                        GUILayout.EndVertical ();
                        // End Navigation

                        Line ( 5 );

                        DrawRights ();

                        GUILayout.EndVertical ();
                    } );
                    break;
            }
        }
    }

    public class Entries
    {
        public static VideoInfoEntry GetFilmEntry ( VideoCamera camera ) // object obj
        {
            if ( camera == null ) return null;

            FieldInfo field = typeof(VideoCamera).GetField("m_recorderInfoEntry", BindingFlags.NonPublic | BindingFlags.Instance);
            if ( field == null ) return null;

            return field?.GetValue ( camera ) as VideoInfoEntry;
        }

        public static BatteryEntry GetBatteryEntry ( IList item, Type batteryType )
        {
            if ( batteryType == null ) return null;

            FieldInfo field = batteryType.GetField("m_batteryEntry", BindingFlags.NonPublic | BindingFlags.Instance);

            foreach ( var obj in item )
                if ( obj != null )
                    return field?.GetValue ( obj ) as BatteryEntry;

            return null;
        }
    }
}