using CWR;
using CWR.Features;

using EPOOutline;

using MelonLoader;

using Photon.Pun;

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.InputSystem;

using static CWR.UIProperties;
using static CWR.CheatProperties;

[assembly: MelonInfo(typeof(Entry),
    "CWR", "1.0.0", "Rift"
)]

namespace CWR
{
    public class Entry : MelonMod
    {
        public static bool
            showGUI = true,
            showWatermark = true,

            showArraylist = true,
            spacialArraylist = false,

            notifications = true;

        public static Rect
            mainRect = new Rect(200, 200, 800, 500);

        public static Texture2D
            menuTexture,
            boxTexture,

            buttonTexture,
            hoverButtonTexture,

            lineTexture;

        public class ContentWarning_Colors
        {
            public static Color32 yellow = new Color32(253, 215, 0, 255);

            public static Color32 lighterGray = new Color32(98, 98, 98, 255);
            public static Color32 gray = new Color32(32, 23, 23, 255);

            public static Color32 lighterBrown = new Color32(77, 67, 54, 255);
            public static Color32 brown = new Color32(40, 31, 29, 255);
        }

        private Color32[]
            menuColors = new Color32[] {
                new Color32(26, 27, 31, 255),
                new Color32(19, 18, 21, 255)
            },

            boxColors = new Color32[] {
                new Color32(44, 45, 48, 255),
                new Color32(47, 46, 50, 255)
            },

            buttonColors = new Color32[] {
                new Color32(70, 70, 76, 255),
                new Color32(36, 37, 43, 255)
            },

            hoverButtonColors = new Color32[] {
                new Color32(19, 18, 21, 255),
                new Color32(19, 18, 21, 255),
            };

        private float
            updateInterval = 2,
            lastInterval = 0,

            lerpTime = 1;

        private Vector2
            guiPosition = Vector2.zero;

        public enum Tab
        {
            Home,
            Player,
            Monsters,
            Visuals,
            Network,

            Session,
            Settings,

            Help
        }

        public static Tab
            interfaceTab;

        public static int
            rows = 200,
            columnAmount = 3;

        public static float
            menuRoundAmount = 8.8f,
            buttonRoundAmount = 5.5f;

        public static float
            fovSlider = 80;

        public static Vector4
            menuEdges = new Vector4(menuRoundAmount, menuRoundAmount, menuRoundAmount, menuRoundAmount),
            buttonEdges = new Vector4(buttonRoundAmount, buttonRoundAmount, buttonRoundAmount, buttonRoundAmount);

        // Entities | Objects
        public static PlayerController[] playerControllers;
        public static Player[] players, monsters;
        public static Bot[] bots;

        public static Player localPlayer;

        public static UseDivingBellButton[] bells;

        public static Item[] items;
        public static ItemInstance[] itemInstances;

        public static Outlinable[] outlinables;

        public static VideoCamera[] cameras;
        public static Camera mainCamera;

        public static RoomStatsHolder roomStats;

        public override void OnUpdate ()
        {
            lastInterval += Time.deltaTime;
            if ( lastInterval >= updateInterval )
            {
                try
                {
                    playerControllers = GameObject.FindObjectsOfType<PlayerController> ();
                    MelonLogger.Msg ( $"playerControllers ; {playerControllers}" );

                    Player[] entities = GameObject.FindObjectsOfType<Player>();

                    players = entities.Where ( p => !p.ai ).ToArray ();
                    MelonLogger.Msg ( $"players ; {players}" );

                    monsters = entities.Where ( m => m.ai ).ToArray ();
                    MelonLogger.Msg ( $"monsters ; {monsters}" );

                    bots = GameObject.FindObjectsOfType<Bot> ();
                    MelonLogger.Msg ( $"bots ; {bots}" );

                    localPlayer = Player.localPlayer;
                    MelonLogger.Msg ( $"localPlayer ; {localPlayer}" );

                    bells = GameObject.FindObjectsOfType<UseDivingBellButton> ();
                    MelonLogger.Msg ( $"bells ; {bells}" );

                    items = GameObject.FindObjectsOfType<Item> ();
                    MelonLogger.Msg ( $"items ; {items}" );

                    itemInstances = GameObject.FindObjectsOfType<ItemInstance> ();
                    MelonLogger.Msg ( $"itemInstances ; {itemInstances}" );

                    outlinables = GameObject.FindObjectsOfType<Outlinable> ();
                    MelonLogger.Msg ( $"outlinables ; {outlinables}" );

                    cameras = GameObject.FindObjectsOfType<VideoCamera> ();
                    MelonLogger.Msg ( $"cameras ; {cameras}" );

                    mainCamera = Camera.main;
                    MelonLogger.Msg ( $"mainCamera ; {mainCamera}" );

                    roomStats = SurfaceNetworkHandler.RoomStats;
                    MelonLogger.Msg ( $"roomStats ; {roomStats}" );
                }
                catch ( Exception ex )
                {
                    MelonLogger.Error ( ex );
                }

                lastInterval = 0;
            }

            Mods ();

            foreach ( CheatButton found in Cheats.cheats )
                RunButton ( found );

            if ( Keyboard.current.rightShiftKey.wasPressedThisFrame )
                showGUI = !showGUI;

            float t = Mathf.PingPong(Time.time / lerpTime, 1);

            Color menuColor = Color.Lerp(menuColors[0], menuColors[1], t);
            Color boxColor = Color.Lerp(boxColors[0], boxColors[1], t);

            Color buttonColor = Color.Lerp(buttonColors[0], buttonColors[1], t);
            Color hoverButtonColor = Color.Lerp(hoverButtonColors[0], hoverButtonColors[1], t);

            Theme ( menuColor, boxColor, buttonColor, hoverButtonColor );
        }

