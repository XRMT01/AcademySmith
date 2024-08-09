
using FrameworkNs;
using FrameworkNs.ArchiveNs;
using FrameworkNs.IoC;
using FrameworkNs.UtilitiesNs;
using Lanka.CharacterSystem;
using Lanka.School.CertificateSystem;
using ProjectSchoolModNs.Publish;
using ProjectSchoolNs;
using ProjectSchoolNs.CertificateNs;
using ProjectSchoolNs.ChallengeNs;
using ProjectSchoolNs.CharacterNs;
using ProjectSchoolNs.DispatchNs;
using ProjectSchoolNs.DLCNs;
using ProjectSchoolNs.RedPointNs;
using ProjectSchoolNs.SchoolNs;
using ProjectSchoolNs.SchoolScoreNs;
using ProjectSchoolNs.SettingNs;
using ProjectSchoolNs.TimeNs;
using ProjectSchoolNs.UINs;
using ProjectSchoolNs.WorldNs;
using ProjectSchoolNs.WorldNs.FacilityNs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.InputSystem;


namespace AcademySmith
{
    // 游戏中的模块
    // In-game modules
    public class Modules : M_GameModuleBase, IArchiveableModule, IArchiveBeforeLoadProcessModule, IArchiveAfterLoadProcessModule, IArchiveBeforeSaveProcessModule, IArchiveAfterSaveProcessModule
    {

        public class Mono : MonoBehaviour
        {
            private bool showUI = false;
            private bool showFPSWindow = false;
            private bool showCurrencyWindow = false;
            private bool showTimeWindow = false;
            private bool showMilestonesWindow = false;
            private bool showTestWindow = false;
            private bool showSettingsWindow = false;
            private bool showAboutWindow = false;
            private bool showCharacterWindow = false;
            private bool showCharacterModifcationWindow = false;
            private bool showRoleModifcationWindow = false;

            private bool showFPSInMain = false; // 新增的变量，控制FPS是否在主窗口内显示
            public Rect windowRect = new Rect(300, 300, 200, 200);
            private Rect fpsWindowRect = new Rect(500, 300, 200, 100);
            private Rect CurrencyWindowRect = new Rect(500, 300, 200, 200);
            private Rect TimeWindowRect = new Rect(500, 300, 200, 200);
            private Rect MilestonesWindowRect = new Rect(500, 300, 200, 200);
            private Rect TestWindowRect = new Rect(500, 300, 200, 200);
            private Rect CharacterWindowRect = new Rect(500, 300, 200, 200);
            private Rect CharacterModifcationWindowRect = new Rect(500, 300, 200, 200);
            private Rect RoleModifcationWindowRect = new Rect(500, 300, 200, 200);
            private Rect settingsWindowRect = new Rect(500, 300, 250, 100);
            private Rect aboutRect = new Rect(500, 300, 200, 100);


            private float refreshInterval = 0.2f;
            private float _duration;
            private int _frames;
            private string _fps;
            private string _fpss;

            /*private AcademyData academyData;*/
            private string[] options = { "中文", "English" };
            public int language = 1;
            public int FonSize = 18;

            //公告部分
            private string NoticeTitle;
            private string NoticeContent;


            //界面设置
            public string Home_widthStr = "300";
            public string Home_heightStr = "0";

            //全地图解锁
            private bool Exposure = false;

            private CommonButton UIbutton;
            public static Sprite CreateSpriteFromPath(string imagePath)
            {
                // 加载图片数据
                byte[] imageData = File.ReadAllBytes(imagePath);

                // 创建Texture2D
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadRawTextureData(imageData); // 加载图片数据到Texture2D

                // 从Texture2D创建Sprite
                Sprite sprite = Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f), // Sprite的原点
                    100.0f // Pixels per Unit
                );

                return sprite;
            }
            void Start()
            {
                UIUpdate();

                GameSpeed = new string[] { language == 1 ? "快速" : "Fast", language == 1 ? "一般" : "Normal", language == 1 ? "漫长" : "Slow" };

                UIbutton = CommonButton.NewButton("CommonButton", language == 1 ? "学府工具" : "Academic Tools", () =>
                {
                    showUI = !showUI;
                });
                UIbutton.GetOrAddComponent<AdaptiveRectTransform>().SetTarget(new Vector2(0f, Screen.height - GetScaledHeight(600f)), AdaptiveRectTransform.CheckMode.All_Best);

                switch (language)
                {
                    case 1:
                        NoticeTitle = Framework.I18nMgr.GetText("学府工具（mod）公告");
                        NoticeContent = Framework.I18nMgr.GetText("点击键盘Home或者Delete打开主菜单，当前版本是0.0.8");
                        break;
                    case 0:
                        NoticeTitle = Framework.I18nMgr.GetText("Academy Tools (mod) Announcement");
                        NoticeContent = Framework.I18nMgr.GetText("Click on the keyboard Home or Delete to open the main menu, the current version is 0.0.8");
                        break;
                    default:
                        break;
                }

                CommonMessageBox.NewBox(NoticeTitle, NoticeContent, (Sprite)null /*CreateSpriteFromPath("Image1.png")*/, (IList<CommonButton.CommonButtonConfig>)new CommonButton.CommonButtonConfig[1] { CommonButton.NewConfirmConfig(Singleton<GameManagerV2>.Instance.SaveGame) }, canClose: false, (CommonMessageBox.OnMessageClose)null, "CommonMessageBox", uiPause: true, Mask: true, 999f);
                /*CommonMessageBox.NewSoftMessageBoxV2(NoticeTitle, NoticeContent, CreateSpriteFromPath("Image1.png"), (IList<CommonButton.CommonButtonConfig>)new CommonButton.CommonButtonConfig[1] { CommonButton.NewConfirmConfig(Singleton<GameManagerV2>.Instance.SaveGame) },  (CommonMessageBox.OnMessageClose)null, "CommonMessageBox",  Mask: true, 999f);*/

                //初始化搜索列表功能
                InitSeachModeList();

            }

            void Update()
            {

                if (Keyboard.current.homeKey.wasPressedThisFrame||Keyboard.current.deleteKey.wasPressedThisFrame)
                {
                    showUI = !showUI;
                }

                if (Keyboard.current.zKey.wasPressedThisFrame && Keyboard.current.leftCtrlKey.isPressed)
                {
                    windowRect = new Rect(300, 300, 200, 200);
                    fpsWindowRect = new Rect(500, 300, 200, 100);
                    CurrencyWindowRect = new Rect(500, 300, 200, 200);
                    TimeWindowRect = new Rect(500, 300, 200, 200);
                    MilestonesWindowRect = new Rect(500, 300, 200, 200);
                    TestWindowRect = new Rect(500, 300, 200, 200);
                    CharacterWindowRect = new Rect(500, 300, 200, 200);
                    CharacterModifcationWindowRect = new Rect(500, 300, 200, 200);
                    settingsWindowRect = new Rect(500, 300, 250, 100);
                    aboutRect = new Rect(500, 300, 200, 100);
                    Home_widthStr = "200";
                    Home_heightStr = "0";
                    FonSize = 13;
                    UIUpdate();
                }

                UIbutton.buttonText.text = language == 1 ? "学府工具" : "Academic Tools";
                UIbutton.GetOrAddComponent<AdaptiveRectTransform>().SetTarget(new Vector2(0f, Screen.height - GetScaledHeight(600f)), AdaptiveRectTransform.CheckMode.All_Best);

                _duration += Time.unscaledDeltaTime;
                _frames++;
                if (_duration >= refreshInterval)
                {
                    _fps = $"FPS: {(float)_frames / _duration:F1}";
                    _duration = 0f;
                    _frames = 0;
                }


                if (showTimeSpeedInMain)
                {
                    if (Module<TimeModule>.Instance.SimulationSpeed.GetSpeed() <= TimeSpeed)
                    {
                        Module<TimeModule>.Instance.SetTimeSpeed((int)TimeSpeed);
                    }
                }

            }

            public void UIUpdate()
            {
                windowRect.width = GetScaledWidth(int.Parse(Home_widthStr));
                fpsWindowRect.width = GetScaledWidth(int.Parse(Home_widthStr));
                fpsWindowRect.height = 0f;
                CurrencyWindowRect.width = GetScaledWidth(int.Parse(Home_widthStr));
                CurrencyWindowRect.height = 0f;
                TimeWindowRect.width = GetScaledWidth(int.Parse(Home_widthStr));
                TimeWindowRect.height = 0f;
                TestWindowRect.width = GetScaledWidth(int.Parse(Home_widthStr));
                TestWindowRect.height = 0f;
                settingsWindowRect.width = GetScaledWidth(int.Parse(Home_widthStr));
                settingsWindowRect.height = 0f;
                CharacterWindowRect.width = GetScaledWidth(int.Parse(Home_widthStr));
                CharacterWindowRect.height = 0f;
                CharacterModifcationWindowRect.width =  GetScaledWidth(int.Parse(Home_widthStr));
                CharacterModifcationWindowRect.height = int.Parse(Home_heightStr);
                aboutRect.width = GetScaledWidth(int.Parse(Home_widthStr));
                aboutRect.height = 0f;
            }

