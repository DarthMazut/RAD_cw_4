using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Cw_4_RAD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //  v_      - variables
        //  p_      - property
        //  sm_     - standard methods
        //  xe_     - XAML elements
        //  em_     - event methods

        //==================================================================================================================
        //============================================== VARIABLES =========================================================
        //==================================================================================================================

        Regex[] v_tabRegex;

        Storyboard v_storyBoardColor;
        ColorAnimation v_animacjaKoloru;

        //==================================================================================================================
        //=============================================== PROPERTY =========================================================
        //==================================================================================================================

        /// <summary>
        /// Przechowuje wynik biezacego dzialania w systemie decymalnym.
        /// </summary>
        public long p_Wynik { get; set; } = 0;

        /// <summary>
        /// Okresla czy wprowadzone do TextBox'ow dane sa poprawne i czy mozna przeprowadzic na nich operacje arytmetyczne.
        /// </summary>
        public bool p_isValid { get; set; } = true;

        /// <summary>
        /// Przechowuje wartosc dziesietna LICZBY 1.
        /// </summary>
        public long p_wartoscTextBox1 { get; set; } = 0;
        
        /// <summary>
        /// Przechowuje wartosc dziesietna LICZBY 2.
        /// </summary>
        public long p_wartoscTextBox2 { get; set; } = 0;

        //==================================================================================================================
        //======================================= CONSTRUCTOR / DESTRUCTOR =================================================
        //==================================================================================================================

        public MainWindow()
        {
            InitializeComponent();

            xe_TextBlock_Clear.Background = Brushes.Gold;
            xe_TextBlock_Dodaj.Background = Brushes.LightGreen;
            xe_TextBlock_Odejmij.Background = Brushes.PaleVioletRed;
            xe_TextBlock_Pomnoz.Background = Brushes.RoyalBlue;
            xe_TextBlock_Dziel.Background = Brushes.MediumPurple;

            xe_Textbox_LICZBA_1.GotFocus += em_TextBox_LICZBA_1_OnFocus;
            xe_Textbox_LICZBA_2.GotFocus += em_TextBox_LICZBA_2_OnFocus;

            xe_Textbox_LICZBA_1.LostFocus += em_TextBox_LICZBA_1_OnLostFocus;
            xe_Textbox_LICZBA_2.LostFocus += em_TextBox_LICZBA_2_OnLostFocus;

            xe_Textbox_LICZBA_1.TextChanged += em_TextBox_LICZBA_1_OnTextChange;
            xe_Textbox_LICZBA_2.TextChanged += em_TextBox_LICZBA_2_OnTextChange;

            v_tabRegex = new Regex[9];
            v_tabRegex[0] = new Regex("^[0-1]*$");
            v_tabRegex[1] = new Regex("^[0-2]*$");
            v_tabRegex[2] = new Regex("^[0-3]*$");
            v_tabRegex[3] = new Regex("^[0-4]*$");
            v_tabRegex[4] = new Regex("^[0-5]*$");
            v_tabRegex[5] = new Regex("^[0-6]*$");
            v_tabRegex[6] = new Regex("^[0-7]*$");
            v_tabRegex[7] = new Regex("^[0-8]*$");
            v_tabRegex[8] = new Regex("^[0-9]*$");

            v_storyBoardColor = new Storyboard();
            v_animacjaKoloru = new ColorAnimation();

            v_animacjaKoloru.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            v_animacjaKoloru.RepeatBehavior = RepeatBehavior.Forever;
            //v_animacjaKoloru.FillBehavior = FillBehavior.Stop;
            v_animacjaKoloru.AutoReverse = true;

            v_storyBoardColor.Children.Add(v_animacjaKoloru);
            Storyboard.SetTargetProperty(v_animacjaKoloru, new PropertyPath("(TextBlock.Background).(SolidColorBrush.Color)"));

        }

        //==================================================================================================================
        //============================================= STD METHODS ========================================================
        //==================================================================================================================

        //Zrodlo: https://stackoverflow.com/questions/923771/quickest-way-to-convert-a-base-10-number-to-any-base-in-net

        /// <summary>
        /// Converts the given decimal number to the numeral system with the
        /// specified radix (in the range [2, 36]).
        /// </summary>
        /// <param name="decimalNumber">The number to convert.</param>
        /// <param name="radix">The radix of the destination numeral system (in the range [2, 36]).</param>
        /// <returns></returns>
        public static string sm_DecimalToArbitrarySystem(long decimalNumber, int radix)
        {
            const int BitsInLong = 64;
            const string Digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            if (radix < 2 || radix > Digits.Length)
                throw new ArgumentException("The radix must be >= 2 and <= " + Digits.Length.ToString());

            if (decimalNumber == 0)
                return "0";

            int index = BitsInLong - 1;
            long currentNumber = Math.Abs(decimalNumber);
            char[] charArray = new char[BitsInLong];

            while (currentNumber != 0)
            {
                int remainder = (int)(currentNumber % radix);
                charArray[index--] = Digits[remainder];
                currentNumber = currentNumber / radix;
            }

            string result = new String(charArray, index + 1, BitsInLong - index - 1);
            if (decimalNumber < 0)
            {
                result = "-" + result;
            }

            return result;
        }

        //Zrodlo: http://www.pvladov.com/2012/07/arbitrary-to-decimal-numeral-system.html

        /// <summary>
        /// Converts the given number from the numeral system with the specified
        /// radix (in the range [2, 36]) to decimal numeral system.
        /// </summary>
        /// <param name="number">The arbitrary numeral system number to convert.</param>
        /// <param name="radix">The radix of the numeral system the given number
        /// is in (in the range [2, 36]).</param>
        /// <returns></returns>
        public static long sm_ArbitraryToDecimalSystem(string number, int radix)
        {
            const string Digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            if (radix < 2 || radix > Digits.Length)
                throw new ArgumentException("The radix must be >= 2 and <= " +
                    Digits.Length.ToString());

            if (String.IsNullOrEmpty(number))
                return 0;

            // Make sure the arbitrary numeral system number is in upper case
            number = number.ToUpperInvariant();

            long result = 0;
            long multiplier = 1;
            for (int i = number.Length - 1; i >= 0; i--)
            {
                char c = number[i];
                if (i == 0 && c == '-')
                {
                    // This is the negative sign symbol
                    result = -result;
                    break;
                }

                int digit = Digits.IndexOf(c);
                if (digit == -1)
                    throw new ArgumentException(
                        "Invalid character in the arbitrary numeral system number",
                        "number");

                result += digit * multiplier;
                multiplier *= radix;
            }

            return result;
        }

        /// <summary>
        /// Ustawia i wlacza animacje kolorw dla TextBoxow (przyciskow dzialan arytmetycznych).
        /// </summary>
        /// <param name="obiekt">TextBox na ktorym animacja ma zostac przeprowadzona.</param>
        /// <param name="kolor">Kolor docelowy.</param>
        private void sm_animuj(TextBlock obiekt, Color kolor)
        {
            Storyboard.SetTargetName(v_animacjaKoloru, obiekt.Name);
            v_animacjaKoloru.To = kolor;
            v_storyBoardColor.Begin(this, true);
        }

        //==================================================================================================================
        //============================================= EVENT METHODS ======================================================
        //==================================================================================================================

        private void em_Clear_OnClick(object sender, MouseButtonEventArgs e)
        {
            xe_Textbox_LICZBA_1.Text = "0";
            xe_Textbox_LICZBA_2.Text = "0";
            xe_WYNIK_Label.Content = "0";
            xe_Label_Komunikat.Content = "";
            p_wartoscTextBox1 = 0;
            p_wartoscTextBox2 = 0;
            p_Wynik = 0;
            p_isValid = true;
        }

        private void em_TextBox_LICZBA_1_OnFocus(object sender, RoutedEventArgs e)
        {
            if (xe_Textbox_LICZBA_1.Text == "0")
            {
                xe_Textbox_LICZBA_1.Text = "";
            }
        }

        private void em_TextBox_LICZBA_2_OnFocus(object sender, RoutedEventArgs e)
        {
            if (xe_Textbox_LICZBA_2.Text == "0")
            {
                xe_Textbox_LICZBA_2.Text = "";
            }
        }

        private void em_TextBox_LICZBA_1_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (xe_Textbox_LICZBA_1.Text == "") xe_Textbox_LICZBA_1.Text = "0";
        }

        private void em_TextBox_LICZBA_2_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (xe_Textbox_LICZBA_2.Text == "") xe_Textbox_LICZBA_2.Text = "0";
        }

        private void em_DODAJ_OnClick(object sender, MouseButtonEventArgs e)
        {

            if (p_isValid)
            {
                long LICZBA_1 = 0;
                long LICZBA_2 = 0;

                LICZBA_1 = sm_ArbitraryToDecimalSystem(xe_Textbox_LICZBA_1.Text, xe_ComboBox_Liczba_1.SelectedIndex + 2);
                LICZBA_2 = sm_ArbitraryToDecimalSystem(xe_Textbox_LICZBA_2.Text, xe_ComboBox_Liczba_2.SelectedIndex + 2);

                long WYNIK = LICZBA_1 + LICZBA_2;
                p_Wynik = WYNIK;
                xe_Label_Komunikat.Content = "";

                xe_WYNIK_Label.Content = sm_DecimalToArbitrarySystem(WYNIK, xe_ComboBox_Wynik.SelectedIndex + 2); 
            }
            else
            {
                MessageBox.Show("Nie możesz wykonać operacji, gdy wprowadzone wartości są nieprawidłowe, ok?", "Ostrzeżenie", MessageBoxButton.OK, MessageBoxImage.Error);
            }

                
        }

        private void em_ComboBox_Wynik_OnChange(object sender, SelectionChangedEventArgs e)
        {
            xe_WYNIK_Label.Content = sm_DecimalToArbitrarySystem(p_Wynik, xe_ComboBox_Wynik.SelectedIndex + 2);
        }

        private void em_ODEJMIJ_OnClick(object sender, MouseButtonEventArgs e)
        {

            if (p_isValid)
            {
                long LICZBA_1 = 0;
                long LICZBA_2 = 0;

                LICZBA_1 = sm_ArbitraryToDecimalSystem(xe_Textbox_LICZBA_1.Text, xe_ComboBox_Liczba_1.SelectedIndex + 2);
                LICZBA_2 = sm_ArbitraryToDecimalSystem(xe_Textbox_LICZBA_2.Text, xe_ComboBox_Liczba_2.SelectedIndex + 2);

                long WYNIK = LICZBA_1 - LICZBA_2;
                if (WYNIK > 0)
                {
                    xe_Label_Komunikat.Content = "";
                    p_Wynik = WYNIK;
                    xe_WYNIK_Label.Content = sm_DecimalToArbitrarySystem(WYNIK, xe_ComboBox_Wynik.SelectedIndex + 2);
                }
                else
                {
                    xe_Label_Komunikat.Content = "Wynik ujemny!";
                }
            }
            else
            {
                MessageBox.Show("Nie możesz wykonać operacji, gdy wprowadzone wartości są nieprawidłowe, ok?", "Ostrzeżenie", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void em_POMNOZ_OnClick(object sender, MouseButtonEventArgs e)
        {
            if (p_isValid)
            {
                long LICZBA_1 = 0;
                long LICZBA_2 = 0;

                LICZBA_1 = sm_ArbitraryToDecimalSystem(xe_Textbox_LICZBA_1.Text, xe_ComboBox_Liczba_1.SelectedIndex + 2);
                LICZBA_2 = sm_ArbitraryToDecimalSystem(xe_Textbox_LICZBA_2.Text, xe_ComboBox_Liczba_2.SelectedIndex + 2);

                long WYNIK = LICZBA_1 * LICZBA_2;
                p_Wynik = WYNIK;
                xe_Label_Komunikat.Content = "";

                xe_WYNIK_Label.Content = sm_DecimalToArbitrarySystem(WYNIK, xe_ComboBox_Wynik.SelectedIndex + 2); 
            }
            else
            {
                MessageBox.Show("Nie możesz wykonać operacji, gdy wprowadzone wartości są nieprawidłowe, ok?", "Ostrzeżenie", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void em_DZIEL_OnClick(object sender, MouseButtonEventArgs e)
        {
            if (p_isValid)
            {
                long LICZBA_1 = 0;
                long LICZBA_2 = 0;

                LICZBA_1 = sm_ArbitraryToDecimalSystem(xe_Textbox_LICZBA_1.Text, xe_ComboBox_Liczba_1.SelectedIndex + 2);
                LICZBA_2 = sm_ArbitraryToDecimalSystem(xe_Textbox_LICZBA_2.Text, xe_ComboBox_Liczba_2.SelectedIndex + 2);

                if (LICZBA_2 == 0)
                {
                    xe_Label_Komunikat.Content = "Dzielenie przez zero nie jest zdefiniowaną operacją.";
                    System.Diagnostics.Process.Start("https://www.youtube.com/watch?v=BRRolKTlF6Q");

                }
                else
                {
                    long WYNIK = LICZBA_1 / LICZBA_2;


                    if (WYNIK >= 0)
                    {
                        if (LICZBA_1 % LICZBA_2 == 0) xe_Label_Komunikat.Content = "";
                        else xe_Label_Komunikat.Content = "Uwaga! Wynik zaokrąglony do najbliższej całkowitej.";
                        p_Wynik = WYNIK;
                        xe_WYNIK_Label.Content = sm_DecimalToArbitrarySystem(WYNIK, xe_ComboBox_Wynik.SelectedIndex + 2);
                    }
                    else if (WYNIK < 0)
                    {
                        xe_Label_Komunikat.Content = "Wynik ujemny!";
                    }
                } 
            }
            else
            {
                MessageBox.Show("Nie możesz wykonać operacji, gdy wprowadzone wartości są nieprawidłowe, ok?", "Ostrzeżenie", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void em_TextBox_LICZBA_2_OnTextChange(object sender, TextChangedEventArgs e)
        {
            if (v_tabRegex[xe_ComboBox_Liczba_2.SelectedIndex].IsMatch(xe_Textbox_LICZBA_2.Text))
            {
                p_wartoscTextBox2 = sm_ArbitraryToDecimalSystem(xe_Textbox_LICZBA_2.Text.ToString(), xe_ComboBox_Liczba_2.SelectedIndex + 2);
                xe_Textbox_LICZBA_2.Background = Brushes.White;
                if (xe_Textbox_LICZBA_1.Background == Brushes.White) p_isValid = true;
                if(p_isValid) xe_Label_Komunikat.Content = "";
            }
            else
            {
                xe_Label_Komunikat.Content = "Podane dane są nieprawidłowe";
                p_isValid = false;
                xe_Textbox_LICZBA_2.Background = Brushes.Red;
                p_wartoscTextBox2 = 0;
            }
        }

        private void em_TextBox_LICZBA_1_OnTextChange(object sender, TextChangedEventArgs e)
        {
            if(v_tabRegex[xe_ComboBox_Liczba_1.SelectedIndex].IsMatch(xe_Textbox_LICZBA_1.Text))
            {
                p_wartoscTextBox1 = sm_ArbitraryToDecimalSystem(xe_Textbox_LICZBA_1.Text.ToString(), xe_ComboBox_Liczba_1.SelectedIndex + 2);
                xe_Textbox_LICZBA_1.Background = Brushes.White;
                if(xe_Textbox_LICZBA_2.Background == Brushes.White)p_isValid = true;
                if (p_isValid) xe_Label_Komunikat.Content = "";
            }
            else
            {
                xe_Label_Komunikat.Content = "Podane dane są nieprawidłowe";
                p_isValid = false;
                xe_Textbox_LICZBA_1.Background = Brushes.Red;
                p_wartoscTextBox1 = 0;
            }
        }

        private void em_ComboBox_LICZBA_1_OnChange(object sender, SelectionChangedEventArgs e)
        {
            xe_Textbox_LICZBA_1.Text = sm_DecimalToArbitrarySystem(p_wartoscTextBox1, xe_ComboBox_Liczba_1.SelectedIndex + 2);
        }

        private void em_ComboBox_LICZBA_3_OnChange(object sender, SelectionChangedEventArgs e)
        {
            xe_Textbox_LICZBA_2.Text = sm_DecimalToArbitrarySystem(p_wartoscTextBox2, xe_ComboBox_Liczba_2.SelectedIndex + 2);
        }

        private void em_CLEAR_OnMouseEnter(object sender, MouseEventArgs e)
        {
            sm_animuj(xe_TextBlock_Clear, Colors.White);
        }

        private void em_CLEAR_OnMouseLEave(object sender, MouseEventArgs e)
        {
            v_storyBoardColor.Stop();
            xe_TextBlock_Clear.Background = Brushes.Gold;
        }

        private void em_DODAJ_OnMouseEnter(object sender, MouseEventArgs e)
        {
            if(p_isValid)sm_animuj(xe_TextBlock_Dodaj, Colors.White);
        }

        private void em_DODAJ_OnMouseLEave(object sender, MouseEventArgs e)
        {
            v_storyBoardColor.Stop();
            xe_TextBlock_Dodaj.Background = Brushes.LightGreen;
        }

        private void em_ODEJMIJ_OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (p_isValid) sm_animuj(xe_TextBlock_Odejmij, Colors.White);
        }

        private void em_ODEJMIJ_OnMouseLeave(object sender, MouseEventArgs e)
        {
            v_storyBoardColor.Stop();
            xe_TextBlock_Odejmij.Background = Brushes.PaleVioletRed;
        }

        private void em_POMNOZ_OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (p_isValid) sm_animuj(xe_TextBlock_Pomnoz, Colors.White);
        }

        private void em_POMNOZ_OnMouseLeave(object sender, MouseEventArgs e)
        {
            v_storyBoardColor.Stop();
            xe_TextBlock_Pomnoz.Background = Brushes.RoyalBlue;
        }

        private void em_DZIEL_OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (p_isValid) sm_animuj(xe_TextBlock_Dziel, Colors.White);
        }

        private void em_DZIEL_OnMouseLeave(object sender, MouseEventArgs e)
        {
            v_storyBoardColor.Stop();
            xe_TextBlock_Dziel.Background = Brushes.MediumPurple;
        }

        private void em_Menu_Close_OnClick(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void em_Menu_Pomoc_OnClick(object sender, EventArgs e)
        {
            MessageBox.Show("Wprowadź odpowiednie liczby do przeznaczonych na to pól po czym kliknij w jedną z umieszczonych pod spodem operacji arytmetycznych. Wynik pokaże się u góry.\n\nAplikacja ma wbudowane zabezpieczenia, wartości zmieniają się automatycznie przy zmianie systemu liczbowego, no i oczywiście są animacje :)","Pomoc", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void em_Menu_About_OnClick(object sender, EventArgs e)
        {
            MessageBox.Show("Aplikacja stworzona na przedmiot 'Programowanie w środowiskach RAD.","O aplikacji...",MessageBoxButton.OK, MessageBoxImage.Information);
        }


        //==================================================================================================================
        //============================================= UNCATEGORIZED ======================================================
        //==================================================================================================================



    }
}
