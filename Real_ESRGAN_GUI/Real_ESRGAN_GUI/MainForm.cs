using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Drawing;
using System.Reflection;
using System.Xml.Linq;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Xml;

namespace Real_ESRGAN_GUI
{
    public partial class MainForm : Form
    {
        private Dictionary<string, Dictionary<string, string>> languageTexts;

        public static class Parameters
        {
            public readonly static string workPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            public readonly static string extractPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            public readonly static string xmlPath = Path.Combine(workPath, "Real_ESRGAN_GUI.xml");
            public readonly static string appConfigPath = Path.Combine(workPath, "Real_ESRGAN_GUI.exe.config");
            public readonly static string realesrganFolderPath = Path.Combine(extractPath, "Real_ESRGAN_GUI_WinForm");
            public readonly static string realesrganPath = Path.Combine(realesrganFolderPath, "realesrgan.exe");
            public readonly static string vcomp140Path = Path.Combine(realesrganFolderPath, "vcomp140.dll");
            public readonly static string vcomp140dPath = Path.Combine(realesrganFolderPath, "vcomp140d.dll");
            public readonly static string modelsPath = Path.Combine(realesrganFolderPath, "models");
            public readonly static string realesr_animevideov3_x2_binPath = Path.Combine(modelsPath, "realesr-animevideov3-x2.bin");
            public readonly static string realesr_animevideov3_x2_paramPath = Path.Combine(modelsPath, "realesr-animevideov3-x2.param");
            public readonly static string realesr_animevideov3_x3_binPath = Path.Combine(modelsPath, "realesr-animevideov3-x3.bin");
            public readonly static string realesr_animevideov3_x3_paramPath = Path.Combine(modelsPath, "realesr-animevideov3-x3.param");
            public readonly static string realesr_animevideov3_x4_binPath = Path.Combine(modelsPath, "realesr-animevideov3-x4.bin");
            public readonly static string realesr_animevideov3_x4_paramPath = Path.Combine(modelsPath, "realesr-animevideov3-x4.param");
            public readonly static string realesrgan_x4plus_binPath = Path.Combine(modelsPath, "realesrgan-x4plus.bin");
            public readonly static string realesrgan_x4plus_paramPath = Path.Combine(modelsPath, "realesrgan-x4plus.param");
            public readonly static string realesrgan_x4plus_anime_binPath = Path.Combine(modelsPath, "realesrgan-x4plus-anime.bin");
            public readonly static string realesrgan_x4plus_anime_paramPath = Path.Combine(modelsPath, "realesrgan-x4plus-anime.param");
            public static int oldScreenWidth { get; set; }
            public static int oldScreenHeight { get; set; }
            public static float oldSystemScale { get; set; }
            public static bool hasPermission { get; set; }
            public static bool isMultipleFiles { get; set; }
            public static bool isCreatedNewFolder { get; set; }
            public static string currentLanguage { get; set; }
            public static float systemScale { get; set; }
            public static string scale { get; set; }
            public static string model { get; set; }
            public static string extension { get; set; }
            public static bool processHidden { get; set; }
            public static string filePath { get; set; }
            public static string directoryPath { get; set; }
            public static string fileName { get; set; }
        }

        protected override void OnDpiChanged(DpiChangedEventArgs e)
        {
            base.OnDpiChanged(e);

            float newScale = e.DeviceDpiNew / 96.0f;
            Parameters.systemScale = newScale;

            INITIALIZE_MAINFORM_SIZE(newScale);
            UPDATE_MENUSTRIP_LAYOUT(MenuStrip, newScale);

            // 通知系统重绘，更新字体和控件尺寸
            Invalidate();
            Update();
        }

        public MainForm(string[] args)
        {
            InitializeComponent();

            Parameters.hasPermission = CHECK_PATH_READ_WRITE(Parameters.workPath, out Exception ex);
            Parameters.isCreatedNewFolder = true;

            if (!Parameters.hasPermission)
            {
                ERROR_NO_PREMISSION(Parameters.workPath, ex);
                Close();
                return;
            }

            CHECK_XML_LEGAL(Parameters.xmlPath);

            Parameters.currentLanguage = GET_CURRENT_LANGUAGE(Parameters.xmlPath);
            Parameters.systemScale = GET_DPI();
            Parameters.oldScreenWidth = GET_SCREEN_WIDTH(Parameters.xmlPath);
            Parameters.oldScreenHeight = GET_SCREEN_HEIGHT(Parameters.xmlPath);
            Parameters.oldSystemScale = GET_SYSTEM_SCALE(Parameters.xmlPath);

            InitializeLanguageTexts();
            UpdateLanguage();

            Parameters.scale = GET_SCALE(Parameters.xmlPath);
            Parameters.model = GET_MODEL(Parameters.xmlPath);
            Parameters.extension = GET_EXTENSION(Parameters.xmlPath);
            Parameters.processHidden = GET_PROCESS_HIDDEN(Parameters.xmlPath);

            DEFAULT_MODEL_MENU();
            DEFAULT_SCALE_MENU();
            DEFAULT_EXTENSION_MENU();
            DEFAULT_PROCESS_HIDDEN();

            if (!CHECK_REAL_ESRGAN_EXIST())
            {
                CREATE_COMPONENTS();
                if (!Parameters.isCreatedNewFolder)
                {
                    Close();
                    return;
                }
            }

            if (args != null && args.Length > 0)
            {
                MAIN_TASK(args);
                Close();
            }
        }

        private void MAIN_TASK(string[] args)
        {
            if (args.Length > 0)
            {
                Parameters.isMultipleFiles = args.Length > 1;

                if (args.All(arg => !string.IsNullOrEmpty(arg)))
                {
                    if (!Parameters.isMultipleFiles)
                    {
                        string filePath = args[0];
                        string fileName = Path.GetFileNameWithoutExtension(filePath);
                        string directoryPath = Path.GetDirectoryName(filePath);
                        Parameters.filePath = filePath;
                        Parameters.fileName = fileName;
                        Parameters.directoryPath = directoryPath;

                        if (!CHECK_REAL_ESRGAN_EXIST())
                        {
                            CREATE_COMPONENTS();
                            if (!Parameters.isCreatedNewFolder)
                            {
                                ERROR_REAL_ESRGAN_EXIST();
                                return;
                            }
                        }

                        if (CHECK_EXTENSION(filePath))
                        {
                            GENERATE_COMMAND();
                        }
                    }
                    else
                    {
                        foreach (var singleFilePath in args)
                        {
                            string filePath = singleFilePath;
                            string fileName = Path.GetFileNameWithoutExtension(filePath);
                            string directoryPath = Path.GetDirectoryName(filePath);
                            Parameters.filePath = filePath;
                            Parameters.fileName = fileName;
                            Parameters.directoryPath = directoryPath;

                            if (!CHECK_REAL_ESRGAN_EXIST())
                            {
                                CREATE_COMPONENTS();
                                if (!Parameters.isCreatedNewFolder)
                                {
                                    ERROR_REAL_ESRGAN_EXIST();
                                    break;
                                }
                            }

                            if (CHECK_EXTENSION(singleFilePath))
                            {
                                GENERATE_COMMAND();
                            }
                        }
                    }
                }
            }
        }

