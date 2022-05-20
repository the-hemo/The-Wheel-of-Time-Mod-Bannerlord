﻿using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Library;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;

namespace WoT_Main.Support
{
    class startScreenSupport
    {
        public static void removeInitialStateOption(String name)
        {
            try
            {

                FieldInfo fieldInfo = typeof(TaleWorlds.MountAndBlade.Module).GetField("_initialStateOptions", BindingFlags.NonPublic | BindingFlags.Instance);
                var v = fieldInfo.GetValue(TaleWorlds.MountAndBlade.Module.CurrentModule);
                List<InitialStateOption> newInitialStateOptionList;

                if (v.GetType() == typeof(List<InitialStateOption>))
                {
                    newInitialStateOptionList = (List<InitialStateOption>)v;
                }
                else
                {
                    return;
                }
                foreach (InitialStateOption initialStateOption1 in newInitialStateOptionList)
                {
                    if (initialStateOption1.Id.Contains(name))
                    {

                        newInitialStateOptionList.Remove(initialStateOption1);

                    }

                }
                fieldInfo.SetValue(typeof(TaleWorlds.MountAndBlade.Module).GetField("_initialStateOptions", BindingFlags.NonPublic | BindingFlags.Instance), newInitialStateOptionList);
            }
            catch (Exception ex)
            {
                
                campaignSupport.displayMessageInChat(ex.Message, Color.White);
            }
        }
        public static void removeGameModel(IGameStarter gameStarter)
        {
            try
            {

                FieldInfo fieldInfo = typeof(CampaignGameStarter).GetField("_models", BindingFlags.NonPublic | BindingFlags.Instance);
                var v = fieldInfo.GetValue(gameStarter);
                List<GameModel> newGameModels;

                if (v.GetType() == typeof(List<GameModel>))
                {
                    newGameModels = (List<GameModel>)v;
                }
                else
                {
                    return;
                }
                foreach (GameModel gameModel in newGameModels)
                {
                    if (gameModel.GetType() == typeof(DefaultMapDistanceModel) || gameModel.GetType() == typeof(MapDistanceModel))
                    {

                        newGameModels.Remove(gameModel);

                    }

                }
                fieldInfo.SetValue(typeof(CampaignGameStarter).GetField("_models", BindingFlags.NonPublic | BindingFlags.Instance), newGameModels);
            }
            catch (Exception ex)
            {

                campaignSupport.displayMessageInChat(ex.Message, Color.White);
            }
        }

        public static void changeInitialStateOptionName(String initialStateOptionName, String newName)
        {
            try
            {
                FieldInfo fieldInfo = typeof(TaleWorlds.MountAndBlade.Module).GetField("_initialStateOption", BindingFlags.NonPublic | BindingFlags.Instance);
                var v = fieldInfo.GetValue(TaleWorlds.MountAndBlade.Module.CurrentModule);
                if(v.GetType() == typeof(List<InitialStateOption>))
                {
                    List<InitialStateOption> initialStateOptions = (List<InitialStateOption>)v;
                    foreach(InitialStateOption initialStateOption in initialStateOptions)
                    {
                        if(initialStateOption.Name.ToString().Contains(initialStateOptionName))
                        {
                            typeof(InitialStateOption).GetProperty("Name", BindingFlags.Public | BindingFlags.Instance).SetValue(initialStateOption, new TextObject (newName, null));
                        }
                    }

                    fieldInfo.SetValue(TaleWorlds.MountAndBlade.Module.CurrentModule, initialStateOptions);
                }
                else
                {
                    return;
                }

            }
            catch (Exception ex)
            {

                campaignSupport.displayMessageInChat(ex.Message, Color.White);
            }
        }
    }
}