            //语言设置
            private void DrawAllWindows()
            {
                if (windowRect.width > GetScaledWidth(200)) 
                {
                    windowRect.width = GetScaledWidth(200);
                }

                if (showUI)
                {
                    windowRect = GUILayout.Window(1, windowRect, DrawWindow, language == 1 ? "学府工具" : "Academic Tools");
                }

                if (showFPSWindow)
                {
                    fpsWindowRect = GUILayout.Window(2, fpsWindowRect, DrawFPSWindow, "FPS");
                }

                if (showCurrencyWindow)
                {
                    CurrencyWindowRect = GUILayout.Window(3, CurrencyWindowRect, DrawCurrencyWindow, language == 1 ? "货币与点数" : "Currency And Points");
                }

                if (showTimeWindow)
                {
                    TimeWindowRect = GUILayout.Window(4, TimeWindowRect, DrawTimeWindow, language == 1 ? "时间" : "Time");
                }

                if (showMilestonesWindow)
                {
                    MilestonesWindowRect = GUILayout.Window(5, MilestonesWindowRect, DrawMilestonesWindow, language == 1 ? "游戏进度里程碑" : "Game Progress Milestones");
                }

                if (showTestWindow)
                {
                    TestWindowRect = GUILayout.Window(6, TestWindowRect, DrawTestWindow, language == 1 ? "测试功能" : "Test Function");
                }

                if (showCharacterWindow)
                {
                    CharacterWindowRect = GUILayout.Window(7, CharacterWindowRect, DrawCharacterWindow, language == 1 ? "角色属性修改" : "Character attribute modification");
                }

                if (showCharacterModifcationWindow)
                {
                    CharacterModifcationWindowRect = GUILayout.Window(8, CharacterModifcationWindowRect, DrawCharacterModificationWindow, language == 1 ? "角色属性修改" : "Character attribute modification");
                }

                if (showRoleModifcationWindow)
                {
                    RoleModifcationWindowRect = GUILayout.Window(9, RoleModifcationWindowRect, DrawCertificateWindow, language == 1 ? "角色属性修改" : "Character attribute modification");
                }

                if (showSettingsWindow)
                {
                    settingsWindowRect = GUILayout.Window(98, settingsWindowRect, DrawSettingsWindow, language == 1 ? "窗口设置" : "Window Settings");
                }

                if (showAboutWindow)
                {
                    aboutRect = GUILayout.Window(99, aboutRect, DrawAboutWindow, language == 1 ? "关于" : "About");
                }
            }
            void OnGUI()
            {
                if (language == 0 || language == 1)
                {
                    DrawAllWindows();
                }
            }


            void DrawWindow(int windowID)
            {
                GUILayout.BeginVertical();

                GUILayout.Label(GetScaledWidth(50).ToString());

                GUIStyle LabelStyle = new GUIStyle(GUI.skin.label);
                LabelStyle.fontSize = FonSize;

                GUIStyle ButtonStyle = new GUIStyle(GUI.skin.button);
                ButtonStyle.fontSize = FonSize;

                GUIStyle ToogleStyle = new GUIStyle(GUI.skin.toggle);
                ToogleStyle.fontSize = FonSize;


                ToogleStyle.stretchWidth = true;
                ToogleStyle.stretchHeight = true;

                GUILayout.Label(language == 1 ? "学府工具" : "Academic Tools", LabelStyle);
                GUILayout.Label(_fpss, LabelStyle);
                GUILayout.Label(language == 1 ? "在设置尺寸的时候出现过大情况下可以按Ctrl+Z恢复默认大小" : "When setting the size, if it is too large, you can press Ctrl+Z to restore the default size", LabelStyle);

                if (GUILayout.Button(options[language], ButtonStyle))
                {
                    language++;
                    if (language > options.Length - 1)
                    {
                        language = 0;
                    }
                    InitSeachModeList();
                }



                if (GUILayout.Button(language == 1 ? "显示FPS" : "Display FPS", ButtonStyle))
                {

                    _fpss = _fps;
                }



                showFPSInMain = GUILayout.Toggle(showFPSInMain, language == 1 ? "在主窗口显示FPS" : "Display FPS in the main window", ToogleStyle);
                if (showFPSInMain)
                {

                    showFPSWindow = true;
                }
                else
                {
                    showFPSWindow = false;
                }

                if (GUILayout.Button(language == 1 ? "货币与点数" : "Currency And Points", ButtonStyle))
                {
                    showCurrencyWindow = !showCurrencyWindow;
                }

                if (GUILayout.Button(language == 1 ? "时间" : "Time", ButtonStyle))
                {
                    showTimeWindow = !showTimeWindow;
                }

                if (GUILayout.Button(language == 1 ? "角色属性修改" : "Character attribute modification", ButtonStyle))
                {
                    showCharacterModifcationWindow = !showCharacterModifcationWindow;
                }

                if (GUILayout.Button(language == 1 ? "游戏进度里程碑" : "Game Progress Milestones", ButtonStyle))
                {
                    showMilestonesWindow = !showMilestonesWindow;
                }

                if (GUILayout.Button(language == 1 ? "测试功能" : "Test Function", ButtonStyle))
                {
                    showTestWindow = !showTestWindow;

                }

                if (GUILayout.Button(language == 1 ? "设置" : "Set Up", ButtonStyle))
                {
                    showSettingsWindow = true;
                }


                if (GUILayout.Button(language == 1 ? "关于" : "About", ButtonStyle))
                {
                    showAboutWindow = !showAboutWindow;
                }
                if (GUILayout.Button(language == 1 ? "关闭" : "Close", ButtonStyle))
                {
                    showUI = !showUI;
                }




                GUILayout.EndVertical();

                GUI.DragWindow();
            }

            void DrawFPSWindow(int windowID)
            {

                GUIStyle LabelStyle = new GUIStyle(GUI.skin.label);
                LabelStyle.fontSize = FonSize;

                GUIStyle ButtonStyle = new GUIStyle(GUI.skin.button);
                ButtonStyle.fontSize = FonSize;
                GUILayout.BeginVertical();

                GUILayout.Label(_fps, LabelStyle);

                if (GUILayout.Button(language == 1 ? "关闭FPS窗口" : "Close FPS Window", ButtonStyle))
                {
                    showFPSWindow = false;
                    showFPSInMain = false;
                }

                GUILayout.EndVertical();
                GUI.DragWindow();
            }


