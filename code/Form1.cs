using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace SQLite_Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            bool blockedWordExists = false;     // Bool to check if a blocked word is inputted
            string temp = (queryBox.Text).ToUpper();        // Makes input uppercase

            string[] queryInput = temp.Split(' ');      // Splits input in array bases on every ' '

            for (int i = 0; i < queryInput.Length; i++)     // For each element in queryInput array
            {
                if (queryInput[i] == "INSERT" || queryInput[i] == "UPDATE" || queryInput[i] == "DELETE" || queryInput[i] == "CREATE"|| queryInput[i] == "DROP")     // If these commands exist in input array
                {
                    blockedWordExists = true;       // Set to true
                }
            }

            if (blockedWordExists == false)     // If blocked word doesnt exist
            {
                temp = temp.Replace(" ", "");       // Gets rid of empty spaces
                if (temp != "")     // If queryBox isnt empty
                {
                    try
                    {
                        string response = dbAccess.sqlQuery(queryBox.Text);      // Query database
                        txtResult.Text = response;      // Put response in txtResult text box
                    }
                    catch
                    {
                        // Do nothing
                    }
                }
            }
            else
            {
                Message("Database cannot be edited", "Error");        // Message if blockedWordExists == true
            }

            queryBox.Clear();       // Clear input box
        }

        void Message(string message, string title)      // Message function
        {
            MessageBox.Show(message, title);        // Shows message with 2 strings when function is called
        }

        private void confirmJobButton_Click(object sender, EventArgs e)
        {
            string maxJobID = dbAccess.sqlQuery("SELECT MAX(JobID) FROM Jobs");     // Gets highest JobID
            int maxJobIDint;

            try
            {
                maxJobIDint = Int32.Parse(maxJobID);     // Converts from string to integer
            }
            catch
            {
                maxJobIDint = 0;     // Sets ID to 0 if table is empty
            }

            int newJobID = maxJobIDint + 1;     // Adds 1 to highest JobID for next JobID

            string JobID = newJobID.ToString();     // Converts to string

            // Gets text from textboxes
            string startPos = startPosBox.Text;
            string endPos = endPosBox.Text;
            string weight = weightBox.Text;

            // Format start and end positions in correct coordinate form
            startPos = startPos.Replace(" ", "").Replace("(", "").Replace(")", "");
            string[] startPosSplit = startPos.Split(',');

            endPos = endPos.Replace(" ", "").Replace("(", "").Replace(")", "");
            string[] endPosSplit = endPos.Split(',');

            // Checks
            bool validInputs = false;
            bool validWeight = false;

            // Define variables
            string startPosX;
            string startPosY;

            string endPosX;           
            string endPosY;

            if (startPosSplit.Length == 2 && endPosSplit.Length == 2)       // Checks if start and end postions have 2 values
            {
                // Define variables
                startPosX = startPosSplit[0];
                startPosY = startPosSplit[1];

                endPosX = endPosSplit[0];
                endPosY = endPosSplit[1];

                try       // Attempts to convert to integer
                {
                    int SX = Int32.Parse(startPosX);
                    int SY = Int32.Parse(startPosY);
                    int EX = Int32.Parse(endPosX);
                    int EY = Int32.Parse(endPosY);

                    if ( SX >= 0 && SX <= 100 && SY >= 0 && SY <= 100 && EX >= 0 && EX <= 100 && EY >= 0 && EY <= 100)
                    {
                        validInputs = true;     // Confirms check if format is correct
                    }
                }
                catch 
                {
                    validInputs = false;        // Incorrect format
                }
            }

            if (validInputs == false)       // Displays error message if start and end positions are in wrong format
            {
                // Error message
                Message("Start and end positions must be coordinates in the form (x , y) with values between 0 and 100", "Error");
            }

            try        // Attempts to convert weight to integer
            {
                int weightInt = Int32.Parse(weight);
                if (weightInt <= 25 && weightInt >= 1)      // Checks if weight is between 1 and 25
                {
                    validWeight = true;     // Confirms conversion and in range
                }
            }
            catch 
            {
                validWeight = false;        // Wrong format or out of range
            }        
            
            if (validWeight == false)
            {
                // Error message
                Message("Weight must be an integer between 1 and 25", "Error");
            }

            if (validInputs == true && validWeight == true)     // If inputs are valid
            {
                XmlDocument Jobs = new XmlDocument();       // Create new XmlDocument called Jobs

                try
                {
                    Jobs.Load("Jobs.xml");      // Attempt to load existing document
                }
                catch
                {
                    Message("XML file doesn't exist or is corrupted", "Warning");       // Warning message

                    // Create and append root element
                    XmlElement rootElem = Jobs.CreateElement("Jobs");
                    Jobs.AppendChild(rootElem);

                    Message("New XML file created", "Success");     // Success message
                }

                XmlNode root = Jobs.DocumentElement;        // Sets root

                // Creates elements and appends to XML
                XmlElement elem = Jobs.CreateElement("start_location");
                elem.InnerText = startPos;
                root.AppendChild(elem);
                elem = Jobs.CreateElement("end_location");
                elem.InnerText = endPos;
                root.AppendChild(elem);
                elem = Jobs.CreateElement("weight");
                elem.InnerText = weight;
                root.AppendChild(elem);

                // Gets values from XML
                XmlNodeList slList = Jobs.GetElementsByTagName("start_location");
                XmlNodeList elList = Jobs.GetElementsByTagName("end_location");
                XmlNodeList wList = Jobs.GetElementsByTagName("weight");

                // Formats variables
                startPos = "("+startPos+")";
                endPos = "("+endPos+")";
                
                string jobToDB = dbAccess.sqlChange("INSERT INTO Jobs VALUES (" + JobID + ",'" + startPos + "','" + endPos + "','" + weight + "','0')");        // Inserts values into database

                // Save XML file
                Jobs.Save("Jobs.xml");

                // Complete message
                Message("Job added successfully", "Success");

                // Displays new job
                string newJob = dbAccess.sqlQuery("SELECT * FROM Jobs WHERE JobID = " + JobID);
                txtResult.Text = newJob;
            }

            // Clear textboxes
            startPosBox.Clear();
            endPosBox.Clear();
            weightBox.Clear();
        }

        private void viewIJButton_Click(object sender, EventArgs e)
        {
            string incompleteJobs = dbAccess.sqlQuery("SELECT * FROM Jobs WHERE Complete = '0'");       // Gets jobs that arent complete
            if (incompleteJobs == "")       // If no incomplete jobs exist
            {
                txtResult.Text = "No incomplete jobs exist";        // Displays txtResult text box
            }
            else
            {
                txtResult.Text = incompleteJobs;        // Displays jobs in txtResult text box
            }
        }

        private void viewRButton_Click(object sender, EventArgs e)
        {
            string robots = dbAccess.sqlQuery("SELECT * FROM RobotStatus");     // Gets robots
            txtResult.Text = robots;        // Displays robots in txtResult text box
        }

        private void assignJobButton_Click(object sender, EventArgs e)
        {
            // Checks
            bool validJI = false;
            bool validR = true;

            // Charge stations coordinates
            int Xc1 = 0;
            int Yc1 = 0;

            int Xc2 = 100;
            int Yc2 = 0;

            int Xc3 = 0;
            int Yc3 = 100;

            int Xc4 = 100;
            int Yc4 = 100;

            // Define variables
            string JobID = jobIDBox.Text;
            string robotName = robotNameBox.Text;
            int weight = 0;
            double batteryLevel = 0;
            double startTime = 0;
            int visitSt = 0;
            double prevDist = 0;
            int noOfJobs = 0;

            string incompleteJobIDs = dbAccess.sqlQuery("SELECT JobID from Jobs WHERE Complete = '0'");     // Gets highest JobID
            incompleteJobIDs = incompleteJobIDs.Replace("\t", "").Replace("\r", "");        // Formats

            string[] incompleteJobIDsArray = incompleteJobIDs.Split('\n');      // Split into array

            for (int i = 0; i < incompleteJobIDsArray.Length; i++)      // For each incomplete job ID
            {
                if (JobID == incompleteJobIDsArray[i] && JobID != "")       // Checks if input matches incomplete JobID
                {
                    validJI = true;     // Sets to true if matches
                }
            }

            if (validJI == false)       // Invalid JobID
            {
                // Error message
                Message("Please match job ID with an incomplete job in the table", "Error");
            }

            try         // Attempts to convert to correct format
            {
                robotName = robotName.Substring(0, 1).ToUpper() + robotName.Substring(1).ToLower();     // Capitalises the first character and makes the rest lower case
                robotName = robotName.Replace(" ", "");     // Removes spaces from robot name input
            }
            catch
            {
                validR = false;     // Invalid robot name
            }

            if (robotName != "Robot1" && robotName != "Robot2" && robotName != "Robot3" && robotName != "Robot4" && robotName != "Robot5")      // Checks if robot name is 1 of the 5 robots
            {
                // Error message
                Message("Please match robot name with a robot from the table", "Error");
                validR = false;     // Incorrect robot name input
            }

            // Define variables
            string robotPos = "(0,0)";
            string startPos = "(0,0)";
            string endPos = "(0,0)";

            if (validJI == true && validR == true)      // If inputs are valid
            {
                // Query from database using inputs
                weight = Int32.Parse((dbAccess.sqlQuery("SELECT Weight from Jobs WHERE JobID = " + JobID)));
                batteryLevel = double.Parse((dbAccess.sqlQuery("SELECT CBattLevel from RobotStatus WHERE RobotName = '" + robotName + "'")));
                robotPos = dbAccess.sqlQuery("SELECT Location from RobotStatus WHERE RobotName = '" + robotName + "'");
                startPos = dbAccess.sqlQuery("SELECT StartPos from Jobs WHERE JobID = " + JobID);
                endPos = dbAccess.sqlQuery("SELECT EndPos from Jobs WHERE JobID = " + JobID);
                try
                {
                    startTime = double.Parse((dbAccess.sqlQuery("SELECT EndTime FROM RobotHistory WHERE RobotName = '" + robotName + "' ORDER BY JobID DESC LIMIT 1")));        // Gets the end time of the jobs done by the robot and orders by JobID descending and limits to 1 job
                } 
                catch
                {
                    // Do nothing
                }
                prevDist = double.Parse((dbAccess.sqlQuery("SELECT TotalDist from RobotStatus WHERE RobotName = '" + robotName + "'")));
                noOfJobs = Int32.Parse((dbAccess.sqlQuery("SELECT NoOfJobs from RobotStatus WHERE RobotName = '" + robotName + "'")));
            }

            // Define variables
            double efficiency = 1 - ((weight / 5) * 0.05);      // Calculates efficiency based on weight
            double distanceWithoutCharge;
            double distanceWithoutChargeE;

            void UpdateDistance()
            {
                // Sets max distance robot can travel with and without weight
                distanceWithoutCharge = (batteryLevel / 100) * 1000;
                distanceWithoutChargeE = (batteryLevel / 100) * 1000 * efficiency;
            }

            UpdateDistance();       // Calls function

            // Formats coordinates
            robotPos = robotPos.Replace("(", "").Replace(")", "").Replace("\t", "").Replace("\r", "").Replace("\n", "");
            startPos = startPos.Replace("(", "").Replace(")", "").Replace("\t", "").Replace("\r", "").Replace("\n", "");
            endPos = endPos.Replace("(", "").Replace(")", "").Replace("\t", "").Replace("\r", "").Replace("\n", "");

            // Creates 3 separate arrays with coordinates for the robots current position, start and end position of the job
            string[] robotPosCO = robotPos.Split(',');     // [Xrp, Yrp]
            string[] startPosCO = startPos.Split(',');     // [Xsp, Ysp]
            string[] endPosCO = endPos.Split(',');     // [Xep, Yep]

            // Creates an array with all coordinates
            string[] coords = robotPosCO.Concat(startPosCO).Concat(endPosCO).ToArray();     // [Xrp, Yrp, Xsp, Ysp, Xep, Yep]

            // Define variables
            double distTravelled = 0;
            double endTime = 0;

            void Travel(double dist, bool carryingItem)
            {
                distTravelled = dist + distTravelled;       // Adds onto distance
                endTime = endTime + (dist / 5);     // Adds to time

                if (carryingItem == true)
                {
                    batteryLevel = batteryLevel - (efficiency * 0.1 * dist);        // Battery level lowers with efficiency if carrying item
                    UpdateDistance();
                }
                else
                {
                    batteryLevel = batteryLevel - (0.1 * dist);     // Battery level lowers without efficiency
                    UpdateDistance();
                }
            }

            void Charge()
            {
                double chargeTime = (180 * ((100 - batteryLevel) / 100));       // Charge time from 0 to 100% is 180 so takes into accoutn current battery level and calculates time to charge accordingly
                endTime = endTime + chargeTime;     // Adds charge time to end time.
                batteryLevel = 100;     // Battery level back to 100%
                visitSt = 1;        // Visited station so update from 0 to 1
            }

            // Calculates distance from robot position to start position and from start to end position
            double distToStart = calcDist(Int32.Parse(coords[2]), Int32.Parse(coords[0]), Int32.Parse(coords[3]), Int32.Parse(coords[1]));
            double distToEnd = calcDist(Int32.Parse(coords[4]), Int32.Parse(coords[2]), Int32.Parse(coords[5]), Int32.Parse(coords[3]));

            double distFromSt = 0;      // Distance from station defined

            double DistFromStation(int st, int X, int Y)
            {
                if (st == 1)
                {
                    distFromSt = calcDist(X, Xc1, Y, Yc1);     // [Xsp or Xep - Xc1] and [Ysp or Yep - Yc1]
                }
                else if (st == 2)
                {
                    distFromSt = calcDist(X, Xc2, Y, Yc1);     // [Xsp or Xep - Xc2] and [Ysp or Yep - Yc2]
                }
                else if (st == 3)
                {
                    distFromSt = calcDist(X, Xc3, Y, Yc1); ;     // [Xsp or Xep - Xc3] and [Ysp or Yep - Yc3]
                }
                else if (st == 4)
                {
                    distFromSt = calcDist(X, Xc4, Y, Yc1);    // [Xsp or Xep - Xc4] and [Ysp or Yep - Yc4]
                }

                return distFromSt;
            }

            if (distanceWithoutCharge < 150)        // If it can't travel 150 m without needing to charge
            {
                // Calculates distance to each charging station
                double distToC1 = calcDist(Xc1, Int32.Parse(coords[0]), Yc1, Int32.Parse(coords[1]));
                double distToC2 = calcDist(Xc2, Int32.Parse(coords[0]), Yc2, Int32.Parse(coords[1]));
                double distToC3 = calcDist(Xc3, Int32.Parse(coords[0]), Yc3, Int32.Parse(coords[1]));
                double distToC4 = calcDist(Xc4, Int32.Parse(coords[0]), Yc4, Int32.Parse(coords[1]));

                List<double> distToC = new List<double> {distToC1, distToC2, distToC3, distToC4};     // Initialize list
                double shortestDistance = distToC.Min();        // Distance to closes charging station

                int station = 0;        // Define station

                for (int i = 0; i < distToC.Count; i++)     // Loop for each station
                {
                    if (distToC[i] == shortestDistance)     // Checks which stations match distance
                    {
                        station = i + 1;        // Sets to corresponding station
                    }
                }

                Travel(shortestDistance, false);     // Travels to closest charging station
                Charge();

                distFromSt = DistFromStation(station, Int32.Parse(coords[2]), Int32.Parse(coords[3]));        // Distance from charging station to start position

                Travel(distFromSt, false);     // Travels to start position
            } 
            else
            {
                Travel(distToStart, false);     // Travels to start position
            }

            if (distanceWithoutChargeE < 150)        // If it can't travel 150 m without needing to charge carrying the weight
            {
                // Calculates distance to each charging station
                double distToC1 = calcDist(Int32.Parse(coords[2]), Xc1, Int32.Parse(coords[3]), Yc1);
                double distToC2 = calcDist(Int32.Parse(coords[2]), Xc2, Int32.Parse(coords[3]), Yc2);
                double distToC3 = calcDist(Int32.Parse(coords[2]), Xc3, Int32.Parse(coords[3]), Yc3);
                double distToC4 = calcDist(Int32.Parse(coords[2]), Xc4, Int32.Parse(coords[3]), Yc4);

                List<double> distToC = new List<double> {distToC1, distToC2, distToC3, distToC4};     // Initialize list
                double shortestDistance = distToC.Min();        // Distance to closes charging station

                int station = 0;        // Define station

                for (int i = 0; i < distToC.Count; i++)     // Loop for each station
                {
                    if (distToC[i] == shortestDistance)     // Checks which stations match distance
                    {
                        station = i + 1;        // Sets to corresponding station
                    }
                }

                Travel(shortestDistance, true);     // Travels to closest charging station
                Charge();

                distFromSt = DistFromStation(station, Int32.Parse(coords[4]), Int32.Parse(coords[5]));        // Distance from charging station to end position

                Travel(distFromSt, true);     // Travels to end position
            }
            else
            {
                Travel(distToEnd, true);     // Travels to end position
            }

            if (validR == true && validJI)      // If inputs are valid
            {
                // Get variables ready to update tables
                endTime = startTime + endTime;
                double totalDist = prevDist + distTravelled;
                noOfJobs = noOfJobs + 1;
                string location = ("(" + coords[4] + "," + coords[5] + ")");

                string updateRobotHistory = dbAccess.sqlChange("INSERT INTO RobotHistory VALUES (" + JobID + ",\"" + robotName + "\",\"" + (startTime.ToString("F2")) + "\",\"" + (endTime.ToString("F2")) + "\",\"" + (batteryLevel.ToString("F2")) + "\",\"" + visitSt + "\")");
                string updateRobotStatus = dbAccess.sqlChange("UPDATE RobotStatus SET CBattLevel = " + (batteryLevel.ToString("F2")) + ", TotalDist = " + (totalDist.ToString("F2")) + ", NoOfJobs = " + noOfJobs + ", Location = '" + location + "' WHERE RobotName = '" + robotName + "'");
                string updateJob = dbAccess.sqlChange("UPDATE Jobs SET Complete = 1 WHERE JobID = " + JobID);
                Message("Job assigned successfully", "Success");       // Complete message

                // Display robot that completed job
                txtResult.Text = dbAccess.sqlQuery("SELECT * FROM RobotStatus WHERE RobotName = '" + robotName + "'"); ;

                // Clear textboxes
                jobIDBox.Clear();
                robotNameBox.Clear();
            }
        }

        double calcDist(int x1, int x2, int y1, int y2)
        {
            int X = x1 - x2;     // Difference in x coordinates
            int Y = y1 - y2;     // Difference in y coordinates
            double dist = Math.Sqrt(X * X + Y * Y);     // Distance between points

            return dist;
        }

        private void updateStatsButton_Click(object sender, EventArgs e)
        {
            // Define variables
            string furthestTravelled = "";
            string mostCompleted = "";

            string distDB = dbAccess.sqlQuery("SELECT RobotName, TotalDist from RobotStatus");     // Orders the robots by distance travelled
            string noOfJobsDB= dbAccess.sqlQuery("SELECT RobotName, NoOfJobs from RobotStatus");     // Orders the robots by number of jobs
            goalPositionsBox.Text = dbAccess.sqlQuery("SELECT RobotName, Location FROM RobotStatus");        // Gets robots names and goal locations and outputs to textbox

            // Format and put "," between them
            distDB = distDB.Replace("(", "").Replace(")", "").Replace("\t", ",").Replace("\r", "").Replace("\n", "");
            noOfJobsDB = noOfJobsDB.Replace("(", "").Replace(")", "").Replace("\t", ",").Replace("\r", "").Replace("\n", "");

            // Split into arrays
            string[] furthestTravelledArray = distDB.Split(',');
            string[] mostCompletedArray = noOfJobsDB.Split(',');

            // Define variables
            double R1 = double.Parse(furthestTravelledArray[1]);
            double R2 = double.Parse(furthestTravelledArray[3]);
            double R3 = double.Parse(furthestTravelledArray[5]);
            double R4 = double.Parse(furthestTravelledArray[7]);
            double R5 = double.Parse(furthestTravelledArray[9]);

            double furthestTravelledMax = Math.Max(R1, Math.Max(R2, Math.Max(R3, Math.Max(R4, R5))));       // Highest value

            void checkFurthestTravelled(double dist, string robot)        // Checks which robots are closes
            {
                if (furthestTravelledMax == dist)       // If biggest value matches the distance robot travelled
                {
                    if (furthestTravelled == "")
                    {
                        furthestTravelled = robot;     // Add to output
                    }
                    else
                    {
                        furthestTravelled = furthestTravelled + ", " + robot;     // If matches more than 1 robot, add "," before to display correctly
                    }
                }
            }

            // Calls function for each robot
            checkFurthestTravelled(R1, "Robot1");
            checkFurthestTravelled(R2, "Robot2");
            checkFurthestTravelled(R3, "Robot3");
            checkFurthestTravelled(R4, "Robot4");
            checkFurthestTravelled(R5, "Robot5");

            // Define variables
            R1 = double.Parse(mostCompletedArray[1]);
            R2 = double.Parse(mostCompletedArray[3]);
            R3 = double.Parse(mostCompletedArray[5]);
            R4 = double.Parse(mostCompletedArray[7]);
            R5 = double.Parse(mostCompletedArray[9]);

            double mostCompletedMax = Math.Max(R1, Math.Max(R2, Math.Max(R3, Math.Max(R4, R5))));       // Highest value

            void checkMostCompleted(double noOfJobs, string robot)        // Checks which robots are closes
            {
                if (mostCompletedMax == noOfJobs)       // If biggest value matches the distance robot travelled
                {
                    if (mostCompleted == "")
                    {
                        mostCompleted = robot;     // Add to output
                    }
                    else
                    {
                        mostCompleted = mostCompleted + ", " + robot;     // If matches more than 1 robot, add "," before to display correctly
                    }
                }
            }

            // Calls function for each robot
            checkMostCompleted(R1, "Robot1");
            checkMostCompleted(R2, "Robot2");
            checkMostCompleted(R3, "Robot3");
            checkMostCompleted(R4, "Robot4");
            checkMostCompleted(R5, "Robot5");

            // Set textbox output
            furthestTravelledBox.Text = furthestTravelled;
            mostCompletedBox.Text = mostCompleted;
        }

        private void checkDistanceFromR_Click(object sender, EventArgs e)
        {
            bool validR = true;     // Check robot names

            // Input from textboxes
            string robot1name = robot1Box.Text;
            string robot2name = robot2Box.Text;

            try
            {
                robot1name = robot1name.Substring(0, 1).ToUpper() + robot1name.Substring(1).ToLower();     // Capitalises the first character and makes the rest lower case
                robot1name = robot1name.Replace(" ", "");     // Removes spaces from robot name input

                robot2name = robot2name.Substring(0, 1).ToUpper() + robot2name.Substring(1).ToLower();     // Capitalises the first character and makes the rest lower case
                robot2name = robot2name.Replace(" ", "");     // Removes spaces from robot name input
            }
            catch
            {
                validR = false;     // Invalid robot name
            }

            if (robot1name != "Robot1" && robot1name != "Robot2" && robot1name != "Robot3" && robot1name != "Robot4" && robot1name != "Robot5")      // Checks if robot name is 1 of the 5 robots
            {
                validR = false;     // Incorrect robot name input
            }

            if (robot2name != "Robot1" && robot2name != "Robot2" && robot2name != "Robot3" && robot2name != "Robot4" && robot2name != "Robot5")      // Checks if robot name is 1 of the 5 robots
            {
                validR = false;     // Incorrect robot name input
            }

            if (validR == true)
            {
                // Query database
                string robot1Location = dbAccess.sqlQuery("SELECT Location from RobotStatus WHERE RobotName = '" + robot1name + "'");
                string robot2Location = dbAccess.sqlQuery("SELECT Location from RobotStatus WHERE RobotName = '" + robot2name + "'");

                // Format
                robot1Location = robot1Location.Replace("(", "").Replace(")", "").Replace("\t", "").Replace("\r", "").Replace("\n", "");
                robot2Location = robot2Location.Replace("(", "").Replace(")", "").Replace("\t", "").Replace("\r", "").Replace("\n", "");

                // Creates 2 separate arrays with coordinates for the robots current position
                string[] robot1Coords = robot1Location.Split(',');     // [Xrp1, Yrp1]
                string[] robot2Coords = robot2Location.Split(',');     // [Xrp2, Yrp2]

                double dist = calcDist(Int32.Parse(robot1Coords[0]), Int32.Parse(robot2Coords[0]), Int32.Parse(robot1Coords[1]), Int32.Parse(robot2Coords[1]));     // Calculates distance
                distanceBetweenRobotsBox.Text = (dist.ToString("F2")) + " m";       // Displays distance in textbox
            } 
            else
            {
                // Error message
                Message("Please match robot names with robots from the table", "Error");
            }
        }

        private void checkClosestButton_Click(object sender, EventArgs e)
        {
            // Define variables
            bool validInputs = false;
            string coords = coordsBox.Text;
            int X;
            int Y;

            coords = coords.Replace("(", "").Replace(")", "").Replace("\t", "").Replace("\r", "").Replace("\n", "");        // Remove unnecessary characters
            string[] inputCoords = coords.Split(',');     // [X1,Y1]

            if (inputCoords.Length == 2)       // Checks if start and end postions have 2 values
            {
                try       // Attempts to convert to integer
                {
                    X = Int32.Parse(inputCoords[0]);
                    Y = Int32.Parse(inputCoords[1]);

                    if (X >= 0 && X <= 100 && Y >= 0 && Y <= 100)
                    {
                        validInputs = true;     // Confirms check if format is correct
                    }
                }
                catch
                {
                    validInputs = false;        // Incorrect format
                }
            }

            if (validInputs == false)       // Invalid inputs
            {
                // Error message
                Message("Coordinates in the form (x , y) with values between 0 and 100", "Error");
            }

            if (validInputs == true)        // Valid inputs
            {
                string getRobotLocations = dbAccess.sqlQuery("SELECT Location from RobotStatus");       // Get location from database

                getRobotLocations = getRobotLocations.Replace("(", "").Replace(")", ",").Replace("\t", "").Replace("\r", "").Replace("\n", "");     // Format correctly
                string[] robotLocations = getRobotLocations.Split(',');     // [Xrp1, Yrp1, Xrp2, Yrp2, Xrp3, Yrp3, Xrp4, Yrp4, Xrp5, Yrp5]

                double distFromR1 = calcDist(Int32.Parse(inputCoords[0]), Int32.Parse(robotLocations[0]), Int32.Parse(inputCoords[1]), Int32.Parse(robotLocations[1]));     // [X1, Y1] - [Xrp1 - Yrp1]
                double distFromR2 = calcDist(Int32.Parse(inputCoords[0]), Int32.Parse(robotLocations[2]), Int32.Parse(inputCoords[1]), Int32.Parse(robotLocations[3]));     // [X1, Y1] - [Xrp2 - Yrp2]
                double distFromR3 = calcDist(Int32.Parse(inputCoords[0]), Int32.Parse(robotLocations[4]), Int32.Parse(inputCoords[1]), Int32.Parse(robotLocations[5]));     // [X1, Y1] - [Xrp3 - Yrp3]
                double distFromR4 = calcDist(Int32.Parse(inputCoords[0]), Int32.Parse(robotLocations[6]), Int32.Parse(inputCoords[1]), Int32.Parse(robotLocations[7]));     // [X1, Y1] - [Xrp4 - Yrp4]
                double distFromR5 = calcDist(Int32.Parse(inputCoords[0]), Int32.Parse(robotLocations[8]), Int32.Parse(inputCoords[1]), Int32.Parse(robotLocations[9]));     // [X1, Y1] - [Xrp5 - Yrp5]

                double smallest = Math.Min(distFromR1, Math.Min(distFromR2, Math.Min(distFromR3, Math.Min(distFromR4, distFromR5))));       // Gets closest distance

                string output = "";     // Define output

                void checkClosest(double dist, string robot)        // Checks which robots are closest
                {
                    if (smallest == dist)       // If smallest value matches the distance from robot
                    {
                        if (output == "")
                        {
                            output = robot;     // Add to output
                        }
                        else
                        {
                            output = output + ", " + robot;     // If matches more than 1 robot, add "," before to display correctly
                        }
                    }
                }

                // Calls function for each robot
                checkClosest(distFromR1, "Robot1");
                checkClosest(distFromR2, "Robot2");
                checkClosest(distFromR3, "Robot3");
                checkClosest(distFromR4, "Robot4");
                checkClosest(distFromR5, "Robot5");

                closestRobotBox.Text = output;      // Displays closest robot(s)
            }
        }
    }
}
