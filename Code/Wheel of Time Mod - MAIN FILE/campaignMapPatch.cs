using HarmonyLib;
using Newtonsoft.Json.Linq;
using SandBox;
using SandBox.View.Map;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using WoT_Main.Support;
using System.IO;
using System.Reflection;
using Extensions = TaleWorlds.Core.Extensions;
using Helpers;
using SandBox.Tournaments.MissionLogics;
using TaleWorlds.CampaignSystem.TournamentGames;

namespace WoT_Main
{
    internal class campaignMapPatch
    {
        [HarmonyPatch(typeof(MapScene), "Load")]
        public static class MainScenePatch
        {
            // Token: 0x06000005 RID: 5 RVA: 0x000021C8 File Offset: 0x000003C8
            private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                int truthOccurance = -1;
                bool truthFlag = false;
                foreach (CodeInstruction instruction in instructions)
                {
                    bool flag = instruction.opcode == OpCodes.Ldstr && instruction.OperandIs("Main_map");
                    if (flag)
                    {
                        instruction.operand = "modded_main_map";
                    }
                    else
                    {
                        bool flag2 = instruction.opcode == OpCodes.Ldloca_S;
                        if (flag2)
                        {
                            int num = truthOccurance;
                            truthOccurance = num + 1;
                            truthFlag = true;
                        }
                        else
                        {
                            bool flag3 = instruction.opcode == OpCodes.Stfld;
                            if (flag3)
                            {
                                truthFlag = false;
                            }
                            else
                            {
                                bool flag4 = instruction.opcode == OpCodes.Ldc_I4_0 && truthFlag && (truthOccurance == 1 || truthOccurance == 3);
                                if (flag4)
                                {
                                    instruction.opcode = OpCodes.Ldc_I4_1;
                                }
                            }
                        }
                    }
                    yield return instruction;
                    //instruction = null;
                }
                IEnumerator<CodeInstruction> enumerator = null;
                yield break;
                yield break;
            }
        }
        [HarmonyPatch]
        public static class GetBattleSceneForMapPatchPatch
        {

            [HarmonyPrefix]
            [HarmonyPatch(typeof(PlayerEncounter), "GetBattleSceneForMapPatch")]
            public static bool PreFix(ref MapPatchData mapPatch, ref string __result)
            {
                PathFaceRecord faceIndex = Campaign.Current.MapSceneWrapper.GetFaceIndex(MobileParty.MainParty.Position2D);


                int id = faceIndex.FaceGroupIndex;

                    string sceneID = "n";


                    try
                    {


                        string assemblyFolder = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                        string path = System.IO.Path.Combine(assemblyFolder, "config.json");
                        JObject config = JObject.Parse(File.ReadAllText(System.IO.Path.GetFullPath(path)));

                        string[] sceneIds = null;


                        switch (id)
                        {
                            case 1: sceneIds = config.GetValue("faceIndex1").ToObject<string[]>(); break;
                            case 2: sceneIds = config.GetValue("faceIndex2").ToObject<string[]>(); break;
                            case 4: sceneIds = config.GetValue("faceIndex4").ToObject<string[]>(); break;
                            case 5: sceneIds = config.GetValue("faceIndex5").ToObject<string[]>(); break;
                            case 7: sceneIds = config.GetValue("faceIndex7").ToObject<string[]>(); break;

                            case 10: sceneIds = config.GetValue("faceIndex10").ToObject<string[]>(); break;
                            case 50: sceneIds = config.GetValue("faceIndex50").ToObject<string[]>(); break;
                            case 51: sceneIds = config.GetValue("faceIndex51").ToObject<string[]>(); break;
                            case 52: sceneIds = config.GetValue("faceIndex52").ToObject<string[]>(); break;
                            case 53: sceneIds = config.GetValue("faceIndex53").ToObject<string[]>(); break;
                            case 54: sceneIds = config.GetValue("faceIndex54").ToObject<string[]>(); break;
                            case 55: sceneIds = config.GetValue("faceIndex55").ToObject<string[]>(); break;
                            case 56: sceneIds = config.GetValue("faceIndex56").ToObject<string[]>(); break;
                            case 57: sceneIds = config.GetValue("faceIndex57").ToObject<string[]>(); break;
                            case 58: sceneIds = config.GetValue("faceIndex58").ToObject<string[]>(); break;
                            case 59: sceneIds = config.GetValue("faceIndex59").ToObject<string[]>(); break;
                            default: config.GetValue("faceIndex1").ToObject<string[]>(); break;
                        }



                        if (sceneID == "n" && sceneIds != null)
                        {
                            sceneID = sceneIds.GetRandomElement();
                        }

                    }
                    catch (Exception)
                    {
                        campaignSupport.displayMessageInChat("config wasn't loaded", Colors.Red);
                    }


                    //__result = "scn_test";
                    LogWriter l = new LogWriter(sceneID);
                    __result = sceneID;

                    return false;
             

                
            }
        }


         [HarmonyPatch]
         public static class DefaultMapDistanceModelPatch
         {


