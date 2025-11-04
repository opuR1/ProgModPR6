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
using sommer_al.Models;
using sommer_al.Services;

namespace sommer_al.Pages
{
    /// <summary>
    /// Логика взаимодействия для Page1.xaml
    /// </summary>
    public partial class Page1 : Page
    {
        int click;
        public Page1()
        {
            InitializeComponent();
            click = 0;
        }

        private void btnEnterGuest_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Client(null, null));
        }
        private void GenerateCapctcha()
        {
            tbCaptcha.Visibility = Visibility.Visible;
            tblCaptcha.Visibility = Visibility.Visible;

            string capctchaText = CaptchaGenerator.GenerateCaptchaText(6);
            tblCaptcha.Text = capctchaText;
            tblCaptcha.TextDecorations = TextDecorations.Strikethrough;
        }

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            click += 1;
            string login = tbLogin.Text.Trim();
            string password = tbPassword.Text.Trim();
            bool isEmployee = chkIsEmployee.IsChecked ?? false;
            int RoleType = isEmployee == true ? 1 : 2;
            string HashedPassword = Hash.HashPassword(password);

            ProgramModPR5Entities db = ProgramModPR5Entities.GetContext();

            var user = db.Users.Where(x => x.Email == login && x.Password == HashedPassword && RoleType == x.RoleID).FirstOrDefault();
            
            if (click == 1)
            {
                if (user != null)
                {
                    MessageBox.Show("Вы вошли под: " + user.Roles.Role);
                    LoadPage(user.Roles.Role.ToString(), user);
                }
                else
                {
                    MessageBox.Show("Вы ввели логин или пароль неверно!");
                    GenerateCapctcha();
                    ClearFields();
                    
                }
            }
            else if (click > 1)
            {
                if (user != null && tbCaptcha.Text == tblCaptcha.Text)
                {
                    MessageBox.Show("Вы вошли под: " + user.Roles.Role.ToString());
                    LoadPage(user.Roles.Role.ToString(), user);
                }
                else
                {
                    MessageBox.Show("Введите данные заново!");
                    ClearFields();
                }
            }
        }
        private void LoadPage(string _role, Users user)
        {
            click = 0;
            switch (_role)
            {
                case "Клиент":
                    NavigationService.Navigate(new Client(user, _role));
                    break;
                case "Сотрудник":
                    NavigationService.Navigate(new Employee(user, _role));
                    break;
            }
        }
        private void ClearFields()
        {
            tbPassword.Clear();
            tbCaptcha.Clear();
            GenerateCapctcha(); // Генерируем новую капчу после очистки
        }


    }
}
