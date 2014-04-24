using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Ports;
using System.Diagnostics;
using System.Threading;

namespace SerialComm
{
    public class serialComm
    {
        private SerialPort comPort;

        private byte[] rx;              //input buffer.
        private bool did_rx;            //flag to determine if DataRecieved event occurred.
        //private int retry;              //can be used to keep track of retries.
        private int bytes_recieved;     //how many bytes have been read into the buffer.

        public serialComm()
        {
            rx = new byte[4];
            did_rx = false;
            //retry = 0;
            bytes_recieved = 0;
        }

        public void start()             //should only be used to if program is run in a console. for headless operation call startSerial directly.
        {
            Console.WriteLine("Welcome to Serial Comm!");

            while (true)
            {
                if (getCommand("Please enter a command:"))
                {
                    Console.WriteLine("Command failed, please try again.");
                }
            }
        }

        private bool runCommand(string com)
        {
            switch (com)
            {
                case "Help": case "help":
                    Console.WriteLine("Help Dialog");       //is it even worth adding at this point?
                    return false;
                case "Exit": case "exit":
                    Console.WriteLine("Closing ports...");
                    if (!closeSerial())                         //make sure to close ports before program exits.
                    {
                        Console.WriteLine("Goodbye");
                        Environment.Exit(0);
                    }
                    else
                    {
                        Console.WriteLine("Could not close serial port!");  //should not be an issue. pray that it never is.
                        return true;
                    }
                    break;
                case "start":
                    if (openPortDialog()) return true;
                    return false;
                case "test":                                                //test wether the connection works.
                    Console.WriteLine("Sending test message...");
                    //if (testSerial()) return true;
                    try
                    {
                        testSerial();
                    }
                    catch (TimeoutException)
                    {
                        Console.WriteLine("Read operation timed out, please check device status!");
                        return true;
                    }
                    catch (InvalidDataException)
                    {
                        Console.WriteLine("Wrong data recieved!");
                        return true;
                    }
                    return false;
                case "test data":                                           //test whether program can catch icorrect data.
                    Console.WriteLine("Sending test message...");
                    //if ( testSerialWrongData() ) return true;
                    try
                    {
                        testSerialWrongData();
                    }
                    catch (TimeoutException)
                    {
                        Console.WriteLine("Read operation timed out, please check device status!");
                        return true;
                    }
                    catch (InvalidDataException)
                    {
                        Console.WriteLine("Wrong data recieved!");
                        return true;
                    }
                    return false;
                case "test timeout":                                        //test whether the timeout works.
                    Console.WriteLine("Sending test message...");
                    //if (testSerialTimeOut()) return true;
                    try
                    {
                        testSerialTimeOut();
                    }
                    catch (TimeoutException)
                    {
                        Console.WriteLine("Read operation timed out, please check device status!");
                        return true;
                    }
                    catch (InvalidDataException)
                    {
                        Console.WriteLine("Wrong data recieved!");
                        return true;
                    }
                    return false;
                case "update player":
                    Console.WriteLine("Which player? [0-5]");               //choose which player to update.
                    string player_string = Console.ReadLine();
                    uint player = Convert.ToUInt32(player_string);

                    if (player > 5)
                    {
                        Console.WriteLine("Not a valid player!");
                        return true;
                    }

                    Console.WriteLine("What position are they in? [0-15]");        //input where are they moving to.
                    string pos_string = Console.ReadLine();
                    uint pos = Convert.ToUInt32(pos_string);

                    if (pos > 16)
                    {
                        Console.WriteLine("Not a valid position!");
                        return true;
                    }

                    Console.WriteLine("Updating player position...");
                    try
                    {
                        requestPlayerUpdate((byte)player, (byte)pos);
                    }
                    catch (TimeoutException)
                    {
                        Console.WriteLine("Read operation timed out, please check device status!");
                        return true;
                    }
                    catch (InvalidDataException)
                    {
                        Console.WriteLine("Recieved incorrect data!");
                        foreach (byte b in rx)
                        {
                            Console.WriteLine(b);
                        }
                        Array.Clear(rx, 0, 3);
                        return true;
                    }

                    return false;
                case "request card":
                    Console.WriteLine("Which player is playing? [0-5]");
                    string player_string_3 = Console.ReadLine();
                    byte player_3 = Convert.ToByte(player_string_3);
                    try
                    {
                        Console.Write("Card id: ");
                        Console.WriteLine(requestRFID(player_3));
                    }
                    catch (TimeoutException)
                    {
                        Console.WriteLine("Operation timed out!");
                        return true;
                    }
                    return false;
                case "request choice":
                    Console.WriteLine("Which player is chosing? [0-5]");
                    string player_string_2 = Console.ReadLine();
                    byte player_2 = Convert.ToByte(player_string_2);
                    try
                    {
                        Console.Write("Choice: ");
                        Console.WriteLine(requestPlayerChoice(player_2));
                    }
                    catch (InvalidDataException)
                    {
                        Console.WriteLine("Invalid data recieved!");
                        return true;
                    }
                    catch (TimeoutException)
                    {
                        Console.WriteLine("Operation timed out!");
                        return true;
                    }
                    return false;
                case "request move":
                    Console.WriteLine("Which player is moving? [0-5]");
                    string player_string_1 = Console.ReadLine();
                    byte player_1 = Convert.ToByte(player_string_1);
                    Console.WriteLine("How many spaces can they move?");
                    string spaces_string = Console.ReadLine();
                    byte spaces = Convert.ToByte(spaces_string);

                    try
                    {
                        Console.Write("Space: ");
                        Console.WriteLine(requestPlayerMove(player_1, spaces));
                    }
                    catch (InvalidDataException)
                    {
                        Console.WriteLine("Invalid data recieved!");
                        return true;
                    }
                    catch (TimeoutException)
                    {
                        Console.WriteLine("Operation timed out!");
                        return true;
                    }
                    return false;
                default:
                    Console.WriteLine("Not a valid command!");
                    return false;
            }

            return true;
        }

