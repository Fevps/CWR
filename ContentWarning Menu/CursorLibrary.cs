using System;

using UnityEngine;
using UnityEngine.InputSystem;

using static CWR.Entry;

using Object = UnityEngine.Object;

namespace CWR
{
    public class CursorLibrary // Gun Library (Cursor Events)
    {
        public static GameObject GunPointer = null;

        public static Player Player = null;

        public static void Gun(Action enable, Action disable = null)
        {
            if (Mouse.current.rightButton.isPressed)
            {
                Physics.Raycast(
                    mainCamera.ScreenPointToRay(
                        Mouse.current.position.ReadValue(),
                        Camera.MonoOrStereoscopicEye.Mono),
                    out RaycastHit hit);

                if (GunPointer == null)
                    GunPointer = GameObject.CreatePrimitive(0);

                GunPointer.transform.position = hit.point;
                GunPointer.transform.rotation = Quaternion.identity;

                GunPointer.transform.localScale = new Vector3(.2f, .2f, .2f);

                Object.Destroy(GunPointer, Time.deltaTime);
                Object.Destroy(GunPointer.GetComponent<Rigidbody>());
                Object.Destroy(GunPointer.GetComponent<SphereCollider>());

                bool enabled = false;

                if (Mouse.current.leftButton.isPressed)
                {
                    Player = GetClosestPlayer(GunPointer);

                    if (!enabled)
                    {
                        enable?.Invoke();
                        enabled = true;
                    }
                }
                else
                {
                    Player = null;

                    disable?.Invoke();

                    enabled = false;
                }
            }
            else
            {
                Player = null;

                GunPointer = null;
            }
        }

        public static Player GetClosestPlayer(GameObject objectr)
        {
            if (players == null) return null;

            float num = 2;

            Player closest = null;

            foreach (Player player in players)
                if (Vector3.Distance(objectr.transform.position, player.transform.position) < num)
                {
                    num = Vector3.Distance(objectr.transform.position, player.transform.position);
                    closest = player;
                }

            return closest;
        }
    }
}