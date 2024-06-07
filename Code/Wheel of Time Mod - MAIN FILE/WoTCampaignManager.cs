using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;
using TaleWorlds.MountAndBlade;
using TaleWorlds.SaveSystem.Load;
using WoT_Main;

namespace SandBox
{
    // Token: 0x02000025 RID: 37
    public class WoTCampaignManager : MBGameManager
    {
        // Token: 0x060000FA RID: 250 RVA: 0x00006E57 File Offset: 0x00005057
        public WoTCampaignManager()
        {
            this._loadingSavedGame = false;
        }

        // Token: 0x060000FB RID: 251 RVA: 0x00006E66 File Offset: 0x00005066
        public WoTCampaignManager(LoadResult loadedGameResult)
        {
            this._loadingSavedGame = true;
            this._loadedGameResult = loadedGameResult;
        }

        // Token: 0x060000FC RID: 252 RVA: 0x00006E7C File Offset: 0x0000507C
        public override void OnGameEnd(Game game)
        {
            MBDebug.SetErrorReportScene(null);
            base.OnGameEnd(game);
        }

        // Token: 0x060000FD RID: 253 RVA: 0x00006E8C File Offset: 0x0000508C
        protected override void DoLoadingForGameManager(GameManagerLoadingSteps gameManagerLoadingStep, out GameManagerLoadingSteps nextStep)
        {
            nextStep = GameManagerLoadingSteps.None;
            switch (gameManagerLoadingStep)
            {
                case GameManagerLoadingSteps.PreInitializeZerothStep:
                    nextStep = GameManagerLoadingSteps.FirstInitializeFirstStep;
                    return;
                case GameManagerLoadingSteps.FirstInitializeFirstStep:
                    MBGameManager.LoadModuleData(this._loadingSavedGame);
                    nextStep = GameManagerLoadingSteps.WaitSecondStep;
                    return;
                case GameManagerLoadingSteps.WaitSecondStep:
                    if (!this._loadingSavedGame)
                    {
                        MBGameManager.StartNewGame();
                    }
                    nextStep = GameManagerLoadingSteps.SecondInitializeThirdState;
                    return;
                case GameManagerLoadingSteps.SecondInitializeThirdState:
                    MBGlobals.InitializeReferences();
                    if (!this._loadingSavedGame)
                    {
                        MBDebug.Print("Initializing new game begin...", 0, Debug.DebugColor.White, 17592186044416UL);
                        Campaign campaign = new Campaign(CampaignGameMode.Campaign);
                        Game.CreateGame(campaign, this);
                        campaign.SetLoadingParameters(Campaign.GameLoadingType.NewCampaign);
                        MBDebug.Print("Initializing new game end...", 0, Debug.DebugColor.White, 17592186044416UL);
                    }
                    else
                    {
                        MBDebug.Print("Initializing saved game begin...", 0, Debug.DebugColor.White, 17592186044416UL);
                        ((Campaign)Game.LoadSaveGame(this._loadedGameResult, this).GameType).SetLoadingParameters(Campaign.GameLoadingType.SavedCampaign);
                        this._loadedGameResult = null;
                        Common.MemoryCleanupGC(false);
                        MBDebug.Print("Initializing saved game end...", 0, Debug.DebugColor.White, 17592186044416UL);
                    }
                    Game.Current.DoLoading();
                    nextStep = GameManagerLoadingSteps.PostInitializeFourthState;
                    return;
                case GameManagerLoadingSteps.PostInitializeFourthState:
                    {
                        bool flag = true;
                        foreach (MBSubModuleBase mbsubModuleBase in Module.CurrentModule.SubModules)
                        {
                            flag = (flag && mbsubModuleBase.DoLoading(Game.Current));
                        }
                        nextStep = (flag ? GameManagerLoadingSteps.FinishLoadingFifthStep : GameManagerLoadingSteps.PostInitializeFourthState);
                        return;
                    }
                case GameManagerLoadingSteps.FinishLoadingFifthStep:
                    nextStep = (Game.Current.DoLoading() ? GameManagerLoadingSteps.None : GameManagerLoadingSteps.FinishLoadingFifthStep);
                    return;
                default:
                    return;
            }
        }
        
        // Token: 0x060000FE RID: 254 RVA: 0x00007008 File Offset: 0x00005208
        public override void OnAfterCampaignStart(Game game)
        {
        }

