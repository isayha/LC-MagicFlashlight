using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MagicFlashlight.Patches
{
    [HarmonyPatch(typeof(ItemDropship))]
    public class ItemDropshipPatch
    {
        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        static void Update(ItemDropship __instance)
        {
            bool trigger = false;
            if (__instance.IsServer)
            {
                if (!__instance.deliveringOrder)
                {
                    if (!trigger)
                    {
                        __instance.shipTimer = 40f;
                        trigger = true;
                    }

                }
            }
        }
    }
}
