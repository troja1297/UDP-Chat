using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace UDPChat
{
    class Program
    {
        private static IPAddress remoteIPAddress;
        private static string senderIp;
        private static int senderPort = -1;
        private static int localPort;

        [STAThread]
        static void Main(string[] args)
        {
            Random random = new Random();
            while (true)
            {
                try
                {
                    Console.WriteLine("Укажите локальный порт");
                    localPort = random.Next(49152, 65535);
                    Console.WriteLine(localPort);
                    Thread recieverThread = new Thread(Receiver);
                    recieverThread.Start();
                
                    Sender();
                
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Возникло исключение: " + ex.ToString() + "\n  " + ex.Message);
                }    
            }
        }

        public static string GetIpFromString(string message)
        {
            try
            {
                for (int i = 0; i < message.Length; i++)
                {
                    if (message[i] == ':')
                    {
                        return message.Substring(0, i);
                    }
                }
                
                throw new ArgumentException("IP введен не правильно");
            }
            catch (Exception e)
            {
                return "empty";
            }
        }
        
        public static int GetPortFromString(string message)
        {
            try
            {
                int a = 0;
                int b = 0;
                for (int i = 0; i < message.Length; i++)
                {
                    if (message[i] == ':')
                    {
                        a = i + 1;
                    }

                    if (message[i] == '>')
                    {
                        b = i - a;
                    }
                }
                
                if (int.TryParse(message.Substring(a, b), out int port))
                {
                    return port;
                }
                else
                {
                    throw new ArgumentException("Порт введен не правильно");
                }
            }
            catch (Exception e)
            {
                return -2;
            }
        }

        private static void Sender()
        {
            string substring = "";
            while (true)
            {
                
                string datagram = Console.ReadLine();
                IPEndPoint endPoint = null;
                
                UdpClient sender = new UdpClient();
                if (senderPort == -1 || senderIp == null) {
                    senderPort = GetPortFromString(datagram);
                    senderIp = GetIpFromString(datagram);
                    endPoint = new IPEndPoint(IPAddress.Parse(senderIp), senderPort);
                    substring = $"{senderIp}:{senderPort}>";
                }
                else
                {
                    if (GetIpFromString(datagram) == "empty" || GetPortFromString(datagram) == -2)
                    {
                        string buf = datagram;
                        datagram = substring + buf;
                        endPoint = new IPEndPoint(IPAddress.Parse(senderIp), senderPort);
                    }
                    else
                    {
                        senderPort = GetPortFromString(datagram);
                        senderIp = GetIpFromString(datagram);
                        endPoint = new IPEndPoint(IPAddress.Parse(senderIp), senderPort);
                        
                    }
                }
                
                try
                {
                    
                    byte[] bytes = Encoding.UTF8.GetBytes(datagram);
                    sender.Send(bytes, bytes.Length, endPoint);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Возникло исключение: " + ex.ToString() + "\n  " + ex.Message);
                }
                finally
                {
                    sender.Close();
                }

                
            }
        }
        
        

        public static void Receiver()
        {
            // Создаем UdpClient для чтения входящих данных
            UdpClient receivingUdpClient = new UdpClient(localPort);

            IPEndPoint RemoteIpEndPoint = null;

            try
            {
                while (true)
                {
                    byte[] receiveBytes = receivingUdpClient.Receive(ref RemoteIpEndPoint);

                    string returnData = Encoding.UTF8.GetString(receiveBytes);
                    Console.WriteLine(" >>> " + returnData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Возникло исключение: " + ex.ToString() + "\n  " + ex.Message);
            }
        }
    }

}