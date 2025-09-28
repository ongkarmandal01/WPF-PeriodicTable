using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PeriodicTable
{
    public partial class MainWindow : Window
    {
        // File to store the high score
        //private const string HighScoreFileName = "highscore.txt";

        // Array holding the symbols of elements in the periodic table
        private readonly string[] elementSymbols = 
        {
            "H", "He", "Li", "Be", "B", "C", "N", "O", "F", "Ne", "Na", "Mg", "Al", "Si", "P", "S", "Cl", "Ar",
            "K", "Ca", "Sc", "Ti", "V", "Cr", "Mn", "Fe", "Co", "Ni", "Cu", "Zn", "Ga", "Ge", "As", "Se", "Br", "Kr",
            "Rb", "Sr", "Y", "Zr", "Nb", "Mo", "Tc", "Ru", "Rh", "Pd", "Ag", "Cd", "In", "Sn", "Sb", "Te", "I", "Xe",
            "Cs", "Ba", "La", "Ce", "Pr", "Nd", "Pm", "Sm", "Eu", "Gd", "Tb", "Dy", "Ho", "Er", "Tm", "Yb", "Lu",
            "Hf", "Ta", "W", "Re", "Os", "Ir", "Pt", "Au", "Hg", "Tl", "Pb", "Bi", "Po", "At", "Rn", "Fr", "Ra", "Ac",
            "Th", "Pa", "U", "Np", "Pu", "Am", "Cm", "Bk", "Cf", "Es", "Fm", "Md", "No", "Lr", "Rf", "Db", "Sg", "Bh",
            "Hs", "Mt", "Ds", "Rg", "Cn", "Nh", "Fl", "Mc", "Lv", "Ts", "Og"
        };

        public MainWindow()
        {
            InitializeComponent();
            InitializeElements();
            bool IsDarkMode = Properties.Settings.Default.IsDarkMode;
            if (IsDarkMode)
            {
                ApplyTheme("Themes/DarkTheme.xaml");
            }
            else
            { 
                ApplyTheme("Themes/LightTheme.xaml"); 
            }
        }


        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox? currentTextBox = sender as TextBox;
            if (currentTextBox == null) return;

            int currentRow = Grid.GetRow(currentTextBox);
            int currentColumn = Grid.GetColumn(currentTextBox);

            if (e.Key == Key.Down)
            {
                MoveFocus(currentRow + 1, currentColumn);
            }
            else if (e.Key == Key.Up)
            {
                MoveFocus(currentRow - 1, currentColumn);
            }
            else if (e.Key == Key.Right)
            {
                MoveFocus(currentRow, currentColumn + 1);
            }
            else if (e.Key == Key.Left)
            {
                MoveFocus(currentRow, currentColumn - 1);
            }
        }

        private void MoveFocus(int rowIndex, int columnIndex)
        {
            foreach (UIElement element in ElementGrid.Children)
            {
                if (element is TextBox nextTextBox && Grid.GetRow(nextTextBox) == rowIndex && Grid.GetColumn(nextTextBox) == columnIndex)
                {
                    nextTextBox.Focus();
                    break;
                }
            }
        }

        // Enable dragging when clicking the title bar
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        // Minimize Window
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        // Maximize/Restore Window
        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = (WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;
        }

        // Close Window
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
 

        private void UiModeHandler(object sender, RoutedEventArgs e)
        {
            bool IsDarkMode = Properties.Settings.Default.IsDarkMode;

                if (!IsDarkMode)
                {
                    ApplyTheme("Themes/DarkTheme.xaml");
                }
                else
                {
                    ApplyTheme("Themes/LightTheme.xaml");
                }

            Properties.Settings.Default.IsDarkMode = !IsDarkMode; // Changes to current UI mode
            Properties.Settings.Default.Save(); // Saves the setting

        }

        private void ApplyTheme(string themePath)
        {
            ResourceDictionary newTheme = new ResourceDictionary { Source = new Uri(themePath, UriKind.Relative) };

            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(newTheme);
        }
    

    /// <summary>
    /// Reveals the answers by filling in the element symbols.
    /// </summary>
    private void RevealAns(object sender, RoutedEventArgs e)
        {
            cb1.IsChecked = true; // Automatically check the checkbox when revealing the answers

            foreach (var element in ElementGrid.Children.OfType<TextBox>())
            {
                if (int.TryParse(element.Tag?.ToString(), out int atomicNo)) // Safely parse the atomic number
                {
                    // Set the TextBox text to the correct element symbol
                    element.Text = elementSymbols[atomicNo - 1];
                }
            }
        }

        /// <summary>
        /// Clears all user inputs when the checkbox is unchecked.
        /// </summary>
        private void HandleUnchecked(object sender, RoutedEventArgs e)
        {
            // Ensure that the user cannot uncheck while answers are revealed
            if (cb1.IsChecked == true) return;

            foreach (var element in ElementGrid.Children.OfType<TextBox>())
            {
                var border = (Border)element.Template.FindName("BorderElement", element);
                if (border?.BorderBrush is SolidColorBrush solidBrush && 
                    solidBrush.Color == (Color)ColorConverter.ConvertFromString("#b54747")) // Red color for incorrect answers
                {
                    element.Text = ""; // Clear the text if the answer was incorrect
                }
            }
            cb1.IsChecked = false; // Uncheck the checkbox
        }

        /// <summary>
        /// Initializes the elements by attaching event handlers.
        /// </summary>
        private void InitializeElements()
        {
            // Attach event handler to each TextBox for input handling
            foreach (var textBox in ElementGrid.Children.OfType<TextBox>())
            {
                textBox.PreviewTextInput += ElementInputHandler;
            }

            // Attach event handlers for the buttons
            SubmitButton.Click += SubmitButtonClickHandler;
            RefreshButton.Click += RefreshButtonClickHandler;
        }

        /// <summary>
        /// Formats the input text to start with an uppercase letter followed by lowercase letters.
        /// </summary>
        private void ElementInputHandler(object sender, TextCompositionEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                string input = textBox.Text + e.Text;
                textBox.Text = input.Length > 0 ? char.ToUpper(input[0]) + input.Substring(1).ToLower() : "";
                textBox.CaretIndex = textBox.Text.Length; // Move the caret to the end
                e.Handled = true; // Prevent further input handling
            }
        }

        /// <summary>
        /// Handles the submission of the quiz by checking the answers and saving the high score.
        /// </summary>
        private void SubmitButtonClickHandler(object sender, RoutedEventArgs e)
        {
            // Prevent submission if the answers have already been revealed
            if (cb1.IsChecked == true) return;

            int correctCount = 0;
            foreach (var textBox in ElementGrid.Children.OfType<TextBox>())
            {
                if (int.TryParse(textBox.Tag?.ToString(), out int atomicNo)) // Safe parsing of atomic number
                {
                    var border = (Border)textBox.Template.FindName("BorderElement", textBox);
                    bool isCorrect = textBox.Text.Trim() == elementSymbols[atomicNo - 1]; // Check if input matches the symbol

                    if (border != null) // Set the border color based on correctness
                    {
                        border.BorderBrush = new SolidColorBrush(isCorrect ? Color.FromRgb(78, 135, 82) : Color.FromRgb(181, 71, 71));
                    }

                    if (isCorrect)
                    {
                        correctCount++;
                    }
                }
            }

            int totalQuestions = 118;
            string message = $"✅ You got {correctCount} correct! ✅";

            if (correctCount == totalQuestions) // Assuming you have totalQuestions defined
            {
                message = "🌟 Perfect Score! 🌟 You got all elements correct!";
            }
            else if (correctCount >= totalQuestions * 0.8) // 80% or higher - Excellent
            {
                message = "Excellent! You did a fantastic job and got a high score!";
            }
            else if (correctCount >= totalQuestions * 0.5) // 50% to 80% - Good
            {
                message = "Good effort! You got a solid score. Keep practicing to improve!";
            }
            else // Below 50% - Needs improvement
            {
                message = "Nice try! You can definitely do better. Review the material and try again!";
            }

            if (correctCount > Properties.Settings.Default.HighScore)
            {
                message += "\n🎉 New High Score! 🎉";
                //Properties.Settings.Default.HighScore = correctCount;
                //Properties.Settings.Default.Save();
            }
            else
            {
                message += $"\nYour High Score is {Properties.Settings.Default.HighScore}. 🎉"; // Keep it simpler for lower scores
            }


            // Save high score if this is the new highest score
            SaveHighScore(correctCount);
            var messageBox = new MessageDialog(message);
            messageBox.ShowDialog();

            cb1.Visibility = Visibility.Visible; // Show the checkbox after submission
        }

        /// <summary>
        /// Handles the refresh button click to reset all inputs and borders.
        /// </summary>
        private void RefreshButtonClickHandler(object sender, RoutedEventArgs e)
        {
            cb1.IsChecked = false; // Uncheck the checkbox
            cb1.Visibility = Visibility.Collapsed; // Hide the checkbox

            foreach (var textBox in ElementGrid.Children.OfType<TextBox>())
            {
                textBox.Text = ""; // Clear text in all TextBoxes
                var border = (Border)textBox.Template.FindName("BorderElement", textBox);
                if (border != null) // Reset the border color to black
                {
                    border.SetResourceReference(Border.BorderBrushProperty, "ForegroundBrush");
                }
            }
        }

        /// <summary>
        /// Saves the current score to the high score file if it is the highest score.
        /// </summary>
        private void SaveHighScore(int score)
        {
            int highScore = Properties.Settings.Default.HighScore;
            if (score > highScore)
            {
                Properties.Settings.Default.HighScore = score;
                Properties.Settings.Default.Save(); // Saves the setting
            }
        }
    }
}