        // Token: 0x060000FF RID: 255 RVA: 0x0000700C File Offset: 0x0000520C
        public override void OnLoadFinished()
        {
            if (!this._loadingSavedGame)
            {
                
                this.LaunchSandboxCharacterCreation();
                
            }
            else
            {
                Game.Current.GameStateManager.OnSavedGameLoadFinished();
                Game.Current.GameStateManager.CleanAndPushState(Game.Current.GameStateManager.CreateState<MapState>(), 0);
                MapState mapState = Game.Current.GameStateManager.ActiveState as MapState;
                string text5 = ((mapState != null) ? mapState.GameMenuId : null);
                if (!string.IsNullOrEmpty(text5))
                {
                    PlayerEncounter playerEncounter = PlayerEncounter.Current;
                    if (playerEncounter != null)
                    {
                        playerEncounter.OnLoad();
                    }
                    Campaign.Current.GameMenuManager.SetNextMenu(text5);
                }
                PartyBase.MainParty.SetVisualAsDirty();
                Campaign.Current.CampaignInformationManager.OnGameLoaded();
                foreach (Settlement settlement in Settlement.All)
                {
                    settlement.Party.SetLevelMaskIsDirty();
                }
                CampaignEventDispatcher.Instance.OnGameLoadFinished();
                if (mapState != null)
                {
                    mapState.OnLoadingFinished();
                }
            }
            base.IsLoaded = true;
        }

        // Token: 0x06000100 RID: 256 RVA: 0x000071CC File Offset: 0x000053CC
        private void LaunchSandboxCharacterCreation()
        {
            CharacterCreationState characterCreationState = Game.Current.GameStateManager.CreateState<CharacterCreationState>(new object[]
            {
                new WoTCharacterCreation()
            });
            Game.Current.GameStateManager.CleanAndPushState(characterCreationState, 0);
        }

        // Token: 0x06000101 RID: 257 RVA: 0x00007208 File Offset: 0x00005408
        [CrashInformationCollector.CrashInformationProvider]
        private static CrashInformationCollector.CrashInformation UsedModuleInfoCrashCallback()
        {
            Campaign campaign = Campaign.Current;
            if (((campaign != null) ? campaign.PreviouslyUsedModules : null) != null)
            {
                string[] moduleNames = SandBoxManager.Instance.ModuleManager.ModuleNames;
                MBList<ValueTuple<string, string>> mblist = new MBList<ValueTuple<string, string>>();
                using (List<string>.Enumerator enumerator = Campaign.Current.PreviouslyUsedModules.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        string module = enumerator.Current;
                        bool flag = moduleNames.FindIndex((string x) => x == module) != -1;
                        mblist.Add(new ValueTuple<string, string>(module, flag ? "1" : "0"));
                    }
                }
                return new CrashInformationCollector.CrashInformation("Used Mods", mblist);
            }
            return null;
        }

        // Token: 0x06000102 RID: 258 RVA: 0x000072D8 File Offset: 0x000054D8
        [CrashInformationCollector.CrashInformationProvider]
        private static CrashInformationCollector.CrashInformation UsedGameVersionsCallback()
        {
            Campaign campaign = Campaign.Current;
            if (((campaign != null) ? campaign.UsedGameVersions : null) != null)
            {
                MBList<ValueTuple<string, string>> mblist = new MBList<ValueTuple<string, string>>();
                for (int i = 0; i < Campaign.Current.UsedGameVersions.Count; i++)
                {
                    string text = "";
                    if (i < Campaign.Current.UsedGameVersions.Count - 1 && ApplicationVersion.FromString(Campaign.Current.UsedGameVersions[i], 38256) > ApplicationVersion.FromString(Campaign.Current.UsedGameVersions[i + 1], 38256))
                    {
                        text = "Error";
                    }
                    mblist.Add(new ValueTuple<string, string>(Campaign.Current.UsedGameVersions[i], text));
                }
                return new CrashInformationCollector.CrashInformation("Used Game Versions", mblist);
            }
            return null;
        }

        // Token: 0x04000048 RID: 72
        private bool _loadingSavedGame;

        // Token: 0x04000049 RID: 73
        private LoadResult _loadedGameResult;
    }
}
