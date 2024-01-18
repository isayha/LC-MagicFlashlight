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
        [HarmonyPostfix]
		static void SwitchFlashlightPatch(FlashlightItem __instance)
		{
			if (__instance.flashlightTypeID == 1 && __instance.isBeingUsed && __instance.insertedBattery.charge > 0)
			{
                PlayerControllerB player = __instance.playerHeldBy;

                System.Random seed = new System.Random();
                Vector3 newposition3 = RoundManager.Instance.insideAINodes[seed.Next(0, RoundManager.Instance.insideAINodes.Length)].transform.position;
                newposition3 = RoundManager.Instance.GetRandomNavMeshPositionInBoxPredictable(newposition3, 10f, RoundManager.Instance.navHit, seed);

                Vector3 oldposition3 = GameNetworkManager.Instance.localPlayerController.gameObject.transform.position;
                Landmine.SpawnExplosion(oldposition3, spawnExplosionEffect: true, 0f, 0f);

                player.TeleportPlayer(newposition3);
                __instance.insertedBattery.charge = 0;
                __instance.flashlightAudio.PlayOneShot(__instance.flashlightClips[UnityEngine.Random.Range(0, __instance.flashlightClips.Length)]);

                Landmine.SpawnExplosion(newposition3, spawnExplosionEffect: true, 0f, 0f);

				// TODO:
				// Prevent usage in space (check if in space?)
				// Prevent weather and clock in factory (set player isInFactory to true?)
				// Fix explosion occurring for all players (oldposition3 might have an incorrect value?)
            }
        }
    }
}

/**

int playerObj, Vector3 teleportPos



PlayerControllerB playerControllerB = StartOfRound.Instance.allPlayerScripts[playerObj];
playerControllerB.isInElevator = false;
playerControllerB.isInHangarShipRoom = false;
playerControllerB.isInsideFactory = true;
playerControllerB.averageVelocity = 0f;
playerControllerB.velocityLastFrame = Vector3.zero;
StartOfRound.Instance.allPlayerScripts[playerObj].TeleportPlayer(teleportPos);
StartOfRound.Instance.allPlayerScripts[playerObj].beamOutParticle.Play();
shipTeleporterAudio.PlayOneShot(teleporterBeamUpSFX);
StartOfRound.Instance.allPlayerScripts[playerObj].movementAudio.PlayOneShot(teleporterBeamUpSFX);



if (RoundManager.Instance.insideAINodes.Length != 0)
{
	Vector3 position3 = RoundManager.Instance.insideAINodes[shipTeleporterSeed.Next(0, RoundManager.Instance.insideAINodes.Length)].transform.position;
	Debug.DrawRay(position3, Vector3.up * 1f, Color.red);
	position3 = RoundManager.Instance.GetRandomNavMeshPositionInBoxPredictable(position3, 10f, default(NavMeshHit), shipTeleporterSeed);
	Debug.DrawRay(position3 + Vector3.right * 0.01f, Vector3.up * 3f, Color.green);
	SetPlayerTeleporterId(playerControllerB, 2);
	if (playerControllerB.deadBody != null)
	{
		TeleportPlayerBodyOutServerRpc((int)playerControllerB.playerClientId, position3);
		continue;
	}
	TeleportPlayerOutWithInverseTeleporter((int)playerControllerB.playerClientId, position3);
	TeleportPlayerOutServerRpc((int)playerControllerB.playerClientId, position3);
}



public void TeleportPlayer(Vector3 pos, bool withRotation = false, float rot = 0f, bool allowInteractTrigger = false, bool enableController = true)
{
	if (base.IsOwner && !allowInteractTrigger)
	{
		CancelSpecialTriggerAnimations();
	}
	else if (!allowInteractTrigger && currentTriggerInAnimationWith != null)
	{
		currentTriggerInAnimationWith.onCancelAnimation.Invoke(this);
		currentTriggerInAnimationWith.SetInteractTriggerNotInAnimation();
	}
	if ((bool)inAnimationWithEnemy)
	{
		inAnimationWithEnemy.CancelSpecialAnimationWithPlayer();
	}
	StartOfRound.Instance.playerTeleportedEvent.Invoke(this);
	if (withRotation)
	{
		targetYRot = rot;
		base.transform.eulerAngles = new Vector3(0f, targetYRot, 0f);
	}
	serverPlayerPosition = pos;
	thisController.enabled = false;
	base.transform.position = pos;
	if (enableController)
	{
		thisController.enabled = true;
	}
	teleportingThisFrame = true;
	teleportedLastFrame = true;
	timeSinceTakingGravityDamage = 1f;
	averageVelocity = 0f;
	if (!isUnderwater && !isSinking)
	{
		return;
	}
	QuicksandTrigger[] array = Object.FindObjectsByType<QuicksandTrigger>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
	for (int i = 0; i < array.Length; i++)
	{
		if (array[i].sinkingLocalPlayer)
		{
			array[i].OnExit(base.gameObject.GetComponent<Collider>());
			break;
		}
	}
}
**/