            private int Currency = 100;//货币
            private int Culture = 100;//文科
            private int ScienceScore = 100;//理科
            private int ArtsScore = 100;//艺术
            private int SportsScore = 100;//体育
            private int PTAPoints = 100;//家
            void DrawCurrencyWindow(int mainWindowID)
            {

                GUIStyle LabelStyle = new GUIStyle(GUI.skin.label);
                LabelStyle.fontSize = FonSize;

                GUIStyle ButtonStyle = new GUIStyle(GUI.skin.button);
                ButtonStyle.fontSize = FonSize;


                GUILayout.BeginVertical();

                GUILayout.Label(language == 1 ? "货币与点数" : "Currency and Points", LabelStyle);


                GUILayout.Label(language == 1 ? "货币:" : "Currency:", LabelStyle);

                GUILayout.BeginHorizontal();
                Currency = int.Parse(GUILayout.TextField(Currency.ToString(), GUILayout.Width(80)));

                if (GUILayout.Button(language == 1 ? "加¥" : "Add ¥", ButtonStyle))
                {
                    Module<SchoolModule>.Instance.AddMoney(Currency, MoneyUseType.other);
                    OnTis(language == 1 ? $"加{Currency}" : $"Add {Currency}");
                }
                GUILayout.EndHorizontal();

                if (GUILayout.Button(language == 1 ? "加500¥" : "Add 500 ¥", ButtonStyle))
                {
                    Module<SchoolModule>.Instance.AddMoney(500, MoneyUseType.other);
                    OnTis(language == 1 ? "加500¥" : "Add 500 ¥");
                }

                if (GUILayout.Button(language == 1 ? "加5000¥" : "Add 5000 ¥", ButtonStyle))
                {

                    Module<SchoolModule>.Instance.AddMoney(5000, MoneyUseType.other);
                    OnTis(language == 1 ? "加5000¥" : "Add 5000 ¥");
                }

                if (GUILayout.Button(language == 1 ? "加5万¥" : "Add 5M ¥", ButtonStyle))
                {

                    Module<SchoolModule>.Instance.AddMoney(50000, MoneyUseType.other);
                    OnTis(language == 1 ? "加50000¥" : "Add 50000 ¥");
                }


                GUILayout.Label(language == 1 ? "文科得分:" : "CultureScore:", LabelStyle);

                GUILayout.BeginHorizontal();
                Culture = int.Parse(GUILayout.TextField(Culture.ToString(), GUILayout.Width(80)));

                if (GUILayout.Button(language == 1 ? "加" : "Add", ButtonStyle))
                {
                    Module<SchoolModule>.Instance.ModifyCurrency(CurrencyType.CultureScore, Culture, MoneyUseType.other);
                    OnTis(language == 1 ? $"加{Culture}" : $"Add {Culture}");
                }
                GUILayout.EndHorizontal();
                if (GUILayout.Button(language == 1 ? "加500" : "Add 500", ButtonStyle))
                {
                    Module<SchoolModule>.Instance.ModifyCurrency(CurrencyType.CultureScore, 500, MoneyUseType.other);
                    OnTis(language == 1 ? "加500" : "Add 500");
                }

                if (GUILayout.Button(language == 1 ? "加5000" : "Add 5000", ButtonStyle))
                {
                    Module<SchoolModule>.Instance.ModifyCurrency(CurrencyType.CultureScore, 5000, MoneyUseType.other);
                    OnTis(language == 1 ? "加5000" : "Add 5000");
                }





                GUILayout.Label(language == 1 ? "理科得分:" : "ScienceScore:", LabelStyle);

                GUILayout.BeginHorizontal();
                ScienceScore = int.Parse(GUILayout.TextField(ScienceScore.ToString(), GUILayout.Width(80)));

                if (GUILayout.Button(language == 1 ? "加" : "Add", ButtonStyle))
                {
                    Module<SchoolModule>.Instance.ModifyCurrency(CurrencyType.ScienceScore, ScienceScore, MoneyUseType.other);
                    OnTis(language == 1 ? $"加{Culture}" : $"Add {Culture}");
                }
                GUILayout.EndHorizontal();
                if (GUILayout.Button(language == 1 ? "加500" : "Add 500", ButtonStyle))
                {
                    Module<SchoolModule>.Instance.ModifyCurrency(CurrencyType.ScienceScore, 500, MoneyUseType.other);
                    OnTis(language == 1 ? "加500" : "Add 500");
                }

                if (GUILayout.Button(language == 1 ? "加5000" : "Add 5000", ButtonStyle))
                {
                    Module<SchoolModule>.Instance.ModifyCurrency(CurrencyType.ScienceScore, 5000, MoneyUseType.other);
                    OnTis(language == 1 ? "加5000" : "Add 5000");
                }


                GUILayout.Label(language == 1 ? "艺术得分:" : "ArtsScore:", LabelStyle);

                GUILayout.BeginHorizontal();
                ArtsScore = int.Parse(GUILayout.TextField(ArtsScore.ToString(), GUILayout.Width(80)));

                if (GUILayout.Button(language == 1 ? "加" : "Add", ButtonStyle))
                {
                    Module<SchoolModule>.Instance.ModifyCurrency(CurrencyType.ArtsScore, ArtsScore, MoneyUseType.other);
                    OnTis(language == 1 ? $"加{Culture}" : $"Add {Culture}");
                }
                GUILayout.EndHorizontal();
                if (GUILayout.Button(language == 1 ? "加500" : "Add 500", ButtonStyle))
                {
                    Module<SchoolModule>.Instance.ModifyCurrency(CurrencyType.ArtsScore, 500, MoneyUseType.other);
                    OnTis(language == 1 ? "加500" : "Add 500");
                }

                if (GUILayout.Button(language == 1 ? "加5000" : "Add 5000", ButtonStyle))
                {
                    Module<SchoolModule>.Instance.ModifyCurrency(CurrencyType.ArtsScore, 5000, MoneyUseType.other);
                    OnTis(language == 1 ? "加5000" : "Add 5000");
                }


                GUILayout.Label(language == 1 ? "体育得分:" : "SportsScore:", LabelStyle);

                GUILayout.BeginHorizontal();
                SportsScore = int.Parse(GUILayout.TextField(SportsScore.ToString(), GUILayout.Width(80)));

                if (GUILayout.Button(language == 1 ? "加" : "Add", ButtonStyle))
                {
                    Module<SchoolModule>.Instance.ModifyCurrency(CurrencyType.SportsScore, SportsScore, MoneyUseType.other);
                    OnTis(language == 1 ? $"加{Culture}" : $"Add {Culture}");
                }
                GUILayout.EndHorizontal();
                if (GUILayout.Button(language == 1 ? "加500" : "Add 500", ButtonStyle))
                {
                    Module<SchoolModule>.Instance.ModifyCurrency(CurrencyType.SportsScore, 500, MoneyUseType.other);
                    OnTis(language == 1 ? "加500" : "Add 500");
                }

                if (GUILayout.Button(language == 1 ? "加5000" : "Add 100", ButtonStyle))
                {
                    Module<SchoolModule>.Instance.ModifyCurrency(CurrencyType.SportsScore, 5000, MoneyUseType.other);
                    OnTis(language == 1 ? "加5000" : "Add 5000");
                }



                GUILayout.Label(language == 1 ? "家长协会点数:" : "PTA Points:", LabelStyle);

                GUILayout.BeginHorizontal();
                PTAPoints = int.Parse(GUILayout.TextField(PTAPoints.ToString(), GUILayout.Width(80)));

                if (GUILayout.Button(language == 1 ? "加" : "Add", ButtonStyle))
                {
                    Module<SchoolModule>.Instance.ModifyCurrency(CurrencyType.PTAPoints, PTAPoints, MoneyUseType.other);
                    OnTis(language == 1 ? $"加{Culture}" : $"Add {Culture}");
                }
                GUILayout.EndHorizontal();
                if (GUILayout.Button(language == 1 ? "加500" : "Add 500", ButtonStyle))
                {
                    Module<SchoolModule>.Instance.ModifyCurrency(CurrencyType.PTAPoints, 500, MoneyUseType.other);
                    OnTis(language == 1 ? "加500" : "Add 500");
                }

                if (GUILayout.Button(language == 1 ? "加5000" : "Add 5000", ButtonStyle))
                {
                    Module<SchoolModule>.Instance.ModifyCurrency(CurrencyType.PTAPoints, 5000, MoneyUseType.other);
                    OnTis(language == 1 ? "加5000" : "Add 100");
                }


                if (GUILayout.Button(language == 1 ? "关闭" : "Close", ButtonStyle))
                {
                    showCurrencyWindow = false;
                }
                GUILayout.EndVertical();

                GUI.DragWindow();
            }

            private float TimeSpeed = 0;
            private bool showTimeSpeedInMain = false;

            private string[] GameSpeed;
            private int GameSpeedSel = 0;
            void DrawTimeWindow(int mainWindowID)
            {
                GUIStyle LabelStyle = new GUIStyle(GUI.skin.label);
                LabelStyle.fontSize = FonSize;

                GUIStyle ButtonStyle = new GUIStyle(GUI.skin.button);
                ButtonStyle.fontSize = FonSize;

                GUIStyle ToogleStyle = new GUIStyle(GUI.skin.toggle);
                ToogleStyle.fontSize = FonSize;


                GUILayout.BeginVertical();

                GUILayout.Label(language == 1 ? "时间" : "Time", LabelStyle);

                GUILayout.Label(language == 1 ? "天:" : "day:", LabelStyle);


                if (GUILayout.Button(language == 1 ? "跳过一天" : "Skip a day", ButtonStyle))
                {
                    Module<TimeModule>.Instance.JumpTimeByDay(1);
                    OnTis(language == 1 ? "跳过一天" : "Skip a day");
                }
                if (GUILayout.Button(language == 1 ? "倒退一天" : "Go back one day", ButtonStyle))
                {
                    Module<TimeModule>.Instance.JumpTimeByDay(-1);
                    OnTis(language == 1 ? "倒退一天" : "Go back one day");
                }


                GUILayout.Label(language == 1 ? "时间速度" : "Time speed", LabelStyle);
                GUILayout.Label(language == 1 ? $"值:{TimeSpeed.ToString()}" : $"value:{TimeSpeed.ToString()}", LabelStyle);
                TimeSpeed = GUILayout.HorizontalSlider(TimeSpeed, 1f, 100f); // 创建一个水平滑动条

                if (GUILayout.Button(language == 1 ? "修改" : "Modify", ButtonStyle))
                {
                    Module<TimeModule>.Instance.SetTimeSpeed((int)TimeSpeed);
                }

                showTimeSpeedInMain = GUILayout.Toggle(showTimeSpeedInMain, language == 1 ? "维持修改后速度" : "Maintain Modified Speed", ToogleStyle);
                if (showTimeSpeedInMain)
                {

                    showTimeSpeedInMain = true;
                }
                else
                {
                    showTimeSpeedInMain = false;
                }


                GUILayout.Label(language == 1 ? "游戏节奏" : "Game Speed", LabelStyle);
                GameSpeedSel = GUILayout.SelectionGrid(GameSpeedSel, GameSpeed, 3, ButtonStyle);
                Module<GameModeModule>.Instance.GameStartParameter.gameLoopSpeed = GameLoopSpeed.fast;
                switch (GameSpeedSel)
                {
                    case 0:
                        Module<GameModeModule>.Instance.GameStartParameter.gameLoopSpeed = GameLoopSpeed.fast;
                        break;
                    case 1:
                        Module<GameModeModule>.Instance.GameStartParameter.gameLoopSpeed = GameLoopSpeed.Normal;
                        break;
                    case 2:
                        Module<GameModeModule>.Instance.GameStartParameter.gameLoopSpeed = GameLoopSpeed.slow;
                        break;
                }

                if (GUILayout.Button(language == 1 ? "关闭" : "Close", ButtonStyle))
                {
                    showTimeWindow = false;
                }


                GUILayout.EndVertical();

                GUI.DragWindow();
            }



