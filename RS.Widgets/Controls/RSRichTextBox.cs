using RS.Widgets.Interfaces;
using RS.Widgets.Models;
using RS.Win32API;
using RS.Win32API.Structs;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Threading;

namespace RS.Widgets.Controls
{

    public class RSRichTextBox : RichTextBox
    {

        public RSRichTextBox()
        {
            this.Loaded += RSRichTextBox_Loaded;
        }

        private void RSRichTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            this.Document.PagePadding = PagePadding;
        }



        public Thickness PagePadding
        {
            get { return (Thickness)GetValue(PagePaddingProperty); }
            set { SetValue(PagePaddingProperty, value); }
        }

        public static readonly DependencyProperty PagePaddingProperty =
            DependencyProperty.Register("PagePadding", typeof(Thickness), typeof(RSRichTextBox), new PropertyMetadata(new Thickness(2, 0, 2, 0)));



        public bool IsRichTextBoxEmpty(RichTextBox richTextBox)
        {
            // get a document contained in a RichTextBox
            FlowDocument document = richTextBox.Document;

            TextPointer normalizedStart = document.ContentStart.GetInsertionPosition(LogicalDirection.Forward);
            TextPointer normalizedEnd = document.ContentEnd.GetInsertionPosition(LogicalDirection.Backward);

            // The character content is empty if normalized start and end pointers are at the same position
            bool isEmpty = normalizedStart.CompareTo(normalizedEnd) == 0;

            return isEmpty;
        }

        public bool IsPositionContainedBetween(TextPointer test, TextPointer start, TextPointer end)
        {
            if (!test.IsInSameDocument(start) || !test.IsInSameDocument(end))
            {
                return false;
            }
            return start.CompareTo(test) <= 0 && test.CompareTo(end) <= 0;
        }


        public int GetElementTagBalance(TextPointer start, TextPointer end)
        {
            int balanse = 0;

            while (start != null && start.CompareTo(end) < 0)
            {
                TextPointerContext forwardContext = start.GetPointerContext(LogicalDirection.Forward);

                if (forwardContext == TextPointerContext.ElementStart)
                {
                    balanse++;
                }
                else if (forwardContext == TextPointerContext.ElementEnd)
                {
                    balanse--;
                }
                start = start.GetNextContextPosition(LogicalDirection.Forward);
            }

            return balanse;
        }

        public int GetParagraphCount(TextPointer start, TextPointer end)
        {
            int paragraphCount = 0;
            while (start != null && start.CompareTo(end) < 0)
            {
                Paragraph paragraph = start.Paragraph;

                if (paragraph != null)
                {
                    paragraphCount++;

                    // Advance start to an end of the paragraph found
                    start = paragraph.ContentEnd;
                }

                // Use GetNextInsertionPosition method to skip a sequence
                // of structural tags
                start = start.GetNextInsertionPosition(LogicalDirection.Forward);
            }

            return paragraphCount;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

    }
}
