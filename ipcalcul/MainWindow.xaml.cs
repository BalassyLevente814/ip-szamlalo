using System;
using System.Net;
using System.Windows;

namespace IPv4Calculator
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IPAddress ip = IPAddress.Parse(IpTextBox.Text); // átalakítjuk a szöveget IP címre
                IPAddress mask = IPAddress.Parse(MaskTextBox.Text); // a maszkot is

                byte[] ipBytes = ip.GetAddressBytes(); // itt eltárolja az 1 vagy 0 ákat amit megadtunk az ip változónak
                byte[] maskBytes = mask.GetAddressBytes(); // itt eltárolja az 1 vagy 0 ákat amit megadtunk az maszk változónak

                byte[] networkBytes = new byte[4];
                byte[] broadcastBytes = new byte[4];

                for (int i = 0; i < 4; i++)
                {
                    networkBytes[i] = (byte)(ipBytes[i] & maskBytes[i]); // ip pl: 11111 + mask pl: 10100 = 10100 hálózati cím
                    broadcastBytes[i] = (byte)(networkBytes[i] | (~maskBytes[i])); // hoszt megtalálása a cél azért fordítjuk meg a maskbytejait
                }

                IPAddress network = new IPAddress(networkBytes); // eltároljuk őket 
                IPAddress broadcast = new IPAddress(broadcastBytes);

                int cidr = 0;
                foreach (byte b in maskBytes)
                    cidr += CountBits(b);

                int hosts = (int)Math.Pow(2, 32 - cidr) - 2;// iszámoljuk a hosztok számát, 2on-ra emeljük, mert minden hostbit vagy 0 v 1, 32 a max és abból kivonjuk a prefixet, ami megadja mennyi a lehetséges hosztbit

                IPAddress firstHost = IncrementIPAddress(network);
                IPAddress lastHost = DecrementIPAddress(broadcast);

                NetworkText.Text = $"Hálózati cím: {network}";
                BroadcastText.Text = $"Broadcast cím: {broadcast}";
                CidrText.Text = $"Maszk: {mask} (/ {cidr})";
                FirstHostText.Text = $"Első host: {firstHost}";
                LastHostText.Text = $"Utolsó host: {lastHost}";
                HostCountText.Text = $"Használható hostok száma: {hosts}";
            }
            catch
            {
                MessageBox.Show("Hibás IP vagy maszk formátum!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private int CountBits(byte b)
        {
            int count = 0;
            while (b > 0)
            {
                count += b & 1;
                b >>= 1;
            }
            return count;
        }

        private IPAddress IncrementIPAddress(IPAddress ip)
        {
            byte[] bytes = ip.GetAddressBytes();
            for (int i = 3; i >= 0; i--)
            {
                if (bytes[i] < 255)
                {
                    bytes[i]++;
                    break;
                }
                bytes[i] = 0;
            }
            return new IPAddress(bytes);
        }

        private IPAddress DecrementIPAddress(IPAddress ip)
        {
            byte[] bytes = ip.GetAddressBytes();
            for (int i = 3; i >= 0; i--)
            {
                if (bytes[i] > 0)
                {
                    bytes[i]--;
                    break;
                }
                bytes[i] = 255;
            }
            return new IPAddress(bytes);
        }
    }
}
