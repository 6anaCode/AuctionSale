using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace Aukcija_f
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AddCollection();


        }

        public MainWindow(string a)
        {
            InitializeComponent();
            AddCollection();

            timer = new DispatcherTimer();
            labelTimer.Content = a;
            timer.Start();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += TimerTick;



        }




        int time;
        private DispatcherTimer timer;

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



        public BooksCollection books = new BooksCollection();

        //adding books to list
        public void AddCollection()
        {
            foreach (Books book in books.GetBooks())
            {
                listBox.Items.Add(book);
            }
        }

        //login user or admin
        private void Login_Click(object sender, RoutedEventArgs e)
        {

            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["ConnString"].ToString();
                conn.Open();
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                String query = "SELECT COUNT(1) FROM Users WHERE UserName=@UserName AND UserPass=@UserPass";

                String query2 = "SELECT UserName FROM Users WHERE UserName=@UserName AND UserPass=@UserPass";
                SqlCommand command = new SqlCommand(query, conn);

                SqlCommand commandu = new SqlCommand(query2, conn);
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@UserName", UserName.Text);
                command.Parameters.AddWithValue("@UserPass", UserPass.Password.ToString());
                commandu.Parameters.AddWithValue("@UserName", UserName.Text);
                commandu.Parameters.AddWithValue("@UserPass", UserPass.Password.ToString());




                int count = Convert.ToInt32(command.ExecuteScalar());



                if (count == 1)
                {
                    string commandu1 = commandu.ExecuteScalar().ToString();
                    string path = @"username.txt";


                    using (StreamWriter sw = new StreamWriter(path, false))
                    {
                        sw.WriteLine(commandu1);

                    }

                    UserWindow window = new UserWindow(labelTimer.Content.ToString());
                    this.Close();
                    window.ShowDialog();
                    UserName.Clear();
                    UserPass.Clear();
                }
                else if (1 != count && UserName.Text == "admin" && UserPass.Password == "admin")
                {

                    AdminWindow window = new AdminWindow(labelTimer.Content.ToString());
                    this.Close();
                    window.ShowDialog();
                    UserName.Clear();
                    UserPass.Clear();
                }
                else
                {
                    MessageBox.Show("Login failed! Try again.");
                }
            }


        }
    }
}

