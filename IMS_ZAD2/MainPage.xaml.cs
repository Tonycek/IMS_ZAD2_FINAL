using IMS_ZAD2.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MySql.Data;
using MySql.Data.MySqlClient;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace IMS_ZAD2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        HttpClient client;
        private List<Test> Testy;
        private ObservableCollection<Post> _posts;        private const string Url = "http://www.automobilova-mechatronika.fei.stuba.sk/webstranka/?q=node/59/";
        public class Post
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Body { get; set; }
        }

        //public class Test
        //{
        //    public string Test1 { get; set; }
        //    public string Test2 { get; set; }
        //    public string Test3 { get; set; }
        //    public string Test4 { get; set; }
        //    public string Test5 { get; set; }
        //    public string Test6 { get; set; }
        //}

        private async Task<string> getText()
        {
            string text = await client.GetStringAsync(Url);
            Int32 indexNodeStart = text.IndexOf("?q=node/") + 12;
            string webStranka = text.Substring(indexNodeStart - 12, 10);
            Int32 indexNodeEnd;

            while (indexNodeStart > 0)
            {
                indexNodeEnd = text.IndexOf("</a>", indexNodeStart);
                string textButton = text.Substring(indexNodeStart, indexNodeEnd - indexNodeStart);
                Button buttonik = new Button();
                buttonik.Content = textButton;//.Substring(0, 11);
                buttonik.Tag = webStranka;
                buttonik.VerticalAlignment = VerticalAlignment.Center;
                buttonik.HorizontalAlignment = HorizontalAlignment.Center;
                buttonik.Click += Buttonik_Click;
                stackPanel1.Children.Add(buttonik);

                if (indexNodeStart > text.LastIndexOf("?q=node/"))
                    break;

                indexNodeStart = text.IndexOf("?q=node/", indexNodeStart + 1) + 12;
                webStranka = text.Substring(indexNodeStart - 12, 10);
            }

            return "";
        }

        private async void Buttonik_Click(object sender, RoutedEventArgs e)
        {
            string content = (sender as Button).Tag.ToString();
            HttpClient webclient1 = new HttpClient();
            string stranka = "http://www.automobilova-mechatronika.fei.stuba.sk/webstranka/" + content;
            string text = await client.GetStringAsync(stranka);
            //var fs = new FormattedString();

            int nadpisZaciatok = text.IndexOf("<h3>");
            int nadpisKoniec = text.IndexOf("</h3>");
            if (text.Substring(nadpisZaciatok, nadpisKoniec - nadpisZaciatok).Contains("<i class="))
            {
                int icko = text.IndexOf("</i>", nadpisZaciatok + 26) + 4;
                string nadpisok = text.Substring(icko, nadpisKoniec - (icko)) + System.Environment.NewLine;
                // fs.Spans.Add(new Span { Text = nadpisok, ForegroundColor = Color.Red, FontSize = 20, FontAttributes = FontAttributes.Italic });
                labelText.Text = labelText.Text + nadpisok + System.Environment.NewLine;
            }

            else
            {
                string nadpisok = text.Substring(nadpisZaciatok, nadpisKoniec - nadpisZaciatok) + System.Environment.NewLine;
                labelText.Text = labelText.Text + nadpisok + System.Environment.NewLine;
            }

            int spanPosition;
            int pPosition;
            string odstavce = "";
            int indexZaciatok = text.IndexOf("<p");
            indexZaciatok = indexZaciatok + 3;

            string nadpis;
            while (nadpisZaciatok != -1)
            {
                spanPosition = text.IndexOf("<span", nadpisKoniec + 3) + 19;
                pPosition = text.IndexOf("<p", nadpisKoniec + 3);

                if (spanPosition < pPosition)
                {
                    odstavce = text.Substring(spanPosition + 4, text.IndexOf("</span", nadpisKoniec + 4) - spanPosition - 4);
                    labelText.Text = labelText.Text + odstavce + System.Environment.NewLine;
                }

                else if (spanPosition > pPosition)
                {
                    odstavce = text.Substring(pPosition + 3, text.IndexOf("</p>", nadpisKoniec + 4) - pPosition);
                    labelText.Text = labelText.Text + odstavce + System.Environment.NewLine;
                }

                nadpisZaciatok = text.IndexOf("<h3>", nadpisKoniec + 1);
                if (nadpisZaciatok < 0)
                    break;

                nadpisKoniec = text.IndexOf("</h3>", nadpisZaciatok);
                if (text.Substring(nadpisZaciatok, nadpisKoniec - (nadpisZaciatok)).Contains("<i class="))
                {
                    int icko = text.IndexOf("</i>", nadpisZaciatok + 26) + 4;
                    nadpis = System.Environment.NewLine + System.Environment.NewLine + text.Substring(icko, nadpisKoniec - icko) + System.Environment.NewLine;
                    labelText.Text = labelText.Text + System.Environment.NewLine + System.Environment.NewLine + nadpis + System.Environment.NewLine;
                }
                else
                {
                    nadpis = System.Environment.NewLine + System.Environment.NewLine + text.Substring(nadpisZaciatok + 4, nadpisKoniec - (nadpisZaciatok + 4)) + System.Environment.NewLine;
                    labelText.Text = labelText.Text + System.Environment.NewLine + System.Environment.NewLine + nadpis + System.Environment.NewLine;
                }
            }
            stackPanel1.Visibility = Visibility.Collapsed;
            stackPanel2.Visibility = Visibility.Visible;
        }

        public MainPage()
        {
            this.InitializeComponent();
            client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;
            Testy = new List<Test>();

            getText();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
           // string content = (sender as Button).Tag.ToString();
            HttpClient webclient1 = new HttpClient();
            string stranka = "http://www.automobilova-mechatronika.fei.stuba.sk/webstranka/?q=node/97/";
            string text = await client.GetStringAsync(stranka);

            int nadpisZaciatok = text.IndexOf("<h3>");
            int nadpisKoniec = text.IndexOf("</h3>");
            if (text.Substring(nadpisZaciatok, nadpisKoniec - nadpisZaciatok).Contains("<i class="))
            {
                int icko = text.IndexOf("</i>", nadpisZaciatok + 26) + 4;
                string nadpisok = text.Substring(icko, nadpisKoniec - (icko)) + System.Environment.NewLine;
                // fs.Spans.Add(new Span { Text = nadpisok, ForegroundColor = Color.Red, FontSize = 20, FontAttributes = FontAttributes.Italic });
                labelText.Text = labelText.Text + nadpisok + System.Environment.NewLine;
            }

            else
            {
                string nadpisok = text.Substring(nadpisZaciatok, nadpisKoniec - nadpisZaciatok) + System.Environment.NewLine;
                labelText.Text = labelText.Text + nadpisok + System.Environment.NewLine;
            }

            int spanPosition;
            int pPosition;
            string odstavce = "";
            int indexZaciatok = text.IndexOf("<p");
            indexZaciatok = indexZaciatok + 3;

            string nadpis;
            while (nadpisZaciatok != -1)
            {
                spanPosition = text.IndexOf("<span", nadpisKoniec + 3) + 19;
                pPosition = text.IndexOf("<p", nadpisKoniec + 3);

                if (spanPosition < pPosition)
                {
                    odstavce = text.Substring(spanPosition + 4, text.IndexOf("</span", nadpisKoniec + 4) - spanPosition - 4);
                    labelText.Text = labelText.Text + odstavce + System.Environment.NewLine;
                }

                else if (spanPosition > pPosition)
                {
                    odstavce = text.Substring(pPosition + 3, text.IndexOf("</p>", nadpisKoniec + 4) - pPosition);
                    labelText.Text = labelText.Text + odstavce + System.Environment.NewLine;
                }

                nadpisZaciatok = text.IndexOf("<h3>", nadpisKoniec + 1);
                if (nadpisZaciatok < 0)
                    break;

                nadpisKoniec = text.IndexOf("</h3>", nadpisZaciatok);
                if (text.Substring(nadpisZaciatok, nadpisKoniec - (nadpisZaciatok)).Contains("<i class="))
                {
                    int icko = text.IndexOf("</i>", nadpisZaciatok + 26) + 4;
                    nadpis = System.Environment.NewLine + System.Environment.NewLine + text.Substring(icko, nadpisKoniec - icko) + System.Environment.NewLine;
                    labelText.Text = labelText.Text + System.Environment.NewLine + System.Environment.NewLine + nadpis + System.Environment.NewLine;
                }
                else
                {
                    nadpis = System.Environment.NewLine + System.Environment.NewLine + text.Substring(nadpisZaciatok + 4, nadpisKoniec - (nadpisZaciatok + 4)) + System.Environment.NewLine;
                    labelText.Text = labelText.Text + System.Environment.NewLine + System.Environment.NewLine + nadpis + System.Environment.NewLine;
                }
            }

            int[] indexTabulka = new int[5];
            indexTabulka[0] = text.IndexOf("tabulka-mvi");
            indexTabulka[1] = text.IndexOf("tabulka-mvi", indexTabulka[0] + 1);
            indexTabulka[2] = text.IndexOf("tabulka-mvi", indexTabulka[1] + 1);
            indexTabulka[3] = text.IndexOf("tabulka-mvi", indexTabulka[2] + 1);
            indexTabulka[4] = 100000000;
            int tdZaciatok, tdKoniec;
            int tdKonecnyKoniec = text.LastIndexOf("</td>");
            string[] textyDoGridu = new string[10];
            int poradoveCisloDoGridu = 0;

            for (int i = 0; i < 4; i++)
            {
                int thPositionStart = text.IndexOf("<th>", indexTabulka[i]);
                int thPositionEnd;
                int trKoniec;

                if (i == 0)
                {
                    //var data = new Test { Test1 = "1.Ročník", Test2 = "Zimný semester" };
                    Testy.Add(new Test { Test1 = "1.Ročník", Test2 = "Zimný semester" });
                    //tabulka.Items.Add(data);
                }

                for (int j = 0; j < 1; j++)
                {
                    thPositionEnd = text.IndexOf("</th>", thPositionStart);
                    string retazec = text.Substring(thPositionStart + 4, thPositionEnd - (thPositionStart + 4));
                    tdZaciatok = text.IndexOf("<td", thPositionEnd) + 4;
                    trKoniec = text.IndexOf("</tr>", tdZaciatok);

                    while (tdZaciatok < indexTabulka[i + 1])
                    {
                        if (tdZaciatok > trKoniec)
                        {
                           // naVypisanie = naVypisanie + System.Environment.NewLine;
                            trKoniec = text.IndexOf("</tr>", trKoniec + 1);

                            var data = new Test { Test1 = textyDoGridu[0], Test2 = textyDoGridu[1], Test3 = textyDoGridu[2], Test4 = textyDoGridu[3], Test5 = textyDoGridu[4] };
                            Testy.Add(new Test { Test1 = textyDoGridu[0], Test2 = textyDoGridu[1], Test3 = textyDoGridu[2], Test4 = textyDoGridu[3], Test5 = textyDoGridu[4] });
                            //tabulka.Items.Add(data);
                            poradoveCisloDoGridu = 0;
                        }
                        tdKoniec = text.IndexOf("</td>", tdZaciatok);
                        string tdText = text.Substring(tdZaciatok, tdKoniec - tdZaciatok);
                        textyDoGridu[poradoveCisloDoGridu] = tdText;

                        //naVypisanie = naVypisanie + tdText + "\t\t";

                        if (tdKonecnyKoniec == tdKoniec)
                            break;
                        tdZaciatok = text.IndexOf("<td", tdZaciatok + 1) + 4;
                        poradoveCisloDoGridu++;
                    }
                    var data1 = new Test { Test1 = textyDoGridu[0], Test2 = textyDoGridu[1], Test3 = textyDoGridu[2], Test4 = textyDoGridu[3], Test5 = textyDoGridu[4] };
                   // tabulka.Items.Add(data1);
                    Testy.Add(new Test { Test1 = textyDoGridu[0], Test2 = textyDoGridu[1], Test3 = textyDoGridu[2], Test4 = textyDoGridu[3], Test5 = textyDoGridu[4] });

                    if ((i + 1) == 1)
                    {
                        var data = new Test { Test1 = "" };
                       // tabulka.Items.Add(data);
                        data = new Test { Test1 = "1.Ročník", Test2 = "Letný semester" };
                        Testy.Add(new Test { Test1 = "1.Ročník", Test2 = "Letný semester" });
                        //tabulka.Items.Add(data);
                    }

                    if ((i + 1) == 2)
                    {
                        var data = new Test { Test1 = "" };
                        //tabulka.Items.Add(data);
                        data = new Test { Test1 = "2.Ročník", Test2 = "Zimný semester" };
                        Testy.Add(new Test { Test1 = "2.Ročník", Test2 = "Zimný semester" });
                        //tabulka.Items.Add(data);
                    }

                    if ((i + 1) == 3)
                    {
                        var data = new Test { Test1 = "" };
                       // tabulka.Items.Add(data);
                        data = new Test { Test1 = "2.Ročník", Test2 = "Letný semester" };
                        Testy.Add(new Test { Test1 = "2.Ročník", Test2 = "Letný semester" });
                        //tabulka.Items.Add(data);
                    }

                    //textyDoGridu = new string[10];
                    poradoveCisloDoGridu = 0;
                    //naVypisanie = naVypisanie + retazec + "         ";

                    thPositionStart = text.IndexOf("<th>", thPositionStart + 5);
                }

                //naVypisanie = naVypisanie + System.Environment.NewLine;
            }

            stackPanel1.Visibility = Visibility.Collapsed;
            stackPanel2.Visibility = Visibility.Visible;
            tabulka.Visibility = Visibility.Visible;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MySqlConnection con = new MySqlConnection("Server=sql11.freemysqlhosting.net ; Port=3306;database=sql11202652;User Id=sql11202652;Password=iPseGcETV3;charset=utf8");
            MySqlCommand command;
            MySqlDataReader mdr;
            con.Open();
            string select = "SELECT * FROM sql11202652.novinky";
            command = new MySqlCommand(select, con);
            mdr = command.ExecuteReader();
            mdr.Read();
            labelText.Text = mdr.GetString(3);

            stackPanel1.Visibility = Visibility.Collapsed;
            stackPanel2.Visibility = Visibility.Visible;
            tabulka.Visibility = Visibility.Collapsed;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            stackPanel1.Visibility = Visibility.Visible;
            stackPanel2.Visibility = Visibility.Collapsed;
            tabulka.Visibility = Visibility.Collapsed;
        }
    }
}
