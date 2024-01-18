using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using MagicFlashlight.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicFlashlight
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class MagicFlashlightBase : BaseUnityPlugin
    {
        private const string modGUID = "MagicFlashlight";
        private const string modName = "Lethal Company - Magic Flashlight";
        private const string modVersion = "1.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static MagicFlashlightBase Instance;

        internal ManualLogSource mls;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            
            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            mls.LogInfo(modName + " " + modVersion + " has awakened");

            harmony.PatchAll(typeof(MagicFlashlightBase));
            harmony.PatchAll(typeof(ShipTeleporterPatch));
            harmony.PatchAll(typeof(StartingCreditsPatch));
            harmony.PatchAll(typeof(FlashlightPatch));
            harmony.PatchAll(typeof(ItemDropshipPatch));

            mls.LogInfo(modName + " " + modVersion + " patching has completed.");
        }
    }
}
