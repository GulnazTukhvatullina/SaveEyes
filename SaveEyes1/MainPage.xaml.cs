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
using System.Collections.ObjectModel;
using SaveEyes1.DB;

namespace SaveEyes1
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public IEnumerable<Agent> Agents { get; set; }
        public int PageNumber { get; set; } = 1;
        public Dictionary<string, Func<Agent, object>> Sortings { get; set; }
        public MainPage()
        {
            InitializeComponent();

            Agents = bd_connection.connection.Agent.ToList();

            agents.ItemsSource = bd_connection.connection.Agent.ToList();

            Sortings = new Dictionary<string, Func<Agent, object>>
            {
                { "Наименование по возрастанию", x => x.Title },
                { "Наименование по убыванию", x => x.Title },
                { "Размер скидки по возрастанию", x => x.ProductSale },
                { "Размер скидки по убыванию", x => x.ProductSale },
                { "Приоритет по возрастанию", x => x.Priority },
                { "Приоритет по убыванию", x => x.Priority },
            };

            var allTypes = bd_connection.connection.AgentType.ToList();
            allTypes.Insert(0, new AgentType
            {
                Title = "Все типы"
            });
            cb_filter.ItemsSource = allTypes;
            cb_filter.SelectedIndex = 0;

            var currentAgent = bd_connection.connection.Agent.ToList();
            agents.ItemsSource = currentAgent;
        }

        public void Filter(bool filtersChanged)
        {
            //var sorting = Sortings[cb_sort.SelectedItem as string];
            if (filtersChanged)
                PageNumber = 1;
            var currentAgent = bd_connection.connection.Agent.ToList();

            if (cb_filter.SelectedIndex > 0)
                currentAgent = currentAgent.Where(a => a.AgentType == cb_filter.SelectedItem as AgentType).ToList();

            //Agents = Agents.OrderBy(sorting).ToList();
            //if ((cb_sort.SelectedItem as string).Contains("убыванию"))
            //    Agents = Agents.Reverse();

            currentAgent = currentAgent.Where(a => a.Title.ToLower().Contains(tb_search.Text.ToLower()) || a.Email.ToLower().Contains(tb_search.Text.ToLower()) 
            || a.Phone.ToLower().Contains(tb_search.Text.ToLower())).ToList();

            agents.ItemsSource = currentAgent.OrderBy(a => a.Phone);

            agents.ItemsSource = new ObservableCollection<Agent>(Agents.Skip((PageNumber - 1) * 10).Take(10).ToList());

            SetPageNumbers();
        }

        private void SetPageNumbers()
        {
            PageNumbersPanel.Children.Clear();
            int pagesCount = Agents.Count() % 10 == 0 ? Agents.Count() / 10 : Agents.Count() / 10 + 1;
            for (int i = 0; i < pagesCount; i++)
            {
                var hyperlink = new Hyperlink()
                {
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize = 18,
                    TextDecorations = null
                };
                hyperlink.Inlines.Add($"{i + 1}");
                hyperlink.Click += NavigateToPage;

                var textBlock = new TextBlock() { Margin = new Thickness(5, 0, 5, 0) };

                if (i == PageNumber - 1)
                    hyperlink.TextDecorations = TextDecorations.Underline;

                textBlock.Inlines.Add(hyperlink);

                PageNumbersPanel.Children.Add(textBlock);
            }
        }

        private void NavigateToPage(object sender, RoutedEventArgs e)
        {
            PageNumber = int.Parse(((sender as Hyperlink).Inlines.FirstOrDefault() as Run).Text);
            (sender as Hyperlink).TextDecorations = TextDecorations.Underline;
            Filter(false);
        }

        private void PreviousPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (PageNumber > 1)
            {
                PageNumber--;
                Filter(false);
            }
        }

        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            int pagesCount = Agents.Count() % 10 == 0 ? Agents.Count() / 10 : Agents.Count() / 10 + 1;
            if (PageNumber < pagesCount)
            {
                PageNumber++;
                Filter(false);
            }
        }

        private void agents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void agents_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new AgentPage(agents.SelectedItem as DB.Agent));
        }

        private void btn_add_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AgentPage(new DB.Agent()));
        }

        private void cb_filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Filter(true);
        }

        private void tb_search_SelectionChanged(object sender, RoutedEventArgs e)
        {
            Filter(true);
        }
    }
}
