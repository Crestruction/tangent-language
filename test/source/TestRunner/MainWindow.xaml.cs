using System.Windows;
using System.Windows.Media;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using NLua;

namespace SakuraGaming.Support.Lang.Tangent.Test
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private TangentScript script;
        private Lua lua;

        public MainWindow()
        {

            InitializeComponent();

            Box.AppendText("————————————Tangent————————————", Brushes.Black);
            Box.AppendText("————————Start Simulation———————", Brushes.Black);

            Define();

            Box.AppendText("———————————————————————————————", Brushes.Black);
        }

        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            var diag = new OpenFileDialog();
            diag.Filter = "Tangent Script|*.tt";

            if (diag.ShowDialog() == true)
            {
                Box.AppendText("Script Loading...", Brushes.Black);

                try
                {
                    Box.AppendText($"Path:{diag.FileName}", Brushes.Black);
                    Box.AppendText("Starting...", Brushes.Black);

                    script = TangentEnv.LoadFile(diag.FileName);

                    //syntaxTree.Step(1);
                }
                catch (System.Exception exception)
                {
                    Box.AppendText($"Error: {exception.Message}", Brushes.Red);
                    Box.AppendText("Aborting...", Brushes.Red);
                }
            }
        }

        private void Define()
        {
            var keywords = new TangentKeywords(
                ("log", obj =>
                {
                    Box.AppendText($"Log: {obj.Value}", Brushes.Purple);
                    obj.Finish();
                }),
                ("conv", obj =>
                {
                    Box.AppendText($"Disp Conversation: {obj.Value}", Brushes.Blue);
                    obj.Finish();
                }),
                ("action", obj =>
                {
                    Box.AppendText($"Run Action: {obj.Value}", Brushes.Green);
                    obj.Finish();
                }),
                ("avatar", obj =>
                {
                    Box.AppendText($"Change Avatar:{obj.Value}", Brushes.Orange);
                    obj.Finish();
                }));

            TangentEnv.SetKeywords(keywords);

            lua = new Lua();
            lua.RegisterFunction("LPrint", this, GetType().GetMethod("LuaPrint"));

            TangentEnv.OnEvalCond = s => (bool)lua.DoString($"return {s}")[0];
            TangentEnv.OnEvalScript = (s, _) => lua.DoString(s);

        }

        public void LuaPrint(string text)
        {
            Box.AppendText($"Lua Print: {text}", Brushes.Brown);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (script.Step())
                {
                    Box.AppendText("Script End...", Brushes.Black);
                }
            }
            catch (System.Exception exception)
            {
                Box.AppendText($"{exception.Message}", Brushes.Red);
                Box.AppendText("Aborting...", Brushes.Red);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //script.Dispose();
            script = null;
        }
    }
}
