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
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        public BooksCollection books = new BooksCollection();
        public Books book = new Books();
        public AdminWindow(string a)
        {
            InitializeComponent();
            AddBooks();

            //sets timer depending on condition
            timer = new DispatcherTimer();
            labelTimer.Content = a;
            if (labelTimer.Content.ToString() != "")
            {
                labelTimer.Content = a;
                timer.Start();
                timer.Interval = new TimeSpan(0, 0, 1);
                timer.Tick += TimerTick;

            }
            else
            {
                timer.Interval = new TimeSpan(0, 0, 1);
                timer.Tick += TimerTick;

            }

        }
        private DispatcherTimer timer;
        int time;

        //timer logic
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
        }


        //adding books to list
        public void AddBooks()
        {

            foreach (Books book in books.GetBooks())
            {
                listBox.Items.Add(book);

            }


        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBox.SelectedIndex > -1)
            {
                Books selectedBook = (Books)listBox.SelectedItem;
                id.Text = selectedBook.ID.ToString();
                title.Text = selectedBook.Title;
                price.Text = selectedBook.Default_Price.ToString();
            }
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            id.Clear();
            title.Clear();
            price.Clear();
            listBox.SelectedIndex = -1;
        }


        //inserting book
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            int index = listBox.SelectedIndex;
            timer.Start();
            time = 120;

            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["ConnString"].ToString();
                conn.Open();
                SqlCommand command = new SqlCommand("INSERT INTO Books(Title, Default_Price, Last_Price, Last_Bidder) VALUES(@Title, @Default_Price, @Default_Price, 'No bidder yet')", conn);


                command.Parameters.AddWithValue("@Title", title.Text);
                command.Parameters.AddWithValue("@Default_Price", price.Text);

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
                AddBooks();



            }
        }


        //deleting book
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            int index = listBox.SelectedIndex;
            if (index > -1)
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = ConfigurationManager.ConnectionStrings["ConnString"].ToString();
                    conn.Open();
                    SqlCommand command = new SqlCommand("DELETE FROM Books WHERE ID=@ID", conn);

                    command.Parameters.AddWithValue("@ID", id.Text);


                    try
                    {

                        command.ExecuteNonQuery();

                    }
                    catch (Exception exc)
                    {

                        MessageBox.Show(exc.Message);
                        return;
                    }
                    listBox.Items.Clear();
                    AddBooks();

                }
            }
        }

        //back button click
        private void Back_Click(object sender, RoutedEventArgs e)
        {

            MainWindow window = new MainWindow(labelTimer.Content.ToString());
            this.Close();
            window.ShowDialog();
        }
    }
}

