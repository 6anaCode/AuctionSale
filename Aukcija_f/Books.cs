namespace Aukcija_f
{
    public class Books
    {

        public int ID { get; set; }
        public string Title { get; set; }
        public decimal Default_Price { get; set; }
        public decimal Last_Price { get; set; }
        public string Last_Bidder { get; set; }

        public override string ToString()
        {
            return string.Format("{0}  {1}  {2}€  {3}€  {4}", ID, Title, Default_Price, Last_Price, Last_Bidder);
        }



    }
}
