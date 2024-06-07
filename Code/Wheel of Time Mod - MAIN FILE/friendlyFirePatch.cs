using HarmonyLib;
using SandBox.GameComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;

namespace WoT_Main
{
	[HarmonyPatch(typeof(SandboxAgentApplyDamageModel), "CalculateDamage")]
	class FriendlyFirePatch
	{
		
		[HarmonyPrefix]
		private static bool Prefix(ref float __result, ref AttackInformation attackInformation)
		{
			if (attackInformation.AttackerFormation != null && attackInformation.VictimFormation != null && attackInformation.AttackerFormation.Team != null && attackInformation.VictimFormation.Team != null && attackInformation.AttackerFormation.Team.Side.Equals(attackInformation.VictimFormation.Team.Side))
			{
				__result = 0f;
				return false;
			}
			else
			{
				return true;
			}
			
		}
	}
}