            private SchoolUpgradeInfo UpgradeInfo = new SchoolUpgradeInfo();
            private CurrencyPacket CurrencyPacket = new CurrencyPacket();
            private int score = 0;
            void DrawMilestonesWindow(int mainWindowID)
            {
                GUIStyle LabelStyle = new GUIStyle(GUI.skin.label);
                LabelStyle.fontSize = FonSize;

                GUIStyle ButtonStyle = new GUIStyle(GUI.skin.button);
                ButtonStyle.fontSize = FonSize;

                GUIStyle ToogleStyle = new GUIStyle(GUI.skin.toggle);
                ToogleStyle.fontSize = FonSize;

                GUIStyle TextField = new GUIStyle(GUI.skin.textField);
                TextField.fontSize = FonSize;

                GUILayout.BeginVertical();

                GUILayout.Label(language == 1 ? "游戏进度里程碑" : "Game Progress Milestones", LabelStyle);

                GUILayout.Space(5);
                if (GUILayout.Button(language == 1 ? "解锁所有研究内容" : "Unlock all research content", ButtonStyle))
                {
                    //获取升级得分

                    UpgradeInfo = Module<SchoolModule>.Instance.SchoolLoopConfig.CalLvData(Module<SchoolScoreModule>.Instance.GetTotalScore());

                    //货币与点数

                    CurrencyPacket = Module<SchoolModule>.Instance.CurrencyPacket;

                    Module<TechnologyModule>.Instance.ForceUnlockAllTechnology();
                    string t = language == 1 ? "你可以选择直接解锁获得奖励\r\n也可以选择只解锁研究内容\r\n点击下方按钮选择即可" : "You can choose to unlock directly to receive rewards\r\nYou can also choose to unlock only the research content\r\nClick the button below to select";
                    CommonButton.CommonButtonConfig[] buttonConfigs = new CommonButton.CommonButtonConfig[2]
                    {
                        CommonButton.NewConfig(Framework.I18nMgr.GetText(language == 1 ?"全部解锁":"unlock all"), UnlockAllStudies),
                        CommonButton.NewConfig(Framework.I18nMgr.GetText(language == 1 ?"仅解锁研究内容":"Only unlock research content"), OnlyResearchContent)
                    };
                    CommonMessageBox.NewBox(Framework.I18nMgr.GetText(language == 1 ? "解锁所有研究内容" : "Unlock all research content"), Framework.I18nMgr.GetText(t), (Sprite)null, buttonConfigs, canClose: false, (CommonMessageBox.OnMessageClose)null, "CommonMessageBox", uiPause: true, Mask: true, 999f);
                }

                //GUILayout.TextArea(UpgradeInfo.Level.ToString() + "\n" + UpgradeInfo.LevelProgress.ToString() + "\n" + UpgradeInfo.Exp.ToString() + "\n" + UpgradeInfo.NextExp.ToString());






                GUILayout.Space(2);
                GUILayout.Label(language == 1 ? "增加评分" : "Increase rating", LabelStyle);
                score = int.Parse(GUILayout.TextField(score.ToString(), TextField));
                if (GUILayout.Button(language == 1 ? "增加" : "Add", ButtonStyle))
                {
                    Module<SchoolScoreModule>.Instance.ModifyScore("挑战", score);
                }

                if (GUILayout.Button(language == 1 ? "升满级级" : "Upgrade to full level", ButtonStyle))
                {
                    Module<SchoolScoreModule>.Instance.ModifyScore("挑战", 99999);
                    Module<SchoolModule>.Instance.RefreshLvData(true);
                }
                GUILayout.Space(2);
                if (GUILayout.Button(language == 1 ? "全地图揭露" : "Full Map Exposure", ButtonStyle))
                {
                    UI_Main.Instance.OpenDispatchPanel();
                    string t = language == 1 ? "1.需要研究全部解锁才能使用\r\n2.若是发现没有完全解锁完，可以尝试保存读档重新尝试" : "1. It is necessary to study unlocking everything before using it\r\nIf you find that you haven't fully unlocked it, you can try saving the file and trying again";
                    CommonButton.CommonButtonConfig[] buttonConfigs = new CommonButton.CommonButtonConfig[2]
                    {
                        CommonButton.NewConfig(Framework.I18nMgr.GetText(language == 1 ?"使用":"Apply"), ExposureOpen),
                        CommonButton.NewConfig(Framework.I18nMgr.GetText(language == 1 ?"取消":"Cancel"), ExposureClose)
                    };
                    CommonMessageBox.NewBox(Framework.I18nMgr.GetText(language == 1 ? "功能使用" : "Function usage", language == 1 ? "功能使用" : "Function usage"), Framework.I18nMgr.GetText(t), (Sprite)null, buttonConfigs, canClose: false, (CommonMessageBox.OnMessageClose)null, "CommonMessageBox", uiPause: true, Mask: true, 999f);
                }


                bool UnlockAllProps = true;
                DLCConfigCollectionConfig a = Singleton<DLCManager>.Instance.Config;
                foreach (DLCPathConfig item in a.configMapper)
                {
                    if (!Singleton<DLCManager>.Instance.IsHaveDlc(item.id))
                    {
                        UnlockAllProps = false;
                    };
                }

                if (UnlockAllProps)
                {
                    if (GUILayout.Button(language == 1 ? "全部家具解锁" : "Unlock all furniture", ButtonStyle))
                    {

                        MapFurnitureSubModule mapFurnitureSubModule = new MapFurnitureSubModule();
                        mapFurnitureSubModule.UnlockAllFurniture();

                    }
                }


                if (GUILayout.Button(language == 1 ? "全部家具解锁(无DLC验证)" : "Unlock all furniture", ButtonStyle))
                {

                    MapFurnitureSubModule mapFurnitureSubModule = new MapFurnitureSubModule();
                    mapFurnitureSubModule.UnlockAllFurniture();

                }

                if (GUILayout.Button(language == 1 ? "关闭" : "Close", ButtonStyle))
                {
                    showMilestonesWindow = false;
                }


                GUILayout.EndVertical();

                GUI.DragWindow();
            }
            private int Dis = 1;

            void UnlockAllStudies()
            {
                OnTis(language == 1 ? "全部解锁研究内容" : "Unlock all research content");
            }

            void OnlyResearchContent()
            {
                foreach (ChallengeInfo item in Module<ChallengeModule>.Instance.ChallengeInfos)
                {
                    item.ResetCompletedState();
                    Singleton<RedPointManager>.Instance.ClearAllRedPoint("Challenge.Panel.{0}", item.Id);
                }

                Module<SchoolModule>.Instance.ModifyCurrency(CurrencyType.Gold, -(Module<SchoolModule>.Instance.CurrencyPacket.Gold - CurrencyPacket.Gold), MoneyUseType.other);
                Module<SchoolModule>.Instance.ModifyCurrency(CurrencyType.ArtsScore, -(Module<SchoolModule>.Instance.CurrencyPacket.PerceptivityContribution - CurrencyPacket.PerceptivityContribution), MoneyUseType.other);
                Module<SchoolModule>.Instance.ModifyCurrency(CurrencyType.ScienceScore, -(Module<SchoolModule>.Instance.CurrencyPacket.IntelligenceContribution - CurrencyPacket.IntelligenceContribution), MoneyUseType.other);
                Module<SchoolModule>.Instance.ModifyCurrency(CurrencyType.CultureScore, -(Module<SchoolModule>.Instance.CurrencyPacket.MemoryContribution - CurrencyPacket.MemoryContribution), MoneyUseType.other);
                Module<SchoolModule>.Instance.ModifyCurrency(CurrencyType.SportsScore, -(Module<SchoolModule>.Instance.CurrencyPacket.CorporeityContribution - CurrencyPacket.CorporeityContribution), MoneyUseType.other);
                Module<SchoolModule>.Instance.ModifyCurrency(CurrencyType.PTAPoints, -(Module<SchoolModule>.Instance.CurrencyPacket.PTAPoint - CurrencyPacket.PTAPoint), MoneyUseType.other);

                Module<SchoolScoreModule>.Instance.ModifyScore("挑战", -(Module<SchoolModule>.Instance.UpgradeInfo.Exp - UpgradeInfo.Exp));

            }



