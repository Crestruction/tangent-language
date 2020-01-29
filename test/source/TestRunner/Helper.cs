using System;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace SakuraGaming.Support.Lang.Tangent.Test
{
    public static class Helper
    {
        public static bool AppendText(this RichTextBox box, string text, Brush color)
        {
            var bc = new BrushConverter();
            var tr = new TextRange(box.Document.ContentEnd, box.Document.ContentEnd);

            tr.Text = $"\r\n{text}";

            try
            {
                tr.ApplyPropertyValue(TextElement.ForegroundProperty,
                    color ?? throw new InvalidOperationException());

                box.ScrollToEnd();

            }
            catch (System.Exception)
            {
                //ignore
                return false;
            }

            return true;
        }
    }
}
