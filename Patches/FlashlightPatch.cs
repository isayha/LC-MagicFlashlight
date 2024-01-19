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
    [HarmonyPatch(typeof(FlashlightItem))]
    public class FlashlightPatch
    {
		[HarmonyPatch("SwitchFlashlight")]
        [HarmonyPrefix]
		static void SwitchFlashlightPatch(FlashlightItem __instance, ref bool on)
		{
			if (on &&
                __instance.flashlightTypeID == 1 &&
				__instance.insertedBattery.charge > 0)
			{
				PlayerControllerB player = __instance.playerHeldBy;

				if (!player.isInHangarShipRoom) // Prevent usage in space and weather and clock in factory
				{
					System.Random seed = new System.Random();
					Vector3 newposition3 = RoundManager.Instance.insideAINodes[seed.Next(0, RoundManager.Instance.insideAINodes.Length)].transform.position;
					newposition3 = RoundManager.Instance.GetRandomNavMeshPositionInBoxPredictable(newposition3, 10f, RoundManager.Instance.navHit, seed);

					Vector3 oldposition3 = player.gameObject.transform.position; // Fix explosion occuring for all players
                    __instance.flashlightAudio.PlayOneShot(__instance.flashlightClips[UnityEngine.Random.Range(0, __instance.flashlightClips.Length)]);

                    Landmine.SpawnExplosion(oldposition3, spawnExplosionEffect: true, 0f, 0f);

					player.TeleportPlayer(newposition3);
					__instance.insertedBattery.charge = 0;

					Landmine.SpawnExplosion(newposition3, spawnExplosionEffect: true, 0f, 0f);

                    // player.DestroyItemInSlotAndSync(player.currentItemSlot); // This would make the flashlight one-time-use-only
                }
				on = false;

				// TODO:
				// Prevent usage in space (check if in space?)
				// Prevent weather and clock in factory (set player isInFactory to true?)
				// Fix explosion occurring for all players (oldposition3 might have an incorrect value?)

			}
        }
    }
}