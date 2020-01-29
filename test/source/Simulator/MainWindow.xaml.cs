using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using NLua;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using SakuraGaming.Support.Lang.Tangent.Model;

namespace SakuraGaming.Support.Lang.Tangent.Example
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private TangentScript script;
        private Lua lua;
        private TangentEventArg current_arg;


        public MainWindow()
        {
            InitializeComponent();
            Define();
        }

        private void Define()
        {
            try
            {
                var keywords = new TangentKeywords(
                    ("character", obj =>
                    {
                        var imgid = obj.ArgPairs["id"];
                        var imgpath = obj.ArgPairs["path"];
                        BitmapImage bitmap;

                        if (imgpath != "nil")
                            bitmap = new BitmapImage(ToImageUri(imgpath));
                        else
                            bitmap = null;

                        if (imgid == "0")
                            IllustraionLeft.Source = bitmap;
                        else
                            IllustraionRight.Source = bitmap;

                        current_arg = null;
                        obj.Finish();
                    }),
                    ("background", obj =>
                    {
                        ImgBackground.Source = new BitmapImage(ToImageUri(obj.Value));
                        current_arg = null;
                        obj.Finish();
                    }),
                    ("conv", obj =>
                    {
                        LblName.Content = obj.ArgPairs["name"];
                        LblContent.Content = obj.ArgPairs["content"];
                        current_arg = obj;
                    }),
                    ("wait", obj =>
                    {
                        current_arg = obj;
                    }),
                    ("branches", obj =>
                    {
                        BtnContinue.IsEnabled = false;
                        current_arg = obj;

                        var argname = obj.ArgPairs["arg"];

                        foreach (var arg in obj.ArgPairs)
                        {
                            if (arg.Key == "arg") continue;

                            var button = new Button
                            {
                                Background =
                                    new ImageBrush(
                                        new BitmapImage(new Uri("pack://application:,,,/UI/longButton.png"))),
                                Width = 500,
                                Height = 40,
                                Foreground = BtnContinue.Foreground,
                                Content = arg.Value,
                                CommandParameter = arg.Key
                            };


                            button.Click += (sender, args) =>
                            {
                                if (sender is Button b)
                                {
                                    lua[argname] = arg.Key;
                                }

                                BtnContinue.IsEnabled = true;
                                BranchesPanel.Children.Clear();

                                obj.Finish();
                            };

                            BranchesPanel.Children.Add(button);
                        }

                    }
                ));

                TangentEnv.SetKeywords(keywords);

                lua = new Lua();
                lua.RegisterFunction("LPrint", this, GetType().GetMethod("LuaPrint"));

                TangentEnv.OnEvalCond = s =>
                {
                    try
                    {
                        return (bool)lua.DoString($"return {s}")[0];
                    }
                    catch (System.Exception e)
                    {
                        MessageBox.Show(e.Message, "Lua Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return null;
                    }
                };
                TangentEnv.OnEvalScript = (s, _) =>
                {
                    try
                    {
                        lua.DoString(s);
                    }
                    catch (System.Exception e)
                    {
                        MessageBox.Show(e.Message, "Lua Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                };

            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void LuaPrint(string s)
        {
            MessageBox.Show(s, "Log", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private Uri ToImageUri(string path)
        {
            return new Uri($"file:///{AppDomain.CurrentDomain.BaseDirectory}res/{path}");
        }

        private void BtnLoadScript_OnClick(object sender, RoutedEventArgs e)
        {
            var diag = new OpenFileDialog();
            diag.Filter = "Tangent Script|*.tt";

            if (diag.ShowDialog() == true)
            {
                try
                {
                    IllustraionRight.Source =
                        IllustraionLeft.Source =
                        ImgBackground.Source = null;

                    LblName.Content = LblContent.Content = string.Empty;

                    script = TangentEnv.LoadFile(diag.FileName);
                }
                catch (System.Exception exception)
                {
                    MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnContinue_OnClick(object sender, RoutedEventArgs e)
        {
            current_arg?.Finish();
        }
    }
}
