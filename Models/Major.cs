namespace Lab4.Models
{
    public class Major
    {

        public Major()
        {
            Learners=new HashSet<Learner>();
        }
        public int MajorID { get; set; }
        public string MajorName { get; set; }
        public ICollection<Learner> Learners { get; set; }

    }
}
