namespace NetProc.Data
{
    /// <summary>
    /// This is just for designing using dotnet EF
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            SeedData();

            //used to create options for sql
            //int[] options = new int[31];
            //int ii = 0;
            //for (int i = 1; i < options.Length; i++)
            //{
            //    ii--;
            //    options[i] = ii;
            //}
            //var arryStr = string.Join(',', options);
        }

        /// <summary>
        /// Initialize data from 'p3roc.sql`
        /// </summary>
        static void SeedData()
        {
            using (var ctx = new NetProcDbContext())
            {
                ctx.InitializeDatabase(true);
            }
        }
    }
}
