using ColossalFramework;
using ColossalFramework.IO;
using ColossalFramework.UI;
using ICities;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using static ColossalFramework.Packaging.Package;
using ColossalFramework.Packaging;
using ColossalFramework.PlatformServices;

namespace HistoryGamePlayByMicroscraft
{
    //By Joel-iunius.ch
    //First version: 28.10.2022

    public class HistoryMod : IUserMod, ILoadingExtension
    {
        public static string source = "By Joel-iunius.ch First version: 28.10.2022 Steam page: https://joel-iunius.ch/historymod.html";

        public static UIHelperBase groupViews = null;

        public static List<UILabel> ViewLabels = new List<UILabel>();
        public static List<UIButton> ViewSeeViewBtns { get; set; } = new List<UIButton>();
        public static List<UIButton> ViewDeleteViewBtns { get; set; } = new List<UIButton>();

        public static bool isDebug = false;
        public static String version = "1.02";

        public static bool wasAutoLoaded = false;
        public static String initiateScreenShoterFinalMsg = "";
        public static SaveGameMetaData ActualSaveGameMetaData = null;
        public static Package.Asset latestSaveGame = null;
        public static String watermark = "";
        public static double lastAddedViewTime = 0;
        public static double lastFocusViewTime = 0;
        public static bool screenshotActualCycle = false;
        public static String startFromThisSave = "";
        public static int screenshotsNamesSelectedOption = 0;
        public static int dayOrNightForScreenshots = 0;
        public static int HowManySaveLoadedSinceLastRestart = 1;
        public static int restartInterval = 0;
        public static bool onlyThisSave = true;
        public static bool CSLExport = false;
        public static bool sessionNameIsCityName = false;
        public static UILabel label;
        public static bool CSLMade = false;
        public static int ProceededView = -1;
        public static int nbOfFrames = -1;
        public static String mapPrefix = "";
        public static String beforeDate = "";
        public static String afterDate = "";
        public static int maxNumberOfSavesToScreenshot = 9999;
        public static int ignoreXNumberOfSavesInBetween = 0;
        public static bool saveWithSameCityNameOnly = true;
        public static bool autoContinueIfGameCrashedWhenRestarting = false;
        public static int mapDate = 0;
        public static UIComponent ViewsPanel = null;
        public static string cityName = "";
        public static string screenshotSessionName = "screenshot";
        public static Boolean isScreenShoterActive = false;
        public static int lastLoadedMapIndex = 0;
        public static List<Asset> savesToLoad = new List<Asset>();
        public static bool stopGameAfterScreenshotSession = false;


        public string Name
        {
            get { return "History Mode by Microscraft"; }
        }

        public string Description
        {
            get { return "History Mode allow you to take multiple screenshots of differents views from all or specific saves of your city automatically. It helps create timelapses of the evolution of your city."; }
        }
        public static void stopScreenShoter()
        {
            //Create a texture the size of the screen, RGB24 format
            if (HistoryMod.source.Equals(""))
            {

            }
            HistoryMod.log("stopScreenShoter");
            if (HistoryMod.label != null)
            {
                HistoryMod.label.Hide();
            }
            CameraController.FindObjectOfType<CameraController>().m_freeCamera = false;
            isScreenShoterActive = false;
            HistoryMod.CSLMade = false;
            HistoryMod.saveConfig("isScreenShoterActive", "false");
            
        }

        public static void restartGameIfCrash(bool active)
        {

            try
            {
                
                debug("restartGame b");
                log("restartGameIfCrash " + active);

                if (Application.platform == RuntimePlatform.OSXPlayer)
                {

                }
                else if (Application.platform == RuntimePlatform.WindowsPlayer)
                {

                    string HistoryModPath = DataLocation.localApplicationData + "\\" + "historyMod" + "\\";

                    if(DoesHistoryFolderExists())
                        if (active)
                        {
                            if(File.Exists(HistoryModPath + "stopBatchNow.txt"))
                                File.Delete(HistoryModPath + "stopBatchNow.txt");

                            String path = Application.dataPath;
                            path += "/../Cities.exe";

                            log("Trying to start a batch for " + path);

                            /*
                            File.WriteAllText(HistoryModPath + "autorestart.txt",
                                ":loop"+Environment.NewLine+
                                "if exist stopBatchNow.txt goto stop" + Environment.NewLine+
                                "tasklist / fi \"ImageName eq cities.exe\" / fo csv 2 > NUL | find / I \"cities.exe\" > NUL" +Environment.NewLine+
                                "if \"%ERRORLEVEL%\" == \"0\" (echo Program is running) else (goto startCities)" + Environment.NewLine+
                                "timeout / t 120" + Environment.NewLine+
                                "goto loop" + Environment.NewLine+
                                ":startCities" + Environment.NewLine+
                                "Start \"\" \""+ path + "\"" + Environment.NewLine+
                                ":stop");
                            */

                            /*
                            System.Diagnostics.Process p = new System.Diagnostics.Process();
                            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
                            info.FileName = "cmd.exe";
                            info.RedirectStandardInput = true;
                            info.UseShellExecute = false;

                            p.StartInfo = info;
                            p.Start();

                            using (StreamWriter sw = p.StandardInput)
                            {
                                if (sw.BaseStream.CanWrite)
                                {
                                    sw.WriteLine(":loop");
                                    sw.WriteLine("if exist stopBatchNow.txt goto stop");
                                    sw.WriteLine("tasklist / fi \"ImageName eq cities.exe\" / fo csv 2 > NUL | find / I \"cities.exe\" > NUL");
                                    sw.WriteLine("if \"%ERRORLEVEL%\" == \"0\" (echo Program is running) else (goto startCities)");
                                    sw.WriteLine("timeout / t 120");
                                    sw.WriteLine("goto loop");
                                    sw.WriteLine(":startCities");
                                    sw.WriteLine("Start \"\" \"" + path + "\"");
                                    sw.WriteLine(":stop");
                                }
                            }
                        
                             */
                            //Cities.exe                   32716 Console                    1 3,858,656 Ko
                            //Cities.exe                   32716 Console                    1 9,865,296 Ko
                            //Cities.exe                   32716 Console                    1 9,865,296 Ko

                            //for /l %a in (0) do (if exist "C:\Users\Joel Iunius\AppData\Local\Colossal Order\Cities_Skylines\historyMod\stopBatchNow.txt" (exit) else ((tasklist /fi "ImageName eq cities.exe" /fo csv 2>NUL | find /I "cities.exe">NUL) & (if "%ERRORLEVEL%"=="0" (timeout /t 120) else ((Start "" "C:\Program Files (x86)\Steam\steamapps\common\Cities_Skylines\Cities.exe") & exit))))
                            //the escape isnt really correct but it looks like its work like that and not with \\\" 0.o
                            String cmd = "/C start cmd /c \"(for /l %a in (0) do (if exist \"" + HistoryModPath + "stopBatchNow.txt\" ((echo stop auto restart>>\"" + HistoryModPath + "batchlogs.txt\") & exit) else (tasklist | find /I \"cities.exe\" &&  ((echo Cities run nicely>>\"" + HistoryModPath + "batchlogs.txt\") & (timeout /t 15)) || ((echo Starting Cities from crash>>\"" + HistoryModPath + "batchlogs.txt\") & (Start \"\" \"" + path + "\") & exit))))\"";
                            log("CMD:"+cmd);

                            System.Diagnostics.Process process = new System.Diagnostics.Process();
                            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                            startInfo.FileName = "cmd.exe";
                            startInfo.Arguments = cmd;
                            startInfo.UseShellExecute = true;
                            process.StartInfo = startInfo;
                            process.Start();
                            log("starting AutoRestart Programme");


                            //System.Diagnostics.Process.Start("CMD.exe", cmd);

                            debug("restartGame b");
                            //we execute the batch file, while it prepare to start the game, we will now close the game.
                            //System.Diagnostics.Process.Start(HistoryModPath + "autorestart.bat");
                        }
                        else
                        {
                            log("Stopping AutoRestart Programme");
                            File.WriteAllText(HistoryModPath + "stopBatchNow.txt", "will stop restarting process");
                        }




                }



            }
            catch (Exception ex)
            {
                log("AutoRestart programme wasn't able to start due to a crash: "+ex.ToString());
            }
        }

        public static void debug(String text)
        {
            if(HistoryMod.isDebug)
                Debug.Log(text);
        }

        public static void setTimeOfDay(bool night)
        {
            debug("set night: " + night);
            if (night)
            {
                debug("set night time");
                Singleton<SimulationManager>.instance.m_currentDayTimeHour = 02f;
                Singleton<SimulationManager>.instance.m_isNightTime = true;                
                DayNightProperties.instance.m_TimeOfDay = 02f;


                // DayNightProperties.instance.Refresh();

                uint num = (uint)(02f / 24f * (float)SimulationManager.DAYTIME_FRAMES);
                uint currentFrameIndex = Singleton<SimulationManager>.instance.m_currentFrameIndex;
                uint dayTimeOffsetFrames = (num - currentFrameIndex) & (SimulationManager.DAYTIME_FRAMES - 1);
                Singleton<SimulationManager>.instance.m_dayTimeOffsetFrames = dayTimeOffsetFrames;


                debug("set night time end");
            }
            else
            {
                debug("set day time");
                Singleton<SimulationManager>.instance.m_currentDayTimeHour = 12f;
                Singleton<SimulationManager>.instance.m_isNightTime = false;
                DayNightProperties.instance.m_TimeOfDay = 12f;

                uint num = (uint)(14f / 24f * (float)SimulationManager.DAYTIME_FRAMES);
                uint currentFrameIndex = Singleton<SimulationManager>.instance.m_currentFrameIndex;
                uint dayTimeOffsetFrames = (num - currentFrameIndex) & (SimulationManager.DAYTIME_FRAMES - 1);
                Singleton<SimulationManager>.instance.m_dayTimeOffsetFrames = dayTimeOffsetFrames;
                 
                debug("set day time end");
            }

        }

