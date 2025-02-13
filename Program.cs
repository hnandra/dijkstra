﻿// Author(s): Michael Koeppl

using System.Text;
using static dijkstra.Parcel;

namespace dijkstra
{
    class Program
    {
        // Our dictionary of nodes. Allows us to quickly change a nodes value
        // through its name (the key).
        static Dictionary<string, Node> nodeDict = new Dictionary<string, Node>();

        // The list of routes.
        static List<Route> routes = new List<Route>();

        // This set allows us to quickly check which nodes we have already
        // visited.
        static HashSet<string> unvisited = new HashSet<string>();

        const string graphFilePath = "./graph.txt";


        static void Main(string[] args)
        {
            Parcel parcel = GetParcelSpecification();

            try
            {
                initGraph();
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            Console.Clear();
            PrintOverview();

            var (startNode, destNode) = GetStartAndEnd();



            // Set our start node. The start node has to have a value
            // of 0 because we're already there.
            nodeDict[startNode].Value = 0;

            var queue = new PrioQueue();
            queue.AddNodeWithPriority(nodeDict[startNode]);

            // Do the calculations to find the shortest path to every node
            // in the graph starting from our starting node.
            CheckNode(queue, destNode);

            // Print out the result
            PrintShortestPath(startNode, destNode, parcel);
        }

        private static Parcel GetParcelSpecification()
        {
            string size = "";
            double weight = 0;
            ParcelType type = 0;


            while (String.IsNullOrEmpty(size)) 
            {
                Console.WriteLine("Parcel size: [A, B, C]");
                size = Console.ReadLine().ToUpper();
            }

            Console.WriteLine("Parcel weight: [Kilograms]");
            while (!double.TryParse(Console.ReadLine(),out weight))
            {
                Console.WriteLine("Please enter a number");
                Console.WriteLine("Parcel weight: [Kilograms]");
            }

            Console.WriteLine("Parcel type: [Weapons = 1, Cautious parcels = 2, Refrigerated goods = 3]");
            while (!Enum.IsDefined(typeof(ParcelType), type))
            {
                while (!Enum.TryParse(Console.ReadLine(), out type))
                {
                    Console.WriteLine("Please enter a number");
                    Console.WriteLine("Parcel type: [Weapons = 1, Cautious parcels = 2, Refrigerated goods = 3]");
                }
                Console.WriteLine("Please enter a number from 1 to 3");
                Console.WriteLine("Parcel type: [Weapons = 1, Cautious parcels = 2, Refrigerated goods = 3]");
            }
            Parcel parcel = new Parcel(weight, size, type);

            return parcel;
        }

        private static void initGraph()
        {
            if (!File.Exists(graphFilePath))
            {
                throw new FileNotFoundException("File not found");
            }

            using (var fileStream = File.OpenRead(graphFilePath))
            {
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, 128))
                {
                    String line;
                    int cost = 8;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        var values = line.Split(",");
                        var (from, to, distance) = (values[0], values[1], cost);
                        if (!nodeDict.ContainsKey(from)) { nodeDict.Add(from, new Node(from)); }
                        if (!nodeDict.ContainsKey(to)) { nodeDict.Add(to, new Node(to)); }
                        unvisited.Add(from);
                        unvisited.Add(to);

                        routes.Add(new Route(from, to, distance));
                    }
                }
            }
        }

        private static void PrintOverview()
        {
            Console.WriteLine("Cities:");
            foreach (Node node in nodeDict.Values)
            {
                Console.WriteLine("\t" + node.Name);
            }
            /*
                        Console.WriteLine("\nRoutes:");
                        foreach (Route route in routes)
                        {
                            Console.WriteLine("\t" + route.From + " -> " + route.To + " Distance: " + route.Distance);
                        }
            */
        }
        private static Boolean CheckNodeInDict(String node)
        {
            if (!nodeDict.ContainsKey(node))
            {
                Console.WriteLine("City is not available by plane.");
                return false;
            } else if (node == "")
            {
                Console.WriteLine("Please enter a city.");
                return false;
            }
            return true;
        }

        // Get user input for the start and destionation nodes.
        private static (string, string) GetStartAndEnd()
        {
            String startNode = null;
            String destNode = null;
            while (startNode == null)
            {
                Console.WriteLine("\nEnter the start city: ");
                startNode = Console.ReadLine();
                if (!CheckNodeInDict(startNode))
                {
                    startNode = null;
                }
            }
            while (destNode == null)
            {
                Console.WriteLine("Enter the destination city: ");
                destNode = Console.ReadLine();
                if (!CheckNodeInDict(destNode))
                {
                    destNode = null;
                }
            }
            return (startNode, destNode);
        }

        // Called for each node in the graph and iterates over its directly
        // connected nodes. The function always handles the node that
        // currently has the highest priority in our queue.
        // So this function checks any directly connected  node and compares
        // the value it currently holds (the shortest path we know to it) is
        // bigger than the distance of the path through the node we're
        // currently checking.
        // If it is, we just found a shorter path to it and we update its
        // 'shortest path' value and also update its previous node to the
        // one we're currently processing.
        // Every directly connected node that we find we also add to the queue
        // (which is sorted by distance), if it's not already in the queue.
        // After we're finished 
        private static void CheckNode(PrioQueue queue, string destinationNode)
        {
            // If there are no nodes left to check in our queue, we're done.
            if (queue.Count == 0)
            {
                return;
            }

            foreach (var route in routes.FindAll(r => r.From == queue.First.Value.Name))
            {
                // Skip routes to nodes that have already been visited.
                if (!unvisited.Contains(route.To))
                {
                    continue;
                }

                double travelledDistance = nodeDict[queue.First.Value.Name].Value + route.Distance;

                // We only look at nodes we haven't visited yet and we only
                // update the node's values if the distance of the path we're
                // currently checking is shorter than the one we found before.
                if (travelledDistance < nodeDict[route.To].Value)
                {
                    nodeDict[route.To].Value = travelledDistance;
                    nodeDict[route.To].PreviousNode = nodeDict[queue.First.Value.Name];
                }

                // We don't add the 'to' node to the queue if it has already been
                // visited and we don't allow duplicates.
                if (!queue.HasLetter(route.To))
                {
                    queue.AddNodeWithPriority(nodeDict[route.To]);
                }
            }
            unvisited.Remove(queue.First.Value.Name);
            queue.RemoveFirst();

            CheckNode(queue, destinationNode);
        }

        // Starts with the destination node and basically adds all the nodes'
        // respective previous nodes to a list, which is then reversed and
        // printed out.
        private static void PrintShortestPath(string startNode, string destNode, Parcel parcel)
        {
            var pathList = new List<String> { destNode };
            Node currentNode = nodeDict[destNode];

            while (currentNode != nodeDict[startNode])
            {
                pathList.Add(currentNode.PreviousNode.Name);
                currentNode = currentNode.PreviousNode;
            }

            pathList.Reverse();
            for (int i = 0; i < pathList.Count; i++)
            {
                Console.Write(pathList[i] + (i < pathList.Count - 1 ? " -> " : "\n"));
            }

            var totaRouteTimeCost = nodeDict[destNode].Value;
            var costPerFlight = totaRouteTimeCost / 8;
            var parcelCost = CalculateParcelCost(parcel);
            var deliveryCost = parcelCost * costPerFlight;


            Console.WriteLine("Overall cost: $" + deliveryCost);
        }

        private static double CalculateParcelCost(Parcel parcel)
        {
            double cost = 0;

            switch (parcel.parcelWeight)
            {
                case >= 1 and < 5:
                    cost += 20;
                    break;
                case > 5:
                    cost += 40;
                    break;
            }

            switch (parcel.parcelSize)
            {
                case "A":
                    cost += 40;
                    break;
                case "B":
                    cost += 48;
                    break;
                case "C":
                    cost += 80;
                    break;
            }

            switch (parcel.parcelType)
            {
                case ParcelType.Weapons:
                    cost *= 2;
                    break;
                case ParcelType.CautiousParcels:
                    cost *= 1.75;
                    break;
                case ParcelType.RefrigeratedGoods:
                    cost *= 1.1;
                    break;
            }

            return cost;
        }
    }
}
