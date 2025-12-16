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

namespace Muhametshin_Autoservice4
{
    /// <summary>
    /// Логика взаимодействия для SignUpPage.xaml
    /// </summary>
    public partial class SignUpPage : Page
    {
        private Service _currentService = new Service();

        public SignUpPage(Service SelectedService)
        {
            InitializeComponent();
            if (SelectedService != null)
                this._currentService = SelectedService;

            DataContext = _currentService;

            var _currentClient = MuhametshinAvtoservisEntities.GetContext().Client.ToList();

            ComboClient.ItemsSource = _currentClient;

            StartDate.DisplayDateStart = DateTime.Today;
        }

        private ClientService _currentClientService = new ClientService();

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (ComboClient.SelectedItem == null)
            {
                errors.AppendLine("Укажите ФИО клиента");
            }
            if (StartDate.Text == "")
            {
                errors.AppendLine("Укажите дату услуги");
            }
            if (TBStart.Text == "")
            {
                errors.AppendLine("Укажите время начала услуги");
            }
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            DateTime startTime;
            try
            {
                startTime = Convert.ToDateTime(StartDate.Text + " " + TBStart.Text);
            }
            catch (FormatException)
            {
                MessageBox.Show("Неверный формат времени");
                return;
            }

            if (startTime < DateTime.Now)
            {
                MessageBox.Show("Нельзя записать на прошедшую дату или время");
                return;
            }

            Client selectedClient = ComboClient.SelectedItem as Client;
            _currentClientService.ClientID = selectedClient.ID;

            _currentClientService.ServiceID = _currentService.ID;
            _currentClientService.StartTime = startTime;

            try
            {
                _currentClientService.StartTime = Convert.ToDateTime(StartDate.Text + " " + TBStart.Text);
            }
            catch (FormatException)
            {
                MessageBox.Show(errors.AppendLine("Неверный ввод").ToString());
                return;
            }

            if (_currentClientService.ID == 0)
            {
                MuhametshinAvtoservisEntities.GetContext().ClientService.Add(_currentClientService);
            }
            try
            {
                MuhametshinAvtoservisEntities.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void TBStart_TextChanged(object sender, TextChangedEventArgs e)
        {
            string s = TBStart.Text;

            if (s.Length < 4 || !s.Contains(':'))
            {
                TBEnd.Text = "";
            }
            else
            {
                string[] start = s.Split(new char[] { ':' });
                int startHour = Convert.ToInt32(start[0]) * 60;
                int startMin = Convert.ToInt32(start[1]);
                int startTotalMinutes = startHour + startMin;


                int endTotalMinutes = startTotalMinutes + _currentService.DurationInSeconds;

                int EndHour = (endTotalMinutes / 60) % 24;
                int EndMin = endTotalMinutes % 60;

                string nextDayIndicator = "";

                if (endTotalMinutes >= 1440)
                {
                    nextDayIndicator = " (след. день)";
                }

                if (startHour < 1440)
                {
                    TBEnd.Text = $"{EndHour}:{EndMin:D2}{nextDayIndicator}";
                }
                else
                {
                    TBEnd.Text = $"*";
                }

            }
        }
    }
}