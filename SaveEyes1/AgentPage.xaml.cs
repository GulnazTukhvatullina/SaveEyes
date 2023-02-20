using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using SaveEyes1.DB;

namespace SaveEyes1
{
    /// <summary>
    /// Логика взаимодействия для AgentPage.xaml
    /// </summary>
    public partial class AgentPage : Page
    {
        public static DB.Agent agent { get; set; }
        public List<DB.AgentType> Type { get; set; }
        public AgentPage(DB.Agent agents)
        {
            InitializeComponent();
            if (agents.ID != 0)
            {
                agent = agents;
                btnRemove.Visibility = Visibility.Visible;
            }
            Type = bd_connection.connection.AgentType.ToList();

            this.DataContext = this;
        }

        private void btn_back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            if (agent.ID == 0)
                bd_connection.connection.Agent.Add(agent);
            bd_connection.connection.SaveChanges();
            NavigationService.Navigate(new MainPage());
        }

        private void btn_photo_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog()
            {
                Filter = "*.jpg|*.jpg|*.png|*.png"
            };
            if (openFile.ShowDialog().GetValueOrDefault())
            {
                agent.Logo = File.ReadAllBytes(openFile.FileName);
                img_agent.Source = new BitmapImage(new Uri(openFile.FileName));
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (agent.ProductSale.Count() == 0)
            {
                if (agent.Shop.Count() != 0)
                    bd_connection.connection.Shop.RemoveRange(agent.Shop);
                if (agent.AgentPriorityHistory.Count() != 0)
                    bd_connection.connection.AgentPriorityHistory.RemoveRange(agent.AgentPriorityHistory);
                
                bd_connection.connection.Agent.Remove(agent);
                bd_connection.connection.SaveChanges();
                NavigationService.Navigate(new MainPage());
            }
            else
                MessageBox.Show("!!");
        }
    }
}
