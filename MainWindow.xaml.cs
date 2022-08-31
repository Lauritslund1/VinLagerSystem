using System;
using System.Windows;
using System.Windows.Media;
namespace LagerMonitorDVI
{

    public partial class MainWindow : Window
    {
        //Aktiverer webservice til at hente lagerdata
        public DVIService.monitorSoapClient ds = new DVIService.monitorSoapClient();
        
        //aktiverer Timer til at styre refresh hastigheden
        System.Windows.Threading.DispatcherTimer Timer = new System.Windows.Threading.DispatcherTimer();
        
        //Aktiverer DateTime så den kan bruges senere til at hente Dato og Tid
        static DateTime dt = DateTime.Now;
       
        //Opretter "count" til at tælle hvor mange sekunder der er gået
        public int count = 0;

        public MainWindow()
        {
            InitializeComponent();
            
            //Opretter en Timer der kører en funktion hvert sekund
            Timer.Tick += new EventHandler(Timer_Click);
            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Start();

            //Køre funktionen der indlæser alle data og skriver dem på skærmen
            RefreshAll();

        }

        // En funktion der genopfrisker alle data og skriver de nye data på skærmen
        private void RefreshAll()
        {

                UnderMin();
                OverMax();
                MostSold();
                TempHumi();
                DateAndTime();

        }

        
        //Indlæser de vin der er under minimunsgrænsen
        private void UnderMin()
        {
            Min.Text = "";
            Min.Text = "Varer under minimum:";

            Min.Foreground = Brushes.Red;

            foreach (string line in ds.StockItemsUnderMin())
            {
                Min.Text += "\n\t"+line;
            }
        }
       
       
        //Indlæser de vin der er over maskimumsgrænsen
        private void OverMax()
        {
            Maks.Text = "";
            Maks.Text = "Varer over maksimum:";

            Maks.Foreground = Brushes.Green;

            foreach (string line in ds.StockItemsOverMax())
            {
                Maks.Text += "\n\t" + line;
            }
        }
        
       
        //Indlæser de vin der er blevet solgt mest af
        private void MostSold()
        {
            Solgt.Text = "";
            Solgt.Text = "Mest solgte varer:";

            foreach (string line in ds.StockItemsMostSold())
            {
                Solgt.Text += "\n\t" + line;
            }
        }
       
        //Indlæser temperatur og fugtighed både udenfor og indenfor lageret
        private void TempHumi()
        {
            Temp.Text = "";
            Temp.Text = "Lager:";
            Temp.Text += "\nTemp: "+ ds.StockTemp() + " C";
            Temp.Text += "\nFugt:   " + ds.StockHumidity() + " %";

            Temp.Text += "\n";
            Temp.Text += "\nUdenfor:";
            Temp.Text += "\nTemp: " + ds.OutdoorTemp() + " C";
            Temp.Text += "\nFugt:   " + ds.OutdoorHumidity() + " %";

        }


        //Opdaterer Dato og Tid i 3 tidszoner
        private void DateAndTime()
        {
            
            DateTime tid = DateTime.Now;
            Tid.Text = "";
            Tid.Text += "København:";
            Tid.Text += "\n"+Convert.ToString(tid.DayOfWeek);
            Tid.Text += "\t"+Convert.ToString(tid);
            
            Tid.Text += "\n\nLondon:";
            Tid.Text += "\n" + Convert.ToString(tid.AddHours(-1).DayOfWeek);
            Tid.Text += "\t" + Convert.ToString(tid.AddHours(-1));

            Tid.Text += "\n\nSingapore:";
            Tid.Text += "\n" + Convert.ToString(tid.AddHours(7).DayOfWeek);
            Tid.Text += "\t" + Convert.ToString(tid.AddHours(7));

        }


        //Funktion der bliver kaldt hvert sekund via Timer
        private void Timer_Click(object sender, EventArgs e)

        {
            //Opdaterer uret hvert sekund
            DateAndTime();
            count++;
            
            //Opdaterer alle data hvis der er gået cirka 5 minutter
            if(count >= 300)
            {
                RefreshAll();
                count = 0;
            }
        }


        //Debugging værktøj der opdaterer alle data når knappen trykkes
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RefreshAll();
        }
    }
}
