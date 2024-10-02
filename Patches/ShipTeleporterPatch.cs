using BepInEx;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;

namespace MagicFlashlight.Patches
{
    [HarmonyPatch(typeof(ShipTeleporter))]
    public class ShipTeleporterPatch
    {
        [HarmonyPatch("TeleportPlayerOutWithInverseTeleporter")]
        [HarmonyPrefix]
        static bool dontDropItemsInverseTeleporter(ShipTeleporter __instance, ref int playerObj, ref Vector3 teleportPos)
        {
            //https://stackoverflow.com/questions/135443/how-do-i-use-reflection-to-invoke-a-private-method
            //https://ludeon.com/forums/index.php?topic=54620.0

            MethodInfo setPlayerTeleporterIdInfo = __instance.GetType().GetMethod("SetPlayerTeleporterId", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo teleportBodyOutInfo = __instance.GetType().GetMethod("TeleportBodyOut", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo spikeTrapsReactToInverseTeleportInfo = __instance.GetType().GetMethod("SpikeTrapsReactToInverseTeleport", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo setCaveReverbInfo = __instance.GetType().GetMethod("SetCaveReverb", BindingFlags.NonPublic | BindingFlags.Instance);

            if (StartOfRound.Instance.allPlayerScripts[playerObj].isPlayerDead)
            {
                var TBOparam = new object[] { playerObj, teleportPos };
                __instance.StartCoroutine((System.Collections.IEnumerator)teleportBodyOutInfo.Invoke(__instance, TBOparam));
                return false;
            }

            spikeTrapsReactToInverseTeleportInfo.Invoke(__instance, null);

            var sCRparam = new object[] { playerObj };
            setCaveReverbInfo.Invoke(__instance, sCRparam);

            PlayerControllerB playerControllerB = StartOfRound.Instance.allPlayerScripts[playerObj];
           
            var sPTIparam = new object[] { playerControllerB, -1 };
            setPlayerTeleporterIdInfo.Invoke(__instance, sPTIparam);

            //playerControllerB.DropAllHeldItems();

            if ((bool)UnityEngine.Object.FindObjectOfType<AudioReverbPresets>())
            {
                UnityEngine.Object.FindObjectOfType<AudioReverbPresets>().audioPresets[2].ChangeAudioReverbForPlayer(playerControllerB);
            }
            playerControllerB.isInElevator = false;
            playerControllerB.isInHangarShipRoom = false;
            playerControllerB.isInsideFactory = true;
            playerControllerB.averageVelocity = 0f;
            playerControllerB.velocityLastFrame = Vector3.zero;
            StartOfRound.Instance.allPlayerScripts[playerObj].TeleportPlayer(teleportPos);
            StartOfRound.Instance.allPlayerScripts[playerObj].beamOutParticle.Play();
            __instance.shipTeleporterAudio.PlayOneShot(__instance.teleporterBeamUpSFX);
            StartOfRound.Instance.allPlayerScripts[playerObj].movementAudio.PlayOneShot(__instance.teleporterBeamUpSFX);
            if (playerControllerB == GameNetworkManager.Instance.localPlayerController)
            {
                Debug.Log("Teleporter shaking camera");
                HUDManager.Instance.ShakeCamera(ScreenShakeType.Big);
            }

            return false;
        }
    }
}
