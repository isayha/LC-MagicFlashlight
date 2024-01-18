using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MagicFlashlight.Patches
{
    [HarmonyPatch(typeof(TimeOfDay))]
    public class StartingCreditsPatch
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        static void Awake(TimeOfDay __instance)
        {
            if (__instance.quotaVariables != null)
            {
                __instance.quotaVariables.startingCredits = 500;
            }
        }
    }
}