             [HarmonyPrefix]
             [HarmonyPatch(typeof(DefaultMapDistanceModel), "GetDistance", new Type[] { typeof(Settlement), typeof(Settlement) })]
             public static bool HarmonyPrefix(ref Settlement fromSettlement, ref Settlement toSettlement, ref float __result)
             {

                 if (fromSettlement != null && toSettlement != null)
                 {
                     return true;
                 }
                 else
                 {
                     __result = 0;
                     return false;


                 }

             }


         }
        [HarmonyPatch]
        public static class DefaultMapDistanceModelPatch2
        {


            [HarmonyPrefix]
            [HarmonyPatch(typeof(DefaultMapDistanceModel), "GetClosestSettlementForNavigationMesh", new Type[] { typeof(PathFaceRecord)  })]
            public static bool HarmonyPrefix(ref PathFaceRecord face,ref DefaultMapDistanceModel __instance, ref Settlement __result)
            {

                Dictionary<int, Settlement> _navigationMeshClosestSettlementCache = (Dictionary<int, Settlement>)typeof(DefaultMapDistanceModel).GetField("_navigationMeshClosestSettlementCache", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
                List<Settlement> _settlementsToConsider = (List<Settlement>) typeof(DefaultMapDistanceModel).GetField("_settlementsToConsider", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

                Settlement settlement = null;
                if (!_navigationMeshClosestSettlementCache.TryGetValue(face.FaceIndex, out settlement))
                {
                    Vec2 navigationMeshCenterPosition = Campaign.Current.MapSceneWrapper.GetNavigationMeshCenterPosition(face);
                    float num = float.MaxValue;
                    foreach (Settlement settlement2 in _settlementsToConsider)
                    {
                        float num2 = settlement2.GatePosition.DistanceSquared(navigationMeshCenterPosition);
                        if (num > num2)
                        {
                            num = num2;
                            settlement = settlement2;
                        }
                    }
                    _navigationMeshClosestSettlementCache[face.FaceIndex] = settlement;
                }
                if(settlement == null)
                {
                    settlement = Settlement.All.GetRandomElement();
                }
                __result = settlement;

                return false;
            }

        }


        










        [HarmonyPatch]
        public static class GetChildPatch
        {


            [HarmonyPrefix]
            [HarmonyPatch(typeof(PartyVisual), "ValidateIsDirty")]
            public static bool HarmonyPrefix(ref float realDt, ref float dt, ref PartyVisual __instance)
            {


                FieldInfo fi = typeof(PartyVisual).GetField("_gateBannerEntitiesWithLevels", BindingFlags.NonPublic | BindingFlags.Instance);


                if (__instance.PartyBase.IsVisualDirty)
                {

                    MobileParty mobileParty = __instance.PartyBase.MobileParty;
                    if (((mobileParty != null) ? mobileParty.CurrentSettlement : null) != null)
                    {
                        PartyVisual partyVisual = PartyVisualManager.Current.GetVisualOfParty(__instance.PartyBase.MobileParty.CurrentSettlement.Party);
                        Dictionary<int, List<GameEntity>> gateBannerEntitiesWithLevels = (Dictionary<int, List<GameEntity>>)fi.GetValue(partyVisual);
                        if (!__instance.PartyBase.MobileParty.MapFaction.IsAtWarWith(__instance.PartyBase.MobileParty.CurrentSettlement.MapFaction) && gateBannerEntitiesWithLevels != null && !Extensions.IsEmpty<KeyValuePair<int, List<GameEntity>>>(gateBannerEntitiesWithLevels))
                        {
                            Hero leaderHero = __instance.PartyBase.LeaderHero;
                            if (((leaderHero != null) ? leaderHero.ClanBanner : null) != null)
                            {


                                int num = 0;
                                foreach (MobileParty mobileParty2 in __instance.PartyBase.MobileParty.CurrentSettlement.Parties)
                                {
                                    if (mobileParty2 == __instance.PartyBase.MobileParty)
                                    {
                                        break;
                                    }
                                    Hero leaderHero2 = mobileParty2.LeaderHero;
                                    if (((leaderHero2 != null) ? leaderHero2.ClanBanner : null) != null)
                                    {
                                        num++;
                                    }
                                }
                                int wallLevel = __instance.PartyBase.MobileParty.CurrentSettlement.Town.GetWallLevel();
                                int count = gateBannerEntitiesWithLevels[wallLevel].Count;

                                if(count == 0)
                                {
                                    return false;
                                }
                                GameEntity gameEntity = gateBannerEntitiesWithLevels[wallLevel][num % count];
                                
                            }
                        }

                    }


                    return true;

                }
                return true;
            }


        }


      
        }