        private bool CHECK_PATH_READ_WRITE(string path, out Exception error)
        {
            error = null;
            string checkFilePath = Path.Combine(path, "~testFile_" + Guid.NewGuid().ToString("N") + ".tmp");
            try
            {
                using (var fs = new FileStream(checkFilePath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    fs.WriteByte(0);
                }
                using (var fs = new FileStream(checkFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    fs.ReadByte();
                }
                return true;
            }
            catch (UnauthorizedAccessException ex) { error = ex; return false; }
            catch (Exception ex) { error = ex; return false; }
            finally
            {
                try { if (File.Exists(checkFilePath)) File.Delete(checkFilePath); }
                catch { /* 清理临时文件失败可忽略 */ }
            }
        }

        private void CHECK_XML_LEGAL(string configFilePath)
        {
            FileInfo configFile = new FileInfo(configFilePath);

            if (configFile.Exists && (configFile.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
            {
                configFile.Attributes = FileAttributes.Normal;
            }

            if (configFile.Exists && (configFile.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                configFile.Attributes = FileAttributes.Normal;
            }

            if (!configFile.Exists)
            {
                CREATE_DEFAULT_CONFIG(configFilePath);
            }
        }

        private void CHECK_FOLDER_LEGAL(string directoryPath)
        {
            DirectoryInfo directory = new DirectoryInfo(directoryPath);

            if (directory.Exists && (directory.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
            {
                directory.Attributes = FileAttributes.Normal;
            }

            if (directory.Exists && (directory.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                directory.Attributes = FileAttributes.Normal;
            }
        }

        private bool CHECK_REAL_ESRGAN_EXIST()
        {
            return File.Exists(Parameters.appConfigPath) && File.Exists(Parameters.realesrganPath) && File.Exists(Parameters.vcomp140Path) && File.Exists(Parameters.vcomp140dPath)
                && Directory.Exists(Parameters.modelsPath) && File.Exists(Parameters.realesr_animevideov3_x2_binPath)
                && File.Exists(Parameters.realesr_animevideov3_x2_paramPath) && File.Exists(Parameters.realesr_animevideov3_x3_binPath)
                && File.Exists(Parameters.realesr_animevideov3_x3_paramPath) && File.Exists(Parameters.realesr_animevideov3_x4_binPath)
                && File.Exists(Parameters.realesr_animevideov3_x4_paramPath) && File.Exists(Parameters.realesrgan_x4plus_binPath)
                && File.Exists(Parameters.realesrgan_x4plus_paramPath) && File.Exists(Parameters.realesrgan_x4plus_anime_binPath)
                && File.Exists(Parameters.realesrgan_x4plus_anime_paramPath);
        }

        private void EXTRACT_RESOURCE(string resourceName, string outputPath)
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    ERROR_RESOURCE_EXIST(resourceName);
                    return;
                }

                using (FileStream fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                {
                    stream.CopyTo(fileStream);
                }
            }
        }

        private bool CREATE_NEW_FOLDER(string newFolderPath)
        {
            if (!Directory.Exists(newFolderPath))
            {
                try
                {
                    Directory.CreateDirectory(newFolderPath);
                    return true;
                }
                catch (Exception ex)
                {
                    ERROR_CREATE_FOLDER_FAILED(ex);
                    return false;
                }
            }
            return true;
        }

        private void CREATE_COMPONENTS()
        {
            if (!File.Exists(Parameters.appConfigPath))
            {
                CREATE_APP_CONFIG();
            }

            if (!Directory.Exists(Parameters.realesrganFolderPath))
            {
                Parameters.isCreatedNewFolder = CREATE_NEW_FOLDER(Parameters.realesrganFolderPath);
            }

            if (Directory.Exists(Parameters.realesrganFolderPath))
            {
                if (!File.Exists(Parameters.realesrganPath))
                {
                    CREATE_REAL_ESRGAN_EXE();
                }

                if (!File.Exists(Parameters.vcomp140Path))
                {
                    CREATE_VCOMP_140_DLL();
                }

                if (!File.Exists(Parameters.vcomp140dPath))
                {
                    CREATE_VCOMP_140D_DLL();
                }

                if (!Directory.Exists(Parameters.modelsPath))
                {
                    Parameters.isCreatedNewFolder = CREATE_NEW_FOLDER(Parameters.modelsPath);
                }

                if (Directory.Exists(Parameters.modelsPath))
                {
                    CHECK_FOLDER_LEGAL(Parameters.modelsPath);

                    if (!File.Exists(Parameters.realesr_animevideov3_x2_binPath))
                    {
                        CREATE_REAL_ESRGAN_ANIMEVIDEO_V3_X2_BIN();
                    }

                    if (!File.Exists(Parameters.realesr_animevideov3_x2_paramPath))
                    {
                        CREATE_REAL_ESRGAN_ANIMEVIDEO_V3_X2_PARAM();
                    }

                    if (!File.Exists(Parameters.realesr_animevideov3_x3_binPath))
                    {
                        CREATE_REAL_ESRGAN_ANIMEVIDEO_V3_X3_BIN();
                    }

                    if (!File.Exists(Parameters.realesr_animevideov3_x3_paramPath))
                    {
                        CREATE_REAL_ESRGAN_ANIMEVIDEO_V3_X3_PARAM();
                    }

                    if (!File.Exists(Parameters.realesr_animevideov3_x4_binPath))
                    {
                        CREATE_REAL_ESRGAN_ANIMEVIDEO_V3_X4_BIN();
                    }

                    if (!File.Exists(Parameters.realesr_animevideov3_x4_paramPath))
                    {
                        CREATE_REAL_ESRGAN_ANIMEVIDEO_V3_X4_PARAM();
                    }

                    if (!File.Exists(Parameters.realesrgan_x4plus_binPath))
                    {
                        CREATE_REAL_ESRGAN_X4PLUS_BIN();
                    }

                    if (!File.Exists(Parameters.realesrgan_x4plus_paramPath))
                    {
                        CREATE_REAL_ESRGAN_X4PLUS_PARAM();
                    }

                    if (!File.Exists(Parameters.realesrgan_x4plus_anime_binPath))
                    {
                        CREATE_REAL_ESRGAN_X4PLUS_ANIME_BIN();
                    }

                    if (!File.Exists(Parameters.realesrgan_x4plus_anime_paramPath))
                    {
                        CREATE_REAL_ESRGAN_X4PLUS_ANIME_PARAM();
                    }
                }
            }
        }

        private void CREATE_APP_CONFIG()
        {
            string resourceName = "Real_ESRGAN_GUI.Resource.Real_ESRGAN_GUI.exe.config";
            string outputFileName = resourceName.Replace("Real_ESRGAN_GUI.Resource.", "");
            string outputPath = Path.Combine(Parameters.workPath, outputFileName);
            EXTRACT_RESOURCE(resourceName, outputPath);
        }

        private void CREATE_REAL_ESRGAN_EXE()
        {
            string resourceName = "Real_ESRGAN_GUI.Resource.realesrgan.exe";
            string outputFileName = resourceName.Replace("Real_ESRGAN_GUI.Resource.", "");
            string outputPath = Path.Combine(Parameters.realesrganFolderPath, outputFileName);
            EXTRACT_RESOURCE(resourceName, outputPath);
        }

        private void CREATE_VCOMP_140_DLL()
        {
            string resourceName = "Real_ESRGAN_GUI.Resource.vcomp140.dll";
            string outputFileName = resourceName.Replace("Real_ESRGAN_GUI.Resource.", "");
            string outputPath = Path.Combine(Parameters.realesrganFolderPath, outputFileName);
            EXTRACT_RESOURCE(resourceName, outputPath);
        }

        private void CREATE_VCOMP_140D_DLL()
        {
            string resourceName = "Real_ESRGAN_GUI.Resource.vcomp140d.dll";
            string outputFileName = resourceName.Replace("Real_ESRGAN_GUI.Resource.", "");
            string outputPath = Path.Combine(Parameters.realesrganFolderPath, outputFileName);
            EXTRACT_RESOURCE(resourceName, outputPath);
        }

        private void CREATE_REAL_ESRGAN_ANIMEVIDEO_V3_X2_BIN()
        {
            string resourceName = "Real_ESRGAN_GUI.Resource.models.realesr-animevideov3-x2.bin";
            string outputFileName = resourceName.Replace("Real_ESRGAN_GUI.Resource.models.", "");
            string outputPath = Path.Combine(Parameters.modelsPath, outputFileName);
            EXTRACT_RESOURCE(resourceName, outputPath);
        }

        private void CREATE_REAL_ESRGAN_ANIMEVIDEO_V3_X2_PARAM()
        {
            string resourceName = "Real_ESRGAN_GUI.Resource.models.realesr-animevideov3-x2.param";
            string outputFileName = resourceName.Replace("Real_ESRGAN_GUI.Resource.models.", "");
            string outputPath = Path.Combine(Parameters.modelsPath, outputFileName);
            EXTRACT_RESOURCE(resourceName, outputPath);
        }

        private void CREATE_REAL_ESRGAN_ANIMEVIDEO_V3_X3_BIN()
        {
            string resourceName = "Real_ESRGAN_GUI.Resource.models.realesr-animevideov3-x3.bin";
            string outputFileName = resourceName.Replace("Real_ESRGAN_GUI.Resource.models.", "");
            string outputPath = Path.Combine(Parameters.modelsPath, outputFileName);
            EXTRACT_RESOURCE(resourceName, outputPath);
        }

        private void CREATE_REAL_ESRGAN_ANIMEVIDEO_V3_X3_PARAM()
        {
            string resourceName = "Real_ESRGAN_GUI.Resource.models.realesr-animevideov3-x3.param";
            string outputFileName = resourceName.Replace("Real_ESRGAN_GUI.Resource.models.", "");
            string outputPath = Path.Combine(Parameters.modelsPath, outputFileName);
            EXTRACT_RESOURCE(resourceName, outputPath);
        }

        private void CREATE_REAL_ESRGAN_ANIMEVIDEO_V3_X4_BIN()
        {
            string resourceName = "Real_ESRGAN_GUI.Resource.models.realesr-animevideov3-x4.bin";
            string outputFileName = resourceName.Replace("Real_ESRGAN_GUI.Resource.models.", "");
            string outputPath = Path.Combine(Parameters.modelsPath, outputFileName);
            EXTRACT_RESOURCE(resourceName, outputPath);
        }

        private void CREATE_REAL_ESRGAN_ANIMEVIDEO_V3_X4_PARAM()
        {
            string resourceName = "Real_ESRGAN_GUI.Resource.models.realesr-animevideov3-x4.param";
            string outputFileName = resourceName.Replace("Real_ESRGAN_GUI.Resource.models.", "");
            string outputPath = Path.Combine(Parameters.modelsPath, outputFileName);
            EXTRACT_RESOURCE(resourceName, outputPath);
        }

        private void CREATE_REAL_ESRGAN_X4PLUS_BIN()
        {
            string resourceName = "Real_ESRGAN_GUI.Resource.models.realesrgan-x4plus.bin";
            string outputFileName = resourceName.Replace("Real_ESRGAN_GUI.Resource.models.", "");
            string outputPath = Path.Combine(Parameters.modelsPath, outputFileName);
            EXTRACT_RESOURCE(resourceName, outputPath);
        }

        private void CREATE_REAL_ESRGAN_X4PLUS_PARAM()
        {
            string resourceName = "Real_ESRGAN_GUI.Resource.models.realesrgan-x4plus.param";
            string outputFileName = resourceName.Replace("Real_ESRGAN_GUI.Resource.models.", "");
            string outputPath = Path.Combine(Parameters.modelsPath, outputFileName);
            EXTRACT_RESOURCE(resourceName, outputPath);
        }

        private void CREATE_REAL_ESRGAN_X4PLUS_ANIME_BIN()
        {
            string resourceName = "Real_ESRGAN_GUI.Resource.models.realesrgan-x4plus-anime.bin";
            string outputFileName = resourceName.Replace("Real_ESRGAN_GUI.Resource.models.", "");
            string outputPath = Path.Combine(Parameters.modelsPath, outputFileName);
            EXTRACT_RESOURCE(resourceName, outputPath);
        }

        private void CREATE_REAL_ESRGAN_X4PLUS_ANIME_PARAM()
        {
            string resourceName = "Real_ESRGAN_GUI.Resource.models.realesrgan-x4plus-anime.param";
            string outputFileName = resourceName.Replace("Real_ESRGAN_GUI.Resource.models.", "");
            string outputPath = Path.Combine(Parameters.modelsPath, outputFileName);
            EXTRACT_RESOURCE(resourceName, outputPath);
        }

        private float GET_DPI()
        {
            float dpi;
            using (Graphics g = CreateGraphics())
            {
                dpi = g.DpiX;
            }
            return dpi / 96.0f;
        }

        private bool CHECK_EXTENSION(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
                if (!allowedExtensions.Contains(Path.GetExtension(filePath).ToLower()))
                {
                    ERROR_UNSUPPORTED_FILE();
                    return false;
                }
                return true;
            }
            return false;
        }

        private void GENERATE_COMMAND()
        {
            string nowTime = DateTime.Now.ToString("HH-mm-ss");

            // 剥离文件名末尾原有的时间戳（如 _12-45-10），防止重复处理时后缀不断累加
            string baseName = Regex.Replace(Parameters.fileName, @"_\d{2}-\d{2}-\d{2}$", "");
            string outputFileName = $"{baseName}_x{Parameters.scale}_{nowTime}.{Parameters.extension}";

            string command = $"-i \"{Parameters.filePath}\" -o \"{Parameters.directoryPath}\\{outputFileName}\" -n {Parameters.model} -s {Parameters.scale}";

            if (CheckBoxHideProcess.Checked)
            {
                EXECUTE_COMMAND_HIDDEN(command);
            }
            else
            {
                EXECUTE_COMMAND_UNHIDDEN(command);
            }
        }

        private void EXECUTE_COMMAND_UNHIDDEN(string command)
        {
            var process = new Process();
            process.StartInfo.FileName = $"\"{Parameters.realesrganPath}\"";
            process.StartInfo.Arguments = command;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = false; // 显示命令行窗口

            process.Start();
        }

        private void EXECUTE_COMMAND_HIDDEN(string command)
        {
            var process = new Process();
            process.StartInfo.FileName = $"\"{Parameters.realesrganPath}\"";
            process.StartInfo.Arguments = command;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true; // 隐藏命令行窗口

            process.Start();
        }

        private void InitializeLanguageTexts()
        {
            languageTexts = new Dictionary<string, Dictionary<string, string>>
            {
                { "zh-CN", new Dictionary<string, string>
                    {
                        { "MainMenuText","文件" },
                        { "MainMenuOpenFiles","打开" },
                        { "MainMenuExit","退出" },
                        { "LanguageMenu","语言" },
                        { "LanguageMenuSelect","选择语言" },
                        { "AboutMenu","关于" },
                        { "LabelScale","放大倍数" },
                        { "LabelModel","放大算法" },
                        { "LabelExtension","生成格式" },
                        { "CheckBoxHideProcess","后台运行" },
                        { "ButtonConfig","保存配置" },
                    }
                },
                { "zh-TW", new Dictionary<string, string>
                    {
                        { "MainMenuText","文件" },
                        { "MainMenuOpenFiles","打開" },
                        { "MainMenuExit","退出" },
                        { "LanguageMenu","語言" },
                        { "LanguageMenuSelect","選擇語言" },
                        { "AboutMenu","關於" },
                        { "LabelScale","放大倍數" },
                        { "LabelModel","放大算法" },
                        { "LabelExtension","生成格式" },
                        { "CheckBoxHideProcess","後臺運行" },
                        { "ButtonConfig","保存配置" },
                    }
                },
                { "en-US", new Dictionary<string, string>
                    {
                        { "MainMenuText","Files" },
                        { "MainMenuOpenFiles","Open" },
                        { "MainMenuExit","Exit" },
                        { "LanguageMenu","Language" },
                        { "LanguageMenuSelect","Select Language" },
                        { "AboutMenu","About" },
                        { "LabelScale","Scale" },
                        { "LabelModel","Model" },
                        { "LabelExtension","Extension" },
                        { "CheckBoxHideProcess","Hide Process" },
                        { "ButtonConfig","Save Config" },
                    }
                }
            };
        }

        private void UpdateLanguage()
        {
            MainMenu.Text = languageTexts[Parameters.currentLanguage]["MainMenuText"];
            MainMenuOpenFiles.Text = languageTexts[Parameters.currentLanguage]["MainMenuOpenFiles"];
            MainMenuExit.Text = languageTexts[Parameters.currentLanguage]["MainMenuExit"];
            LanguageMenu.Text = languageTexts[Parameters.currentLanguage]["LanguageMenu"];
            LanguageMenuSelect.Text = languageTexts[Parameters.currentLanguage]["LanguageMenuSelect"];
            AboutMenu.Text = languageTexts[Parameters.currentLanguage]["AboutMenu"];
            LabelScale.Text = languageTexts[Parameters.currentLanguage]["LabelScale"];
            LabelModel.Text = languageTexts[Parameters.currentLanguage]["LabelModel"];
            LabelExtension.Text = languageTexts[Parameters.currentLanguage]["LabelExtension"];
            CheckBoxHideProcess.Text = languageTexts[Parameters.currentLanguage]["CheckBoxHideProcess"];
            ButtonConfig.Text = languageTexts[Parameters.currentLanguage]["ButtonConfig"];
        }

        private void CREATE_DEFAULT_CONFIG(string configFilePath)
        {
            if (File.Exists(configFilePath))
            {
                try
                {
                    File.WriteAllText(configFilePath, string.Empty);
                }
                catch (Exception ex)
                {
                    ERROR_EXCEPTION_MESSAGE(ex);
                    Close();
                }
            }

            int newLocationX = Screen.FromControl(this).Bounds.Width / 2 - Width / 2;
            int newLocationY = Screen.FromControl(this).Bounds.Height / 2 - Height / 2;

            var currentCulture = CultureInfo.CurrentUICulture;

            var supportedLanguages = new HashSet<string>
            {
                "zh-CN",
                "zh-TW",
                "en-US"
            };

            XElement defaultConfig;

            if (supportedLanguages.Contains(currentCulture.Name))
            {
                if (Parameters.currentLanguage != null)
                {
                    defaultConfig = new XElement("Configuration",
                        new XElement("Language", $"{Parameters.currentLanguage}"),
                        new XElement("LocationX", $"{newLocationX}"),
                        new XElement("LocationY", $"{newLocationY}"),
                        new XElement("Scale", "4"),
                        new XElement("Model", "realesrgan-x4plus"),
                        new XElement("Extension", "png"),
                        new XElement("ProcessHidden", "false")
                    );
                }
                else
                {
                    defaultConfig = new XElement("Configuration",
                        new XElement("Language", $"{currentCulture.Name}"),
                        new XElement("LocationX", $"{newLocationX}"),
                        new XElement("LocationY", $"{newLocationY}"),
                        new XElement("Scale", "4"),
                        new XElement("Model", "realesrgan-x4plus"),
                        new XElement("Extension", "png"),
                        new XElement("ProcessHidden", "false")
                    );
                }
            }
            else
            {
                defaultConfig = new XElement("Configuration",
                    new XElement("Language", "en-US"),
                    new XElement("LocationX", $"{newLocationX}"),
                    new XElement("LocationY", $"{newLocationY}"),
                    new XElement("Scale", "4"),
                    new XElement("Model", "realesrgan-x4plus"),
                    new XElement("Extension", "png"),
                    new XElement("ProcessHidden", "false")
                );
            }

            try
            {
                defaultConfig.Save(configFilePath);
            }
            catch (Exception ex)
            {
                ERROR_EXCEPTION_MESSAGE(ex);
            }
        }

        private void UPDATE_CONFIG(string filePath, string key, string newValue)
        {
            try
            {
                XDocument xdoc = XDocument.Load(filePath);

                var element = xdoc.Descendants(key).FirstOrDefault();
                if (element != null)
                {
                    element.Value = newValue;
                }
                else
                {
                    xdoc.Root.Add(new XElement(key, newValue));
                }

                xdoc.Save(filePath);
            }
            catch (Exception)
            {
                CREATE_DEFAULT_CONFIG(Parameters.xmlPath);
            }
        }

        private string GET_CURRENT_LANGUAGE(string configFilePath)
        {
            if (!File.Exists(configFilePath))
            {
                CREATE_DEFAULT_CONFIG(configFilePath);
            }

            var currentCulture = CultureInfo.CurrentUICulture;

            var supportedLanguages = new HashSet<string>
            {
                "zh-CN",
                "zh-TW",
                "en-US"
            };

            XDocument xdoc;

            try
            {
                xdoc = XDocument.Load(configFilePath);
            }
            catch (XmlException)
            {
                CREATE_DEFAULT_CONFIG(configFilePath);
                return supportedLanguages.Contains(currentCulture.Name) ? currentCulture.Name : "en-US";
            }

            var languageNode = xdoc.Descendants("Language").FirstOrDefault();

            if (languageNode == null)
            {
                if (supportedLanguages.Contains(currentCulture.Name))
                {
                    XElement newNode = new XElement("Language", $"{currentCulture.Name}");
                    xdoc.Root.Add(newNode);
                    xdoc.Save(configFilePath);
                    return currentCulture.Name;
                }
                else
                {
                    XElement newNode = new XElement("Language", "en-US");
                    xdoc.Root.Add(newNode);
                    xdoc.Save(configFilePath);
                    return "en-US";
                }
            }

            var language = languageNode.Value;

            if (string.IsNullOrEmpty(language))
            {
                return supportedLanguages.Contains(currentCulture.Name) ? currentCulture.Name : "en-US";
            }

            if (!supportedLanguages.Contains(language))
            {
                return supportedLanguages.Contains(currentCulture.Name) ? currentCulture.Name : "en-US";
            }

            return language;
        }

        private int GET_SCREEN_WIDTH(string configFilePath)
        {
            if (!File.Exists(configFilePath))
            {
                CREATE_DEFAULT_CONFIG(configFilePath);
            }

            int newWidth = Screen.FromControl(this).Bounds.Width;

            XDocument xdoc;

            try
            {
                xdoc = XDocument.Load(configFilePath);
            }
            catch (XmlException)
            {
                CREATE_DEFAULT_CONFIG(configFilePath);
                return 0;
            }

            var widthNode = xdoc.Descendants("ScreenWidth").FirstOrDefault();

            if (widthNode == null)
            {
                XElement newNode = new XElement("ScreenWidth", newWidth);
                xdoc.Root.Add(newNode);
                xdoc.Save(configFilePath);
                return 0;
            }

            var width = widthNode.Value;
            int widthToInt;

            if (string.IsNullOrEmpty(width)) return 0;
            if (!int.TryParse(width, out widthToInt)) return 0;
            if (widthToInt <= 0) return 0;

            if (widthToInt == Screen.FromControl(this).Bounds.Width)
            {
                return widthToInt;
            }

            return 0;
        }

        private int GET_SCREEN_HEIGHT(string configFilePath)
        {
            if (!File.Exists(configFilePath))
            {
                CREATE_DEFAULT_CONFIG(configFilePath);
            }

            int newHeight = Screen.FromControl(this).Bounds.Height;

            XDocument xdoc;

            try
            {
                xdoc = XDocument.Load(configFilePath);
            }
            catch (XmlException)
            {
                CREATE_DEFAULT_CONFIG(configFilePath);
                return 0;
            }

            var heightNode = xdoc.Descendants("ScreenHeight").FirstOrDefault();

            if (heightNode == null)
            {
                XElement newNode = new XElement("ScreenHeight", newHeight);
                xdoc.Root.Add(newNode);
                xdoc.Save(configFilePath);
                return 0;
            }

            var height = heightNode.Value;
            int heightToInt;

            if (string.IsNullOrEmpty(height)) return 0;
            if (!int.TryParse(height, out heightToInt)) return 0;
            if (heightToInt <= 0) return 0;

            if (heightToInt == Screen.FromControl(this).Bounds.Height)
            {
                return heightToInt;
            }

            return 0;
        }

        private float GET_SYSTEM_SCALE(string configFilePath)
        {
            if (!File.Exists(configFilePath))
            {
                CREATE_DEFAULT_CONFIG(configFilePath);
            }

            XDocument xdoc;

            try
            {
                xdoc = XDocument.Load(configFilePath);
            }
            catch (XmlException)
            {
                CREATE_DEFAULT_CONFIG(configFilePath);
                return 0;
            }

            var scaleNode = xdoc.Descendants("SystemScale").FirstOrDefault();

            if (scaleNode == null)
            {
                XElement newNode = new XElement("SystemScale", Parameters.systemScale);
                xdoc.Root.Add(newNode);
                xdoc.Save(configFilePath);
                return 0;
            }

            var scale = scaleNode.Value;
            float scaleToFloat;

            if (string.IsNullOrEmpty(scale)) return 0;
            if (!float.TryParse(scale, out scaleToFloat)) return 0;
            if (scaleToFloat <= 0) return 0;

            if ((int)scaleToFloat == (int)Parameters.systemScale)
            {
                return scaleToFloat;
            }

            return 0;
        }

        private int GET_LOCATION_X(string configFilePath)
        {
            if (!File.Exists(configFilePath))
            {
                CREATE_DEFAULT_CONFIG(configFilePath);
            }

            int newLocationX = Screen.FromControl(this).Bounds.Width / 2 - Size.Width / 2;

            XDocument xdoc;

            try
            {
                xdoc = XDocument.Load(configFilePath);
            }
            catch (XmlException)
            {
                CREATE_DEFAULT_CONFIG(configFilePath);
                return newLocationX;
            }

            var locationXNode = xdoc.Descendants("LocationX").FirstOrDefault();

            if (locationXNode == null)
            {
                XElement newNode = new XElement("LocationX", newLocationX);
                xdoc.Root.Add(newNode);
                xdoc.Save(configFilePath);
                return newLocationX;
            }

            var locationX = locationXNode.Value;
            int locationXToInt;

            if (string.IsNullOrEmpty(locationX)) return newLocationX;
            if (!int.TryParse(locationX, out locationXToInt)) return newLocationX;

            if (locationXToInt > Screen.FromControl(this).Bounds.Width - Size.Width || locationXToInt < -10)
            {
                return newLocationX;
            }

            return locationXToInt;
        }

        private int GET_LOCATION_Y(string configFilePath)
        {
            if (!File.Exists(configFilePath))
            {
                CREATE_DEFAULT_CONFIG(configFilePath);
            }

            int newLocationY = Screen.FromControl(this).Bounds.Height / 2 - Size.Height / 2;

            XDocument xdoc;

            try
            {
                xdoc = XDocument.Load(configFilePath);
            }
            catch (XmlException)
            {
                CREATE_DEFAULT_CONFIG(configFilePath);
                return newLocationY;
            }

            var locationYNode = xdoc.Descendants("LocationY").FirstOrDefault();

            if (locationYNode == null)
            {
                XElement newNode = new XElement("LocationY", newLocationY);
                xdoc.Root.Add(newNode);
                xdoc.Save(configFilePath);
                return newLocationY;
            }

            var locationY = locationYNode.Value;
            int locationYToInt;

            if (string.IsNullOrEmpty(locationY)) return newLocationY;
            if (!int.TryParse(locationY, out locationYToInt)) return newLocationY;

            if (locationYToInt > Screen.FromControl(this).Bounds.Height - Size.Height || locationYToInt < 0)
            {
                return newLocationY;
            }

            return locationYToInt;
        }

        private string GET_SCALE(string configFilePath)
        {
            if (!File.Exists(configFilePath))
            {
                CREATE_DEFAULT_CONFIG(configFilePath);
            }

            XDocument xdoc;

            try
            {
                xdoc = XDocument.Load(configFilePath);
            }
            catch (XmlException)
            {
                CREATE_DEFAULT_CONFIG(configFilePath);
                return "4";
            }

            var scaleNode = xdoc.Descendants("Scale").FirstOrDefault();

            var supportedScale = new HashSet<string> { "2", "3", "4" };

            if (scaleNode == null)
            {
                XElement newNode = new XElement("Scale", "4");
                xdoc.Root.Add(newNode);
                xdoc.Save(configFilePath);
                return "4";
            }

            var scale = scaleNode.Value;

            if (string.IsNullOrEmpty(scale)) return "4";
            if (!supportedScale.Contains(scale)) return "4";

            return scale;
        }

        private string GET_MODEL(string configFilePath)
        {
            if (!File.Exists(configFilePath))
            {
                CREATE_DEFAULT_CONFIG(configFilePath);
            }

            XDocument xdoc;

            try
            {
                xdoc = XDocument.Load(configFilePath);
            }
            catch (XmlException)
            {
                CREATE_DEFAULT_CONFIG(configFilePath);
                return "realesrgan-x4plus";
            }

            var modelNode = xdoc.Descendants("Model").FirstOrDefault();

            var supportedModel = new HashSet<string>
            {
                "realesrgan-x4plus",
                "realesrgan-x4plus-anime",
                "realesr-animevideov3"
            };

            if (modelNode == null)
            {
                XElement newNode = new XElement("Model", "realesrgan-x4plus");
                xdoc.Root.Add(newNode);
                xdoc.Save(configFilePath);
                return "realesrgan-x4plus";
            }

            var model = modelNode.Value;

            if (string.IsNullOrEmpty(model)) return "realesrgan-x4plus";
            if (!supportedModel.Contains(model)) return "realesrgan-x4plus";

            return model;
        }

        private string GET_EXTENSION(string configFilePath)
        {
            if (!File.Exists(configFilePath))
            {
                CREATE_DEFAULT_CONFIG(configFilePath);
            }

            XDocument xdoc;

            try
            {
                xdoc = XDocument.Load(configFilePath);
            }
            catch (XmlException)
            {
                CREATE_DEFAULT_CONFIG(configFilePath);
                return "png";
            }

            var extensionNode = xdoc.Descendants("Extension").FirstOrDefault();

            var supportedExtension = new HashSet<string> { "jpg", "png", "webp" };

            if (extensionNode == null)
            {
                XElement newNode = new XElement("Extension", "png");
                xdoc.Root.Add(newNode);
                xdoc.Save(configFilePath);
                return "png";
            }

            var extension = extensionNode.Value;

            if (string.IsNullOrEmpty(extension)) return "png";
            if (!supportedExtension.Contains(extension)) return "png";

            return extension;
        }

        private bool GET_PROCESS_HIDDEN(string configFilePath)
        {
            if (!File.Exists(configFilePath))
            {
                CREATE_DEFAULT_CONFIG(configFilePath);
            }

            XDocument xdoc;

            try
            {
                xdoc = XDocument.Load(configFilePath);
            }
            catch (XmlException)
            {
                CREATE_DEFAULT_CONFIG(configFilePath);
                return false;
            }

            var processHiddenNode = xdoc.Descendants("ProcessHidden").FirstOrDefault();

            var supportedProcessHidden = new HashSet<string> { "True", "False" };

            if (processHiddenNode == null)
            {
                XElement newNode = new XElement("ProcessHidden", "false");
                xdoc.Root.Add(newNode);
                xdoc.Save(configFilePath);
                return false;
            }

            var processHidden = processHiddenNode.Value;
            bool processHiddenToBool;

            if (string.IsNullOrEmpty(processHidden)) return false;
            if (!supportedProcessHidden.Contains(processHidden)) return false;
            if (!bool.TryParse(processHidden, out processHiddenToBool)) return false;

            return processHiddenToBool;
        }

        private void DEFAULT_SCALE_MENU()
        {
            ComboBoxScale.Items.Clear();

            bool scaleAssigned = false;

            if (Parameters.model == "realesr-animevideov3")
            {
                ComboBoxScale.Items.Add("2");
                ComboBoxScale.Items.Add("3");
                ComboBoxScale.Items.Add("4");

                if (Parameters.scale == "2")
                {
                    ComboBoxScale.SelectedIndex = 0;
                    scaleAssigned = true;
                }
                if (Parameters.scale == "3")
                {
                    ComboBoxScale.SelectedIndex = 1;
                    scaleAssigned = true;
                }
                if (Parameters.scale == "4")
                {
                    ComboBoxScale.SelectedIndex = 2;
                    scaleAssigned = true;
                }
            }
            else
            {
                ComboBoxScale.Items.Add("4");

                if (Parameters.scale == "4")
                {
                    ComboBoxScale.SelectedIndex = 0;
                    scaleAssigned = true;
                }
            }

            // 如果当前缩放比例与模型不兼容，强制重置为4
            if (!scaleAssigned)
            {
                Parameters.scale = "4";
                ComboBoxScale.SelectedIndex = 0;
            }
        }

        private void DEFAULT_MODEL_MENU()
        {
            ComboBoxModel.Items.Add("realesrgan-x4plus");
            ComboBoxModel.Items.Add("realesrgan-x4plus-anime");
            ComboBoxModel.Items.Add("realesr-animevideov3");

            if (Parameters.model == "realesrgan-x4plus")
            {
                ComboBoxModel.SelectedIndex = 0;
            }
            if (Parameters.model == "realesrgan-x4plus-anime")
            {
                ComboBoxModel.SelectedIndex = 1;
            }
            if (Parameters.model == "realesr-animevideov3")
            {
                ComboBoxModel.SelectedIndex = 2;
            }
        }

        private void DEFAULT_EXTENSION_MENU()
        {
            ComboBoxExtension.Items.Add("jpg");
            ComboBoxExtension.Items.Add("png");
            ComboBoxExtension.Items.Add("webp");

            if (Parameters.extension == "jpg")
            {
                ComboBoxExtension.SelectedIndex = 0;
            }
            if (Parameters.extension == "png")
            {
                ComboBoxExtension.SelectedIndex = 1;
            }
            if (Parameters.extension == "webp")
            {
                ComboBoxExtension.SelectedIndex = 2;
            }
        }

        private void DEFAULT_PROCESS_HIDDEN()
        {
            CheckBoxHideProcess.Checked = Parameters.processHidden;
        }

        private void NOTICE_CONFIG_SAVED()
        {
            switch (Parameters.currentLanguage)
            {
                case "zh-CN":
                    MessageBox.Show("配置已保存。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                case "zh-TW":
                    MessageBox.Show("配置已保存。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                case "en-US":
                    MessageBox.Show("Configuration Saved.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
            }
        }

        private void ERROR_UNSUPPORTED_FILE()
        {
            switch (Parameters.currentLanguage)
            {
                case "zh-CN":
                    MessageBox.Show("不支持的文件格式。请提供 JPG、PNG 或 WEBP 文件。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case "zh-TW":
                    MessageBox.Show("不支持的文件格式。請提供 JPG、PNG 或 WEBP 文件。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case "en-US":
                    MessageBox.Show("Unsupported file format. Please provide JPG, PNG, or WEBP files.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        private void ERROR_NO_PREMISSION(string directoryPath, Exception error)
        {
            var currentCulture = CultureInfo.CurrentUICulture;

            var supportedLanguages = new HashSet<string>
            {
                "zh-CN",
                "zh-TW",
                "en-US"
            };

            if (supportedLanguages.Contains(currentCulture.Name))
            {
                Parameters.currentLanguage = currentCulture.Name;

                switch (Parameters.currentLanguage)
                {
                    case "zh-CN":
                        MessageBox.Show($"应用程序没有在{directoryPath}内运行的权限，错误为: {error.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case "zh-TW":
                        MessageBox.Show($"應用程式沒有在{directoryPath}内運行的權限，錯誤為: {error.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case "en-US":
                        MessageBox.Show($"Permission denied to run the application in {directoryPath},error message is: {error.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            else
            {
                MessageBox.Show($"Permission denied to run the application in {directoryPath},error message is: {error.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ERROR_REAL_ESRGAN_EXIST()
        {
            switch (Parameters.currentLanguage)
            {
                case "zh-CN":
                    MessageBox.Show("程序工作路径下Real ESRGAN组件不完整，无法启动Real ESRGAN处理流程。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case "zh-TW":
                    MessageBox.Show("程式工作路徑下Real ESRGAN組件不完整，無法啓動Real ESRGAN處理流程。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case "en-US":
                    MessageBox.Show("The Real ESRGAN components in the program's working directory are incomplete and can not start the Real ESRGAN processing flow.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        private void ERROR_RESOURCE_EXIST(string resourceName)
        {
            switch (Parameters.currentLanguage)
            {
                case "zh-CN":
                    MessageBox.Show("没有找到资源: " + resourceName, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case "zh-TW":
                    MessageBox.Show("沒有找到資源: " + resourceName, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case "en-US":
                    MessageBox.Show("Resource not found: " + resourceName, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        private void ERROR_CREATE_FOLDER_FAILED(Exception error)
        {
            switch (Parameters.currentLanguage)
            {
                case "zh-CN":
                    MessageBox.Show($"创建目标文件夹时出错，错误为: {error.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case "zh-TW":
                    MessageBox.Show($"創建目標文件夾時出錯，錯誤為: {error.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case "en-US":
                    MessageBox.Show($"The error occurred while creating the target folder,error message is: {error.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        private void ERROR_EXCEPTION_MESSAGE(Exception error)
        {
            switch (Parameters.currentLanguage)
            {
                case "zh-CN":
                    MessageBox.Show($"出现错误: {error.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case "zh-TW":
                    MessageBox.Show($"出現錯誤: {error.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case "en-US":
                    MessageBox.Show($"An error occurred: {error.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        private void SAVE_CONFIG()
        {
            UPDATE_CONFIG($"{Parameters.xmlPath}", "Language", $"{Parameters.currentLanguage}");
            UPDATE_CONFIG($"{Parameters.xmlPath}", "Scale", $"{Parameters.scale}");
            UPDATE_CONFIG($"{Parameters.xmlPath}", "Model", $"{Parameters.model}");
            UPDATE_CONFIG($"{Parameters.xmlPath}", "Extension", $"{Parameters.extension}");
            UPDATE_CONFIG($"{Parameters.xmlPath}", "ProcessHidden", $"{Parameters.processHidden}");
        }

        private void INITIALIZE_MAINFORM_SIZE(float scale)
        {
            AutoScaleMode = AutoScaleMode.Dpi;

            MinimumSize = new Size(0, 0);
            MaximumSize = MinimumSize;
            UPDATE_MIN_MAX_SIZE(scale);

            float baseWidth = 280f;
            float baseHeight = 280f;
            Size = new Size((int)(baseWidth * scale), (int)(baseHeight * scale));

            INITIALIZE_TABLE_LAYOUT_PANEL_PIXEL(scale);
            INITIALIZE_UI_FONT_SIZE();
        }

        private void UPDATE_MIN_MAX_SIZE(float scale)
        {
            int width = (int)(280 * scale);
            int height = (int)(280 * scale);
            MinimumSize = new Size(width, height);
            MaximumSize = MinimumSize;
        }

        private void INITIALIZE_TABLE_LAYOUT_PANEL_PIXEL(float scale)
        {
            // TableLayoutPanel 布局设置，索引均从0开始（先列后行）

            SET_COLUMN_SIZE(tableLayoutPanel, 0, SizeType.Absolute, 10f);
            SET_COLUMN_SIZE(tableLayoutPanel, 1, SizeType.Absolute, 70f * scale);
            SET_COLUMN_SIZE(tableLayoutPanel, 2, SizeType.Percent, 35f);
            SET_COLUMN_SIZE(tableLayoutPanel, 3, SizeType.Percent, 20f);
            SET_COLUMN_SIZE(tableLayoutPanel, 4, SizeType.Percent, 20f);
            SET_COLUMN_SIZE(tableLayoutPanel, 5, SizeType.Percent, 22f);
            SET_COLUMN_SIZE(tableLayoutPanel, 6, SizeType.Percent, 3f);
            SET_COLUMN_SIZE(tableLayoutPanel, 7, SizeType.Absolute, 10f);

            SET_ROW_SIZE(tableLayoutPanel, 0, SizeType.Percent, 16f);
            SET_ROW_SIZE(tableLayoutPanel, 1, SizeType.Absolute, 20f);
            SET_ROW_SIZE(tableLayoutPanel, 2, SizeType.Percent, 22f);
            SET_ROW_SIZE(tableLayoutPanel, 3, SizeType.Absolute, 10f);
            SET_ROW_SIZE(tableLayoutPanel, 4, SizeType.Percent, 22f);
            SET_ROW_SIZE(tableLayoutPanel, 5, SizeType.Absolute, 10f);
            SET_ROW_SIZE(tableLayoutPanel, 6, SizeType.Percent, 22f);
            SET_ROW_SIZE(tableLayoutPanel, 7, SizeType.Absolute, 10f);
            SET_ROW_SIZE(tableLayoutPanel, 8, SizeType.Percent, 18f);
            SET_ROW_SIZE(tableLayoutPanel, 9, SizeType.Absolute, 5f);
        }

        private void SET_COLUMN_SIZE(TableLayoutPanel panel, int num, SizeType type, float fontSize)
        {
            panel.ColumnStyles[num].SizeType = type;
            panel.ColumnStyles[num].Width = fontSize;
        }

        private void SET_ROW_SIZE(TableLayoutPanel panel, int num, SizeType type, float fontSize)
        {
            panel.RowStyles[num].SizeType = type;
            panel.RowStyles[num].Height = fontSize;
        }

        private void INITIALIZE_UI_FONT_SIZE()
        {
            SET_FONT_SIZE(MenuStrip, Font.Size);
            SET_FONT_SIZE(LabelScale, Font.Size);
            SET_FONT_SIZE(ComboBoxScale, Font.Size);
            SET_FONT_SIZE(LabelModel, Font.Size);
            SET_FONT_SIZE(ComboBoxModel, Font.Size);
            SET_FONT_SIZE(LabelExtension, Font.Size);
            SET_FONT_SIZE(ComboBoxExtension, Font.Size);
            SET_FONT_SIZE(CheckBoxHideProcess, Font.Size);
            SET_FONT_SIZE(ButtonConfig, Font.Size);
        }

        private void SET_FONT_SIZE(Control obj, float fontSize)
        {
            obj.Font = new Font(obj.Font.FontFamily, fontSize, obj.Font.Style, GraphicsUnit.Point);
        }

        private void UPDATE_MENUSTRIP_LAYOUT(MenuStrip MenuStrip, float scale)
        {
            if (MenuStrip != null)
            {
                int iconSize = (int)(16 * scale);
                MenuStrip.ImageScalingSize = new Size(iconSize, iconSize);

                foreach (ToolStripItem item in MenuStrip.Items)
                {
                    UPDATE_TOOL_STRIP_ITEM(item);
                }
            }
        }

        private void UPDATE_TOOL_STRIP_ITEM(ToolStripItem item)
        {
            item.Font = new Font(item.Font.FontFamily, Font.Size, item.Font.Style);

            if (item is ToolStripMenuItem menuItem)
            {
                menuItem.ImageScaling = ToolStripItemImageScaling.SizeToFit;

                foreach (ToolStripItem subItem in menuItem.DropDownItems)
                {
                    UPDATE_TOOL_STRIP_ITEM(subItem);
                }
            }
        }

        private void MAINFORM_LOAD(object sender, EventArgs e)
        {
            int locationX;
            int locationY;

            if (Parameters.oldScreenWidth == Screen.FromControl(this).Bounds.Width && Parameters.oldScreenHeight == Screen.FromControl(this).Bounds.Height && Parameters.oldSystemScale == Parameters.systemScale)
            {
                locationX = GET_LOCATION_X(Parameters.xmlPath);
                locationY = GET_LOCATION_Y(Parameters.xmlPath);
            }
            else
            {
                locationX = Screen.FromControl(this).Bounds.Width / 2 - Size.Width / 2;
                locationY = Screen.FromControl(this).Bounds.Height / 2 - Size.Height / 2;
            }

            Location = new Point(locationX, locationY);
            INITIALIZE_MAINFORM_SIZE(Parameters.systemScale);
        }

        private void MAINFORM_DRAGENTER(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                bool isValidFile = true;

                foreach (string file in files)
                {
                    string extension = Path.GetExtension(file).ToLower();
                    if (extension != ".jpg" && extension != ".jpeg" && extension != ".png" && extension != ".webp")
                    {
                        isValidFile = false;
                        break;
                    }
                }

                e.Effect = isValidFile ? DragDropEffects.Copy : DragDropEffects.None;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
            Cursor.Current = Cursors.Default;
        }

        private async void MAINFORM_DRAGDROP(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            await Task.Run(() => MAIN_TASK(files));
        }

        private void MAINFORM_DRAGOVER(object sender, DragEventArgs e)
        {
            MAINFORM_DRAGENTER(sender, e);
        }

        private void BUTTON_CONFIG_CLICK(object sender, EventArgs e)
        {
            NOTICE_CONFIG_SAVED();
            SAVE_CONFIG();
            SAVE_LOCATION();
        }

        private void CHECKBOX_HIDE_PROCESS_CHECKED_CHANGED(object sender, EventArgs e)
        {
            Parameters.processHidden = CheckBoxHideProcess.Checked;
        }

        private void COMBOBOX_SCALE_SELECTED_INDEX_CHANGNED(object sender, EventArgs e)
        {
            Parameters.scale = ComboBoxScale.SelectedItem.ToString();
        }

        private void COMBOBOX_MODEL_SELECTED_INDEX_CHANGED(object sender, EventArgs e)
        {
            Parameters.model = ComboBoxModel.SelectedItem.ToString();
            DEFAULT_SCALE_MENU();
        }

        private void COMBOBOX_EXTENSION_SELECTED_INDEX_CHANGED(object sender, EventArgs e)
        {
            Parameters.extension = ComboBoxExtension.SelectedItem.ToString();
        }

        private async void MAINMENU_OPENFILES_CLICK(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JPG文件 (*.jpg)|*.jpg|JPEG文件 (*.jpeg)|*.jpeg|PNG文件 (*.png)|*.png|WEBP文件 (*.webp)|*.webp";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                Parameters.filePath = filePath;
                Parameters.fileName = Path.GetFileNameWithoutExtension(filePath);
                Parameters.directoryPath = Path.GetDirectoryName(filePath);

                if (!CHECK_REAL_ESRGAN_EXIST())
                {
                    CREATE_COMPONENTS();
                    if (!Parameters.isCreatedNewFolder)
                    {
                        ERROR_REAL_ESRGAN_EXIST();
                        return;
                    }
                }

                await Task.Run(() => GENERATE_COMMAND());
            }
        }

        private void MAINMENU_EXIT_CLICK(object sender, EventArgs e)
        {
            SAVE_LOCATION();
            Close();
        }

        private void LANGUAGE_MENU_SELECT_zhCN_CLICK(object sender, EventArgs e)
        {
            Parameters.currentLanguage = "zh-CN";
            UpdateLanguage();
            UPDATE_CONFIG($"{Parameters.xmlPath}", "Language", $"{Parameters.currentLanguage}");
        }

        private void LANGUAGE_MENU_SELECT_zhTW_CLICK(object sender, EventArgs e)
        {
            Parameters.currentLanguage = "zh-TW";
            UpdateLanguage();
            UPDATE_CONFIG($"{Parameters.xmlPath}", "Language", $"{Parameters.currentLanguage}");
        }

        private void LANGUAGE_MENU_SELECT_enUS_CLICK(object sender, EventArgs e)
        {
            Parameters.currentLanguage = "en-US";
            UpdateLanguage();
            UPDATE_CONFIG($"{Parameters.xmlPath}", "Language", $"{Parameters.currentLanguage}");
        }

        private void ABOUTMENU_ABOUT(object sender, EventArgs e)
        {
            AboutForm aboutForm = new AboutForm();
            aboutForm.ShowDialog();
        }

        private void MAINFORM_FORM_CLOSING(object sender, FormClosingEventArgs e)
        {
            SAVE_LOCATION();
        }

        private void SAVE_LOCATION()
        {
            int locationX = Location.X;
            int locationY = Location.Y;

            int width = Screen.FromControl(this).Bounds.Width;
            int height = Screen.FromControl(this).Bounds.Height;

            UPDATE_CONFIG($"{Parameters.xmlPath}", "ScreenWidth", $"{width}");
            UPDATE_CONFIG($"{Parameters.xmlPath}", "ScreenHeight", $"{height}");
            UPDATE_CONFIG($"{Parameters.xmlPath}", "SystemScale", $"{Parameters.systemScale}");
            UPDATE_CONFIG($"{Parameters.xmlPath}", "LocationX", $"{locationX}");
            UPDATE_CONFIG($"{Parameters.xmlPath}", "LocationY", $"{locationY}");
        }
    }
}