        public static bool isKeyComboPressed(String action)
        {
            switch (action)
            {
                case "addView":
                    if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.V) && Input.GetKey(KeyCode.LeftAlt))// && Input.GetKey(KeyCode.RightControl))
                    {
                        log("Shortcut to add a View used");
                        return true;
                    }
                    break;
                case "stopRendering":
                    if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.X) && Input.GetKey(KeyCode.LeftAlt))// && Input.GetKey(KeyCode.RightControl))
                    {
                        log("Shortcut to stop randering was used");
                        return true;
                    }
                    break;
                case "seeView":
                    if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt))// && Input.GetKey(KeyCode.RightControl))
                    {
                        return true;
                    }
                    break;
                default:
                    return false;
            }
            return false;
        }

        public static void closeAllPanels()
        {
            //while (UIView.GetModalComponent() != null)
            //{ make it crash
            //    UIView.GetModalComponent().Hide();
            //}

            //do not try to close confirm here as it will already be closed. otherwise crash
            try
            {
                debug("Try popModal 3");

                if (Singleton<OptionsMainPanel>.instance != null)
                {
                    Singleton<OptionsMainPanel>.instance.OnClosed();
                }
                if (Singleton<UnlockingPanel>.instance != null) // not working ??
                {
                    Singleton<UnlockingPanel>.instance.OnClosed();
                }

                debug("Try popModal");

                while(UIView.GetModalComponent() != null)
                {
                    UIView.PopModal();
                }
                UIView.GetAView().panelsLibraryModalEffect.Hide()
                    ;
                debug("Try popModal Succcess");

            }
            catch (Exception ex)
            {
                debug("Exception closeAllPanels");
            }

        }
        public static void resumeScreenShoter()
        {
            if (canScreenshotNow())
            {
                debug("HISTORYMOD: resumeScreenShoter.");

                isScreenShoterActive = true;
                HistoryMod.saveConfig("isScreenShoterActive", "true");
                saveConfig("lastScreenshoterSessionName", HistoryMod.screenshotSessionName);
                closeAllPanels();
                SimulationManager.instance.SimulationPaused = true;
                HistoryMod.ProceededView = 0;//when >= 0 it will proceed to the mecansim of screenshot in the frames observer.
                HistoryMod.nbOfFrames = 0;

                HistoryMod.dayOrNightForScreenshots = int.Parse(getConfig("dayOrNightForScreenshots", "0"));
                if (HistoryMod.dayOrNightForScreenshots == 0)
                    HistoryMod.screenshotActualCycle = false;
                else if (HistoryMod.dayOrNightForScreenshots == 2)
                    HistoryMod.screenshotActualCycle = false;
                else if (HistoryMod.dayOrNightForScreenshots == 1)
                    HistoryMod.screenshotActualCycle = true;

                if (HistoryMod.label == null)
                {
                    HistoryMod.label = GameObject.Find("OptionsBar").GetComponent<UIPanel>().AddUIComponent<UILabel>();
                    HistoryMod.label.relativePosition += new Vector3(-100f, -20f, 0f);
                }
                HistoryMod.label.text = "HistoryMod: screenshot Session initizialised: [starting... ("+ getViewsCount()+ ")] "+(getConfig("CSLExport","false").Equals("false") ? "":"IF THIS STAY STUCK: CSL MAP EXPORT MAY HAVE FAILED due to you not being subscribe to the mod, subscribe and restart.");
                HistoryMod.label.Show();

                HistoryMod.log("resumeScreenShoter");
            }
        }
        
        public static void clearAllViews()
        {
            HistoryMod.saveConfig("views", "");
            refreshViewBtns();
        }

        public static void removeView(int Index)
        {
            if (Index < getViewsCount())
            {
                log("a view was removed");
                String CRD = HistoryMod.getConfig("views", "");
                String[] CRDSTRINGS = CRD.Split(new string[] { "[nextView]" }, StringSplitOptions.None);
                List<string> CRDS = new List<String>(CRDSTRINGS);
                CRDS.RemoveAt((Index+1));
                CRD = string.Join("[nextView]", CRDS.ToArray());
                HistoryMod.saveConfig("views", CRD);
                refreshViewBtns();
            }
        }
        public static int getViewsCount()
        {
            String CRD = HistoryMod.getConfig("views", "");
            if (CRD.Contains("[nextView]"))
            {
                return CRD.Split(new string[] { "[nextView]" }, StringSplitOptions.None).Length - 1;
            }
            return 0;
        }


        public static Dictionary<String, String> getCurrentView()
        {
            
            try
            {
                CameraController controller = GameObject.FindGameObjectWithTag("MainCamera")?.GetComponent<CameraController>();
		        Vector3 Position = controller.m_targetPosition;
		        Vector2 Angle = controller.m_targetAngle;
		        float Height = controller.m_targetHeight;
                float Size = controller.m_currentSize;
                float FieldOfView = controller.GetComponent<Camera>().fieldOfView;


                Dictionary<String, String> view2 = new Dictionary<String, String>();
                view2.Add("x", ((int)Position.x).ToString());
                view2.Add("z", ((int)Position.z).ToString());
                view2.Add("y", ((int)Position.y).ToString());
                view2.Add("Ax", ((int)Angle.x).ToString());
                view2.Add("Ay", ((int)Angle.y).ToString());
                view2.Add("size", ((float)Size).ToString());
                view2.Add("height", ((float)Height).ToString());
                view2.Add("field", ((float)FieldOfView).ToString());

                return view2;

            }
            catch(Exception e)
            {

                ExceptionPanel panel = UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel");
                panel.SetMessage("Screenshoter", "Unable to use Camera", true);
            }

            Dictionary<String, String> view = new Dictionary<String, String>();
            view.Add("x", "0");
            view.Add("z", "0");
            view.Add("y", "0");
            view.Add("Ax", "0");
            view.Add("Ay", "0");
            view.Add("size", "0");
            view.Add("height", "0");
            view.Add("field", "0");
            return view;
        }
        public static void addView()
        {
            try
            {
                Dictionary<String, String> view = getCurrentView();


                String CRD = HistoryMod.getConfig("views", "");

                CRD += "[nextView]"+ view["x"] + "|"+ view["z"] + "|"+ view["y"] + "|"+ view["Ax"] + "|" + view["Ay"] + "|" + view["size"] + "|" + view["height"] + "|" + view["field"];

                HistoryMod.saveConfig("views", " "+CRD);
                refreshViewBtns();
                log("a view was added: "+CRD);

            }catch(Exception e)
            {

                ExceptionPanel panel = UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel");
                panel.SetMessage("Screenshoter", "Unable to use Camera", true);
            }

        }
        public static Dictionary<String, String> getView(int Index)
        {
            debug("getView ");

            String CRD = HistoryMod.getConfig("views", "");

            debug("getView " + Index +" : "+ CRD);

            if (CRD.Contains("[nextView]"))
            {
                debug("getView has nextView");

                String[] CRDSTRINGS = CRD.Split(new string[] { "[nextView]" }, StringSplitOptions.None);

                debug("getView has "+CRDSTRINGS.Length);

                if (CRDSTRINGS.Length > (Index + 1))
                {
                    debug("getView has view");

                    String[] Crds = CRDSTRINGS[(Index + 1)].Split(new string[] { "|" }, StringSplitOptions.None);

                    Dictionary<String, String> view2 = new Dictionary<String, String>();
                    view2.Add("x", Crds[0]);
                    view2.Add("z", Crds[1]);
                    view2.Add("y", Crds[2]);
                    view2.Add("Ax", Crds[3]);
                    view2.Add("Ay", Crds[4]);
                    view2.Add("size", Crds[5]);
                    view2.Add("height", Crds[6]);
                    view2.Add("field", Crds[7]);
                    return view2;
                }
            }



            ExceptionPanel panel = UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel");
            panel.SetMessage("Unable to find view", "This view doesn't exist ?", false);

            return getCurrentView();

        }

        public static void refreshViewBtns()
        {

            debug("refreshViewBtns init");
            if (ViewLabels != null && ViewLabels.Count > 0 && groupViews != null)
            {
                UIPanel uIPanel = ((UIHelper)groupViews).self as UIPanel;
                foreach (var label in ViewLabels)
                {
                    uIPanel.RemoveUIComponent(label);
                    if (label.isVisible)
                        label.Hide();
                }
                ViewLabels.Clear();
            }

            if (ViewSeeViewBtns != null && ViewSeeViewBtns.Count > 0 && groupViews != null)
            {
                UIPanel uIPanel = ((UIHelper)groupViews).self as UIPanel;
                foreach (var btn in ViewSeeViewBtns)
                {
                    uIPanel.RemoveUIComponent(btn);
                    if (btn.isVisible)
                        btn.Hide();
                }
                ViewSeeViewBtns.Clear();
            }

            if (ViewDeleteViewBtns != null && ViewDeleteViewBtns.Count > 0 && groupViews != null)
            {
                UIPanel uIPanel = ((UIHelper)groupViews).self as UIPanel;
                foreach (var btn in ViewDeleteViewBtns)
                {
                    uIPanel.RemoveUIComponent(btn);
                    if(btn.isVisible)
                        btn.Hide();
                }
                ViewDeleteViewBtns.Clear();
            }

            debug("refreshViewBtns cleared");

            if (groupViews != null)
            {
                debug("refreshViewBtns groupViews");

                for (int i = 0; i < getViewsCount(); i++)
                {
                    int id = i;

                    debug("refreshViewBtns groupViews 1");

                    UIPanel uIPanel = ((UIHelper)groupViews).self as UIPanel;

                    UILabel uILabel2 = uIPanel.AddUIComponent<UILabel>();
                    uILabel2.name = "View " + (i + 1);
                    uILabel2.textScale = 1.2f;
                    uILabel2.text = "View " + (i + 1);

                    debug("refreshViewBtns groupViews 1 add");

                    ViewLabels.Add(uILabel2);

                    debug("refreshViewBtns groupViews 1 add 1");

                    ViewSeeViewBtns.Add(groupViews.AddButton("View", () => {
                        closeAllPanels();
                        Dictionary<String, String> view = getView(id);

                        debug(id + "Frame10 x" + view["x"] + " y" + view["y"] + " z" + view["z"] + " Ax" + view["Ax"] + " Ay" + view["Ay"] + " height" + view["height"] + " size" + view["size"] + " field" + view["field"]);


                        loadView(view);
                    }) as UIButton);

                    debug("refreshViewBtns groupViews 1 add 2");

                    ViewDeleteViewBtns.Add(groupViews.AddButton("Delete", () => {
                        removeView(id);
                        closeAllPanels();
                    }) as UIButton);

                    debug("refreshViewBtns groupViews 1 add 3");

                }
            }
        }


        public static bool canScreenshotNow()
        {
            if (getViewsCount() < 1)
            {
                try
                {
                    closeAllPanels();
                    stopScreenShoter();
                    HistoryMod.label.Hide();
                }catch (Exception ex) {
                    debug("Exception CanScreenshotNow");
                }

                ExceptionPanel panel = UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel");
                panel.SetMessage("Screenshoter", "No Views to screenshot found." + Environment.NewLine + "Please add views to screenshot from the options" + Environment.NewLine + " menu before trying to render any screenshot.", true);
                return false;
            }
            else
                return true;
        }

        public static void loadSave(SaveGameMetaData saveGameMetaData)
        {
            debug("load save " + HistoryMod.latestSaveGame.name.ToString());
            log("a save is loading: "+HistoryMod.latestSaveGame.name.ToString());

            SimulationMetaData simulationMetaData = new SimulationMetaData();
            simulationMetaData.m_CityName = saveGameMetaData.cityName;
            simulationMetaData.m_updateMode = SimulationManager.UpdateMode.LoadGame;
            SimulationMetaData simulationMetaData2 = simulationMetaData;
            if (HistoryMod.latestSaveGame.package != null && HistoryMod.latestSaveGame.package.GetPublishedFileID() != PublishedFileId.invalid)
            {
                simulationMetaData2.m_disableAchievements = SimulationMetaData.MetaBool.True;
            }
            Singleton<LoadingManager>.instance.LoadLevel(saveGameMetaData.assetRef, "Game", "InGame", simulationMetaData2);

        }
        public static void stopGame(bool restart)
        {
            if (restart)
            {
                HistoryMod.saveConfig("restartGameWasAutomatic", "true");

                debug("restartGame");
                //for mac ? https://steamcommunity.com/sharedfiles/filedetails/?id=1976349559


                //we create a batch file that will restart the game after a while, to be sure it has the time to shutdown before

                try
                {

                    /* Looks like we can start the game before it's close. so no need for that batch that doesnt work always as it disapear sometimes 0.o
                    string HistoryModPath = DataLocation.localApplicationData + "\\" + "historyMod" + "\\";
                    File.WriteAllText(HistoryModPath + "restart2.bat", "timeout /t 20 /nobreak > NUL" + Environment.NewLine +
                        "Start \"\"  \"C:\\Program Files (x86)\\Steam\\steamapps\\common\\Cities_Skylines\\Cities.exe\"");

                    debug("restartGame b");
                    //we execute the batch file, while it prepare to start the game, we will now close the game.
                    System.Diagnostics.Process.Start(HistoryModPath + "restart2.bat");
                    */
                    debug("restartGame b");

                    if (Application.platform == RuntimePlatform.OSXPlayer)
                    {

                    }
                    else if (Application.platform == RuntimePlatform.WindowsPlayer)
                    {
                        String path = Application.dataPath;
                        path += "/../Cities.exe";
                        debug(path);
                        log("Restarting Game from autoRestart");
                        System.Diagnostics.Process.Start(path);

                    }
                    
                    

                }
                catch(Exception ex)
                {

                    log("AutoRestart error");
                    debug("restartGame bug");
                    ExceptionPanel panel = UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel");
                    panel.SetMessage("Unable to finish", "An error is preventing us from restarting the game," + Environment.NewLine + 
                        "you can restart manually, the correct save will automatically " + Environment.NewLine + " load and the session will continue." + Environment.NewLine +
                        "Do not restart by clicking on load last save.", false);
                    return;

                }

            }

            debug("restartGame c");

            Singleton<LoadingManager>.instance.QuitApplication();

            debug("restartGame d");


        }
        public static void log(String value)
        {
            if(DoesHistoryFolderExists())
                using (var streamWriter = new StreamWriter(DataLocation.localApplicationData + "\\" + "historyMod" + "\\logs.txt", true))
                {
                    var time = DateTime.Now.ToString("yyyy mm dd HH:mm:ss.fff");
                    streamWriter.WriteLine(time +" : "+ value);
                }
        }

        public static void saveConfig(String key, String value)
        {
            if (DoesHistoryFolderExists())
            {
                string HistoryModPath = DataLocation.localApplicationData + "\\" + "historyMod" + "\\";
                Dictionary<String, String> configs = getConfigs();
                if(configs.ContainsKey(key))
                {
                    configs.Remove(key);
                }
                configs.Add(key, value);

                string readText = "";
                foreach (KeyValuePair<String, String> entry in configs)
                {
                    readText += ":::" + entry.Key + ":::" + entry.Value + Environment.NewLine;
                }

                debug("saveConfig " + readText);

                File.WriteAllText(HistoryModPath + "config.txt",readText);
                //for whatever reason it happend that it put empty thing in config.txt so now I'm doing a backup too.
                File.WriteAllText(HistoryModPath + "configSave.txt", readText);
            }

        }

        public static bool DoesHistoryFolderExists()
        {
            try
            {
                if (!Directory.Exists(DataLocation.localApplicationData + "\\" + "historyMod" + "\\"))
                {
                    Directory.CreateDirectory(DataLocation.localApplicationData + "\\" + "historyMod" + "\\");
                }
            }catch(Exception ex)
            {
                
            }

            if (Directory.Exists(DataLocation.localApplicationData + "\\" + "historyMod" + "\\"))
            {
                return true;
            }
            return false;
        }

        public static Dictionary<String, String> getConfigs()
        {
            Dictionary<String, String> configs = new Dictionary<String, String>();


            if (DoesHistoryFolderExists())
            {
                string HistoryModPath = DataLocation.localApplicationData + "\\" + "historyMod" + "\\";
                bool trySave = false;
                if (HistoryMod.DoesHistoryFolderExists() && File.Exists(HistoryModPath + "config.txt"))
                {
                    var lines = File.ReadAllLines(HistoryModPath + "config.txt");
                    if (lines.Length > 0)
                        for (var i = 0; i < lines.Length; i += 1)
                        {
                            var line = lines[i];
                            String[] split = line.Split(new string[] { ":::" }, StringSplitOptions.None);
                            configs.Add(split[1], split[2]);
                        }
                    else
                        trySave = true;
                }

                if (trySave && File.Exists(HistoryModPath + "configSave.txt"))
                {
                    var lines = File.ReadAllLines(HistoryModPath + "configSave.txt");
                    for (var i = 0; i < lines.Length; i += 1)
                    {
                        var line = lines[i];
                        String[] split = line.Split(new string[] { ":::" }, StringSplitOptions.None);
                        configs.Add(split[1], split[2]);
                    }
                }
            }

            return configs;
        }
        public static void loadView(Dictionary<String, String> view)
        {

            CameraController controller = GameObject.FindGameObjectWithTag("MainCamera")?.GetComponent<CameraController>();
            if(controller != null)
            {

                try
                {
                    controller.m_targetPosition = new Vector3(int.Parse(view["x"]), int.Parse(view["y"]), int.Parse(view["z"]));
                    controller.m_currentPosition = new Vector3(int.Parse(view["x"]), int.Parse(view["y"]), int.Parse(view["z"]));
                    controller.m_targetAngle = new Vector2(int.Parse(view["Ax"]), int.Parse(view["Ay"]));
                    controller.m_currentAngle = new Vector2(int.Parse(view["Ax"]), int.Parse(view["Ay"]));
                    controller.m_targetHeight = float.Parse(view["height"]);
                    controller.m_currentHeight = float.Parse(view["height"]);
                    controller.m_targetSize = float.Parse(view["size"]);
                    controller.m_currentSize = float.Parse(view["size"]);
                    controller.GetComponent<Camera>().fieldOfView = float.Parse(view["field"]);

                }
                catch (Exception e)
                {
                    HistoryMod.stopScreenShoter();
                    HistoryMod.log("unable to use Camera to set the view, mod problem ? Let me know thanks.");

                    ExceptionPanel panel = UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel");
                    panel.SetMessage("Screenshoter", "unable to use Camera to set the view, mod problem ? Let me know thanks.", false);

                }


            }

        }
        public static void cslExportNow()
        {
            try
            {
                
                CSLMod.CSLMapView.Modding.Config.CSLMapViewModConfig config = CSLMod.CSLMapView.Modding.Config.CSLMapViewModConfig.LoadOrDefault();
                CSLMod.CSLMapView.Modding.Exporter.MapInfoExporter CSL = new CSLMod.CSLMapView.Modding.Exporter.MapInfoExporter();
                CSL.IsCompress = config.UseCompression;
                CSL.AddTimestampToFileName = config.FileNameWithTimestamp;
                CSL.TargetDirectory = Path.GetFullPath(string.IsNullOrEmpty(config.MapOutputPath) ? "CSLMapView" : config.MapOutputPath);
                CSL.Export();
                

                HistoryMod.log("CSL EXPORTED ");
            }
            catch (Exception ex)
            {

                HistoryMod.log("Error with CSLMod, does the player don't have it ? ");
            }

        }

        public static String getConfig(String key, String def)
        {
            Dictionary<String, String> configs = getConfigs();
            if (configs.ContainsKey(key))
            {
                return configs[key];
            }
            else
                return def;
        }

        public static void cancelSpecialWeather()
        {
            WeatherManager weatherManager = Singleton<WeatherManager>.instance;
            if (weatherManager != null)
            {
                weatherManager.m_currentRainbow = 0;
                weatherManager.m_targetRainbow = 0f;
                weatherManager.m_currentNorthernLights = 0;
                weatherManager.m_targetNorthernLights = 0;
                weatherManager.m_groundWetness = 0;
                weatherManager.m_currentCloud = 0;
                weatherManager.m_targetCloud = 0;
                weatherManager.m_currentFog = 0;
                weatherManager.m_targetFog = 0;
                weatherManager.m_currentRain = 0;
                weatherManager.m_targetRain = 0;
                weatherManager.m_currentTemperature = 10;
                weatherManager.m_targetTemperature = 10;
                weatherManager.m_temperatureSpeed = 0;
                weatherManager.m_windDirection = 0;
                weatherManager.m_targetDirection = 0;
                weatherManager.m_directionSpeed = 0;
            }
        }

        public static void initiateScreenShoter(bool onlyThisOne, bool autoConfirm, String screenshotSessionName2,bool dontAddFirstSave, bool dontResetGameMeta)
        {
            stopScreenShoter();
            HistoryMod.onlyThisSave = onlyThisOne;

            if (canScreenshotNow())
            {

                List<Asset> saves = SaveHelper.GetSavesOnDisk();
                Dictionary<Int32, Asset> savesToOrder = new Dictionary<Int32, Asset>();
                Dictionary<Int32, String> savesToOrderName = new Dictionary<Int32, String>();
                List<Int32> savesInts = new List<Int32>();

                if (!screenshotSessionName2.Equals(""))
                    screenshotSessionName = screenshotSessionName2;
                else if (HistoryMod.sessionNameIsCityName)
                    screenshotSessionName = SimulationManager.instance.m_metaData.m_CityName;
                else
                    screenshotSessionName = SimulationManager.instance.m_metaData.m_CityName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");

                if (!dontResetGameMeta)
                {
                    ActualSaveGameMetaData = null;
                    latestSaveGame = null;
                }


                if (HistoryMod.onlyThisSave)
                {
                    resumeScreenShoter();
                    return;
                }

                try
                {

                    DateTime dateBefore = DateTime.ParseExact("31-10-2037", "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);

                    if (HistoryMod.beforeDate.Length > 0)
                    {
                        if (HistoryMod.beforeDate.Contains("-"))
                        {
                            dateBefore = DateTime.ParseExact(HistoryMod.beforeDate, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        }
                    }

                    double dateBeforeTS = (Int32)(dateBefore.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;


                    DateTime dateAfter = DateTime.ParseExact("31-10-1999", "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);

                    if (HistoryMod.afterDate.Length > 0)
                    {
                        if (HistoryMod.afterDate.Contains("-"))
                        {
                            dateAfter = DateTime.ParseExact(HistoryMod.afterDate, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        }
                    }
                    double dateAfterTS = (Int32)(dateAfter.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;


                    debug("Found Map by cityName for " + cityName);
                    debug("test " + SimulationManager.instance.m_metaData.m_CityName);
                    debug("test done");
                    foreach (Package.Asset item in PackageManager.FilterAssets(UserAssetType.SaveGameMetaData))
                    {
                        if (item.name != null && !PackageHelper.IsDemoModeSave(item) && item != null && item.isEnabled && (HistoryMod.mapPrefix.Length < 1 || item.name.StartsWith(HistoryMod.mapPrefix)))
                        {
                            SaveGameMetaData saveGameMetaData = item.Instantiate<SaveGameMetaData>();
                            double timeStampSave = (Int32)(saveGameMetaData.timeStamp.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                            if (saveGameMetaData != null)
                            {
                                if (!HistoryMod.saveWithSameCityNameOnly || (HistoryMod.saveWithSameCityNameOnly && saveGameMetaData.cityName != null && saveGameMetaData.cityName.Equals(cityName)))
                                {
                                    debug("time save: " + timeStampSave + ":before: " + dateBeforeTS + " after: " + dateAfterTS + " donc: " + (timeStampSave < dateBeforeTS && timeStampSave > dateAfterTS));
                                    if (timeStampSave < dateBeforeTS && timeStampSave > dateAfterTS)
                                    {
                                        HistoryMod.debug("Found Map to add " + item.name);
                                        Int32 unixTimestamp = (Int32)(saveGameMetaData.timeStamp.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                                        if (!savesToOrder.ContainsKey(unixTimestamp))
                                        {
                                            savesToOrderName.Add(unixTimestamp, item.name.ToString());
                                            savesToOrder.Add(unixTimestamp, item);
                                            savesInts.Add(unixTimestamp);
                                        }
                                    }


                                }
                            }
                        }
                    }

                }
                catch (Exception)
                {

                    ExceptionPanel panel = UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel");
                    panel.SetMessage("Screenshoter", "Wrong syntax with one of the date set in options.", true);
                    return;
                }

                debug("Found " + savesToOrder.Count + " maps");
                savesInts.Sort();

                var plusseur = 1;
                plusseur += HistoryMod.ignoreXNumberOfSavesInBetween;

                var until = 0;
                if (HistoryMod.maxNumberOfSavesToScreenshot > 0)
                {
                    until = savesInts.Count - HistoryMod.maxNumberOfSavesToScreenshot;
                }
                if (until < 0)
                    until = 0;

                savesToLoad.Clear();

                HistoryMod.startFromThisSave = HistoryMod.getConfig("startFromThisSave", "");
                bool checkForSaveName = false;
                String checkForSaveMsg = "";
                int checkForSaveCount = 0;
                if (!HistoryMod.startFromThisSave.Equals(""))
                {
                    checkForSaveMsg = " Saves will be ignored because you  " + Environment.NewLine + " start from save named: " + Environment.NewLine + HistoryMod.startFromThisSave + Environment.NewLine;
                    checkForSaveName = true;
                }

                for (int i = (savesInts.Count - 1); i >= until; i = i - plusseur)
                {
                    if (!checkForSaveName || HistoryMod.startFromThisSave.Equals(savesToOrderName[savesInts[i]]))
                    {
                        checkForSaveName = false;
                        if(dontAddFirstSave)
                        {
                            dontAddFirstSave = false;
                        }
                        else
                            savesToLoad.Add(savesToOrder[savesInts[i]]);
                    }
                    else
                    {
                        checkForSaveCount++;
                    }
                }

                if (checkForSaveName)
                {
                    ExceptionPanel panel = UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel");
                    panel.SetMessage("Screenshoter", "We can't find the save named " + HistoryMod.startFromThisSave, true);
                    return;
                }

                if (!checkForSaveMsg.Equals(""))
                    checkForSaveMsg = checkForSaveCount + " " + checkForSaveMsg;


                debug("Found " + savesToLoad.Count + " savesToLoad");
                debug("HISTORYMOD: initiateScreenShoter.");

                lastLoadedMapIndex = 0;

                String msg = "We Found " + savesToLoad.Count + " saves to load" + Environment.NewLine +
                        "Day/Night: " + (HistoryMod.dayOrNightForScreenshots == 0 ? "Day" : (HistoryMod.dayOrNightForScreenshots == 2 ? "Both" : "Night")) + " With " + getViewsCount() + " Views " + Environment.NewLine +
                        "With prefix: " + (HistoryMod.mapPrefix.Length > 0 ? HistoryMod.mapPrefix : "No") + Environment.NewLine +
                        "After: " + (HistoryMod.afterDate.Length > 0 ? HistoryMod.afterDate : "Any date") + " Before: " + (HistoryMod.beforeDate.Length > 0 ? HistoryMod.beforeDate : "Any date") + Environment.NewLine +
                        "Max saves: " + HistoryMod.maxNumberOfSavesToScreenshot + ", ignore each " + HistoryMod.ignoreXNumberOfSavesInBetween + " saves " + Environment.NewLine +
                        "Estimated time of work: from " + (savesToLoad.Count * 1) + " to " + (savesToLoad.Count * 4) + " Minutes. Proceed ?";

                log("Start screenshoter ? "+(autoConfirm ? "[AUTOMATICLY ACCEPTED]": ("1/2 (not necessarly confirmed)" + (savesToLoad.Count < 1 ? "NO SAVE FOUND" : ""))) + msg);

                if (savesToLoad.Count < 1 && !autoConfirm) //because count may be empty but we still want current screenshot map.
                {
                    ExceptionPanel panel = UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel");
                    panel.SetMessage("Screenshoter", "No save found ! Check the criteria in mod options.", true);
                }
                else
                {
                    if (autoConfirm)
                    {
                        resumeScreenShoter();
                    }
                    else
                    {
                        ConfirmPanel panel = UIView.library.ShowModal<ConfirmPanel>("ConfirmPanel", (UIView.ModalPoppedReturnCallback)((comp, ret) =>
                        {
                            if (ret != 1)
                                return;
                            //this will start the process, we can directly called initiateScreenShoterFinal here as the panel will not display then..
                            HistoryMod.initiateScreenShoterFinalMsg = "WARNING: Save game before proceeding if needed!" + Environment.NewLine + "Game is not saved before loading next save." + Environment.NewLine + checkForSaveMsg;


                        }));
                        panel.SetMessage("Screenshoter", msg);
                    }

                }

            }
        }

        public static void initiateScreenShoterFinal(String msg)
        {

            debug("second modal");

            log("Start screenshoter 2/2 ? " + msg);

            ConfirmPanel panel = UIView.library.ShowModal<ConfirmPanel>("ConfirmPanel", (UIView.ModalPoppedReturnCallback)((comp, ret) =>
            {
                if (ret != 1)
                    return;
                HistoryMod.log("initiateScreenShoterFinal");
                resumeScreenShoter();

                if (HistoryMod.getConfig("autoContinueIfGameCrashedWhenRestarting", "false").Equals("true"))
                    restartGameIfCrash(true);

            }));
            panel.SetMessage("Screenshoter", msg);

        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            UIHelperBase gV = helper.AddGroup("HistoryMod - Automatically take screenshots - V"+HistoryMod.version);

            UIPanel uIPanel = ((UIHelper)gV).self as UIPanel;

            UILabel uILabel = uIPanel.AddUIComponent<UILabel>();
            uILabel.name = "SettingViews";
            uILabel.textScale = 1f;
            uILabel.text = 
                "HistoryMod allow you to take multiple screenshots of various views" + Environment.NewLine + 
                "from all the saves of your city automatically." +Environment.NewLine +
                "The process will load a save, take the screenshots from all views," + Environment.NewLine +
                "go to the next save to load and start the process again.";


            UILabel uILabel23 = uIPanel.AddUIComponent<UILabel>();
            uILabel23.name = "SettingViews";
            uILabel23.textScale = 0.9f;
            uILabel23.text =
                "I hope you like this mod, maybe you may help me on something !" + Environment.NewLine +
                "I have my own IT company, where I create Tourist AudioGuides for Public" + Environment.NewLine +
                "Transport Companies, by using the smartphone, without an APP and sync by GPS." + Environment.NewLine +
                "If you happen to know someone working in such" + Environment.NewLine +
                "a company, can you email us ? :) citytourplayer@nexswiss.ch";

            UILabel uILabel52 = uIPanel.AddUIComponent<UILabel>();
            uILabel52.text = "Click to discover our audioguide tool citytourplayer.com";
            uILabel52.textScale = 0.7f;
            uILabel52.textColor = new Color32(0, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            uILabel52.eventClicked += delegate
            {
                System.Diagnostics.Process.Start("https://citytourplayer.com/en");
            };
            uILabel52.eventMouseHover += delegate (UIComponent comp, UIMouseEventParameter e)
            {
                ((UILabel)comp).textColor = new Color32(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue);
            };
            uILabel52.eventMouseLeave += delegate (UIComponent comp, UIMouseEventParameter e)
            {
                ((UILabel)comp).textColor = new Color32(0, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            };



            UILabel uILabel23242 = uIPanel.AddUIComponent<UILabel>();
            uILabel23242.name = "SettingViews";
            uILabel23242.textScale = 0.9f;
            uILabel23242.textColor = new Color32(0, byte.MaxValue, 0, byte.MaxValue);
            uILabel23242.text =
                "BEFORE starting a long screenshot session, turn off the automatic sleep mode" + Environment.NewLine +
                "of your computer AND check that you have enough space on your disk for a lot" + Environment.NewLine +
                "Of screenshot. Not doing so may crash the session.";


            UILabel uILabel22323 = uIPanel.AddUIComponent<UILabel>();
            uILabel22323.name = "SettingViews";
            uILabel22323.textScale = 0.7f;
            uILabel22323.textColor = new Color32(0, byte.MaxValue, 0, byte.MaxValue);
            uILabel22323.text = "Careful during the screenshot session: do not move the mouse or use the computer. WARNING: The selected " + Environment.NewLine +
            "save will be loaded with the game, if you have any mod that rewrite the saves when doing this," + Environment.NewLine +
            "it may be good to think about this as this mod will not prevent them to change/corrupt the selected saves." + Environment.NewLine +
            "To avoid any risk, copy all the saves of your game before using HistoryMod with other mods.";



            UILabel uILabel232 = uIPanel.AddUIComponent<UILabel>();
            uILabel232.name = "SettingViews";
            uILabel232.textScale = 0.9f;
            uILabel232.text =
                "We also make digital Reports, invoices or Quotation systems" + Environment.NewLine +
                "For any company that still use the paper when being on an intervention" + Environment.NewLine +
                "They can then use our iOS/Android App to create the report and automatize." + Environment.NewLine +
                "everything, if you have 2 minutes, would be nice to have a look on our website" + Environment.NewLine +
                "and let us know if you know anyone who may be interested :) pepadocs@nexswiss.ch";


            UILabel uILabel2324 = uIPanel.AddUIComponent<UILabel>();
            uILabel2324.name = "SettingViews";
            uILabel2324.textScale = 0.9f;
            uILabel2324.textColor = new Color32(0, byte.MaxValue, 0, byte.MaxValue);
            uILabel2324.text =
                "If we get new clients thanks to you, we will surely find a way to thank you :)";

            UILabel uILabel51 = uIPanel.AddUIComponent<UILabel>();
            uILabel51.text = "Click to discover Pepadocs.com";
            uILabel51.textScale = 0.7f;
            uILabel51.textColor = new Color32(0, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            uILabel51.eventClicked += delegate
            {
                System.Diagnostics.Process.Start("https://pepadocs.com/en/discover");
            };
            uILabel51.eventMouseHover += delegate (UIComponent comp, UIMouseEventParameter e)
            {
                ((UILabel)comp).textColor = new Color32(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue);
            };
            uILabel51.eventMouseLeave += delegate (UIComponent comp, UIMouseEventParameter e)
            {
                ((UILabel)comp).textColor = new Color32(0, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            };

            UILabel uILabel2223 = uIPanel.AddUIComponent<UILabel>();
            uILabel2223.name = "SettingViews";
            uILabel2223.textScale = 0.9f;
            uILabel2223.text = "For any suggestion, if you wish to help or anything regarding this mod:" + Environment.NewLine +
                "microscraft@gmail.com - By Joel Iunius :)";


            UILabel uILabel521 = uIPanel.AddUIComponent<UILabel>();
            uILabel521.text = "Click to go to the Steam page of the Mod";
            uILabel521.textScale = 0.7f;
            uILabel521.textColor = new Color32(0, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            uILabel521.eventClicked += delegate
            {
                System.Diagnostics.Process.Start("https://joel-iunius.ch/historymod.html");
            };
            uILabel521.eventMouseHover += delegate (UIComponent comp, UIMouseEventParameter e)
            {
                ((UILabel)comp).textColor = new Color32(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue);
            };
            uILabel521.eventMouseLeave += delegate (UIComponent comp, UIMouseEventParameter e)
            {
                ((UILabel)comp).textColor = new Color32(0, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            };


            UILabel uILabel5213 = uIPanel.AddUIComponent<UILabel>();
            uILabel5213.text = "Click to see my beautiful timelapse video example :P";
            uILabel5213.textScale = 0.7f;
            uILabel5213.textColor = new Color32(0, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            uILabel5213.eventClicked += delegate
            {
                System.Diagnostics.Process.Start("https://joel-iunius.ch/historymodvideo.html");
            };
            uILabel5213.eventMouseHover += delegate (UIComponent comp, UIMouseEventParameter e)
            {
                ((UILabel)comp).textColor = new Color32(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue);
            };
            uILabel5213.eventMouseLeave += delegate (UIComponent comp, UIMouseEventParameter e)
            {
                ((UILabel)comp).textColor = new Color32(0, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            };

            if (HistoryMod.isDebug)
            {


                gV.AddButton("GetPaths", () => {

                    if (Application.platform == RuntimePlatform.OSXPlayer)
                    {
                        
                    }
                    else if (Application.platform == RuntimePlatform.WindowsPlayer)
                    {
                        String path = Application.dataPath;
                        path += "/../Cities.exe";
                        debug(path);
                        ExceptionPanel panel = UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel");
                        panel.SetMessage("Screenshoter", path, true);
                    }

                });


                gV.AddButton("restartGame", () => {
                    stopGame(true);
                });

                gV.AddButton("panel", () => {
                    closeAllPanels();
                });

            }

            gV.AddButton("Render Screenshots without loading other saves", () => {
                initiateScreenShoter(true, false,"",false,false);
            });

            gV.AddButton("Render screenshots from this save and others", () => {
                initiateScreenShoter(false, false, "",false, false);
            });

            gV.AddButton("Open Screenshot Folder", () => {

                if (DoesHistoryFolderExists())
                {

                    if (Application.platform == RuntimePlatform.WindowsPlayer)
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("explorer.exe", DataLocation.localApplicationData + "\\" + "historyMod" + "\\"));
                    }
                    else
                    {

                        ExceptionPanel panel = UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel");
                        panel.SetMessage("Oups!", "Looks like you are not on windows :/ I can't open finder, but the folder should be in: " + Environment.NewLine + DataLocation.localApplicationData + "\\" + "historyMod" + "\\", false);
                    }

                }
                else
                {
                    ExceptionPanel panel = UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel");
                    panel.SetMessage("Oups!", "Looks like historyMod folder can't be created. The mod will not work.", false);

                }

            });



            groupViews = helper.AddGroup("Views");

            UIPanel uIPanel2 = ((UIHelper)groupViews).self as UIPanel;
            UILabel uILabel2 = uIPanel2.AddUIComponent<UILabel>();
            uILabel2.name = "SettingViews";
            uILabel2.textScale = 1f;
            uILabel2.text = "You can here set as many view to screenshot as you wish " +Environment.NewLine+ 
                " for that, set the camera view as wished, then open " + Environment.NewLine + 
                "  these options and click on Add Actual View to List." + Environment.NewLine +
                "  Views stay saved after exiting the game too.";


            groupViews.AddButton("Add actual view to list", () => {
                addView();
                closeAllPanels();
            });

            groupViews.AddButton("Remove all views from list", () => {
                clearAllViews();
                closeAllPanels();
            });


            refreshViewBtns();


            UIHelperBase groupGeneral = helper.AddGroup("General Configuration");

            UIPanel uIPanel3 = ((UIHelper)groupGeneral).self as UIPanel;
            UILabel uILabel3 = uIPanel3.AddUIComponent<UILabel>();
            uILabel3.name = "SettingViews";
            uILabel3.textScale = 1f;
            uILabel3.text =
                "If you happen to use the CSL Map View Mod, and" + Environment.NewLine +
                "wish the mod to generate a new CSL Export for each save" + Environment.NewLine +
                "then, you can check the next checkbox. The exports will" + Environment.NewLine +
                "be exported in the usual CSL export Folder." + Environment.NewLine +
                "WARNING: if you don't have the mod and check this, it will crash.";


            UIPanel uIPanel5 = ((UIHelper)groupGeneral).self as UIPanel;
            UILabel uILabel5 = uIPanel5.AddUIComponent<UILabel>();
            uILabel5.text = "Click to get the CSL Mod (Create a shematic Map of your city)";
            uILabel5.textScale = 0.7f;
            uILabel5.textColor = new Color32(0, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            uILabel5.eventClicked += delegate
            {
                System.Diagnostics.Process.Start("https://steamcommunity.com/sharedfiles/filedetails/?id=845665815");
            };
            uILabel5.eventMouseHover += delegate (UIComponent comp, UIMouseEventParameter e)
            {
                ((UILabel)comp).textColor = new Color32(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue);
            };
            uILabel5.eventMouseLeave += delegate (UIComponent comp, UIMouseEventParameter e)
            {
                ((UILabel)comp).textColor = new Color32(0, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            };


            HistoryMod.CSLExport = (getConfig("CSLExport","false") == "true");

            groupGeneral.AddCheckbox("Create a CSL Export for each saves", HistoryMod.CSLExport, (value) => { 
                HistoryMod.CSLExport = value;
                saveConfig("CSLExport", (HistoryMod.CSLExport ? "true":"false"));
            });


            HistoryMod.dayOrNightForScreenshots = int.Parse(getConfig("dayOrNightForScreenshots", "0"));
            String[] daysOptions = { "Day", "Night", "Both" };
            groupGeneral.AddDropdown("DayTime For Screenshots", daysOptions, HistoryMod.dayOrNightForScreenshots, (value) => {
                HistoryMod.dayOrNightForScreenshots = value;
                saveConfig("dayOrNightForScreenshots", value.ToString());
            });


            HistoryMod.screenshotsNamesSelectedOption = int.Parse(getConfig("screenshotsNamesSelectedOption", "0"));
            String[] screenshotsNamesOptions = { "date of save", "view id, from now to past", "view id inversed, from past to now", "only save, view and city name" };
            groupGeneral.AddDropdown("Order screenshots with specific names:", screenshotsNamesOptions, HistoryMod.screenshotsNamesSelectedOption, (value) => {
                HistoryMod.screenshotsNamesSelectedOption = value;
                saveConfig("screenshotsNamesSelectedOption", value.ToString());
            });

            HistoryMod.stopGameAfterScreenshotSession = (getConfig("stopGameAfterScreenshotSession", "false") == "true");
            groupGeneral.AddCheckbox("Automatically shutdown the game after screenshots", HistoryMod.stopGameAfterScreenshotSession, (value) => {
                HistoryMod.stopGameAfterScreenshotSession = value;
                saveConfig("stopGameAfterScreenshotSession", (HistoryMod.stopGameAfterScreenshotSession ? "true" : "false"));
            });
            UILabel uILabel332 = uIPanel3.AddUIComponent<UILabel>();
            uILabel332.name = "SettingViews";
            uILabel332.textScale = 0.8f;
            uILabel332.text =
                "In any case, it will not stop the game if you only screenshot current save.";


            HistoryMod.sessionNameIsCityName = (getConfig("sessionNameIsCityName", "false") == "true");
            groupGeneral.AddCheckbox("Don't create a folder on every screenshot session", HistoryMod.sessionNameIsCityName, (value) => {
                HistoryMod.sessionNameIsCityName = value;
                saveConfig("sessionNameIsCityName", (HistoryMod.sessionNameIsCityName ? "true" : "false"));
            });

            UILabel uILabel3123 = uIPanel3.AddUIComponent<UILabel>();
            uILabel3123.name = "SettingViews";
            uILabel3123.textScale = 0.8f;
            uILabel3123.text =
                "If checked, screenshots will be always saved in the same folder named as the CityName" + Environment.NewLine +
                "If not checked, every screenshot session will create a new folder where putting pictures.";

            UILabel uILabel3323 = uIPanel3.AddUIComponent<UILabel>();
            uILabel3323.name = "SettingViews";
            uILabel3323.textScale = 1f;
            uILabel3323.textColor = new Color32(0, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            uILabel3323.text =
                "You can use ctrl + alt + v to add a view directly from the game" + Environment.NewLine +
                "You can use ctrl + alt + 1 to 9 to see the corresponding view (Numerical keypad)." + Environment.NewLine +
                "You can use ctrl + alt + x to stop a rendering process at anytime.";



            UIHelperBase randomTools = helper.AddGroup("Random tools");


            randomTools.AddButton("set clean weather", () => {
                cancelSpecialWeather();
            });

            randomTools.AddButton("set Day Time", () => {
                HistoryMod.setTimeOfDay(false);
            });

            randomTools.AddButton("set Night Time", () => {
                HistoryMod.setTimeOfDay(true);
            });


            UIHelperBase groupType = helper.AddGroup("Saves to Load");

            UIPanel uIPanel4 = ((UIHelper)groupType).self as UIPanel;
            UILabel uILabel4 = uIPanel4.AddUIComponent<UILabel>();
            uILabel4.name = "SettingViews";
            uILabel4.textScale = 1f;
            uILabel4.text =
                "The Mod will look into all your saves by default" + Environment.NewLine +
                "You can filter saves from this specific city by checking" + Environment.NewLine +
                "the next checkbox. Or you can choose to use a prefix, if" + Environment.NewLine +
                "not empty, only saves starting with it will be loaded." + Environment.NewLine +
                "Both options can be combined";



            HistoryMod.saveWithSameCityNameOnly = (getConfig("saveWithSameCityNameOnly", "true") == "true");

            groupType.AddCheckbox("Load saves with same cityName only", HistoryMod.saveWithSameCityNameOnly, (value) => {
                HistoryMod.saveWithSameCityNameOnly = value;
                saveConfig("saveWithSameCityNameOnly", (HistoryMod.saveWithSameCityNameOnly ? "true" : "false"));
            });


            HistoryMod.maxNumberOfSavesToScreenshot = int.Parse(getConfig("maxNumberOfSavesToScreenshot", "9999"));
            groupType.AddTextfield("Max number of saves to load (to get only the last 10 for example)", HistoryMod.maxNumberOfSavesToScreenshot.ToString(), (value) => {
                HistoryMod.maxNumberOfSavesToScreenshot = int.Parse(value);
                saveConfig("maxNumberOfSavesToScreenshot", value);
            }, (value) => {
                HistoryMod.maxNumberOfSavesToScreenshot = int.Parse(value);
                saveConfig("maxNumberOfSavesToScreenshot", value);
            });

            HistoryMod.ignoreXNumberOfSavesInBetween = int.Parse(getConfig("ignoreXNumberOfSavesInBetween", "0"));
            groupType.AddTextfield("Ignore each x saves (0 by default)", HistoryMod.ignoreXNumberOfSavesInBetween.ToString(), (value) => {
                HistoryMod.ignoreXNumberOfSavesInBetween = int.Parse(value);
                saveConfig("ignoreXNumberOfSavesInBetween", value);
            }, (value) => {
                HistoryMod.ignoreXNumberOfSavesInBetween = int.Parse(value);
                saveConfig("ignoreXNumberOfSavesInBetween", value);
            });

            HistoryMod.mapPrefix = getConfig("mapPrefix", "");
            groupType.AddTextfield("Only load saves with this prefix", HistoryMod.mapPrefix, (value) => {
                HistoryMod.mapPrefix = value;
                saveConfig("mapPrefix", value);
            }, (value) => {
                HistoryMod.mapPrefix = value;
                saveConfig("mapPrefix", value);
            });

            HistoryMod.afterDate = getConfig("afterDate", "");
            groupType.AddTextfield("Only load saves made after date: (dd-mm-yyyy)", HistoryMod.afterDate, (value) => {
                HistoryMod.afterDate = value;
                saveConfig("afterDate", value);
            }, (value) => {
                HistoryMod.afterDate = value;
                saveConfig("afterDate", value);
            });

            HistoryMod.beforeDate = getConfig("beforeDate", "");
            groupType.AddTextfield("Only load saves made before date: (dd-mm-yyyy)", HistoryMod.beforeDate, (value) => {
                HistoryMod.beforeDate = value;
                saveConfig("beforeDate", value);
            }, (value) => {
                HistoryMod.beforeDate = value;
                saveConfig("beforeDate", value);
            });





            UIHelperBase antiCrashOptions = helper.AddGroup("Anti Crash Options");


            UIPanel uIPanelA32 = ((UIHelper)antiCrashOptions).self as UIPanel;


            UILabel uILabel324 = uIPanelA32.AddUIComponent<UILabel>();
            uILabel324.name = "SettingViews";
            uILabel324.textScale = 0.7f;
            uILabel324.text =
                "If Assets are missing or you get a red error message on loading some specific saves," + Environment.NewLine +
                "Don't worry, the mod will ignore these windows and problems and still do the screenshots." + Environment.NewLine +
                "If a crash happen during the screenshot session, you can start a new session" + Environment.NewLine +
                "with the same filter criteria but by starting from the savegame name that crashed." + Environment.NewLine +
                "Crash may happen on long session (more than 2 hours and if you didn't set up an " + Environment.NewLine +
                " auto restart the game option), if it happen usually" + Environment.NewLine +
                "the name of last not made save will be automatically put here. So if you see something" + Environment.NewLine +
                "here, it may means the last session crashed and you just have to start it again.";

            HistoryMod.startFromThisSave = getConfig("startFromThisSave", "");
            antiCrashOptions.AddTextfield("name of the save from wich to start", HistoryMod.startFromThisSave, (value) => {
                HistoryMod.startFromThisSave = value;
                saveConfig("startFromThisSave", value);
            }, (value) => {
                HistoryMod.startFromThisSave = value;
                saveConfig("startFromThisSave", value);
            });


            HistoryMod.autoContinueIfGameCrashedWhenRestarting = (getConfig("autoContinueIfGameCrashedWhenRestarting", "false") == "true");

            antiCrashOptions.AddCheckbox("When restarting game, automatically continue the screenshots ?", HistoryMod.autoContinueIfGameCrashedWhenRestarting, (value) => {
                HistoryMod.autoContinueIfGameCrashedWhenRestarting = value;
                saveConfig("autoContinueIfGameCrashedWhenRestarting", (HistoryMod.autoContinueIfGameCrashedWhenRestarting ? "true" : "false"));
            });



            UILabel uILabelA343 = uIPanelA32.AddUIComponent<UILabel>();
            uILabelA343.name = "SettingViews";
            uILabelA343.textScale = 0.8f;
            uILabelA343.text =
                "Checking this will make the screesnhot session start back from last not screenshoted "+ Environment.NewLine+
                "save if a crash happend during the process and if you start the game without loading " + Environment.NewLine +
                "the previous save. On windows it will also attempt to restart the game automatically." + Environment.NewLine;

            UILabel uILabelA3432 = uIPanelA32.AddUIComponent<UILabel>();
            uILabelA3432.name = "SettingViews";
            uILabelA3432.textScale = 0.8f;
            uILabelA3432.textColor = new Color32(0, byte.MaxValue, 0, byte.MaxValue);
            uILabelA3432.text =
                "On windows this option will open a black terminal with some code, this is made so it can " + Environment.NewLine +
                "check every 15 seconds if the game still run when doing the screenshot. On long sessions" + Environment.NewLine +
                "This will ensure that your game automatically restart and continue the screenshots if it crashed." + Environment.NewLine;



            UILabel uILabelA34 = uIPanelA32.AddUIComponent<UILabel>();
            uILabelA34.name = "SettingViews";
            uILabelA34.textScale = 0.9f;
            uILabelA34.text =
                "WINDOWS ONLY (if you know how to code for mac let me know):";
           



            UIPanel uIPanelA3 = ((UIHelper)antiCrashOptions).self as UIPanel;
            UILabel uILabelA3 = uIPanelA3.AddUIComponent<UILabel>();
            uILabelA3.name = "SettingViews";
            uILabelA3.textScale = 0.7f;
            uILabelA3.text =
                "Loading a lot of different saves, specially if you play with mod, may result" + Environment.NewLine +
                "in crash (due to memory lost mainly) or in the game not being able to load a" + Environment.NewLine +
                "save after a while automatically. Thus, if you play modded or have more than 50" + Environment.NewLine +
                "saves to load, it is recommanded to let the game restart automatically during the" + Environment.NewLine +
                "screenshot session. We recommand you to let the game restart every 20 saves, but " + Environment.NewLine +
                "if you already know it may crash faster or so, don't hesitate to change this value. " + Environment.NewLine +
                "Every restart add up to 5-10 minutes to the processs.";

            UILabel uILabelA32 = uIPanelA3.AddUIComponent<UILabel>();
            uILabelA32.name = "SettingViews";
            uILabelA32.textScale = 0.7f;
            uILabelA32.textColor = new Color32(0, byte.MaxValue, 0, byte.MaxValue);
            uILabelA32.text =
                "WARNING: auto restarting may not work on every computer, let me know if you have issues." + Environment.NewLine +
                "ALSO: this option may create two instance of the game on the same time for ~1min, it's normal.";



            UILabel uILabelA323 = uIPanelA3.AddUIComponent<UILabel>();
            uILabelA323.name = "SettingViews";
            uILabelA323.textScale = 0.7f;
            uILabelA323.textColor = new Color32(0, byte.MaxValue, 0, byte.MaxValue);
            uILabelA323.text =
                "WARNING: I didn't found a reliable way to avoid the Paradox Launcher, therefore the auto start" + Environment.NewLine +
                "MAY NOT WORK if you don't use Coyotee's launcher to avoid Paradox Launcher. (You may still get" + Environment.NewLine +
                "it back after.) To use Coyotee's launcher, download the correct version and start it:";

            UILabel uILabel53 = uIPanelA3.AddUIComponent<UILabel>();
            uILabel53.text = "Click to download Coyotee's launcher";
            uILabel53.textScale = 0.7f;
            uILabel53.textColor = new Color32(0, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            uILabel53.eventClicked += delegate
            {
                System.Diagnostics.Process.Start("https://github.com/shusaura85/notparadoxlauncher/releases");
            };
            uILabel53.eventMouseHover += delegate (UIComponent comp, UIMouseEventParameter e)
            {
                ((UILabel)comp).textColor = new Color32(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue);
            };
            uILabel53.eventMouseLeave += delegate (UIComponent comp, UIMouseEventParameter e)
            {
                ((UILabel)comp).textColor = new Color32(0, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            };

            UILabel uILabel533 = uIPanelA3.AddUIComponent<UILabel>();
            uILabel533.text = "Optional: What is Coyotee's launcher ? And how to not have Paradox Launcher";
            uILabel533.textScale = 0.7f;
            uILabel533.textColor = new Color32(0, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            uILabel533.eventClicked += delegate
            {
                System.Diagnostics.Process.Start("https://steamcommunity.com/sharedfiles/filedetails/?id=1976349559");
            };
            uILabel533.eventMouseHover += delegate (UIComponent comp, UIMouseEventParameter e)
            {
                ((UILabel)comp).textColor = new Color32(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue);
            };
            uILabel533.eventMouseLeave += delegate (UIComponent comp, UIMouseEventParameter e)
            {
                ((UILabel)comp).textColor = new Color32(0, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            };

            HistoryMod.restartInterval = int.Parse(getConfig("restartInterval", "0"));
            String[] restartIntervalOptions = { "Do not restart Automatically", "Restart every 20 loaded saves", "Restart every 30 loaded saves", "Restart every 40 loaded saves", "Restart every 10 loaded saves", "Restart every 5 loaded saves", "Restart every 3 loaded saves", "Restart every 2 loaded saves", "Restart every time a save is loaded" };
            antiCrashOptions.AddDropdown("Restart game every x loaded saves ?", restartIntervalOptions, HistoryMod.restartInterval, (value) => {
                HistoryMod.restartInterval = value;
                saveConfig("restartInterval", value.ToString());
            });



        }

        public void OnCreated(ILoading loading)
        {
        }

        public void OnReleased()
        {
        }

        public void OnLevelLoaded(LoadMode mode)
        {
        }

        public void OnLevelUnloading()
        {
        }

        public void OnEnabled()
        {

            log("Mod was just enabled/started");
            debug("LOADING COMPLETE");
            restartGameIfCrash(false); 

            if (HistoryMod.getConfig("isScreenShoterActive", "false").Equals("true") && HistoryMod.getConfig("autoContinueIfGameCrashedWhenRestarting", "false").Equals("true"))
            {
                HistoryMod.saveConfig("restartGameWasAutomatic", "true");
            }


            if (HistoryMod.getConfig("restartGameWasAutomatic", "false").Equals("true"))
            {

                

                HistoryMod.saveConfig("restartGameWasAutomatic", "false");
                String startFromThisSave = HistoryMod.getConfig("startFromThisSave", "");

                if(!startFromThisSave.Equals(""))
                    foreach (Package.Asset item in PackageManager.FilterAssets(UserAssetType.SaveGameMetaData))
                    {
                        if (item.name != null && !PackageHelper.IsDemoModeSave(item) && item != null && item.isEnabled && item.name.Equals(startFromThisSave))
                        {
                            SaveGameMetaData saveGameMetaData = item.Instantiate<SaveGameMetaData>();
                            HistoryMod.wasAutoLoaded = true;
                            HistoryMod.ActualSaveGameMetaData = saveGameMetaData; //to avoid "actual" as name in screenshots.
                            HistoryMod.latestSaveGame = item;
                            HistoryMod.loadSave(saveGameMetaData);
                            return;
                        }
                    }

            }
        }

        public void OnDisabled()
        {

        }

    }



    public class Loading : LoadingExtensionBase
	{

        public override void OnCreated(ILoading loading)
		{
			base.OnCreated(loading);
		}

		public override void OnReleased()
		{
			base.OnReleased();
		}

		public override void OnLevelLoaded(LoadMode mode)
		{
			base.OnLevelLoaded(mode);
            HistoryMod.cityName = ((!Singleton<SimulationManager>.exists) ? null : Singleton<SimulationManager>.instance.m_metaData.m_CityName);

            HistoryMod.ProceededView = -1;
            HistoryMod.nbOfFrames = -1;

            if (HistoryMod.wasAutoLoaded) //means its the first time we loaded a map, we need to go back on work then.
            {
                HistoryMod.log("Loading automatically a save right now");
                if (HistoryMod.getConfig("autoContinueIfGameCrashedWhenRestarting", "false").Equals("true"))
                    HistoryMod.restartGameIfCrash(true);

                HistoryMod.wasAutoLoaded = false;
                HistoryMod.initiateScreenShoter(false, true,HistoryMod.getConfig("lastScreenshoterSessionName",""),true,true); //so we stay in the same session name.

            }
            else {
                if (HistoryMod.isScreenShoterActive)
                {
                    HistoryMod.resumeScreenShoter();
                }
            }
            

            
        }

		public override void OnLevelUnloading()
		{
			base.OnLevelUnloading();
		}

    }


    public class Screenshoteur2 : IThreadingExtension
    {
        
        public void OnAfterSimulationFrame()
        {
        }

        public void OnAfterSimulationTick()
        {
        }

        public void OnBeforeSimulationFrame()
        {
        }

        public void OnBeforeSimulationTick()
        {
        }

        public void OnCreated(IThreading threading)
        {
        }

        public void OnReleased()
        {
        }

        public static void takeScreenShot(int viewIndex, String subFolder, String screenshotName)
        {
            


            HistoryMod.debug("HISTORYMOD: CaptureScreen"); 
            HistoryMod.log("CaptureScreen");

            Texture2D screenshot = SC_ScreenAPI.CaptureScreen();

            HistoryMod.watermark = HistoryMod.getConfig("watermark", "");
            if (HistoryMod.watermark.Equals(""))
            {

                int AddPosX = 60;
                int AddPosY = 5;

                for (int y = 0; y < 20; y++)
                {
                    for (int x = screenshot.width - 200; x < screenshot.width; x++)
                    {
                        //HistoryMod
                        if(
                            //H
                            (x >= screenshot.width - 200 && x <= screenshot.width - 200 + 2)
                            ||
                            (x >= screenshot.width - 200 + 9 && x <= screenshot.width - 200 + 10)
                            ||
                            (
                                (x >= screenshot.width - 200 + 3 && x <= screenshot.width - 200 + 8)
                                &&
                                (y >= 9 && y <= 10)
                            )
                            ||
                            //i
                            (
                                (x >= screenshot.width - 200 + 15 && x <= screenshot.width - 200 + 16)
                                &&
                                ((y >= 0 && y <= 13) || (y >= 17 && y <= 20))
                            )
                            ||
                            //S
                            (
                                ((y >= 0 && y <= 2) || (y >= 10 && y <= 11) || (y >= 18 && y <= 20))
                                &&
                                (x >= screenshot.width - 200 + 20 && x <= screenshot.width - 200 + 26)
                            )
                            ||
                            (
                                (x >= screenshot.width - 200 + 20 && x <= screenshot.width - 200 + 21)
                                &&
                                (y >= 11 && y <= 20)
                            )
                            ||
                            (
                                (x >= screenshot.width - 200 + 25 && x <= screenshot.width - 200 + 26)
                                &&
                                (y >= 2 && y <= 11)
                            )
                            ||
                            //T
                            (
                                (x >= screenshot.width - 200 + 30 && x <= screenshot.width - 200 + 37)
                                &&
                                (y >= 19 && y <= 20)
                            )
                            ||
                            (x >= screenshot.width - 200 + 33 && x <= screenshot.width - 200 + 34)
                            ||
                            //O
                            (
                                (x >= screenshot.width - 200 + 42 && x <= screenshot.width - 200 + 45)
                                &&
                                ((y >= 0 && y <= 2) || (y >= 19 && y <= 20))
                            )
                            ||
                            (x >= screenshot.width - 200 + 40 && x <= screenshot.width - 200 + 41)
                            ||
                            (x >= screenshot.width - 200 + 46 && x <= screenshot.width - 200 + 47)
                            ||
                            //R
                            (
                                (x >= screenshot.width - 200 + 50 && x <= screenshot.width - 200 + 57)
                                &&
                                ((y >= 10 && y <= 11) || (y >= 19 && y <= 20))
                            )
                            ||
                            (x >= screenshot.width - 200 + 50 && x <= screenshot.width - 200 + 51)
                            ||
                            (
                                (x >= screenshot.width - 200 + 56 && x <= screenshot.width - 200 + 57)
                                &&
                                (y >= 10 && y <= 20)
                            )
                            ||
                            (
                                (y == 0 && x == screenshot.width - 200 + 57)
                                ||
                                (y == 1 && x == screenshot.width - 200 + 56)
                                ||
                                (y == 2 && x == screenshot.width - 200 + 56)
                                ||
                                (y == 3 && x == screenshot.width - 200 + 55)
                                ||
                                (y == 3 && x == screenshot.width - 200 + 55)
                                ||
                                (y == 4 && x == screenshot.width - 200 + 54)
                                ||
                                (y == 5 && x == screenshot.width - 200 + 54)
                                ||
                                (y == 6 && x == screenshot.width - 200 + 53)
                                ||
                                (y == 7 && x == screenshot.width - 200 + 53)
                                ||
                                (y == 7 && x == screenshot.width - 200 + 52)
                                ||
                                (y == 8 && x == screenshot.width - 200 + 52)
                                ||
                                (y == 9 && x == screenshot.width - 200 + 51)
                                ||
                                (y == 9 && x == screenshot.width - 200 + 51)
                            )
                            ||
                            //Y
                            (
                                (x >= screenshot.width - 200 + 63 && x <= screenshot.width - 200 + 64)
                                &&
                                (y >= 0 && y <= 10)
                            )
                            ||
                                (
                                    (y == 20 && (x == screenshot.width - 200 + 59 || x == screenshot.width - 200 + 68))
                                    ||
                                    (y == 19 && (x == screenshot.width - 200 + 59 || x == screenshot.width - 200 + 68))
                                    ||
                                    (y == 18 && (x == screenshot.width - 200 + 60 || x == screenshot.width - 200 + 67))
                                    ||
                                    (y == 17 && (x == screenshot.width - 200 + 60 || x == screenshot.width - 200 + 67))
                                    ||
                                    (y == 16 && (x == screenshot.width - 200 + 60 || x == screenshot.width - 200 + 67))
                                    ||
                                    (y == 15 && (x == screenshot.width - 200 + 61 || x == screenshot.width - 200 + 66))
                                    ||
                                    (y == 14 && (x == screenshot.width - 200 + 61 || x == screenshot.width - 200 + 66))
                                    ||
                                    (y == 13 && (x == screenshot.width - 200 + 62 || x == screenshot.width - 200 + 65))
                                    ||
                                    (y == 12 && (x == screenshot.width - 200 + 62 || x == screenshot.width - 200 + 65))
                                    ||
                                    (y == 11 && (x == screenshot.width - 200 + 63 || x == screenshot.width - 200 + 64))
                                    ||
                                    (y == 10 && (x == screenshot.width - 200 + 63 || x == screenshot.width - 200 + 64))
                                )
                           ||
                            //M
                            (x >= screenshot.width - 200 + 72 && x <= screenshot.width - 200 + 73)
                            ||
                            (x >= screenshot.width - 200 + 77 && x <= screenshot.width - 200 + 78)
                            ||
                            (x >= screenshot.width - 200 + 82 && x <= screenshot.width - 200 + 83)
                            ||
                            (
                                (x >= screenshot.width - 200 + 72 && x <= screenshot.width - 200 + 83)
                                &&
                                (y >= 19 && y <= 20)
                            )
                            ||
                            //O
                            (
                                (x >= screenshot.width - 200 + 90 && x <= screenshot.width - 200 + 93)
                                &&
                                ((y >= 0 && y <= 2) || (y >= 19 && y <= 20))
                            )
                            ||
                            (x >= screenshot.width - 200 + 88 && x <= screenshot.width - 200 + 89)
                            ||
                            (x >= screenshot.width - 200 + 94 && x <= screenshot.width - 200 + 95)
                            ||
                            //D
                            (
                                (x >= screenshot.width - 200 + 98 && x <= screenshot.width - 200 + 102)
                                &&
                                ((y >= 0 && y <= 2) || (y >= 19 && y <= 20))
                            )
                            ||
                            (x >= screenshot.width - 200 + 98 && x <= screenshot.width - 200 + 99)
                            ||
                            (
                                (x >= screenshot.width - 200 + 104 && x <= screenshot.width - 200 + 105)
                                && 
                                (y >= 4 && y <= 16)
                            )
                            ||
                            (x == screenshot.width - 200 + 103 && (y == 17 || y == 3))
                            ||
                            (x == screenshot.width - 200 + 103 && (y == 18 || y == 2))
                            ||
                            (x == screenshot.width - 200 + 102 && (y == 19 || y == 1))
                            ||
                            (x == screenshot.width - 200 + 102 && (y == 19 || y == 1))
                        )
                        screenshot.SetPixel(x + AddPosX, y + AddPosY, Color.white);
                    }
                }
                screenshot.Apply();

            }


            HistoryMod.debug("HISTORYMOD: CaptureScreen Made");
            byte[] bytes = screenshot.EncodeToPNG();

            HistoryMod.debug("HISTORYMOD: CaptureScreen Encoded");
            string ScreenShotFolder = DataLocation.localApplicationData + "\\" + "historyMod" + "\\";
            HistoryMod.DoesHistoryFolderExists();

            ScreenShotFolder += HistoryMod.screenshotSessionName + "\\";
            if (!Directory.Exists(ScreenShotFolder))
            {
                Directory.CreateDirectory(ScreenShotFolder);
            }
            ScreenShotFolder += "View" + (viewIndex + 1) + "\\";
            if (!Directory.Exists(ScreenShotFolder))
            {
                Directory.CreateDirectory(ScreenShotFolder);
            }
            ScreenShotFolder += subFolder + "\\";
            if (!Directory.Exists(ScreenShotFolder))
            {
                Directory.CreateDirectory(ScreenShotFolder);
            }

            HistoryMod.debug("HISTORYMOD: midscreesnhotting");

            String FilePath = ScreenShotFolder + screenshotName + ".png";

            FileStream swr = File.Create(FilePath);
            swr.Write(bytes, 0, bytes.Length);
            swr.Close();

            /*
            HistoryMod.watermark = HistoryMod.getConfig("watermark", "");
            if (HistoryMod.watermark.Equals(""))
            {
                System.Drawing.Bitmap bitmap = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(FilePath);//load the image file

                using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap))
                {
                    using (System.Drawing.Font arialFont = new System.Drawing.Font("Arial", 10))
                    {
                        graphics.DrawString("Timelapse by", arialFont, System.Drawing.Brushes.Blue, new System.Drawing.PointF(bitmap.Width - 100, bitmap.Height - 24));
                        graphics.DrawString("HistoryMod", arialFont, System.Drawing.Brushes.Red, new System.Drawing.PointF(bitmap.Width - 50, bitmap.Height - 12));
                    }
                }

                bitmap.Save(FilePath);

            }
            */

            HistoryMod.debug("HISTORYMOD: prescreesnhot");

        }


        public void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            if (!HistoryMod.initiateScreenShoterFinalMsg.Equals(""))
            {
                String msg = HistoryMod.initiateScreenShoterFinalMsg;
                HistoryMod.initiateScreenShoterFinalMsg = "";
                HistoryMod.initiateScreenShoterFinal(msg);
            }

            if (HistoryMod.isKeyComboPressed("addView") && HistoryMod.lastAddedViewTime < (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds-4)
            {
                HistoryMod.lastAddedViewTime = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
                HistoryMod.addView();
                ExceptionPanel panel = UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel");
                panel.SetMessage("Screenshoter", "View added", false);
                return;
            }

            if (HistoryMod.isKeyComboPressed("seeView") && HistoryMod.lastFocusViewTime < (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds - 1)
            {
                int view = -1;

                if (Input.GetKeyDown(KeyCode.Keypad1))
                    view = 0;
                else if (Input.GetKeyDown(KeyCode.Keypad2))
                    view = 1;
                else if (Input.GetKeyDown(KeyCode.Keypad3))
                    view = 2;
                else if (Input.GetKeyDown(KeyCode.Keypad4))
                    view = 3;
                else if (Input.GetKeyDown(KeyCode.Keypad5))
                    view = 4;
                else if (Input.GetKeyDown(KeyCode.Keypad6))
                    view = 5;
                else if (Input.GetKeyDown(KeyCode.Keypad7))
                    view = 6;
                else if (Input.GetKeyDown(KeyCode.Keypad8))
                    view = 7;
                else if (Input.GetKeyDown(KeyCode.Keypad9))
                    view = 8;

                if (view >= 0)
                {
                    HistoryMod.lastFocusViewTime = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
                    HistoryMod.loadView(HistoryMod.getView(view));
                    HistoryMod.log("Shortcut to see a view ended");
                }


                return;
            }



            if (HistoryMod.isScreenShoterActive)
            {
                if (HistoryMod.isKeyComboPressed("stopRendering"))
                {
                    HistoryMod.log("stopRendering by shortcut");
                    HistoryMod.stopScreenShoter();
                    HistoryMod.restartGameIfCrash(false);
                    ExceptionPanel panel = UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel");
                    panel.SetMessage("Screenshoter", "Rendering stopped by shift + alt + x pressed.", false);
                    return;
                }

                HistoryMod.debug("HISTORYMOD: isScreenShoterActive yes.");

                if (Singleton<LoadingManager>.instance.m_loadingComplete)
                {
                    if (HistoryMod.ProceededView >= 0)
                    {
                        HistoryMod.nbOfFrames += 1;
                        String labelText = "HistoryMod: screenshot Session initizialised (to stop: shift + alt + x): View " + (HistoryMod.ProceededView + 1) + "/" + HistoryMod.getViewsCount() + " Save " + (HistoryMod.lastLoadedMapIndex + 1) + "/" + (HistoryMod.savesToLoad.Count + 1) + (HistoryMod.restartInterval > 0 ? " Saves loaded since restart: "+ HistoryMod.HowManySaveLoadedSinceLastRestart:"")+" ";
                        if (HistoryMod.nbOfFrames < 10)
                        {
                            HistoryMod.label.text = labelText + " [Starting Frame Count (" + HistoryMod.nbOfFrames + " / 10)]";
                        }

                        if (HistoryMod.getViewsCount() > HistoryMod.ProceededView)
                        {

                            if (HistoryMod.nbOfFrames == 10)
                            {
                                HistoryMod.label.text = labelText + " [Starting Session]";

                                HistoryMod.closeAllPanels();
                                CameraController.FindObjectOfType<CameraController>().m_freeCamera = true;
                                Dictionary<String, String> view = HistoryMod.getView(HistoryMod.ProceededView);

                                HistoryMod.debug("Frame10 x" + view["x"] + " y" + view["y"] + " z" + view["z"] + " Ax" + view["Ax"] + " Ay" + view["Ay"] + " height" + view["height"] + " size" + view["size"] + " field" + view["field"]);

                                HistoryMod.loadView(view);

                                HistoryMod.cancelSpecialWeather();
                                HistoryMod.setTimeOfDay(HistoryMod.screenshotActualCycle);
                            }
                            else if (HistoryMod.nbOfFrames == 20)
                            {
                                HistoryMod.label.text = labelText + " [Taking Screenshot]";

                                String name = "view" + (HistoryMod.ProceededView + 1) + "_" + HistoryMod.cityName;

                                if(HistoryMod.latestSaveGame != null)
                                {
                                    name = name + "_" + HistoryMod.latestSaveGame.name.ToString();
                                }
                                else
                                {
                                    name = name + "_actual";
                                }

                                if (HistoryMod.screenshotsNamesSelectedOption == 3) //only city, view and save name
                                {

                                }
                                else if(HistoryMod.screenshotsNamesSelectedOption == 2) //only city, view and save name
                                {
                                    name = (HistoryMod.savesToLoad.Count - HistoryMod.lastLoadedMapIndex) +"_"+ name;
                                }
                                else if (HistoryMod.screenshotsNamesSelectedOption == 1) //only city, view and save name
                                {
                                    name = HistoryMod.lastLoadedMapIndex + "_" + name;
                                }
                                else if (HistoryMod.screenshotsNamesSelectedOption == 0) //only city, view and save name
                                {
                                    if (HistoryMod.ActualSaveGameMetaData != null)
                                    {
                                        name = HistoryMod.ActualSaveGameMetaData.timeStamp.ToString("yyyyMMddHHmmss") + "_" + name;
                                    }
                                    else
                                    {
                                        name = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + name;
                                    }
                                }

                                takeScreenShot(HistoryMod.ProceededView,(HistoryMod.screenshotActualCycle ? "night": "day"), name);

                                CameraController.FindObjectOfType<CameraController>().m_freeCamera = false;
                            }
                            else if (HistoryMod.nbOfFrames == 30 && HistoryMod.CSLExport && HistoryMod.CSLMade == false)
                            {
                                HistoryMod.CSLMade = true;
                                HistoryMod.label.text = labelText + " [Exporting CSL View]";
                                
                                HistoryMod.cslExportNow();
                                

                                

                            }
                            else if (HistoryMod.nbOfFrames > 40)
                            {
                                HistoryMod.label.text = labelText + " [Ending Session]";
                                HistoryMod.nbOfFrames = 0;
                                HistoryMod.ProceededView += 1;
                            }
                        }
                        else
                        {
                            if (HistoryMod.nbOfFrames == 5)
                            {
                                if (HistoryMod.dayOrNightForScreenshots == 2 && HistoryMod.screenshotActualCycle == false)
                                {
                                    HistoryMod.nbOfFrames = 0;
                                    HistoryMod.ProceededView = 0;
                                    HistoryMod.screenshotActualCycle = true;
                                }
                                else if(HistoryMod.dayOrNightForScreenshots == 2 && HistoryMod.screenshotActualCycle == true)
                                {
                                    HistoryMod.screenshotActualCycle = false;
                                }
                            }

                            if (HistoryMod.nbOfFrames == 10)
                            {
                                HistoryMod.CSLMade = false;
                                HistoryMod.label.text = labelText + " [Looking for next save]";
                                if (!HistoryMod.onlyThisSave && HistoryMod.lastLoadedMapIndex < HistoryMod.savesToLoad.Count)
                                {
                                    HistoryMod.latestSaveGame = HistoryMod.savesToLoad[HistoryMod.lastLoadedMapIndex];
                                    SaveGameMetaData saveGameMetaData = HistoryMod.latestSaveGame.Instantiate<SaveGameMetaData>();
                                    HistoryMod.lastLoadedMapIndex++;

                                    if (saveGameMetaData != null)
                                    {
                                        HistoryMod.ActualSaveGameMetaData = saveGameMetaData;
                                        HistoryMod.saveConfig("startFromThisSave", HistoryMod.latestSaveGame.name.ToString());
                                        HistoryMod.HowManySaveLoadedSinceLastRestart++;

                                        bool restartGame = false;
                                        if(Application.platform == RuntimePlatform.WindowsPlayer)
                                            switch (HistoryMod.restartInterval)
                                            {
                                                case 0:
                                                    break;
                                                case 1: // 20
                                                    if (HistoryMod.HowManySaveLoadedSinceLastRestart > 20)
                                                        restartGame = true;
                                                    break;
                                                case 2: // 30
                                                    if (HistoryMod.HowManySaveLoadedSinceLastRestart > 30)
                                                        restartGame = true;
                                                    break;
                                                case 3: // 40
                                                    if (HistoryMod.HowManySaveLoadedSinceLastRestart > 40)
                                                        restartGame = true;
                                                    break;
                                                case 4: // 10
                                                    if (HistoryMod.HowManySaveLoadedSinceLastRestart > 10)
                                                        restartGame = true;
                                                    break;
                                                case 5: // 5
                                                    if (HistoryMod.HowManySaveLoadedSinceLastRestart > 5)
                                                        restartGame = true;
                                                    break;
                                                case 6: // 3
                                                    if (HistoryMod.HowManySaveLoadedSinceLastRestart > 3)
                                                        restartGame = true;
                                                    break;
                                                case 7: // 2
                                                    if (HistoryMod.HowManySaveLoadedSinceLastRestart > 2)
                                                        restartGame = true;
                                                    break;
                                                case 8: // 1
                                                    if (HistoryMod.HowManySaveLoadedSinceLastRestart > 1)
                                                        restartGame = true;
                                                    break;
                                                default:
                                                    break;
                                            }

                                        if (restartGame)
                                        {
                                            HistoryMod.log("RESTART ");
                                            HistoryMod.stopGame(true);
                                        }
                                        else
                                        {
                                            HistoryMod.log("LOAD NEXT SAVE ");
                                            HistoryMod.loadSave(saveGameMetaData);

                                        }

                                    }
                                }
                                else
                                {
                                    HistoryMod.log("FINISHED");
                                    HistoryMod.saveConfig("startFromThisSave","");
                                    HistoryMod.stopScreenShoter();

                                    HistoryMod.restartGameIfCrash(false);
                                    if (!HistoryMod.onlyThisSave && HistoryMod.stopGameAfterScreenshotSession)
                                        HistoryMod.stopGame(false);

                                    ExceptionPanel panel = UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel");
                                    panel.SetMessage("Screenshoter is finished !", "Success ! All screenshots were taken with success.", false);
                                }


                            }
                        }
                    }
                }
            }
            else
            {
                HistoryMod.debug("HISTORYMOD: isScreenShoterActive false.");
            }
        }
    }

}