        public bool openPortDialog()
        {
            string[] ports = SerialPort.GetPortNames();
				if (ports.Length == 0) {
					Console.WriteLine("No available ports");
					return true;
				}
            Console.WriteLine("Avilable ports:");


            int i = 0;
            foreach (string port in ports)
            {
                Console.Write(i + " : ");
                i++;
                Console.WriteLine(port);
            }

            Console.WriteLine("Please choose COM port.");

            int portnum=0;

            try
            {
                string portnumstring = Console.ReadLine();
                portnum = Convert.ToInt32(portnumstring);
            }
            catch (FormatException)
            {
                Console.WriteLine ("That is not a valid port!");
                return true;
            }

            if (ports.Length < portnum)
            {
                Console.WriteLine("That is not a valid port!");
                return true;
            }
            Console.WriteLine("Opening port...");
            startSerial(ports[portnum]);
            return false;
        }

        private bool getCommand(string mes)
        {
            Console.WriteLine(mes);
            string input;
            input = Console.ReadLine();
            if (runCommand(input))
            {
                //Console.WriteLine("Invalid Command");
                return true;
            }
            return false;
        }

        public bool startSerial(string port, int baud = 9600, 
            Parity parity = Parity.None, 
            int dataBits = 8, StopBits stopBits = StopBits.One)     //opens serial port.
        {
            if (comPort != null && comPort.IsOpen)
            {
                Console.WriteLine("Port is already open!");
                return true;
            }

            comPort = new SerialPort(port, baud, parity, dataBits, stopBits);

            comPort.DataReceived += new SerialDataReceivedEventHandler(DataRecievedHandler);

            try
            {
                comPort.Open();
            }
            catch (UnauthorizedAccessException)
            {
                return true;
            }
            catch (IOException)
            {
                return true;
            }
            //comPort.ReadTimeout(4000);

            if (comPort.IsOpen)
            {
                comPort.DiscardInBuffer();
                return false;
            }
            else Console.WriteLine("Could not open port!");
            return true;
        }

        public bool closeSerial()                                   //closes serial port. MAKE SURE THIS IS RUN BEFORE EXITING THE PROGRAM!!!!!!!
        {
            if (comPort != null)    //check to see if comPort object was initialized. Otherwise, exiting the program will throw an exception.
            {
                if (comPort.IsOpen)
                {
                    comPort.Close();
                    Console.WriteLine("Port closed.");
                    return false;
                }
                else return true;
            }
            else return false;
        }

        private void DataRecievedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            int n = sp.BytesToRead;
            byte[] buffer = new byte[n];
            
            sp.Read(buffer, 0, n);

            getFromBuffer(ref buffer);                  //copies to rx array

            bytes_recieved += n;                        //indicate how many bytes have been read.
            did_rx = true;
            //Console.WriteLine("Data recieved");
        }

        private bool sendSerial(ref Byte[] tx)
        {
            if (comPort.IsOpen)
            {
                comPort.Write(tx, 0, tx.Length);
            }
            else
            {
                Console.WriteLine("Must open port first!");
                return true;
            }
            return false;
        }

        private bool readSerial(int n)                              //n represents how many bytes are expected.
        {
            TimeSpan to = TimeSpan.FromMinutes(1);      //timer used to set timeout in case device is disconnected or nothing is returned.
            Stopwatch toClock = Stopwatch.StartNew();   //because the protocol is call-response, something should always be returned.

            bytes_recieved = 0;                         //ensure that recieved counter is 0 (maybe in case a different operation failed?)
            bool done = true;                           //used to exit the while loop

            while (toClock.Elapsed < to && done)        //exits once the timeout has elapsed or all the data is recieved.
            {
                if (did_rx)
                {
                    //Console.WriteLine("Success!");
                    did_rx = false;
                    if (bytes_recieved == n)
                    {
                        bytes_recieved = 0;
                        done = false;
                    }
                    else if (bytes_recieved < n)
                    {
                        did_rx = false;
                    }
                    else if (bytes_recieved > n)
                    {
                        return true;
                    }
                }
            }
            toClock.Stop();
            if (toClock.Elapsed >= to) throw new TimeoutException();    //throw timeout exception if timeout occurs.
            else return false;
        }
        private void getFromBuffer(ref byte[] buffer)
        {
            Array.Copy(buffer, 0, rx, bytes_recieved, buffer.Length);
        }