            private int builNum = 0;
            void DrawTestWindow(int mainWindowID)
            {
                GUIStyle LabelStyle = new GUIStyle(GUI.skin.label);
                LabelStyle.fontSize = FonSize;

                GUIStyle LabelStyles = new GUIStyle(GUI.skin.label);
                LabelStyles.fontSize = FonSize - 5;

                GUIStyle ButtonStyle = new GUIStyle(GUI.skin.button);
                ButtonStyle.fontSize = FonSize;

                GUIStyle ToogleStyle = new GUIStyle(GUI.skin.toggle);
                ToogleStyle.fontSize = FonSize;

                GUILayout.BeginVertical();

                GUILayout.Label(language == 1 ? "测试功能" : "Test Function", LabelStyle);

                GUILayout.Label(language == 1 ? "测试功能可能会出现游戏崩溃谨慎使用" : "Testing functions may cause game crashes. Use with caution", LabelStyles);

                /*GlobalToolsModule.BuilderType[] builderTypes = new GlobalToolsModule.BuilderType[]
                {
                    GlobalToolsModule.BuilderType.empty,
                    GlobalToolsModule.BuilderType.furnitureBuilder,
                    GlobalToolsModule.BuilderType.OrganizationBuilder,
                    GlobalToolsModule.BuilderType.furnitureBuilderV2
                };


                BuildingBuilder building = GlobalToolsModule.buildingBuilder;
                if (GUILayout.Button(language == 1 ? "切换建筑模式" : "Switch building mode", ButtonStyle))    
                {
                    Building building1 = building.currentArea as Building;
                    building1.SetBuidingName("XRMT");
                }*/

                if (GUILayout.Button(language == 1 ? "关闭" : "Close", ButtonStyle))
                {
                    showTestWindow = false;

                }
                GUILayout.EndVertical();

                GUI.DragWindow();
            }

            void ExposureOpen()
            {
                Module<DispatchModule>.Instance.UncoverAllZones();
            }

            private void ExposureClose()
            {

            }

            private Vector2 CharacterModificationScrollPosition = Vector2.zero;
            private CharacterEntity characterEntity;
            private Teacher teacher;
            private Staff staff;

            private float CharacteModificationX = 0;
            private float CharacteModificationY = 200;
            private bool CharacteModificationScrollView = false;
            private Dictionary<string, bool> SearchModeList = new Dictionary<string, bool>();
            private string[] SearchMode;


            private CharacterJobType jobType;//选择的职业
            private Sex sex;

            //角色属性修改
            void DrawCharacterModificationWindow(int mainWindowID)
            {
                SearchMode = new string[]
                {
                    language == 1 ? "性别" : "gender",
                    language == 1 ? "职业" : "career",
                };

                string[] jobTypeList = new string[]
                {
                    language == 1 ? "全部" : "all",
                    language == 1 ? "老师" : "teacher",
                    language == 1 ? "其他员工" : "otherStaff",
                    language == 1 ? "学生" : "student"
                };

                Dictionary<string, CharacterJobType> jobTypeDict = new Dictionary<string, CharacterJobType>()
                {
                    {language == 1 ? "全部" : "all", CharacterJobType.all},
                    {language == 1 ? "老师" : "teacher", CharacterJobType.teacher},
                    {language == 1 ? "其他员工" : "otherStaff", CharacterJobType.otherStaff},
                    {language == 1 ? "学生" : "student", CharacterJobType.student}
                };

                string[] genderList = new string[]
                {
                    language == 1 ? "男" : "male",
                    language == 1 ? "女" : "female",
                    language == 1 ? "其他" : "other"
                };

                Dictionary<string, Sex> genderDict = new Dictionary<string, Sex>()
                {
                    {language == 1 ? "男" : "male", Sex.male},
                    {language == 1 ? "女" : "female", Sex.female},
                    {language == 1 ? "其他" : "other", Sex.other}
                };

                //防止窗口过于小
                if (CharacterModifcationWindowRect.width < GetScaledWidth(200))
                {
                    CharacterModifcationWindowRect.width = GetScaledWidth(200);
                    CharacterWindowRect.width = GetScaledWidth(200);
                }

                GUIStyle LabelStyle = new GUIStyle(GUI.skin.label);
                LabelStyle.fontSize = FonSize;

                GUIStyle ButtonStyle = new GUIStyle(GUI.skin.button);
                ButtonStyle.fontSize = FonSize;

                GUIStyle TextField = new GUIStyle(GUI.skin.textField);
                TextField.fontSize = FonSize;

                GUILayout.BeginVertical();

                GUILayout.Label(language == 1 ? "只有来到学校的角色修改" : "Only the characters who have arrived to school can be modified", LabelStyle);


                if (GUILayout.Button(language == 1 ? "关闭设置" : "Shutdown Settings", ButtonStyle))
                {
                    showCharacterModifcationWindow = false;
                }
                GUIStyle ButtonStyles = new GUIStyle(GUI.skin.button);

                GUILayout.Label(language == 1 ? "查找模式" : "Search Mode", LabelStyle);


                for (int i = 0; i < ((int)Math.Ceiling((double)SearchMode.Length / 3)); i++)
                {
                    GUILayout.BeginHorizontal();
                    int len = i * 3 + 3 > SearchMode.Length ? SearchMode.Length : i * 3 + 3;
                    for (int j = i * 3; j < len; j++)
                    {
                        if (GUILayout.Button(SearchMode[j], ButtonStyles, GUILayout.Width(GetScaledWidth(100))))
                        {
                            SetSearchModeListAllbool(false);
                            SearchModeList[SearchMode[j]] = !SearchModeList[SearchMode[j]];
                            if (SearchModeList[SearchMode[j]])
                            {
                                CharacteModificationX = 0;
                                CharacteModificationY += GetScaledHeight(75);
                            }
                            else
                            {
                                CharacteModificationX = 0;
                                CharacteModificationY -= GetScaledHeight(75);
                            }
                        }
                    }
                    GUILayout.EndHorizontal();
                }

                //职业筛选
                if (SearchModeList[language == 1 ? "职业" : "career"])
                {
                    GUILayout.Label(language == 1 ? "职业:" : "Career:", LabelStyle);
                    for (int i = 0; i < ((int)Math.Ceiling((double)jobTypeList.Length / 3)); i++)
                    {
                        GUILayout.BeginHorizontal();
                        int len = i * 3 + 3 > jobTypeList.Length ? jobTypeList.Length : i * 3 + 3;
                        for (int j = i * 3; j < len; j++)
                        {
                            if (GUILayout.Button(jobTypeList[j], ButtonStyles, GUILayout.Width(GetScaledWidth(100))))
                            {
                                jobType = jobTypeDict[jobTypeList[j]];
                            }
                        }
                        GUILayout.EndHorizontal();
                    }
                }


                //性别筛选
                if (SearchModeList[language == 1 ? "性别" : "gender"])
                {
                    GUILayout.Label(language == 1 ? "性别" : "gender", LabelStyle);
                    for (int i = 0; i < ((int)Math.Ceiling((double)genderList.Length / 3)); i++)
                    {
                        GUILayout.BeginHorizontal();
                        int len = i * 3 + 3 > genderList.Length ? genderList.Length : i * 3 + 3;
                        for (int j = i * 3; j < len; j++)
                        {
                            if (GUILayout.Button(genderList[j], ButtonStyles, GUILayout.Width(GetScaledWidth(100))))
                            {
                                sex = genderDict[genderList[j]];
                            }
                        }
                        GUILayout.EndHorizontal();
                    }
                }


                /*string searchModeTxt = "";
                for (int i = 0; i < SearchMode.Length; i++)
                {
                    searchModeTxt += SearchMode[i] + "-" + SearchModeList[SearchMode[i]] + "\n";
                }

                GUILayout.TextArea(searchModeTxt);*/

                //人员生成
                CharacterModifcationWindowRect.height = GetScaledHeight(800);
                Rect srollRect = new Rect(CharacteModificationX, CharacteModificationY, CharacterModifcationWindowRect.width, CharacterModifcationWindowRect.height - GetScaledHeight(400));
                GUILayout.BeginArea(srollRect); // 定义滚动区域的大小和位置
                CharacterModificationScrollPosition = GUILayout.BeginScrollView(CharacterModificationScrollPosition); // 开始可滚动区域，并存储滚动位置


                foreach (CharacterEntity item in Module<CharacterModule>.Instance.AttendanceList)
                {
                    if (SearchModeList[language == 1 ? "性别" : "gender"])
                    {

                        if (sex == item.CharacterInfo.Sex)
                        {
                            CreteCharacterInfo(item, ButtonStyle, LabelStyle);
                        }
                    }
                    else if (SearchModeList[language == 1 ? "职业" : "career"])
                    {
                        if (jobType == CharacterJobType.all)
                        {
                            CreteCharacterInfo(item, ButtonStyle, LabelStyle);
                        }
                        else if (item.Job.JobType == jobType || (item.Job.JobType == CharacterJobType.president && jobType == CharacterJobType.teacher))
                        {
                            CreteCharacterInfo(item, ButtonStyle, LabelStyle);
                        }
                    }
                }

                GUILayout.EndScrollView(); // 结束可滚动区域
                GUILayout.EndArea(); // 结束区域



                GUILayout.EndVertical();

                GUI.DragWindow();
            }

            void SetSearchModeListAllbool(bool b) 
            {
                for (int i = 0; i < SearchMode.Length ; i++) 
                {
                    SearchModeList[SearchMode[i]] = b;
                }
            }

            void InitSeachModeList()
            {

                //一级菜单
                SearchModeList = new Dictionary<string, bool>()
                {
                    {language == 1 ? "性别" : "gender", false},
                    {language == 1 ? "职业" : "career", false}
                };
            }


            void CreteCharacterInfo(CharacterEntity item, GUIStyle ButtonStyle, GUIStyle LabelStyle)
            {
                GUILayout.Label(item.Job.JobName, LabelStyle);
                if (GUILayout.Button(item.Name, ButtonStyle))
                {
                    showCharacterWindow = true;
                    characterEntity = item;
                    teacher = item.Job as Teacher;
                    staff = item.Job as Staff;
                }
            }


