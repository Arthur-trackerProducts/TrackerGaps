namespace TrackerGaps
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var arr = new[] { 0, 2, 3, 3, 4, 6, 6, 7, 7 }.Select(Seq.Parse).ToList();
            var arr2 = new[] { 8, 9, 9, 10, 11, 13, 14 }.Select(Seq.Parse).ToList();

            var gaps = new Queue<int>();
            var dups = new Queue<Seq>();
            bool firstRun = true;

            var prev = -1;
            ProcessArray(arr, gaps, dups, ref firstRun, ref prev);
            ProcessArray(arr2, gaps, dups, ref firstRun, ref prev);


            //output
            Console.WriteLine($"Gaps: {string.Join(", ", gaps)}");
            Console.WriteLine($"Dups: {string.Join(", ", dups.Select(x => x.Id))}");


            Console.WriteLine($"PROCESS\n");

            while (gaps.Any())
            {
                var gap = gaps.Dequeue();
                var dup = dups.Peek();
                while (gap < dup.SeqId) //If the gap is behaind, then leave it, only move forward for the dups
                {
                    gap = gaps.Dequeue();

                    if (gap < dup.SeqId && !gaps.Any()) //This is the last Gap and it is lower than current Id
                    {
                        gap = -1;
                        break;
                    }
                }
                if (gap != -1)
                {
                    //There is a gap to fit the dup inside
                    dup = dups.Dequeue();
                    dup.SeqId = gap;
                }
            }

            //Add after the last
            while (dups.Any())
            {
                var dup = dups.Dequeue();
                dup.SeqId = ++prev;
            }

            var final = arr.Union(arr2).OrderBy(x => x.SeqId);

            Console.WriteLine($"IDS: {string.Join(", ", final.Select(x => x.Id))}");
            Console.WriteLine($"SEQ: {string.Join(", ", final.Select(x => x.SeqId))}");


        }

        private static void ProcessArray(List<Seq> arr, Queue<int> gaps, Queue<Seq> dups, ref bool firstRun, ref int prev)
        {
            for (int i = 0; i < arr.Count; i++)
            {
                if (firstRun)
                {
                    firstRun = false;
                    prev = arr[i].SeqId;
                    continue;
                }

                if (arr[i].SeqId == prev)
                {
                    //dup
                    dups.Enqueue(arr[i]);
                }
                else if (arr[i].SeqId < prev)// not possible since sorted
                {
                    //dup
                    dups.Enqueue(arr[i]);
                }
                else
                {
                    //gap
                    if (arr[i].SeqId - prev == 1)
                    {
                        prev = arr[i].SeqId;
                        continue;
                    }

                    //There is some gaps, find the number of gaps
                    foreach (var gap in Enumerable.Range(prev + 1, arr[i].SeqId - (prev + 1)))
                    {
                        gaps.Enqueue(gap);
                    }
                }
                prev = arr[i].SeqId;

            }
        }
    }

    public class Seq
    {
        public int SeqId { get; set; }
        public string Id { get; set; }

        public static Seq Parse(int id, int index)
        {
            return new Seq
            {
                SeqId = id,
                Id = $"x-{index}-{id}"
            };
        }
    }

}