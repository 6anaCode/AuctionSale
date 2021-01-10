using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows;

namespace Aukcija_f
{
    public class BooksCollection
    {
        public ObservableCollection<Books> GetBooks()
        {
            ObservableCollection<Books> booksCollection = new ObservableCollection<Books>();

            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["ConnString"].ToString();
                conn.Open();

                SqlCommand command = new SqlCommand("SELECT * FROM Books", conn);

                try
                {

                    SqlDataReader dr = command.ExecuteReader();
                    while (dr.Read())
                    {
                        Books book = new Books();
                        book.ID = dr.GetInt32(0);
                        book.Title = dr.GetString(1);
                        book.Default_Price = dr.GetDecimal(2);
                        book.Last_Price = dr.GetDecimal(3);
                        book.Last_Bidder = dr.GetString(4);
                        booksCollection.Add(book);
                    }


                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);

                }


                return booksCollection;
            }


        }
    }
}

