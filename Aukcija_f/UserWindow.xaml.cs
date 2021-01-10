using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Aukcija_f
{
    /// <summary>
    /// Interaction logic for UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {

        public int price = 1;
        public decimal bid_value;
        private int time = 120;
        private DispatcherTimer timer;
        public BooksCollection books = new BooksCollection();
        public Books book = new Books();




        public UserWindow(string a)
        {
            InitializeComponent();




            timer = new DispatcherTimer();
            labelTimer.Content = a;
            if (labelTimer.Content.ToString() != "")
            {
                buttonBid.IsEnabled = true;
                labelTimer.Content = a;
                timer.Start();
                timer.Interval = new TimeSpan(0, 0, 1);
                timer.Tick += TimerTick;
                foreach (Books book in books.GetBooks())
                {
                    listBox.Items.Add(book);
                }
            }
            else
            {
                timer.Interval = new TimeSpan(0, 0, 1);
                timer.Tick += TimerTick;
                foreach (Books book in books.GetBooks())
                {
                    listBox.Items.Add(book);
                }
            }


        }



        private void TimerTick(object sender, EventArgs e)
        {
            if (labelTimer.Content.ToString() != "")
            {
                string[] split = labelTimer.Content.ToString().Split(':');

                int minute = int.Parse(split[0]);
                int second = int.Parse(split[1]);
                time = minute * 60 + second;
            }
            if (time > 0)
            {
                if (time <= 10)
                {

                    time--;
                    labelTimer.Content = string.Format("0{0}:0{1}", time / 60, time % 60);
                }
                else
                {
                    if (labelTimer.Content.ToString() == "01:57")
                    {
                        bidmessage.Visibility = Visibility.Hidden;
                    }
                    time--;
                    labelTimer.Content = string.Format("0{0}:{1}", time / 60, time % 60);
                }

            }
            else if (time == 0 && labelTimer.Content.ToString() == "00:00")
            {
                string path = @"username.txt";
                string username = "";

                using (StreamReader sr = File.OpenText(path))
                {
                    string s;
                    while ((s = sr.ReadLine()) != null)
                    {
                        username = username + s;
                    }
                }
                timer.Stop();
                buttonBid.IsEnabled = false;
                if (listBox.Items[listBox.Items.Count - 1].ToString().EndsWith("No bidder yet") == false)
                {
                    message.Content = "Auction Finished. \nThe winner is " + username;
                }
                if (listBox.Items[listBox.Items.Count - 1].ToString().EndsWith("No bidder yet") == true)
                {
                    message.Content = "There is no winner";


                }
                labelTimer.Content = "";
            }
            else
            {

                decimal price = 1;
                decimal price1;
                price1 = Convert.ToDecimal(bid.Text);
                price1 = decimal.Parse(bid.Text);

                decimal final = price + price1;

                timer.Stop();


            }
        }

        //logic for bidding
        private void Bid_Click(object sender, RoutedEventArgs e)
        {

            bidmessage.Visibility = Visibility.Visible;


            labelTimer.Content = "02:00";


            decimal price = 1;
            decimal price1;





            string path = @"username.txt";
            string username = "";

            using (StreamReader sr = File.OpenText(path))
            {
                string s;
                while ((s = sr.ReadLine()) != null)
                {
                    username = username + s;
                }
            }


            using (SqlConnection conn = new SqlConnection())
            {



                conn.ConnectionString = ConfigurationManager.ConnectionStrings["ConnString"].ToString();
                conn.Open();
                SqlCommand command = new SqlCommand("UPDATE Books SET Last_Price = @Last_Price, Last_Bidder=@Last_Bidder WHERE ID = (SELECT MAX(ID) FROM Books) ", conn);
                SqlCommand command1 = new SqlCommand("SELECT Last_Price FROM Books WHERE ID = (SELECT MAX(ID) FROM Books)", conn);



                bid.Text = command1.ExecuteScalar().ToString();
                price1 = Convert.ToDecimal(bid.Text);
                price1 = decimal.Parse(bid.Text);
                decimal final = price + price1;

                command.Parameters.AddWithValue("@Last_Price", final);
                command.Parameters.AddWithValue("@Last_Bidder", username);


                try
                {

                    command.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
                listBox.Items.Clear();
                foreach (Books book in books.GetBooks())
                {
                    listBox.Items.Add(book);
                }




            }



        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBox.SelectedIndex > -1)
            {
                Books selectedBook = (Books)listBox.SelectedItem;
                id.Text = selectedBook.ID.ToString();

                bid.Text = selectedBook.Last_Price.ToString();
            }
        }


        //back button
        private void Back_Click(object sender, RoutedEventArgs e)
        {


            MainWindow window = new MainWindow(labelTimer.Content.ToString());
            this.Close();
            window.ShowDialog();
        }
    }
}