            //公用
            private string CharacterName;

            //学生
            private float Memory;
            private float Intelligence;
            private float Perceptivity;
            private float Corporeity;

            //老师
            private float TeachingAbility;//教学
            private float ResearchingAbility;//科研
            private float ManagingAbility;//管理
            private float CultivatingAbility;//培养
            private float MedicalAbility;//医疗
            private float CookingAbility;//烹饪
            private float SaleAbility;//销售
            private float SecurityAbility;//安保
            private float AllAbility;//全部
            private int staffabliityNum = 0;
            private int staffNum = 0;//薪资
            private int staffLeve = 0;//等级

            //角色其他属性修改
            void DrawCharacterWindow(int mainWindowID)
            {
                GUIStyle LabelStyle = new GUIStyle(GUI.skin.label);
                LabelStyle.fontSize = FonSize;

                GUIStyle ButtonStyle = new GUIStyle(GUI.skin.button);
                ButtonStyle.fontSize = FonSize;

                GUIStyle TextField = new GUIStyle(GUI.skin.textField);
                TextField.fontSize = FonSize;


                //家境
                string[] familyCircumstancesList = new string[]
                {
                    language == 1 ? "超级贫困" : "SuperPoor",
                    language == 1 ? "贫穷的" : "Poor",
                    language == 1 ? "正常" : "Normal",
                    language == 1 ? "富有的" : "Rich",
                    language == 1 ? "超级富豪" : "HSuperRich"
                };

                Dictionary<string, FamilyCircumstancesType> familyCircumstancesDict = new Dictionary<string, FamilyCircumstancesType>()
                {
                    {language == 1 ? "超级贫困" : "SuperPoor", FamilyCircumstancesType.SuperPoor},
                    {language == 1 ? "贫穷的" : "Poor", FamilyCircumstancesType.Poor},
                    {language == 1 ? "正常" : "Normal", FamilyCircumstancesType.Normal},
                    {language == 1 ? "富有的" : "Rich", FamilyCircumstancesType.Rich},
                    {language == 1 ? "超级富豪" : "HSuperRich", FamilyCircumstancesType.SuperRich
                    }
                };


                //出现方式
                string[] appearanceWayList = new string[]
                {
                    language == 1 ? "步行" : "walk",
                    language == 1 ? "自行车" : "Bicycle",
                    language == 1 ? "公共汽车" : "Bus",
                    language == 1 ? "电、缆车" : "Subway",
                    language == 1 ? "私家车" : "PrivateCar",
                    language == 1 ? "住宿" : "Stay",
                    language == 1 ? "飞机" : "Airplane"
                };

                Dictionary<string,CommutingType > appearanceWayDict = new Dictionary<string,CommutingType>()
                {
                    {language == 1 ? "步行" : "walk", CommutingType.Foot},
                    {language == 1 ? "自行车" : "Bicycle", CommutingType.Bicycle},
                    {language == 1 ? "公共汽车" : "Bus", CommutingType.Bus},
                    {language == 1 ? "电、缆车" : "Subway", CommutingType.Subway},
                    {language == 1 ? "私家车" : "PrivateCar", CommutingType.PrivateCar},
                    {language == 1 ? "住宿" : "Stay", CommutingType.Stay},
                    {language == 1 ? "飞机" : "Airplane",CommutingType.Airplane }
                };


                GUILayout.BeginVertical();


                if (characterEntity.Job.JobType == CharacterJobType.teacher || characterEntity.Job.JobType == CharacterJobType.president || characterEntity.Job.JobType == CharacterJobType.otherStaff)
                {

                    GUILayout.Label(language == 1 ? $"{characterEntity.Name}老师，数据修改" : $"{characterEntity.Name}Teacher, data modification", LabelStyle);
                    GUILayout.Label(language == 1 ? "名字修改" : "Name modification", LabelStyle);
                    GUILayout.BeginHorizontal();
                    CharacterName = GUILayout.TextField(CharacterName, TextField);
                    if (GUILayout.Button(language == 1 ? "设置" : "Set Up", ButtonStyle))
                    {
                        characterEntity.CharacterInfo.Name = I18nText.NewRawValue(CharacterName);
                    }
                    GUILayout.EndHorizontal();


                    GUILayout.Label(language == 1 ? "家境修改" : "Family background modification", LabelStyle);
                    for (int i = 0; i < ((int)Math.Ceiling((double)familyCircumstancesList.Length / 3)); i++)
                    {
                        GUILayout.BeginHorizontal();
                        int len = i * 3 + 3 > familyCircumstancesList.Length ? familyCircumstancesList.Length : i * 3 + 3;
                        for (int j = i * 3; j < len; j++)
                        {
                            if (GUILayout.Button(familyCircumstancesList[j], ButtonStyle, GUILayout.Width(GetScaledWidth(100))))
                            {
                                characterEntity.CharacterInfo.FamilyCircumstancesType = familyCircumstancesDict[familyCircumstancesList[j]];
                            }
                        }
                        GUILayout.EndHorizontal();
                    }

                    GUILayout.Label(language == 1 ? "出现方式" : "Travel mode", LabelStyle);
                    for (int i = 0; i < ((int)Math.Ceiling((double)appearanceWayList.Length / 3)); i++)
                    {
                        GUILayout.BeginHorizontal();
                        int len = i * 3 + 3 > appearanceWayList.Length ? appearanceWayList.Length : i * 3 + 3;
                        for (int j = i * 3; j < len; j++)
                        {
                            if (GUILayout.Button(appearanceWayList[j], ButtonStyle, GUILayout.Width(GetScaledWidth(100))))
                            {
                                characterEntity.CharacterInfo.SetCommutingType(appearanceWayDict[appearanceWayList[j]]);
                            }
                        }
                        GUILayout.EndHorizontal();
                    }



                    GUILayout.BeginHorizontal();
                    GUILayout.Label(language == 1 ? "教学能力" : "Teaching Ability", LabelStyle);
                    GUILayout.Label(language == 1 ? "研究能力" : "Researching Ability", LabelStyle);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    TeachingAbility = float.Parse(GUILayout.TextField(TeachingAbility.ToString(), TextField));
                    if (GUILayout.Button(language == 1 ? "设置" : "Set Up", ButtonStyle))
                    {
                        teacher.TeachingAbility.RawValue = TeachingAbility;
                    }

                    
                    ResearchingAbility = float.Parse(GUILayout.TextField(ResearchingAbility.ToString(), TextField));
                    if (GUILayout.Button(language == 1 ? "设置" : "Set Up", ButtonStyle))
                    {
                        teacher.ResearchingAbility.RawValue = ResearchingAbility;
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label(language == 1 ? "管理能力" : "Managing Ability", LabelStyle);
                    GUILayout.Label(language == 1 ? "进修能力" : "Learning Ability", LabelStyle);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    ManagingAbility = float.Parse(GUILayout.TextField(ManagingAbility.ToString(), TextField));
                    if (GUILayout.Button(language == 1 ? "设置" : "Set Up", ButtonStyle))
                    {
                        teacher.ManagingAbility.RawValue = ManagingAbility;
                    }

                    
                    CultivatingAbility = float.Parse(GUILayout.TextField(CultivatingAbility.ToString(), TextField));
                    if (GUILayout.Button(language == 1 ? "设置" : "Set Up", ButtonStyle))
                    {
                        teacher.CultivatingAbility.RawValue = CultivatingAbility;
                    }

                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label(language == 1 ? "薪资调整" : "Salary Adjustment", LabelStyle);
                    GUILayout.Label(language == 1 ? "老师等级" : "Teacher level", LabelStyle);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    staffNum = int.Parse(GUILayout.TextField(staffNum.ToString(), TextField));
                    if (GUILayout.Button(language == 1 ? "设置" : "Set Up", ButtonStyle))
                    {
                        staff.RawSalary = staffNum;
                        staff.RawexpectSalary = staffNum;
                    }


                   

                    staffLeve = int.Parse(GUILayout.TextField(staffLeve.ToString(), TextField));
                    if (GUILayout.Button(language == 1 ? "设置" : "Set Up", ButtonStyle))
                    {
                        staff.staffRace.Rank = staffLeve;
                    }

                    GUILayout.EndHorizontal();

                    if (staff.abilityDict.ContainsKey(StaffAbilityType.MedicalAbility))
                    {
                        GUILayout.Label(language == 1 ? "医疗能力" : "Medical Ability", LabelStyle);
                        MedicalAbility = int.Parse(GUILayout.TextField(MedicalAbility.ToString(), TextField));
                        if (GUILayout.Button(language == 1 ? "设置" : "Set Up", ButtonStyle))
                        {
                            staff.abilityDict[StaffAbilityType.MedicalAbility].RawValue = MedicalAbility;
                        }
                    }

                    if (staff.abilityDict.ContainsKey(StaffAbilityType.CookingAbility))
                    {
                        GUILayout.Label(language == 1 ? "烹饪能力" : "Cooking Ability", LabelStyle);
                        CookingAbility = int.Parse(GUILayout.TextField(CookingAbility.ToString(), TextField));
                        if (GUILayout.Button(language == 1 ? "设置" : "Set Up", ButtonStyle))
                        {
                            staff.abilityDict[StaffAbilityType.CookingAbility].RawValue = CookingAbility;
                        }
                    }

                    if (staff.abilityDict.ContainsKey(StaffAbilityType.SaleAbility))
                    {
                        GUILayout.Label(language == 1 ? "销售能力" : "Sale Ability", LabelStyle);
                        SaleAbility = int.Parse(GUILayout.TextField(SaleAbility.ToString(), TextField));
                        if (GUILayout.Button(language == 1 ? "设置" : "Set Up", ButtonStyle))
                        {
                            staff.abilityDict[StaffAbilityType.SaleAbility].RawValue = SaleAbility;
                        }
                    }

                    if (staff.abilityDict.ContainsKey(StaffAbilityType.SecurityAbility))
                    {
                        GUILayout.Label(language == 1 ? "安保能力" : "Security Ability", LabelStyle);
                        SecurityAbility = int.Parse(GUILayout.TextField(SecurityAbility.ToString(), TextField));
                        if (GUILayout.Button(language == 1 ? "设置" : "Set Up", ButtonStyle))
                        {
                            staff.abilityDict[StaffAbilityType.SecurityAbility].RawValue = SecurityAbility;
                        }
                    }

                    GUILayout.Label(language == 1 ? "所有能力一键修改" : "All abilities can be modified with just one click", LabelStyle);
                    AllAbility = int.Parse(GUILayout.TextField(AllAbility.ToString(), TextField));
                    if (GUILayout.Button(language == 1 ? "一键设置" : "One click settings", ButtonStyle))
                    {
                        foreach (var item in staff.abilityDict)
                        {
                            item.Value.RawValue = AllAbility;
                        }
                    }

                    if (GUILayout.Button(language == 1 ? "证书修改" : "Certificate Modification", ButtonStyle))
                    {
                        showRoleModifcationWindow = true;
                    }


                }
                else if (characterEntity.Job.JobType == CharacterJobType.student)
                {

                    GUILayout.Label(language == 1 ? "名字修改" : "Name modification", LabelStyle);

                    GUILayout.BeginHorizontal();
                    CharacterName = GUILayout.TextField(CharacterName, TextField);
                    if (GUILayout.Button(language == 1 ? "设置" : "Set Up", ButtonStyle))
                    {
                        characterEntity.CharacterInfo.Name = I18nText.NewRawValue(CharacterName);
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Label(language == 1 ? "家境修改" : "Family background modification", LabelStyle);
                    for (int i = 0; i < ((int)Math.Ceiling((double)familyCircumstancesList.Length / 3)); i++)
                    {
                        GUILayout.BeginHorizontal();
                        int len = i * 3 + 3 > familyCircumstancesList.Length ? familyCircumstancesList.Length : i * 3 + 3;
                        for (int j = i * 3; j < len; j++)
                        {
                            if (GUILayout.Button(familyCircumstancesList[j], ButtonStyle, GUILayout.Width(GetScaledWidth(100))))
                            {
                                characterEntity.CharacterInfo.FamilyCircumstancesType = familyCircumstancesDict[familyCircumstancesList[j]];
                            }
                        }
                        GUILayout.EndHorizontal();
                    }


                    GUILayout.Label(language == 1 ? "出现方式" : "Travel mode", LabelStyle);
                    for (int i = 0; i < ((int)Math.Ceiling((double)appearanceWayList.Length / 3)); i++)
                    {
                        GUILayout.BeginHorizontal();
                        int len = i * 3 + 3 > appearanceWayList.Length ? appearanceWayList.Length : i * 3 + 3;
                        for (int j = i * 3; j < len; j++)
                        {
                            if (GUILayout.Button(appearanceWayList[j], ButtonStyle, GUILayout.Width(GetScaledWidth(100))))
                            {
                                characterEntity.CharacterInfo.SetCommutingType(appearanceWayDict[appearanceWayList[j]]);
                            }
                        }
                        GUILayout.EndHorizontal();
                    }


                    GUILayout.Label($"{characterEntity.Name}同学，数据修改", LabelStyle);
                    GUILayout.Label(language == 1 ? "记忆" : "Memory", LabelStyle);
                    Memory = float.Parse(GUILayout.TextField(Memory.ToString(), TextField));
                    if (GUILayout.Button(language == 1 ? "设置" : "Set Up", ButtonStyle))
                    {
                        characterEntity.ModifyBaseProperty(Memory, BasePropertyType.Memory);
                    }

                    GUILayout.Label(language == 1 ? "智慧" : "Memory", LabelStyle);
                    Intelligence = float.Parse(GUILayout.TextField(Intelligence.ToString(), TextField));
                    if (GUILayout.Button(language == 1 ? "设置" : "Set Up", ButtonStyle))
                    {
                        characterEntity.ModifyBaseProperty(Intelligence, BasePropertyType.Intelligence);
                    }

                    GUILayout.Label(language == 1 ? "感知" : "Perceptivity", LabelStyle);
                    Perceptivity = float.Parse(GUILayout.TextField(Perceptivity.ToString(), TextField));
                    if (GUILayout.Button(language == 1 ? "设置" : "Set Up", ButtonStyle))
                    {
                        characterEntity.ModifyBaseProperty(Perceptivity, BasePropertyType.Perceptivity);
                    }

                    GUILayout.Label(language == 1 ? "体质" : "Corporeity", LabelStyle);
                    Corporeity = float.Parse(GUILayout.TextField(Corporeity.ToString(), TextField));
                    if (GUILayout.Button(language == 1 ? "设置" : "Set Up", ButtonStyle))
                    {
                        characterEntity.ModifyBaseProperty(Corporeity, BasePropertyType.Corporeity);
                    }
                }

                if (GUILayout.Button(language == 1 ? "关闭设置" : "Shutdown Settings", ButtonStyle))
                {
                    showCharacterWindow = false;
                }

                GUILayout.EndVertical();

                GUI.DragWindow();
            }
            //证书修改界面
            private Vector2 CertificateScrollPosition = Vector2.zero;
            void DrawCertificateWindow(int mainWindowID)
            {
                if (RoleModifcationWindowRect.width < GetScaledWidth(300f))
                {
                    RoleModifcationWindowRect.width = GetScaledWidth(300f);
                }

                GUIStyle LabelStyle = new GUIStyle(GUI.skin.label);
                LabelStyle.fontSize = FonSize;

                GUIStyle ButtonStyle = new GUIStyle(GUI.skin.button);
                ButtonStyle.fontSize = FonSize;

                GUIStyle TextField = new GUIStyle(GUI.skin.textField);
                TextField.fontSize = FonSize;

                CertificateBigType[] certificateBigType = new CertificateBigType[]
                    {
                        CertificateBigType.principal,
                        CertificateBigType.management,
                        CertificateBigType.research,
                        CertificateBigType.service,
                        CertificateBigType.teaching_culture,
                        CertificateBigType.teaching_science,
                        CertificateBigType.teaching_art,
                        CertificateBigType.teaching_sports,
                        CertificateBigType.staff_security,
                        CertificateBigType.staff_doctor,
                        CertificateBigType.staff_sell,
                        CertificateBigType.staff_chef,
                    };

                GUILayout.BeginVertical();

                RoleModifcationWindowRect.height = GetScaledHeight(800);

                if (GUILayout.Button(language == 1 ? "全部证书解锁" : "Unlock all certificates", ButtonStyle))
                {
                    foreach (CertificateBigType types in certificateBigType)
                    {
                        foreach (CertificateData items in Module<CultivationModule>.Instance.certificateBigTypeDict[types])
                        {
                            staff.UnlockCertificate(items.Id, true);
                        }
                    }
                }



                GUILayout.BeginArea(new Rect(0, GetScaledHeight(400), RoleModifcationWindowRect.width, GetScaledHeight(600))); // 定义滚动区域的大小和位置
                CertificateScrollPosition = GUILayout.BeginScrollView(CertificateScrollPosition); // 开始可滚动区域，并存储滚动位置
                for(int i = 0;i< certificateBigType.Length;i++)
                {
                    GUILayout.BeginHorizontal();
                    foreach (CertificateData item in Module<CultivationModule>.Instance.certificateBigTypeDict[certificateBigType[i]])
                    {
                        if (GUILayout.Button(item.CertificateNameI18n.GetText(), ButtonStyle))
                        {
                            staff.UnlockCertificate(item.Id, true);
                        }
                    }
                    GUILayout.EndHorizontal();

                }
                GUILayout.EndScrollView(); // 结束可滚动区域
                GUILayout.EndArea(); // 结束区域

                GUILayout.Space(10);

                if (GUILayout.Button(language == 1 ? "关闭设置" : "Shutdown Settings", ButtonStyle))
                {
                    showRoleModifcationWindow = false;
                }
                GUILayout.EndVertical();

                GUI.DragWindow();
            }


            //设置界面
            void DrawSettingsWindow(int mainWindowID)
            {
                GUIStyle LabelStyle = new GUIStyle(GUI.skin.label);
                LabelStyle.fontSize = FonSize;

                GUIStyle ButtonStyle = new GUIStyle(GUI.skin.button);
                ButtonStyle.fontSize = FonSize;

                GUIStyle TextField = new GUIStyle(GUI.skin.textField);
                TextField.fontSize = FonSize;

                GUILayout.BeginVertical();


                GUILayout.Label(language == 1 ? "主界面尺寸设置" : "Main interface size setting", LabelStyle);

                GUILayout.Label(language == 1 ? "在设置尺寸的时候出现过大情况下可以按Ctrl+Z恢复默认大小" : "When setting the size, if it is too large, you can press Ctrl+Z to restore the default size", LabelStyle);

                GUILayout.BeginHorizontal();

                GUILayout.Label(language == 1 ? "宽度: " : "Width:", LabelStyle);
                Home_widthStr = GUILayout.TextField(Home_widthStr, GUILayout.Width(GetScaledWidth(50)));

                GUILayout.Label(language == 1 ? "高度: " : "Height:", LabelStyle);
                Home_heightStr = GUILayout.TextField(Home_heightStr, GUILayout.Width(GetScaledWidth(50)));
                if (GUILayout.Button(language == 1 ? "设置" : "Set Up", ButtonStyle))
                {
                    windowRect.width = Mathf.Max(100, float.Parse(Home_widthStr));
                    windowRect.height = Mathf.Max(100, float.Parse(Home_heightStr));
                    FonSize = (int)Math.Floor(int.Parse(Home_widthStr) / 15f);
                    UIUpdate();
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label(language == 1 ? "字体大小: " : "FonSize:", LabelStyle);
                FonSize = int.Parse(GUILayout.TextField(FonSize.ToString(), GUILayout.Width(GetScaledWidth(80))));

                GUILayout.EndHorizontal();



                GUILayout.Space(10);

                if (GUILayout.Button(language == 1 ? "关闭设置" : "Shutdown Settings", ButtonStyle))
                {
                    showSettingsWindow = false;
                }

                GUILayout.EndVertical();

                GUI.DragWindow();
            }

            // 函数用于根据当前屏幕宽度计算缩放后的宽度
            float GetScaledWidth(float targetWidth)
            {
                // 获取屏幕宽度
                float screenWidth = Screen.width;

                // 计算缩放因子
                float scaleFactor = screenWidth / 1920f;

                // 返回缩放后的宽度
                return targetWidth * scaleFactor;
            }

            float GetScaledHeight(float targetHeight)
            {
                // 获取屏幕宽度
                float screenHeight = Screen.height;

                // 计算缩放因子
                float scaleFactor = targetHeight / 1080f;

                // 返回缩放后的宽度
                return targetHeight * scaleFactor;
            }
            void DrawAboutWindow(int windowID)
            {

                GUIStyle LabelStyle = new GUIStyle(GUI.skin.label);
                LabelStyle.fontSize = FonSize;

                GUIStyle ButtonStyle = new GUIStyle(GUI.skin.button);
                ButtonStyle.fontSize = FonSize;

                GUILayout.BeginVertical();

                GUILayout.Label(language == 1 ? "名称：学府工具" : "Name: Academy Tools", LabelStyle);
                GUILayout.Label(language == 1 ? "版本：0.0.8" : "Version: 0.0.8", LabelStyle);
                GUILayout.Label(language == 1 ? "作者：XRMT" : "Author: XRMT", LabelStyle);

                if (GUILayout.Button("关闭"))
                {
                    showAboutWindow = false;
                }



                GUILayout.EndVertical();

                GUI.DragWindow();
            }


            bool IsNumber(string str)
            {
                return Regex.IsMatch(str, @"^-?\d+(\.\d+)?$");
            }

            private void OnFestivalStart(FestivalConfig config)
            {
                // 这里是当节日开始时执行的代码
                Debug.Log("Festival started with config: " + config.name);
                // 根据config做些事情...
            }

            private void OnDestroy()
            {

            }

            void OnTis(string txt)
            {
                string text = Framework.I18nMgr.GetText(txt);
                CommonMessageBox.NewSoftMessageBoxV2("", text, null, null, null, 5f, useUnScaleTime: true);
                Singleton<RedPointManager>.Instance.AddRedPointPrompt($"Build.Envirounment.Wall.123");
            }

        }
        private Mono mono;

        // 游戏上下文接口
        // Game context interface
        M_IGameContext gameContext;
        M_ITimeContext timeContext;
        M_IUIContext uiContext;




        // 服务容器
        // service container
        IServiceContainer serviceContainer;

        protected override void OnCreate()
        {
            // 从模组上下文中获取游戏上下文接口
            // Gets the game context interface from the module context
            modContext.RunnerContext.TryGetContext(out gameContext);
            // 获取服务容器
            // Get service container
            serviceContainer = gameContext.Publish.FirstTWithGC<IServiceContainer>();
            uiContext = gameContext.UIContext;
            timeContext = gameContext.TimeContext;
        }

        protected override void OnLoad()
        {
            // 模块加载
            modContext.LogError("Module Load");
            timeContext.OnMinuteUpdate += OpenFPS;
        }

        protected override void OnStart()
        {
            // 模块启动
            modContext.LogError("Module Start");
            timeContext.OnMinuteUpdate += OnMinuteUpdate;

            mono = new GameObject("AcademySmithModule_UI").AddComponent<Mono>();
            UnityEngine.Object.DontDestroyOnLoad(mono.gameObject);

        }



        void IArchiveBeforeLoadProcessModule.OnBeforeLoadArchive(ArchiveContext archiveContext)
        {
            // 开始加载存档
            modContext.LogError("Before Load Archive");

        }

        void IArchiveableModule.OnLoadArchive(ArchiveContext archiveContext)
        {
            // 加载存档
            modContext.LogError("Load Archive");
            if (archiveContext.IsNewArchive)
            {
                // TODO: 新游戏处理
                // TODO: new game process
            }
            else
            {
                // 获取存档数据
                // Get archive data
                var saveData = archiveContext.GetData<SaveData>();
                mono.Home_widthStr = saveData.H_width.ToString();
                mono.Home_heightStr = saveData.H_height.ToString();
                mono.FonSize = saveData.FontSize;
                mono.UIUpdate();
                // TODO: 非新游戏处理
                // TODO: not new game process
            }
        }

        void IArchiveAfterLoadProcessModule.OnAfterLoadArchive(ArchiveContext archiveContext)
        {
            // 结束加载存档
            modContext.LogError("After Load Archive");

        }

        void IArchiveBeforeSaveProcessModule.OnBeforeSaveArchive(ArchiveContext archiveContext)
        {
            // 开始保存存档
            modContext.LogError("Before Save Archive");

        }

        void IArchiveableModule.OnSaveArchive(ArchiveContext archiveContext)
        {
            // 保存存档
            modContext.LogError("Save Archive");

            // 获取模块管理器
            // Gets the module manager
            var moduleManager = serviceContainer.GetService<ModuleManager>();
            // 获取地图模块
            // Get map module
            var mapModule = moduleManager.GetModule<MapModule>();
            // 获取角色模块
            // Get character module
            var characterModule = moduleManager.GetModule<CharacterModule>();
            // 获取用于存档的数据
            // Gets data for archiving


            var saveData = archiveContext.GetData<SaveData>();

            saveData.language = mono.language;
            saveData.H_width = float.Parse(mono.Home_widthStr);
            saveData.H_height = float.Parse(mono.Home_heightStr);
            saveData.FontSize = mono.FonSize;
        }

        void IArchiveAfterSaveProcessModule.OnAfterSaveArchive(ArchiveContext archiveContext)
        {
            // 结束保存存档
            modContext.LogError("After Save Archive");
        }

        protected override void OnStop()
        {
            // 模块停止
            modContext.LogError("Module Stop");
            if ((bool)mono)
            {
                UnityEngine.Object.Destroy(mono.gameObject);
                mono = null;
            }
        }

        protected override void OnUnload()
        {
            // 模块卸载
            modContext.LogError("Module Unload");
        }

        protected override void OnDispose()
        {
            // 模块释放
            modContext.LogError("Module Dispose");
        }
        void OnMinuteUpdate()
        {

        }

        void OpenFPS()
        {
            var gameSettingManager = Singleton<GameSettingManager>.Instance;
            if (gameSettingManager != null)
            {
                // 获取OtherSettings中的ShowFPS设置
                var showFPS = gameSettingManager.GameSettings.OtherSettings.ShowFPS;

                // 切换FPS显示状态
                showFPS.SetModifyValue(true);

            }

        }
        void ColseFPS()
        {
            var gameSettingManager = Singleton<GameSettingManager>.Instance;
            if (gameSettingManager != null)
            {
                // 获取OtherSettings中的ShowFPS设置
                var showFPS = gameSettingManager.GameSettings.OtherSettings.ShowFPS;

                // 切换FPS显示状态
                showFPS.SetModifyValue(false);
            }
        }

        // 数据唯一标识
        // Data unique identification
        [ArchiveDataIdentity("Mod.TestModule")]
        // 可序列化标识
        // Serializable identifier
        [Serializable]
        class SaveData : ArchiveData
        {
            public int language;
            public int FontSize;
            public float H_width;
            public float H_height;
        }

    }
}