        public bool testSerial()                                    //test whether the serial connection works. should succeed.
        {
            byte[] tx = new byte[1];    //sending a 0x01 is a test signal. should recieve 0x01 (ack) back. 
            tx[0] = 1;
            sendSerial(ref tx);

            try
            {
                readSerial(1);
            }
            catch (TimeoutException)
            {
                throw new TimeoutException();
                //Console.WriteLine("Read operation timed out, please check device status!");
                //return true;
            }

            if (rx[0] != 1)
            {
                throw new InvalidDataException();
                //Console.WriteLine("Wrong data recieved!");
                //return true;
            }

            Array.Clear(rx, 0, 3);          //clear recieve array for next operation.
            Console.WriteLine("Correct data recieved!");
            return false;
        }

        public bool testSerialTimeOut()                             //test whether the timeout works. device will not send any data back. Should throw timeout exception.
        {
            byte[] tx = new byte[1];    //sending a 0x01 is a test signal. should recieve 0x01 (ack) back. 
            tx[0] = 2;
            sendSerial(ref tx);

            try
            {
                readSerial(1);
            }
            catch (TimeoutException)
            {
                throw new TimeoutException();
                //Console.WriteLine("Read operation timed out, please check device status!");
                //return true;
            }

            if (rx[0] != 1)
            {
                Console.WriteLine("Wrong data recieved!");
                return true;
            }

            Console.WriteLine("Correct data recieved!");
            Array.Clear(rx, 0, 3);
            return false;
        }

        public bool testSerialWrongData()                           //tests whether program can interpret incorrect data. should fail.
        {
            byte[] tx = new byte[1];    //sending a 0x01 is a test signal. should recieve 0x01 (ack) back. 
            tx[0] = 3;
            sendSerial(ref tx);

            try
            {
                readSerial(1);
            }
            catch (TimeoutException)
            {
                throw new TimeoutException();
                //Console.WriteLine("Read operation timed out, please check device status!");
                //return true;
            }

            if (rx[0] != 1)
            {
                Console.WriteLine("Wrong data recieved!");
                return true;
            }

            Console.WriteLine("Correct data recieved!");
            Array.Clear(rx, 0, 3);
            return false;
        }

        public bool requestPlayerUpdate(byte player, byte pos)      //updates the player position. input the player number and where his location is.
        {
            if (pos > 0x0F || player > 6) 
            {
                throw new ArgumentOutOfRangeException();    //argument has to be less than or equal to 16 as that is how many spaces there are.
            }

            byte[] tx = new byte[3];
            tx[0] = 0x10;
            tx[1] = player;
            tx[2] = pos;
            sendSerial(ref tx);                                  //transmit command byte.

            try
            {
                readSerial(3);                               //wait for confirmation.
            }
            catch (TimeoutException)
            {
                throw new TimeoutException();
                //Console.WriteLine("Read operation timed out, please check device status!");
                //return true;
            }

            if (rx[0] != 1 || rx[1] != player || rx[2] != pos)  //validate correct data was recieved.
            {
                throw new InvalidDataException();
            }

            //Console.WriteLine("Success!");
            Array.Clear(rx, 0, 3);                          //clear array after it has been used.
            return false;
        }

        public string requestRFID(byte player)                                 //requests an RFID tag from the device. returns a hex formatted string.
        {
		  return "A2FE3F7";
            byte[] tx = new byte[2];
            tx[0] = 0x20;
            tx[1] = player;

            sendSerial(ref tx);

            try
            {
                readSerial(4);
            }
            catch (TimeoutException)
            {
                throw new TimeoutException();
            }

            string id = BitConverter.ToString(rx);
            Array.Clear(rx, 0, 4);
            return id.Replace("-", "");
        }

        public int requestPlayerChoice(byte player)                            //request player to make a choice. returns a value 1 - 6.
        {
            byte[] tx = new byte[2];
            tx[0] = 0x30;
            tx[1] = player;
            sendSerial(ref tx);

            try
            {
                readSerial(2);
            }
            catch (TimeoutException)
            {
                throw new TimeoutException();
            }

            if (rx[0] != 1 || rx[1] > 6 || rx[1]==0)
            {
                throw new InvalidDataException();
            }

            int c = rx[1];
            Array.Clear(rx, 0, 2);                          //clear recieve array for next operation.
            return c;
        }

        public int requestPlayerMove(byte player, byte spaces)                   //request player move. input how many spaces they can move. returns they're position.
        {
            byte[] tx = new byte[3];
            tx[0] = 0x40;
            tx[1] = player;
            tx[2] = spaces;
            sendSerial(ref tx);

            try
            {
                readSerial(2);
            }
            catch (TimeoutException)
            {
                throw new TimeoutException();
            }

            if (rx[0]!=1 || rx[1]>16) {
                throw new InvalidDataException();
            }

            int c = rx[1];
            Array.Clear(rx, 0, 2);
            return c;
        }
    }
}
