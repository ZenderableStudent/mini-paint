using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace miniPaint
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        bool czyRysuje = false;
        Point punktStartowy;
        SolidColorBrush pisak = new SolidColorBrush(Windows.UI.Colors.Black);
        private Line poprzedniaKreska;
        private Stack<Shape> stosUndo = new Stack<Shape>();
        private Stack<int> stosPunkty = new Stack<int>();
        private int punktyRysowania = 0;
        public MainPage()
        {
            this.InitializeComponent();
            ApplicationView.PreferredLaunchViewSize = new Size(1200, 900);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
        }

        SolidColorBrush pędzel;
        private void poleRysowania_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (czyRysuje)
            {
            Point punktAktualny = e.GetCurrentPoint(poleRysowania).Position;
            Line kreska = new Line()
                {
                    Stroke = pisak,
                    StrokeThickness = SldGrubosc.IntermediateValue,
                    X2 = punktAktualny.X,
                    Y2 = punktAktualny.Y,
                    X1 = punktStartowy.X,
                    Y1 = punktStartowy.Y,
                    StrokeStartLineCap = PenLineCap.Round,
                    StrokeEndLineCap = PenLineCap.Round
                };
            poleRysowania.Children.Add(kreska);

            if (rdbDowolna.IsChecked == true)
            {
                punktStartowy = punktAktualny;
                punktyRysowania++;
                stosUndo.Push(kreska);
            }
            else
            {
                if (poprzedniaKreska != null)
                    poleRysowania.Children.Remove(poprzedniaKreska);
                poprzedniaKreska = kreska;
            }
            }
        }

        private void poleRysowania_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            czyRysuje = true;
            punktStartowy = e.GetCurrentPoint(poleRysowania).Position;
        }

        private void poleRysowania_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            czyRysuje = false;
            if (rdbProsta.IsChecked == true)
            {
                stosPunkty.Push(0);
                stosUndo.Push(poprzedniaKreska);
            }
            else
                stosPunkty.Push(punktyRysowania);

            punktyRysowania = 0;
            poprzedniaKreska = null;

        }

        private void kolorCzerwony(object sender, PointerRoutedEventArgs e)
        {
            pisak = new SolidColorBrush(Windows.UI.Colors.Red);
        }
        private void kolorZielony(object sender, PointerRoutedEventArgs e)
        {
            pisak = new SolidColorBrush(Windows.UI.Colors.Green);
        }
        private void kolorNiebieski(object sender, PointerRoutedEventArgs e)
        {
            pisak = new SolidColorBrush(Windows.UI.Colors.Blue);
        }

        private void BtnUndo_Click(object sender, RoutedEventArgs e)
        {
            if (stosUndo.Count > 0)
            {
                var drawPoints = stosPunkty.Pop();
                if (drawPoints == 0)
                {
                    var undo = stosUndo.Pop();
                    poleRysowania.Children.Remove(undo);
                }
                else
                {
                    for (var i = 0; i < drawPoints; i++)
                    {
                        var undo = stosUndo.Pop();
                        poleRysowania.Children.Remove(undo);
                    }
                }
            }
        }
        private async void BtnOff_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new MessageDialog("Czy na pewno chcesz wyjść?");
            var yes = new UICommand("Tak", c => Application.Current.Exit());
            var no = new UICommand("Nie");

            dialog.Commands.Add(yes);
            dialog.Commands.Add(no);
            dialog.DefaultCommandIndex = 0;

            await dialog.ShowAsync();
        }
    }
}