        public override void OnGUI ()
        {
            Visuals.ESP ();

            GUI.backgroundColor = Color.clear;

            GUI.skin.label.richText = true;
            GUI.skin.button.richText = true;
            GUI.skin.window.richText = true;
            GUI.skin.textField.richText = true;
            GUI.skin.box.richText = true;

            GUI.skin.label.fontStyle = FontStyle.Bold;
            GUI.skin.button.fontStyle = FontStyle.Bold;
            GUI.skin.window.fontStyle = FontStyle.Bold;
            GUI.skin.textArea.fontStyle = FontStyle.Bold;
            GUI.skin.textField.fontStyle = FontStyle.Bold;

            GUI.skin.button.active.background = buttonTexture;
            GUI.skin.button.normal.background = buttonTexture;
            GUI.skin.button.hover.background = hoverButtonTexture;

            GUI.skin.box.active.background = boxTexture;
            GUI.skin.box.normal.background = boxTexture;
            GUI.skin.box.hover.background = boxTexture;

            GUI.skin.button.normal.textColor = Color.white;
            GUI.skin.button.active.textColor = Color.white;
            GUI.skin.button.hover.textColor = new Color ( .1f, .1f, .1f );

            GUI.skin.toggle.normal.textColor = Color.white;
            GUI.skin.toggle.active.textColor = Color.white;
            GUI.skin.toggle.hover.textColor = new Color ( .1f, .1f, .1f );

            GUI.skin.box.normal.textColor = Color.white;
            GUI.skin.box.active.textColor = Color.white;
            GUI.skin.box.hover.textColor = new Color ( .1f, .1f, .1f );

            Backend ();
        }

        private void Mods ()
        {
            try
            {
                mainCamera.fieldOfView = GetButton ( "Fov" ).appliedValue;
            }
            catch ( Exception e )
            {
                MelonLogger.Error ( e );
            }
        }

        private void Backend ()
        {
            if ( showGUI )
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                mainRect.position += guiPosition - mainRect.center;

                if ( ( mainRect.Contains ( Event.current.mousePosition ) && Mouse.current.rightButton.isPressed ) || Mouse.current.middleButton.isPressed )
                    guiPosition = Vector2.Lerp ( guiPosition, Event.current.mousePosition, Time.deltaTime * 2.7f );

                mainRect = GUI.Window ( 100, mainRect, Interface, "" );

                GUI.DrawTexture ( mainRect, menuTexture, ScaleMode.StretchToFill, false, 0, GUI.color, Vector4.zero, menuEdges );
            }

            if ( showWatermark )
            {
                Rect wm_Rect = new Rect(Screen.width / 2 - 160, 10, 320, 30);

                GUI.DrawTexture ( wm_Rect, menuTexture, ScaleMode.StretchToFill, false, 0, GUI.color, Vector4.zero, menuEdges );

                string isConnected = !PhotonNetwork.InRoom ? "Not Connected" : $"Connected {PhotonNetwork.GetPing()}";

                GUI.Label ( wm_Rect, $"Rift {Mathf.Ceil ( 1 / Time.unscaledDeltaTime )} | {isConnected}", new GUIStyle ( GUI.skin.label )
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 18,
                    fontStyle = FontStyle.Bold
                } );
            }

            if ( showArraylist )
            {
                CheatButton[] button = Cheats.cheats.Where(b => b.enabled).ToArray();

                for ( int i = 0; i < button.Length; i++ )
                {
                    Rect al_Rect = new Rect(Screen.width - 210, 10, 200, 45);

                    al_Rect.y += i * ( spacialArraylist ? 55 : 32.5f );

                    GUI.DrawTexture ( al_Rect, menuTexture, ScaleMode.StretchToFill, false, 0, GUI.color, Vector4.zero, menuEdges );

                    GUI.Label ( al_Rect, button[ i ].name, new GUIStyle ( GUI.skin.label )
                    {
                        alignment = TextAnchor.MiddleCenter,
                        fontSize = 17,
                        fontStyle = FontStyle.Bold
                    } );
                }
            }
        }

        private void Interface ( int id )
        {
            GUILayout.BeginHorizontal ();
            GUILayout.BeginVertical ();
            GUILayout.Space ( -15 );

            GUI.backgroundColor = buttonColors[ 0 ];

            GUILayout.Label ( "Rift", new GUIStyle ( GUI.skin.label )
            {
                alignment = TextAnchor.UpperLeft,
                fontStyle = FontStyle.Bold,
                fontSize = 25
            } );

            GUILayout.Space ( 5 );

            foreach ( object tab in Enum.GetValues ( typeof ( Tab ) ) )
            {
                if ( ( Tab ) tab == Tab.Session || ( Tab ) tab == Tab.Help )
                    GUILayout.Space ( 17.5f );
                CreateTab ( ( Tab ) tab, 160, tab.ToString () );
            }

            GUILayout.Space ( 22.5f );

            GUILayout.Label ( "Right Shift - Close", new GUIStyle ( GUI.skin.label )
            {
                alignment = TextAnchor.MiddleLeft,
                fontStyle = FontStyle.Bold,
                fontSize = 17
            } );

            GUI.backgroundColor = Color.clear;

            GUILayout.EndVertical ();
            GUILayout.Space ( 5 );
            GUILayout.BeginVertical ();

            GUI.backgroundColor = buttonColors[ 0 ];

            DrawTab ( interfaceTab );

            GUI.backgroundColor = Color.clear;

            GUILayout.EndVertical ();
            GUILayout.EndHorizontal ();
        }
    }
}