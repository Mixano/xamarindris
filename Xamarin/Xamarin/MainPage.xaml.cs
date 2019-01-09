using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainPage : ContentPage
	{

        private string userName;
        private string host;
        private int port;
        private TcpClient client;
        private NetworkStream stream;
        private ConnectionPage connectionPage;

        public MainPage()
		{
			InitializeComponent();
            connectionPage = new ConnectionPage(this);
            GetConnectionData();
        }

        public void StartConnection(string userName, string ip, int port)
        {
            this.userName = userName;
            this.host = ip;
            this.port = port;

            ConnectToServer();

            SendMessage(new ClientMessage(userName, ClientMessageType.CLIENT_CONNECTED));

            Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
            receiveThread.Start();
        }

        private async void GetConnectionData()
        {
            await Navigation.PushModalAsync(connectionPage);
        }

        private void SendMessage(ClientMessage clientMessage)
        {
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(ClientMessage));
            MemoryStream memoryStream = new MemoryStream();
            jsonFormatter.WriteObject(memoryStream, clientMessage);
            byte[] jsonMessageBytes = memoryStream.ToArray();
            string jsonMessage = Encoding.UTF8.GetString(jsonMessageBytes, 0, jsonMessageBytes.Length);
            stream.Write(jsonMessageBytes, 0, jsonMessageBytes.Length);
            memoryStream.Close();
        }

        private void SendButton_Clicked(object sender, EventArgs e)
        {
            SendMessage(new ClientMessage(entryInputText.Text, ClientMessageType.CLIENT_MESSAGE));
            entryInputText.Text = null;
        }

        private ServerMessage GetMessage()
        {
            byte[] data = new byte[512];
            StringBuilder stringBuilder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = stream.Read(data, 0, data.Length);
                stringBuilder.Append(Encoding.UTF8.GetString(data, 0, bytes));
            }
            while (stream.DataAvailable);

            DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(ServerMessage));
            MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(stringBuilder.ToString()));
            ServerMessage serverMessage = dataContractJsonSerializer.ReadObject(memoryStream) as ServerMessage;
            memoryStream.Close();
            return serverMessage;
        }

        private void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    ServerMessage serverMessage = GetMessage();
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        labelOutput.Text += "[" + serverMessage.Time.ToLongTimeString() + "]" + serverMessage.Message + "\n";
                    });
                }
                catch
                {
                    Disconnect();
                }
            }
        }

        private void Disconnect()
        {
            if (stream != null)
            {
                stream.Close();
            }
            if (client != null)
            {
                client.Close();
            }
            Environment.Exit(0);
        }

        private void ConnectToServer()
        {
            try
            {
                client = new TcpClient();
                client.Connect(host, port);
                stream = client.GetStream();
            }
            catch (SocketException exception)
            {
                DisplayAlert("Chat", "Could not connect to server: " + exception.Message, "OK");
                Environment.Exit(0);
            }
        }

        private void EntryInputText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (entryInputText.Text == null)
            {
                sendButton.IsEnabled = false;
            }
            else
            {
                sendButton.IsEnabled = true;
            }
        }
    }
}