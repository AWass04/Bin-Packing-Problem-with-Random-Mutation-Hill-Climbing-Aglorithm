using System;
using System.Runtime.InteropServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace p4cs
{
    class Assignment
    {
        public static void Main()
        {
            List<float> data = new List<float>();
            string fileName = @"C:\Users\Aaron Wass\Documents\Uni\Yr 2 - Semester 1\Algorithms and Data Structures\Assignment\Project 4\Bin Packing with Hill Climbing\Bin Packing with Hill Climbing\dataset.csv";

            // Put dataset into a list
            using (StreamReader sr = new StreamReader(fileName)) {
                
                string line;

                while ((line = sr.ReadLine()) != null) {
                   
                    // Convert each line to a double and add to the list
                    if (float.TryParse(line, out float value)) {
                        data.Add(value);
                    }
                }
            }

            HillClimbing hc = new HillClimbing(data);
            hc.RunHC(600);
            
        }
    }

    public class HillClimbing
    {
        public static List<float> data;
        public HillClimbing(List<float> d)
        {
            data = new List<float>(d);
        }

        public void RunHC(int iter)
        {
            const int MAX_HEIGHT = 130; 

            List<List<float>> currentSolution = CreateInitalSolution(data); // Get inital solution
            float fitness = CalculateFitness(MAX_HEIGHT, currentSolution); // Get inital solutions fitness

            Console.WriteLine("      Inital Solution    ");
            Console.WriteLine("-------------------------\n");

            Console.WriteLine("Number of bins = " + currentSolution.Count);
            Console.WriteLine("Fitness = " + fitness + "\n\n");

            PrintBins(currentSolution);

            for (int i = 0; i < iter; i++) {
                float newFitness = 0;
                List<List<float>> newSolution  = new List<List<float>>();

                newSolution = SmallChange(currentSolution, MAX_HEIGHT); // Make a small change
                newFitness = CalculateFitness(MAX_HEIGHT, newSolution); // Get the fitness of the mutated solution

                if (newFitness < fitness && newFitness != fitness) { // Check to see if its better
                
                    // Accept it if its better
                    currentSolution = newSolution;
                    fitness = newFitness;
                }
            }

            Console.WriteLine("\n\n           Result        ");
            Console.WriteLine("-------------------------\n");

            Console.WriteLine("Number of bins = " + currentSolution.Count);
            Console.WriteLine("Fitness = " + fitness + "\n\n");

            PrintBins(currentSolution);
        }

        public List<List<float>> CreateInitalSolution(List<float> data)
        {
            Random random = new Random();
            List<List<float>> bins = new List<List<float>>();

            foreach (float item in data) {

                // Randomly assign each box into a ranom bin or create a new bin
                if (bins.Count == 0 || random.NextDouble() < 0.5) { // 50% chance to create a new bin

                    bins.Add(new List<float> { item });
                }
                else {
                    // Add to a random existing bin
                    int randomBinIndex = random.Next(bins.Count);
                    bins[randomBinIndex].Add(item);
                }
            }
            return bins;
        }
        public List<List<float>> SmallChange(List<List<float>> newSoloution, int MAX_HEIGHT)
        {
            Random random = new Random();

            // Choose a random bin and get a random box in siad bin
            int randomBin = random.Next(newSoloution.Count);
            int randomBox = random.Next(newSoloution[randomBin].Count);
            float box = newSoloution[randomBin][randomBox];

            newSoloution[randomBin].RemoveAt(randomBox); // Remove the selected box from the bin

            newSoloution.RemoveAll(bin => bin.Count == 0); // Normalise the bins

            bool boxPlaced = false;
            int newRandomBin = random.Next(newSoloution.Count);

            // Try to randomly place box into a bin 
            for (int i = 0; i < newSoloution.Count && !boxPlaced; i++) {

                newRandomBin = random.Next(newSoloution.Count); // Get a random bin

                if (newSoloution[newRandomBin].Sum() + box <= MAX_HEIGHT)  { // Check it wouldnt overflow

                    newSoloution[newRandomBin].Add(box);
                    boxPlaced = true;
                }
            }

            if (!boxPlaced) { // If did not place box in bin make a new bin

                newSoloution.Add(new List<float> { box });
            }

            return newSoloution;
        }

        public float CalculateFitness(int MAX_HEIGHT, List<List<float>> solution)
        {
            float fitness = 0;
            int binsUsed = solution.Count;
            float overflow = 0;
            float wastedCapacity = 0;

            foreach (List<float> bin in solution)
            {
                float height = bin.Sum();

                // Calculate overflow if the bin exceeds the maximum height
                if (height > MAX_HEIGHT) overflow += height - MAX_HEIGHT;
                else wastedCapacity += MAX_HEIGHT - height;
            }

            fitness = (float)(binsUsed + (overflow * 10) + (wastedCapacity * 0.5)); // Calaculate

            return fitness;
        }

        public void PrintBins(List<List<float>> bins)
        {
            int count = 0;
            foreach (List<float> bin in bins) {

                float height = 0;
                count++;
                Console.Write(count + ": ");

                foreach (float box in bin) {
                    
                    Console.Write(box + " ");
                }

                Console.WriteLine();
            }
        }
    }
}
