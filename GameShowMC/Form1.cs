using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameShowMC
{
    public partial class Form1 : Form
    {
        public int soCauTraLoiDung = 0;
        int indexQuestion = 0;

        public Form1()
        {
            InitializeComponent();

            //create server
            //TcpListener ServerSocket = new TcpListener(IPAddress.Any, 6000);
            //ServerSocket.Start();

            //TcpClient client = ServerSocket.AcceptTcpClient();

            FormClosing += Form1_FormClosing;
        }

        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_ns != null)
            {
                _ns.Close();
            }
            if (_client != null)
            {
                _client.Close();
            }

            if (ServerSocket != null)
            {
                ServerSocket.Stop();
            }

        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(ConnectServer);
            thread.Start();
        }

        TcpClient _client = null;
        Thread _thread = null;
        NetworkStream _ns = null;
        TcpListener ServerSocket = null;
        void ConnectServer()
        {
            //create server
            ServerSocket = new TcpListener(IPAddress.Any, 6000);
            ServerSocket.Start();

            _client = ServerSocket.AcceptTcpClient();
            MessageBox.Show("Player connected!!");

            //IPAddress ip = IPAddress.Parse("127.0.0.1");
            //int port = 5000;
            //_client = new TcpClient();
            //_client.Connect(ip, port);

            //Console.WriteLine("client connected!!");
            //_ns = _client.GetStream();
            //_thread = new Thread(o => ReceiveData((TcpClient)o));
            //_thread.Start(_client);

            //string s;
            //while (!string.IsNullOrEmpty((s = Console.ReadLine())))
            {
                //byte[] buffer = Encoding.ASCII.GetBytes(s);
                //ns.Write(buffer, 0, buffer.Length);
            }

            //client.Client.Shutdown(SocketShutdown.Send);
            //thread.Join();
            //ns.Close();
            //client.Close();
        }
        static void ReceiveData(TcpClient client)
        {
            NetworkStream ns = client.GetStream();
            byte[] receivedBytes = new byte[1024];
            int byte_count;

            while ((byte_count = ns.Read(receivedBytes, 0, receivedBytes.Length)) > 0)
            {
                Console.Write(Encoding.ASCII.GetString(receivedBytes, 0, byte_count));
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string question = rtbQuestion.Text;
            string a = txtA.Text;
            string b = txtB.Text;
            string c = txtC.Text;
            string d = txtD.Text;

            string data = string.Format("{0}@@{1}@@{2}@@{3}@@{4}"
                , question, a, b, c, d);
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            _ns = _client.GetStream();
            _ns.Write(buffer, 0, buffer.Length);

            ReceiverAnswer();
        }

        private void ReceiverAnswer()
        {
            NetworkStream ns = _client.GetStream();
            byte[] receivedBytes = new byte[1024];
            int byte_count;

            while ((byte_count = ns.Read(receivedBytes, 0, receivedBytes.Length)) > 0)
            {
                string data = Encoding.ASCII.GetString(receivedBytes, 0, byte_count);

                // kiem tra tra loi dung
                Question question = _lstQuestions[indexQuestion - 1];

                string correctAnswer = "";

                foreach (Answer ans in question.ListAnswers)
                {
                    if (ans.Id == question.CorrectAnswerId)
                    {
                        correctAnswer = ans.Content;
                        break;
                    }
                }

                if (correctAnswer == data)
                {
                    soCauTraLoiDung++;
                    MessageBox.Show("Chuc mung, ban da tra loi dung");
                    break;
                }
                else
                {
                    string tmp = "Rat tiec, ban da tra loi sai. So cau dung cua ban la: " + soCauTraLoiDung;
                    MessageBox.Show(tmp);
                    break;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //_client.Client.Shutdown(SocketShutdown.Send);
            //_thread.Join();
            //_ns.Close();
            //_client.Close();
            //ServerSocket.Stop();

            if (_ns != null)
            {
                _ns.Close();
            }
            if (_client != null)
            {
                _client.Close();
            }

            if (ServerSocket != null)
            {
                ServerSocket.Stop();
            }

        }

        List<Question> _lstQuestions;
        private void button2_Click(object sender, EventArgs e)
        {
            // Read a text file line by line.  
            string path = @"C:\Users\Vong Nguyet\Desktop\dack\questions.txt";
            string[] lines = File.ReadAllLines(path);

            _lstQuestions = new List<Question>();
            Question question = null;
            foreach (string line in lines)
            {
                if (line.StartsWith("@@"))//Question
                {
                    question = new Question();
                    question.Content = line.Substring(2);
                }
                if (line.StartsWith("--"))//Image
                {
                    question.ImageLink = line.Substring(2);
                }
                if (line.StartsWith("$$"))//Answer
                {
                    Answer answer = new Answer();
                    string[] M = line.Substring(2).Split(new char[] { '.' });
                    answer.Id = M[0];
                    answer.Content = M[1];

                    question.ListAnswers.Add(answer);
                }
                if (line.StartsWith("&&"))
                {
                    question.CorrectAnswerId = line.Substring(2);
                }

                if (line.StartsWith("%%"))
                    _lstQuestions.Add(question);
            }

            int a = 1;
        }


        private void button3_Click(object sender, EventArgs e)
        {
            Question question = _lstQuestions[indexQuestion];

            rtbQuestion.Text = question.Content;
            txtA.Text = question.ListAnswers[0].Content;
            txtB.Text = question.ListAnswers[1].Content;
            txtC.Text = question.ListAnswers[2].Content;
            txtD.Text = question.ListAnswers[3].Content;

            indexQuestion++;

        }
    }
}
