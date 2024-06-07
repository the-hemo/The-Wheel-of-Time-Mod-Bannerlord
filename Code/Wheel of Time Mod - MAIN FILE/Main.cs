using HarmonyLib;
using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Core;

using TaleWorlds.CampaignSystem;

using TaleWorlds.Library;
using System.Collections.Generic;
using System.Reflection;

using System.IO;
using TaleWorlds.Engine.Screens;

using SandBox;
using Newtonsoft.Json.Linq;
using WoT_Main.Support;

using WoT_Main.Behaviours;
using static WoT_Main.campaignMapPatch;
using TaleWorlds.ModuleManager;
using TaleWorlds.Engine;
using System.Linq;
using System.Xml;

using WoT.Shotgun;
using TaleWorlds.Localization;

namespace WoT_Main
{
    public class Main : MBSubModuleBase
    {
        private JObject config;
        public static string MainMapName = "modded_main_map";


        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {

            base.OnBeforeInitialModuleScreenSetAsRoot();
            //Just for confirmation
           

        }

        protected override void OnSubModuleLoad()
        {

            base.OnSubModuleLoad();


            try
            {
                string assemblyFolder = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string path = System.IO.Path.Combine(assemblyFolder, "config.json");
                this.config = JObject.Parse(File.ReadAllText(System.IO.Path.GetFullPath(path)));
            }
            catch (Exception)
            {
            }

            
            
            //Removing Start screen options which are not needed for the mod
            TextObject coreContentDisabledReason = new TextObject("Disabled during installation.", null);

            startScreenSupport.removeInitialStateOption("CustomBattle");
            startScreenSupport.removeInitialStateOption("StoryModeNewGame");
;
            startScreenSupport.removeInitialStateOption("SandBoxNewGame");
            
            //Adds out custom gamemode, currently not much
            TaleWorlds.MountAndBlade.Module.CurrentModule.AddInitialStateOption(new InitialStateOption("SandBoxNewGame", new TextObject("New WoT Campaign", null), 3, delegate ()
            {
                MBGameManager.StartNewGame(new WoTCampaignManager());
            }, () => new ValueTuple<bool, TextObject>(TaleWorlds.MountAndBlade.Module.CurrentModule.IsOnlyCoreContentEnabled, coreContentDisabledReason)));

            
            Harmony harmony = new Harmony("WoT_Main.HarmonyPatches");
            Harmony.DEBUG = true;
            harmony.PatchAll();
        }
        

        public override void OnMissionBehaviorInitialize(Mission mission)
        {

            base.OnMissionBehaviorInitialize(mission);

            //Mission behaviour which disables friendly fire
           // mission.AddMissionBehavior(new NoFriendlyFire());
            //mission.AddMissionBehavior(new ChannelingSound());
            mission.AddMissionBehavior(new DeathBarrier2());
            mission.AddMissionBehavior(new ShotgunEquipmentMissionLogic());
           // mission.AddMissionBehavior(new ShotgunCheatMissionLogic());

        }
      
        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {

            base.OnGameStart(game, gameStarterObject);

            /*if(game.GameType is Campaign)
            {

                CampaignGameStarter starter = gameStarterObject as CampaignGameStarter;

                
                

                //starter.AddBehavior(constantWars);
                //starter.AddBehavior(new ShayolGhulCaptureMechanic());
                //starter.AddBehavior(new RandomEvents(constantWars));
                starter.AddBehavior(new ShadowAlwaysAtWar());


                starter.AddBehavior(new PartyMapStuckFix());

                
            }*/
        }  
        
 
        
        private (bool, TextObject) menuFunc()
        {
            return (false, null);
        }

       
    }
    public class LogWriter
    {
        private string m_exePath = string.Empty;
        public LogWriter(string logMessage)
        {
            LogWrite(logMessage);
        }
        public void LogWrite(string logMessage)
        {
            m_exePath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            try
            {
                using (StreamWriter w = File.AppendText(m_exePath + "\\" + "log.txt"))
                {
                    Log(logMessage, w);
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void Log(string logMessage, TextWriter txtWriter)
        {
            try
            {
                txtWriter.Write("\r\nLog Entry : ");
                txtWriter.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                    DateTime.Now.ToLongDateString());
                txtWriter.WriteLine("  :");
                txtWriter.WriteLine("  :{0}", logMessage);
                txtWriter.WriteLine("-------------------------------");
            }
            catch (Exception ex)
            {
            }
        }
    }
}