    /*[HarmonyPatch]
    public static class TournamentsPatch
    {


        [HarmonyPrefix]
        [HarmonyPatch(typeof(TournamentFightMissionController), "PrepareForMatch")]
        public static bool HarmonyPrefix(ref TournamentFightMissionController __instance)

        {
            MethodInfo GetTeamWeaponEquipmentList = typeof(TournamentFightMissionController).GetMethod("GetTeamWeaponEquipmentList", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo AddRandomClothes = typeof(TournamentFightMissionController).GetMethod("AddRandomClothes", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo _match = typeof(TournamentFightMissionController).GetField("_match", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo _culture = typeof(TournamentFightMissionController).GetField("_culture", BindingFlags.NonPublic | BindingFlags.Instance);
            TournamentMatch match = ((TournamentMatch)_match.GetValue(__instance));
            List<Equipment> teamWeaponEquipmentList = (List<Equipment>)GetTeamWeaponEquipmentList.Invoke(__instance, new object[] { match.Teams.First<TournamentTeam>().Participants.Count<TournamentParticipant>() });

            foreach (TournamentTeam tournamentTeam in match.Teams)
            {
                int num = 0;
                foreach (TournamentParticipant tournamentParticipant in tournamentTeam.Participants)
                {
                    LogWriter l = new LogWriter("num: " + num + " teamWeaponEquipmentList length: " + teamWeaponEquipmentList.Count());

                    tournamentParticipant.MatchEquipment = teamWeaponEquipmentList[num].Clone(false);
                    AddRandomClothes.Invoke(__instance, new object[] { _culture.GetValue(__instance), tournamentParticipant });
                    num++;
                }
            }

            return false;

        }


    }*/

}

/*
{
    "faceIndex50": [ "scn_Wotbattlescene1", "scn_Wotbattlescene3Halfforestbarren", "wot_barren_blight_variant_a", "wot_barren_blight_variant_b", "wot_barren_blight_variant_c"],
  "faceIndex51": [ "scn_Wotbattlescene1", "scn_Wotbattlescene3Halfforestbarren", "wot_barren_blight_variant_a", "wot_barren_blight_variant_b", "wot_barren_blight_variant_c" ],
  "faceIndex52": [ "wot_barren_blight_variant_a", "wot_barren_blight_variant_b", "wot_barren_blight_variant_c" ],
  "faceIndex53": [ "scn_Wotbattlescene1", "scn_Wotbattlescene3Halfforestbarren", "wot_barren_blight_variant_a", "wot_barren_blight_variant_b", "wot_barren_blight_variant_c" ],
  "faceIndex54": [ "scn_Wotbattlescene2Blightforest2", "wot_forest_blight_variant_a" , "wot_forest_blight_variant_b"],
  "faceIndex55": [ "scn_Wotbattlescene2Blightforest2", "wot_forest_blight_variant_a", "wot_forest_blight_variant_b" ],
  "faceIndex56": [ "scn_Wotbattlescene4tropicalswamp", "scn_Wotbattlescene4tropicalswamp"],
  "faceIndex57": [ "wot_barren_blight_variant_a", "wot_barren_blight_variant_b" ],
  "faceIndex58": [ "scn_Wotbattlescene4tropicalswamp", "scn_Wotbattlescene4tropicalswamp"],
  "faceIndex59": [ "scn_Wotbattlescene4tropicalswamp", "scn_Wotbattlescene4tropicalswamp" ],

  "faceIndex2": [ "battle_terrain_g", "battle_terrain_010", "battle_terrain_b", "battle_terrain_d", "battle_terrain_008", "battle_terrain_009", "battle_terrain_biome_067" ],
  "faceIndex10": [ "battle_terrain_f", "battle_terrain_s", "battle_terrain_011" ],
  "faceIndex5": [ "battle_terrain_017", "battle_terrain_biome_044", "battle_terrain_biome_076", "battle_terrain_biome_040", "battle_terrain_biome_075" ],
  "faceIndex1": [ "battle_terrain_a", "battle_terrain_f", "battle_terrain_h", "battle_terrain_J", "battle_terrain_L", "battle_terrain_m", "battle_terrain_n", "battle_terrain_t", "battle_terrain_z", "battle_terrain_003" ],
  "faceIndex4": [ "battle_terrain_h", "battle_terrain_k", "battle_terrain_001", "battle_terrain_004" ],
  "faceIndex7": [ "battle_terrain_004", "battle_terrain_020", "battle_terrain_h", "battle_terrain_O" ]
  
}
forrest battle_terrain_003 battle_terrain_001 battle_terrain_004 battle_terrain_024 battle_terrain_025 battle_terrain_026 battle_terrain_028 battle_terrain_029
swamp battle_terrain_005 battle_terrain_034 battle_terrain_035
plain/mountain battle_terrain_006 battle_terrain_007 battle_terrain_031 battle_terrain_033
desert/mountain battle_terrain_008 battle_terrain_009
desert battle_terrain_010 battle_terrain_022
forrest/plain battle_terrain_011 battle_terrain_016 battle_terrain_019  battle_terrain_021 battle_terrain_023 battle_terrain_027
plain/steppe battle_terrain_012 battle_terrain_013 battle_terrain_014 battle_terrain_015 battle_terrain_017 battle_terrain_018 battle_terrain_020 
plain battle_terrain_030 battle_terrain_032
broken battle_terrain_002


5 is forrest

 
 
 
 */